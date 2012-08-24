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
		public T Persist<T>(T resource, HalLink link = null) where T : HalResource
		{
			link = link ?? resource.Links.FirstOrDefault(l => l.Rel == "self");
			if (link == null) {
				throw new HalPersisterException("No link found for persisting: " + resource);
			}
			var json = JsonConvert.SerializeObject(resource);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var result = 
				resource.IsNew ? HttpClient.PostAsync(link.Href, content).Result : HttpClient.PutAsync(link.Href, content).Result;
			return JsonConvert.DeserializeObject<T>(result.Content.ReadAsStringAsync().Result, new JsonConverter[] { new HalResourceConverter()});
		}

		public bool CanPersist(Type type)
		{
			return type.IsSubclassOf(typeof (HalResource));
		}

		public HalClient HalClient { get; set; }

		public HttpClient HttpClient { get; set; }
	}
}
