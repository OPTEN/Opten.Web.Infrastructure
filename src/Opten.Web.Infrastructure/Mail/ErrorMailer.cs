using Opten.Common.Extensions;
using Opten.Common.Ip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace Opten.Web.Infrastructure.Mail
{
	/// <summary>
	/// The Error Mailer.
	/// </summary>
	public class ErrorMailer
	{

		#region Private fields

		private readonly Lazy<SimpleMailer> _mailer;
		private readonly string _subject;

		#endregion

		#region Constructors

		/// <summary>
		/// Gets the mail configuration from appSettings.
		/// </summary>
		public ErrorMailer()
			: this(
			from: GetSetting(key: "from"),
			displayName: GetSetting(key: "displayName"),
			to: GetSetting(key: "to")
				.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
		}

		/// <summary>
		/// Overrides the mail configuration from appSettings.
		/// </summary>
		/// <param name="from">The from address.</param>
		/// <param name="displayName">The from display name.</param>
		/// <param name="to">The recipient.</param>
		public ErrorMailer(
			string from,
			string displayName,
			string to)
			: this(from, displayName, new string[] { to })
		{ }

		/// <summary>
		/// Overrides the mail configuration from appSettings.
		/// </summary>
		/// <param name="from">The from address.</param>
		/// <param name="displayName">The from display name.</param>
		/// <param name="to">The recipients.</param>
		public ErrorMailer(
			string from,
			string displayName,
			string[] to)
		{
			if (string.IsNullOrWhiteSpace(from)) throw new ArgumentNullException("from");
			if (to == null || to.Any() == false) throw new ArgumentNullException("to");

			_mailer = new Lazy<SimpleMailer>(() =>
			{
				SimpleMailer mailer = new SimpleMailer(
					from: from,
					displayName: string.IsNullOrWhiteSpace(displayName) ? from : displayName,
					to: to);

				mailer.IsBodyHtml = false;

				return mailer;
			});

			string subject = GetSetting(key: "subject");

			if (string.IsNullOrWhiteSpace(subject))
			{
				subject = "Der Webserver meldet einen Fehler!";
			}

			_subject = subject;
		}

		#endregion

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void SendFromWebConfigSettings(
			Exception exception)
		{
			this.SendFromWebConfigSettings(
				subject: _subject, // otherwise own is called...
				exception: exception);
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="exception">The exception.</param>
		public void SendFromWebConfigSettings(
			string subject,
			Exception exception)
		{
			HttpContextWrapper context = null;
			HttpRequestWrapper request = null;

			//TODO: Is there a better/nicer way?
			if (HttpContext.Current != null)
			{
				context = new HttpContextWrapper(HttpContext.Current);
				request = new HttpRequestWrapper(HttpContext.Current.Request);
			}

			this.SendFromWebConfigSettings(
				subject: subject,
				context: context,
				request: request,
				exception: exception);
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			string subject,
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					subject: subject,
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			HttpContext context,
			HttpRequest request,
			Exception exception)
		{
			this.SendFromWebConfigSettings(
				context: new HttpContextWrapper(context),
				request: new HttpRequestWrapper(request),
				exception: exception);
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			HttpContext context,
			HttpRequest request,
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					context: context,
					request: request,
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			string subject,
			HttpContext context,
			HttpRequest request,
			Exception exception)
		{
			this.SendFromWebConfigSettings(
				context: new HttpContextWrapper(context),
				subject: subject,
				request: new HttpRequestWrapper(request),
				exception: exception);
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			string subject,
			HttpContext context,
			HttpRequest request,
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					subject: subject,
					context: context,
					request: request,
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			HttpContextBase context,
			HttpRequestBase request,
			Exception exception)
		{
			this.SendFromWebConfigSettings(
				context: context,
				subject: _subject,
				request: request,
				exception: exception);
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			HttpContextBase context,
			HttpRequestBase request,
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					context: context,
					request: request,
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		/// <summary>
		/// Sends the error by generating the body from the request and exception.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			string subject,
			HttpContextBase context,
			HttpRequestBase request,
			Exception exception)
		{
			// Check if a custom subject is provided otherwise take from ctor
			if (string.IsNullOrWhiteSpace(subject))
			{
				subject = _subject;
			}

			using (MailMessage mailMessage = new MailMessage())
			{
				_mailer.Value.SendFromWebConfigSettings(
					subject: subject.Trim(),
					message: GetErrorBody(
						context: context,
						request: request,
						exception: exception));
			}
		}

		/// <summary>
		/// Tries to send the error by generating the body from the request and exception
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		/// <param name="exception">The exception.</param>
		/// <returns></returns>
		public bool TrySendFromWebConfigSettings(
			string subject,
			HttpContextBase context,
			HttpRequestBase request,
			Exception exception)
		{
			try
			{
				this.SendFromWebConfigSettings(
					subject: subject,
					context: context,
					request: request,
					exception: exception);

				return true;
			}
			catch
			{
				//TODO: Exception handling?
				return false;
			}
		}

		#region Private methods

		private string GetErrorBody(HttpContextBase context, HttpRequestBase request, Exception exception)
		{
			string username = null;
			bool? isAuthenticated = context?.User?.Identity?.IsAuthenticated;

			if (isAuthenticated.HasValue && isAuthenticated.Value)
			{
				username = context?.User?.Identity?.Name;
			}

			string culture = Thread.CurrentThread?.CurrentCulture?.Name;
			string uiCulture = Thread.CurrentThread?.CurrentUICulture?.Name;

			string currentPageUrl = request?.Url?.AbsoluteUri;
			string referrerPageUrl = request?.UrlReferrer?.AbsoluteUri;

			string userAgent = request?.UserAgent;

			StringBuilder body = new StringBuilder();

			if (exception != null)
			{
				string[] messages = GetExceptionMessages(exception).Reverse().ToArray();

				if (messages.Any())
				{
					body.AppendLine($"Error Message: {messages.First()}");

					foreach (string message in messages.Skip(1))
					{
						body.AppendLine(message);
					}
				}

				if (exception is HttpException)
				{
					body.AppendLine();
					body.AppendLine($"Http code: {(exception as HttpException).GetHttpCode()}");
				}
			}

			body.AppendLine();
			if (string.IsNullOrWhiteSpace(username) == false)
			{
				body.AppendLine($"Username: {username}");
				body.AppendLine();
			}
			if (string.IsNullOrWhiteSpace(culture) == false)
			{
				body.AppendLine($"Thread Culture: {culture}");
			}
			if (string.IsNullOrWhiteSpace(uiCulture) == false)
			{
				body.AppendLine($"Thread UI Culture: {uiCulture}");
			}
			if (string.IsNullOrWhiteSpace(culture) == false || string.IsNullOrWhiteSpace(uiCulture) == false)
			{
				body.AppendLine();
			}
			if (string.IsNullOrWhiteSpace(userAgent) == false)
			{
				body.AppendLine($"User Agent: {userAgent}");
				body.AppendLine();
			}
			body.AppendLine($"IP Address: {IpAddress.GetIpAddress()}");
			body.AppendLine();
			body.AppendLine($"Time: {DateTime.Now.ToString("G")}");
			body.AppendLine();
			if (string.IsNullOrWhiteSpace(currentPageUrl) == false)
			{
				body.AppendLine($"Current Page: {currentPageUrl}");
				body.AppendLine();
			}
			if (string.IsNullOrWhiteSpace(referrerPageUrl) == false)
			{
				body.AppendLine($"Referrer Page: {referrerPageUrl}");
			}

			if (exception != null)
			{
				string[] traces = GetExceptionStackTrace(exception).Reverse().ToArray();

				if (traces.Any())
				{
					body.AppendLine();
					body.AppendLine($"Stack Trace: {traces.First()}");

					foreach (string trace in traces.Skip(1))
					{
						body.AppendLine(trace);
					}
				}
			}

			return body.ToString();
		}

		private string[] GetExceptionMessages(Exception exception)
		{
			List<string> exceptions = new List<string>();

			if (exception != null)
			{
				exceptions.Add(exception.Message);

				if (exception.InnerException != null)
				{
					exceptions.AddRange(GetExceptionMessages(exception.InnerException));
				}
			}

			return exceptions.ToArray();
		}

		private string[] GetExceptionStackTrace(Exception exception)
		{
			List<string> exceptions = new List<string>();

			if (exception != null)
			{
				// Maybe the current doesn't have a stack trace but the inner could!
				if (string.IsNullOrWhiteSpace(exception.StackTrace) == false)
				{
					exceptions.Add(exception.StackTrace);
				}

				if (exception.InnerException != null)
				{
					exceptions.AddRange(GetExceptionStackTrace(exception.InnerException));
				}
			}

			return exceptions.ToArray();
		}

		private static string GetSetting(string key)
		{
			string lookup = "OPTEN:mailer:error:" + key;
			string value = WebConfigurationManager.AppSettings.Get<string>(key: lookup);

			if (string.IsNullOrWhiteSpace(value))
			{
				// try legacy
				lookup = "OPTEN:errorMailer:" + key;
				value = WebConfigurationManager.AppSettings.Get<string>(key: lookup);
			}

			return value;
		}

		#endregion

	}
}