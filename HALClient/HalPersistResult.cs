using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecom.Hal
{
	public class HalPersistResult<T> : IHalPersistResult<T>
	{
		public bool Success { get; set; }

		public int HttpStatusCode { get; set; }

		public T Resource { get; set; }
	}

	public interface IHalPersistResult<T>
	{
		bool Success { get; set; }
		int HttpStatusCode { get; set; }
		T Resource { get; }
	}
}
