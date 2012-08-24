using System;
using System.Collections.Generic;
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
	public class HalClient
	{
		public HalClient(Uri endpoint)
		{
			Client = new HttpClient {BaseAddress = endpoint};
			Strategies = new List<IHalPersisterStrategy>();
		}

		public T Parse<T>(string content)
		{
			return JsonConvert.DeserializeObject<T>(content, new JsonConverter[] {new HalResourceConverter()});
		}

		public Task<T> Get<T>(string path)
		{
			return Task<T>
				.Factory
				.StartNew(() =>
				          	{
				          		var body = Client.GetStringAsync(new Uri(path, UriKind.Relative)).Result;
				          		return Parse<T>(body);
				          	});
		}

		public Task<IHalPersistResult<T>> Persist<T>(T resource, HalLink link = null, IHalPersisterStrategy strategy = null) where T : IHalResource
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

		public Task<T> Get<T>(HalLink link)
		{
			return Get<T>(link.Href);
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
