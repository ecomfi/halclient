using Newtonsoft.Json;

namespace Ecom.Hal
{
	public class HalLink
	{
		public string Rel { get; set; }
		[JsonProperty("href")]
		public string Href { get; set; }
		[JsonProperty("templated")]
		public bool IsTemplated { get; set; }
	}
}