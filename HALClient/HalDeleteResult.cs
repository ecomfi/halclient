using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecom.Hal
{
	public class HalDeleteResult : IHalDeleteResult
	{
		public bool Success { get; set; }
	}

	public interface IHalDeleteResult
	{
		bool Success { get; set; }
	}
}
