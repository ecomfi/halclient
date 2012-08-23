using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using Ecom.Hal.Attributes;

namespace HALClient.Tests.Support
{
	class Products : HalResource
	{
		[HalEmbedded("products")]
		public List<Product> Items { get; set; }
	}
}
