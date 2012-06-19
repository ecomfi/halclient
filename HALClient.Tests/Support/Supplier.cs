using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using Newtonsoft.Json;

namespace HALClient.Tests.Support
{
	class Supplier : HalResource
	{
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
