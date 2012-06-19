using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecom.Hal.JSON
{
	class HalLinkCollectionConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
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
