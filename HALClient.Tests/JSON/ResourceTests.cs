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
	class ResourceTests
	{
		[Test]
		public void TestParseParsesLinks()
		{
			var str = @"{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}}}";
			var client = new HalClient(new Uri("http://test"));
			var model = client.Parse<Product>(str);
			Assert.NotNull(model);
			Assert.AreEqual("Foo bar", model.Name);
			Assert.IsInstanceOf<HalLinkCollection>(model.Links);
			Assert.AreEqual(1, model.Links.Count);
			Assert.AreEqual("self", model.Links.First().Rel);
			Assert.AreEqual("/products/123", model.Links.First().Href);
		}

		[Test]
		public void TestParseParsesEmbedded()
		{
			var str =
				@"{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}},""_embedded"":{""supplier"":{""name"":""Test supplier""}}}";
			var client = new HalClient(new Uri("http://test"));
			var model = client.Parse<Product>(str);
			Assert.IsInstanceOf<Supplier>(model.Supplier);
			Assert.AreEqual("Test supplier", model.Supplier.Name);
		}
	}
}
