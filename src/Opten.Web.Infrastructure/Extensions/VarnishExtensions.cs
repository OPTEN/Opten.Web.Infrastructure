using System;
using System.Linq;

using Opten.Web.Infrastructure.Cache;

namespace Opten.Common.Extensions // Add it to the global Opten.Common.Extensions
{
	/// <summary>
	/// The Varnish Extensions.
	/// </summary>
	public static class VarnishExtensions
	{

		/// <summary>
		/// Determines whether [is varnish request].
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public static bool IsVarnishRequest(this Uri uri)
		{
			return VarnishCache.Addresses.Contains(uri.Host);
		}

	}
}