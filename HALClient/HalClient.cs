using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ecom.Hal.Exception;
using Ecom.Hal.JSON;
using Ecom.Hal.Persister;
using Newtonsoft.Json;
using System.Linq;
using Tavis.UriTemplates;

namespace Ecom.Hal
{
	public interface IHalClient
	{
		Task<IHalPersistResult<T>> Persist<T>(T resource, HalLink link = null, IHalPersisterStrategy strategy = null) where T : class, IHalResource;
		void RegisterPersisterStrategy(IHalPersisterStrategy strategy);
		void SetCredentials(string username, string password);
		Task<T> Get<T>(HalLink link, NameValueCollection parameters) where T : class;
		Task<T> Get<T>(HalLink link, Dictionary<string, object> parameters = null) where T : class;
		Task<IHalDeleteResult> Delete(IHalResource resource, IHalPersisterStrategy strategy = null);
		HttpClient HttpClient { get; }
		T Parse<T>(string content);
	}

	public class HalClient : IHalClient
	{
		public HalClient(Uri endpoint)
		{
			HttpClient = new HttpClient(new HttpClientHandler() {AutomaticDecompression = DecompressionMethods.GZip}) {BaseAddress = endpoint};
			HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			Strategies = new List<IHalPersisterStrategy>();
		}

		public T Parse<T>(string content)
		{
			return JsonConvert.DeserializeObject<T>(content, new JsonConverter[] {new HalResourceConverter()});
		}

		public Task<IHalPersistResult<T>> Persist<T>(T resource, HalLink link = null, IHalPersisterStrategy strategy = null) where T : class, IHalResource
		{
			strategy = strategy ?? GetDefaultPersisterStrategy(resource);
			if (strategy == null)
				throw new HalPersisterException("No persister found for resource: " + resource);
			return Task<IHalPersistResult<T>>
				.Factory
				.StartNew(() => strategy.Persist(resource, link));
		}

		private IHalPersisterStrategy GetDefaultPersisterStrategy(IHalResource resource)
		{
			return Strategies
				.FirstOrDefault(str => str.CanPersist(resource.GetType()));
		}

		public void RegisterPersisterStrategy(IHalPersisterStrategy strategy)
		{
			strategy.HalClient = this;
			strategy.HttpClient = HttpClient;
			Strategies.Add(strategy);
		}

		protected List<IHalPersisterStrategy> Strategies { get; set; }

		public void SetCredentials(string username, string password)
		{
			HttpClient
				.DefaultRequestHeaders
				.Authorization = new AuthenticationHeaderValue(
					"Basic", 
					Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))));
		}

		public Task<T> Get<T>(HalLink link, NameValueCollection parameters) where T : class
		{
			var dict = new Dictionary<string, object>();
			foreach (var key in parameters.AllKeys) {
				dict[key] = parameters[key];
			}
			return Get<T>(link, dict);
		}

		public Task<T> Get<T>(HalLink link, Dictionary<string, object> parameters = null) where T : class
		{
			if (parameters == null)
				parameters = new Dictionary<string, object>();
			return Task<T>
				.Factory
				.StartNew(() =>
				          	{
				          		Uri uri;
				          		uri = link.IsTemplated ? ResolveTemplate(link, parameters) : new Uri(link.Href, UriKind.Relative);
				          		var getResult = HttpClient.GetAsync(uri).Result;
											if (!getResult.IsSuccessStatusCode)
												return null;
				          		var body = getResult.Content.ReadAsStringAsync().Result;
				          		var ret = Parse<T>(body);
											if (ret is IHalResource) {
												((IHalResource) ret).IsNew = false;
											}
				          		return ret;
				          	});
		}

		internal Uri ResolveTemplate(HalLink link, Dictionary<string, object> parameters)
		{
			var template = new UriTemplate(link.Href);
			foreach (var key in parameters.Keys) {
				template.SetParameter(key, parameters[key]);
			}
			return new Uri(template.Resolve(), UriKind.Relative);
		}

		public Task<IHalDeleteResult> Delete(IHalResource resource, IHalPersisterStrategy strategy = null)
		{
			strategy = strategy ?? GetDefaultPersisterStrategy(resource);
			if (strategy == null)
				throw new HalPersisterException("No persister found for resource: " + resource);
			return Task<IHalDeleteResult>
				.Factory
				.StartNew(() => strategy.Delete(resource));
		}

		public HttpClient HttpClient { get; internal set; }
	}
}
