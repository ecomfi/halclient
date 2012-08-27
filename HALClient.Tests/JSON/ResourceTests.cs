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
			var model = Ecom.Hal.HalClient.Parse<Product>(str);
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
			var model = Ecom.Hal.HalClient.Parse<Product>(str);
			Assert.IsInstanceOf<Supplier>(model.Supplier);
			Assert.AreEqual("Test supplier", model.Supplier.Name);
		}

		[Test]
		public void TestParseParsesNestedEmbedded()
		{
			var str =
				@"{""_embedded"":{""products"":[{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}},""_embedded"":{""supplier"":{""name"":""Test supplier""}}}]}}";
			var model = Ecom.Hal.HalClient.Parse<Products>(str);
			Assert.AreEqual(1, model.Items.Count);
			var product = model.Items.First();
			Assert.AreEqual("Foo bar", product.Name);
			Assert.NotNull(product.Supplier);
			Assert.AreEqual("Test supplier", product.Supplier.Name);
		}
	}
}
