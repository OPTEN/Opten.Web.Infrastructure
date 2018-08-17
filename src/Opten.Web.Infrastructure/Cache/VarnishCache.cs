using Opten.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Opten.Web.Infrastructure.Cache
{
	/// <summary>
	/// The Varnish Cache Layer (to issue BAN commands).
	/// </summary>
	public class VarnishCache
	{

		#region Properties

		/// <summary>
		/// The address which sends the HttpMethod(BAN).
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		protected readonly Uri BanAddress;
		
		/// <summary>
		/// The sitename of the application. This is when you're using one varnish for more instances.
		/// </summary>
		/// <value>
		/// The sitename.
		/// </value>
		public static string Sitename
		{
			get
			{
				return ConfigurationManager.AppSettings.Get<string>(key: "OPTEN:varnish:sitename");
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="VarnishCache"/> class.
		/// </summary>
		public VarnishCache() : this(ConfigurationManager.AppSettings.Get<string>(key: "OPTEN:varnish:banAddress")) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="VarnishCache"/> class.
		/// </summary>
		/// <param name="banAddress">The ban address.</param>
		/// <exception cref="System.ArgumentNullException">banAddress;The ban address cannot be null when issue BANs!</exception>
		protected VarnishCache(string banAddress)
		{
			if (string.IsNullOrWhiteSpace(banAddress))
			{
				throw new ArgumentNullException("banAddress", "The BAN address cannot be null when issue BANs!");
			}

			this.BanAddress = new Uri(uriString: banAddress, uriKind: UriKind.Absolute);
		}

		#endregion

		/// <summary>
		/// Issue BAN for whole site.
		/// </summary>
		/// <returns></returns>
		public bool BanAll()
		{
			return Execute(
				taskToRun: this.Ban(headers: new Dictionary<string, string> { { "Varnish-Ban-All", "1" } }),
				wait: false);
		}

		/// <summary>
		/// Creates the BAN method task.
		/// </summary>
		/// <param name="headers">The headers.</param>
		/// <returns></returns>
		/// <exception cref="System.NullReferenceException">Missing headers for HttpClient.</exception>
		public async Task<bool> Ban(IDictionary<string, string> headers)
		{
			if (headers == null || headers.Any() == false)
			{
				throw new NullReferenceException("Missing headers for HttpClient.");
			}

			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.BaseAddress = this.BanAddress;

				httpClient.DefaultRequestHeaders.Clear();

				foreach (KeyValuePair<string, string> header in headers)
				{
					httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				HttpMethod method = new HttpMethod("BAN");

				HttpRequestMessage request = new HttpRequestMessage(method, httpClient.BaseAddress);
				
				HttpResponseMessage response = await httpClient.SendAsync(request);

				return response.IsSuccessStatusCode;
			}
		}

		#region Private methods

		private bool Execute<T>(Task<T> taskToRun, bool wait)
		{
			T result = default(T);

			Task task = taskToRun.ContinueWith((response) =>
			{
				result = response.Result;
			});

			if (wait)
			{
				task.Wait();

				return (bool)((object)result);
			}

			return true;
		}

		#endregion

	}
}