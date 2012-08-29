using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using Ecom.Hal.Attributes;
using Newtonsoft.Json;

namespace HALClient.Tests.Support
{
	class Foo : HalResource
	{
		[JsonProperty("bar")]
		public string Bar { get; set; }
		[HalEmbedded(Rel = "baz", Type = typeof(Baz))]
		public IBaz Baz { get; set; }
	}
}
