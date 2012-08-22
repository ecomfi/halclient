using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ecom.Hal.JSON;
using Newtonsoft.Json;

namespace Ecom.Hal
{
	public class HalClient
	{
		public HalClient(Uri endpoint)
		{
			Client = new HttpClient {BaseAddress = endpoint};
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

		public void SetCredentials(string username, string password)
		{
			Client
				.DefaultRequestHeaders
				.Authorization = new AuthenticationHeaderValue(
					"Basic", 
					Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))));
		}

		protected HttpClient Client { get; set; }
	}
}
