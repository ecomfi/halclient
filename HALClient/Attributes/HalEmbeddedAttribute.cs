using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecom.Hal.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class HalEmbeddedAttribute : Attribute
	{
		public HalEmbeddedAttribute()
		{
		} 
		public HalEmbeddedAttribute(string rel) : this()
		{
			Rel = rel;
		}
		public string Rel { get; set; }

		public Type Type { get; set; }

		public Type CollectionMemberType { get; set; }
	}
}
