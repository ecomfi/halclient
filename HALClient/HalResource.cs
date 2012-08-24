using Ecom.Hal.JSON;
using Newtonsoft.Json;

namespace Ecom.Hal
{
	public interface IHalResource
	{
		HalLinkCollection Links { get; set; }
		bool IsNew { get; set; }
	}

	public abstract class HalResource : IHalResource
	{
		private HalLinkCollection _links = new HalLinkCollection();

		[JsonIgnore]
		public HalLinkCollection Links
		{
			get { return _links; }
			set { _links = value; }
		}

		private bool _isNew = true;

		[JsonIgnore]
		public bool IsNew
		{
			get { return _isNew; }
			set { _isNew = value; }
		}
	}
}