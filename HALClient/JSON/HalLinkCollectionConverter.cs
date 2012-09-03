using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecom.Hal.JSON
{
	public class HalLinkCollectionConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var collection = value as HalLinkCollection;
			if (collection != null) {
				writer.WriteStartObject();
				foreach (var link in collection) {
					writer.WritePropertyName(link.Rel);
					writer.WriteStartObject();
					writer.WritePropertyName("href");
					writer.WriteValue(link.Href);
					writer.WriteEndObject();
				}
				writer.WriteEndObject();
			}
			else
				writer.WriteNull();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var ret = new HalLinkCollection();
			var obj = JObject.Load(reader);
			var enumerator = obj.GetEnumerator();
			while (enumerator.MoveNext()) {
				var link = JsonConvert.DeserializeObject<HalLink>(enumerator.Current.Value.ToString());
				link.Rel = enumerator.Current.Key;
				ret.Add(link);
			}
			return ret;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
