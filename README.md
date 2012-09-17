ecomfi/halclient
=========

HALClient is a standalone client consuming HAL powered hypermedia APIs

Usage
-----

### Embedded resources

```cs
class Products : HalResource
{
	[HalEmbedded("products")]
	public List<Product> Items { get; set; }
}

class Product : HalResource
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[HalEmbedded("supplier")]
	public Supplier Supplier { get; set; }

	public int Id { get; set; }
}

class Supplier : HalResource
{
	[JsonProperty("name")]
	public string Name { get; set; }
}


var client = new HalClient("http://exampleapi.com");
client.Get<Products>("/products")
  .ContinueWith(res =>
    {
      var products = res.Result;
      Console.WriteLine("Name of first product is {0}", products.Items.First().Name);
      Console.WriteLine("Supplier of first product is {0}", products.Items.First().Supplier.Name);
    });
```

### Using links

```json
{
  "name": "Test product",
  "_links": {
    "self": {
      "href": "/products/123"
    },
    "supplier": {
      "href": "/suppliers/321"
    }
  }
}
```

```cs
var client = new HalCLient("http://exampleapi.com");
//Product is previously fetched
client.Get<Supplier>(product.Links.GetLink("supplier")).ContinueWith(res => { ...}); //res.Result contains the Supplier
```

