using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using Ecom.Hal.Attributes;
using Newtonsoft.Json;

namespace HALClient.Tests.Support
{
	class Product : HalResource
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[HalEmbedded("supplier")]
		public Supplier Supplier { get; set; }

		public int Id { get; set; }
	}
}
