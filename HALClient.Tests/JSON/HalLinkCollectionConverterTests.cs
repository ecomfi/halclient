using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using HALClient.Tests.Support;
using NUnit.Framework;
using Newtonsoft.Json;

namespace HALClient.Tests.JSON
{
	[TestFixture]
	class HalLinkCollectionConverterTests
	{

		[Test]
		public void TestWriteJson()
		{
			var model = new Supplier();
			model.Links.Add(new HalLink() { Href = "/foo", Rel = "foo"});
			model.Links.Add(new HalLink() { Href = "/bar", Rel = "bar"});
			var json = JsonConvert.SerializeObject(model);
			Assert.AreEqual(@"{""name"":null,""_links"":{""foo"":{""href"":""/foo""},""bar"":{""href"":""/bar""}}}", json);
		}
	}
}
