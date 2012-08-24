using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Ecom.Hal.Exception;
using Ecom.Hal.JSON;
using Newtonsoft.Json;

namespace Ecom.Hal.Persister
{
	public class BasicHalPersisterStrategy : IHalPersisterStrategy
	{
		public IHalPersistResult<T> Persist<T>(T resource, HalLink link = null) where T : IHalResource
		{
			link = link ?? resource.Links.FirstOrDefault(l => l.Rel == "self");
			if (link == null) {
				throw new HalPersisterException("No link found for persisting: " + resource);
			}
			var json = JsonConvert.SerializeObject(resource);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var result = 
				resource.IsNew ? HttpClient.PostAsync(link.Href, content).Result : HttpClient.PutAsync(link.Href, content).Result;
			var ret = new HalPersistResult<T> {Success = result.IsSuccessStatusCode, HttpStatusCode = (int) result.StatusCode};
			if (result.IsSuccessStatusCode) {
				var settings = new JsonSerializerSettings {Converters = new List<JsonConverter>() {new HalResourceConverter()}};
				JsonConvert.PopulateObject(result.Content.ReadAsStringAsync().Result, resource, settings);
				ret.Resource = resource;
			}
			return ret;
		}

		public bool CanPersist(Type type)
		{
			return type.IsSubclassOf(typeof (HalResource));
		}

		public HalClient HalClient { get; set; }

		public HttpClient HttpClient { get; set; }
		public IHalDeleteResult Delete(IHalResource resource)
		{
			var link = resource.Links.FirstOrDefault(l => l.Rel == "self");
			if (link == null)
				throw new HalPersisterException("No link found for deleting: " + resource);
			var result = HttpClient.DeleteAsync(link.Href).Result;
			return new HalDeleteResult {Success = result.IsSuccessStatusCode};
		}
	}
}
