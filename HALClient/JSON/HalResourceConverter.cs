using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecom.Hal.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecom.Hal.JSON
{
	class HalResourceConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var obj = JToken.ReadFrom(reader);
			var ret = JsonConvert.DeserializeObject(obj.ToString(), objectType, new JsonConverter[] { });
			if (obj["_embedded"] != null && obj["_embedded"].HasValues) {
				var enumerator = ((JObject) obj["_embedded"]).GetEnumerator();
				while (enumerator.MoveNext()) {
					var rel = enumerator.Current.Key;
					foreach (var property in objectType.GetProperties()) {
						var attribute = property.GetCustomAttributes(true).FirstOrDefault(attr => attr is HalEmbeddedAttribute &&
																																											((HalEmbeddedAttribute) attr).Rel == rel);
						if (attribute != null) {
							property.SetValue(ret,
																JsonConvert.DeserializeObject(enumerator.Current.Value.ToString(), property.PropertyType,
																															new JsonConverter[] {new HalResourceConverter()}), null);
						}
					}
				}
			}
			if (obj["_links"] != null && obj["_links"].HasValues && typeof (IHalResource).IsAssignableFrom(objectType)) {
				((HalResource) ret).Links = JsonConvert.DeserializeObject<HalLinkCollection>(obj["_links"].ToString(),
				                                                                             new JsonConverter[]
				                                                                             	{new HalLinkCollectionConverter()});
			}
			if (ret is IHalResource)
				((IHalResource) ret).IsNew = false;
			return ret;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (IHalResource).IsAssignableFrom(objectType);
		}
	}
}
