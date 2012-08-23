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
			if (obj.Type == JTokenType.Object) {
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
				return ret;
			}
			if (obj.Type == JTokenType.Array) {
				IEnumerable ret;
				if (objectType == typeof(IEnumerable<>)) {
					ret = Activator.CreateInstance(typeof(List<>).MakeGenericType(objectType.GetGenericArguments())) as IEnumerable;
				}
				else {
					ret = objectType.GetConstructor(new Type[] {}).Invoke(new object[] {}) as IEnumerable;
				}
				foreach (var jObject in ((JArray) obj)) {
					((IList) ret).Add(JsonConvert.DeserializeObject(jObject.ToString(), objectType.GetGenericArguments().FirstOrDefault(),
					                                                new JsonConverter[] {new HalResourceConverter()}));
				}
				return ret;
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
