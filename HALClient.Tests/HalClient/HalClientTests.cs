using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Ecom.Hal;
using NUnit.Framework;
using Tavis.UriTemplates;

namespace HALClient.Tests.HalClient
{
	[TestFixture]
	class HalClientTests
	{
		[Test]
		public void TestExpandsUriTemplatesQueryParams()
		{
			var template = "/foo{?bar,baz}";
			var link = new HalLink {Href = template};
			var client = new Ecom.Hal.HalClient(new Uri("http://example.com"));
			var parameters = new NameValueCollection();
			parameters["bar"] = "val1";
			parameters["baz"] = "val2";
			Assert.AreEqual("/foo?bar=val1&baz=val2", client.ResolveTemplate(link, parameters).ToString());
		}

		[Test]
		public void TestExpandsUriTemplatesNoQueryParams()
		{
			var template = "/foo{?bar,baz}";
			var link = new HalLink { Href = template };
			var client = new Ecom.Hal.HalClient(new Uri("http://example.com"));
			var parameters = new NameValueCollection();
			Assert.AreEqual("/foo", client.ResolveTemplate(link, parameters).ToString());
		}

		[Test]
		public void TestExpandsUriTemplates()
		{
			var template = "/foo/{bar}/baz";
			var link = new HalLink {Href = template};
			var client = new Ecom.Hal.HalClient(new Uri("http://example.com"));
			var parameters = new NameValueCollection();
			parameters["bar"] = "bar1";
			Assert.AreEqual("/foo/bar1/baz", client.ResolveTemplate(link, parameters).ToString());
		}
	}
}
