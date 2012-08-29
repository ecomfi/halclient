using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace HALClient.Tests.Support
{
	class Baz : IBaz
	{
		[JsonProperty("quux")]
		public string Quux { get; set; }
	}

	internal interface IBaz
	{
		string Quux { get; set; }
	}
}
