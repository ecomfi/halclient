using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal;
using Ecom.Hal.JSON;
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
			var settings = new JsonSerializerSettings();
			settings.Converters = new List<JsonConverter>() { new HalResourceConverter()};
			var str = @"{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}}}";
			var model = JsonConvert.DeserializeObject<Product>(str, settings);
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
			var settings = new JsonSerializerSettings();
			settings.Converters = new List<JsonConverter>() { new HalResourceConverter() };
			var str =
				@"{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}},""_embedded"":{""supplier"":{""name"":""Test supplier""}}}";
			var model = JsonConvert.DeserializeObject<Product>(str, settings);
			Assert.IsInstanceOf<Supplier>(model.Supplier);
			Assert.AreEqual("Test supplier", model.Supplier.Name);
		}

		[Test]
		public void TestParseParsesNestedEmbedded()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters = new List<JsonConverter>() { new HalResourceConverter() };
			var str =
				@"{""_embedded"":{""products"":[{""name"":""Foo bar"",""_links"":{""self"":{""href"":""/products/123""}},""_embedded"":{""supplier"":{""name"":""Test supplier""}}}]}}";
			var model = JsonConvert.DeserializeObject<Products>(str, settings);
			Assert.AreEqual(1, model.Items.Count);
			var product = model.Items.First();
			Assert.AreEqual("Foo bar", product.Name);
			Assert.NotNull(product.Supplier);
			Assert.AreEqual("Test supplier", product.Supplier.Name);
		}

		[Test]
		public void TestSupportsNestedInterfaces()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters = new List<JsonConverter>() { new HalResourceConverter() };
			var str = @"{""bar"":""bar1"",""_embedded"":{""baz"":{""quux"":""quux1""}}}";
			var model = JsonConvert.DeserializeObject<Foo>(str, settings);
			Assert.AreEqual("bar1", model.Bar);
			Assert.NotNull(model.Baz);
			Assert.AreEqual("quux1", model.Baz.Quux);
		}
	}
}
