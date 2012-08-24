using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecom.Hal.Exception
{
	class HalPersisterException : System.Exception
	{
		public HalPersisterException(string message) : base(message) {}
	}
}
