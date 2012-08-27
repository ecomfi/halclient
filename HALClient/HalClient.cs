using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ecom.Hal.Exception;
using Ecom.Hal.JSON;
using Ecom.Hal.Persister;
using Newtonsoft.Json;
using System.Linq;

namespace Ecom.Hal
{
	public interface IHalClient
	{
		Task<IHalPersistResult<T>> Persist<T>(T resource, HalLink link = null, IHalPersisterStrategy strategy = null) where T : class, IHalResource;
		void RegisterPersisterStrategy(IHalPersisterStrategy strategy);
		void SetCredentials(string username, string password);
		Task<T> Get<T>(HalLink link, NameValueCollection parameters = null) where T : class;
		Task<IHalDeleteResult> Delete(IHalResource resource, IHalPersisterStrategy strategy = null);
	}

	public class HalClient : IHalClient
	{
		public HalClient(Uri endpoint)
		{
			Client = new HttpClient {BaseAddress = endpoint};
			Strategies = new List<IHalPersisterStrategy>();
		}

		public static T Parse<T>(string content)
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
			strategy.HttpClient = Client;
			Strategies.Add(strategy);
		}

		protected List<IHalPersisterStrategy> Strategies { get; set; }

		public void SetCredentials(string username, string password)
		{
			Client
				.DefaultRequestHeaders
				.Authorization = new AuthenticationHeaderValue(
					"Basic", 
					Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))));
		}

		internal HttpClient Client { get; set; }

		public Task<T> Get<T>(HalLink link, NameValueCollection parameters = null) where T : class
		{
			if (parameters == null)
				parameters = new NameValueCollection();
			return Task<T>
				.Factory
				.StartNew(() =>
				          	{
				          		Uri uri;
				          		if (link.IsTemplated) {
				          			var template = new UriTemplate(link.Href);
				          			var baseUri = Client.BaseAddress;
				          			uri = template.BindByName(baseUri, parameters);
				          		}
				          		else {
				          			uri = new Uri(link.Href, UriKind.Relative);
				          		}
				          		var body = Client.GetStringAsync(uri).Result;
				          		return Parse<T>(body);
				          	});
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
	}
}
