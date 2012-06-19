using Ecom.Hal.JSON;
using Newtonsoft.Json;

namespace Ecom.Hal
{
	public abstract class HalResource
	{
		private HalLinkCollection _links = new HalLinkCollection();

		[JsonProperty("_links")]
		[JsonConverter(typeof(HalLinkCollectionConverter))]
		public HalLinkCollection Links
		{
			get { return _links; }
			set { _links = value; }
		}
	}
}