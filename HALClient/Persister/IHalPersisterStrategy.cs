using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Ecom.Hal.Persister
{
	public interface IHalPersisterStrategy
	{
		IHalPersistResult<T> Persist<T>(T resource, HalLink link = null) where T : IHalResource;
		bool CanPersist(Type type);
		HalClient HalClient { get; set; }
		HttpClient HttpClient { get; set; }
	}
}
