using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Opten.Web.Infrastructure.Attributes
{
	/// <summary>
	/// Sends the required cache headers to varnish and let the varnish server cache (instead of server side caching).
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class VarnishCacheOutputAttribute : ActionFilterAttribute
	{

		/// <summary> 
		/// Corresponds to MustRevalidate HTTP header - indicates whether the origin server requires revalidation of a cache entry on any subsequent use when the cache entry becomes stale 
		/// </summary> 
		public bool MustRevalidate { get; set; }

		/// <summary> 
		/// Corresponds to CacheControl MaxAge HTTP header (in seconds) 
		/// </summary> 
		public int ClientTimeSpan { get; set; }

		/// <summary> 
		/// Corresponds to CacheControl NoCache HTTP header 
		/// </summary> 
		public bool NoCache { get; set; }

		/// <summary> 
		/// Corresponds to CacheControl Private HTTP header. Response can be cached by browser but not by intermediary cache 
		/// </summary> 
		public bool Private { get; set; }

		/// <summary>
		/// Occurs before the action method is invoked.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <exception cref="System.ArgumentNullException">actionContext</exception>
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (actionContext == null) throw new ArgumentNullException("actionContext");

			base.OnActionExecuting(actionContext);

			ApplyCacheHeaders(response: actionContext.Response);
		}

		/// <summary>
		/// Applies the cache headers.
		/// </summary>
		/// <param name="response">The response.</param>
		protected virtual void ApplyCacheHeaders(HttpResponseMessage response)
		{
			// This is for Varnish resp.http.Varnish-Cache-Output
			response.Headers.Add("Varnish-Cache-Output", "true"); //TODO: Is there a better name?

			if (NoCache)
			{
				response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
				//response.Headers.Add("Pragma", "no-cache");
			}
			else
			{
				response.Headers.CacheControl = new CacheControlHeaderValue
				{
					MaxAge = new TimeSpan(0, 0, ClientTimeSpan),
					MustRevalidate = MustRevalidate,
					Private = Private
				};
			}
		}
	}
}