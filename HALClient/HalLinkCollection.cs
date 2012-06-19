using System.Collections.Generic;
using System.Linq;

namespace Ecom.Hal
{
	public class HalLinkCollection : List<HalLink>
	{
		public HalLink GetLink(string rel)
		{
			return (from link in this where link.Rel == rel select link).FirstOrDefault();
		}
	}
}