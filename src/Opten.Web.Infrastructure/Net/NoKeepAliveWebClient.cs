using System;
using System.Net;

namespace Opten.Web.Infrastructure.Net
{
	/// <summary>
	/// A Web Client which doesn't keep the connection alive.
	/// </summary>
	/// <seealso cref="System.Net.WebClient" />
	public class NoKeepAliveWebClient : WebClient
	{

		/// <summary>
		/// Returns a <see cref="T:System.Net.WebRequest" /> object for the specified resource.
		/// </summary>
		/// <param name="address">A <see cref="T:System.Uri" /> that identifies the resource to request.</param>
		/// <returns>
		/// A new <see cref="T:System.Net.WebRequest" /> object for the specified resource.
		/// </returns>
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			if (request is HttpWebRequest)
			{
				// Recommended by SIX Payment
				((HttpWebRequest)request).KeepAlive = false;
			}

			return request;
		}

	}
}