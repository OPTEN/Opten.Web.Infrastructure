using Opten.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Opten.Web.Infrastructure.Mail
{
	/// <summary>
	/// A SMTP mailer (e.g. to send e-mails from web.config settings).
	/// </summary>
	public class SimpleMailer
	{

		#region Private fields

		private readonly string _from;
		private readonly string _displayName;
		private readonly string[] _to;
		private readonly string[] _cc;
		private readonly string[] _bcc;
		private readonly string[] _replyTo;

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets a value indicating whether this instance is body HTML.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is body HTML; otherwise, <c>false</c>.
		/// </value>
		public bool IsBodyHtml { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleMailer" /> class.
		/// </summary>
		/// <param name="from">From address.</param>
		/// <param name="displayName">From display name.</param>
		/// <param name="to">The recipients.</param>
		/// <param name="cc">The CC addresses.</param>
		/// <param name="bcc">The BCC addresses.</param>
		/// <param name="replyTo">The reply to addresses.</param>
		/// <exception cref="System.ArgumentNullException">
		/// form - Please provide a from e-mail-address.
		/// or
		/// displayName - Please provide a display name.
		/// </exception>
		public SimpleMailer(
			string from,
			string displayName,
			string[] to = null,
			string[] cc = null,
			string[] bcc = null,
			string[] replyTo = null)
		{
			if (string.IsNullOrWhiteSpace(from))
				throw new ArgumentNullException("from", "Please provide a from address.");
			if (string.IsNullOrWhiteSpace(displayName))
				throw new ArgumentNullException("displayName", "Please provide a display name.");

			_from = from;
			_displayName = displayName;
			_to = to;
			_cc = cc;
			_bcc = bcc;
			_replyTo = replyTo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleMailer" /> class.
		/// </summary>
		/// <param name="from">From address.</param>
		/// <param name="displayName">From display name.</param>
		/// <param name="to">To recipient.</param>
		/// <param name="cc">The CC addresses.</param>
		/// <param name="bcc">The BCC addresses.</param>
		/// <param name="replyTo">The reply to addresses.</param>
		public SimpleMailer(
			string from,
			string displayName,
			string to,
			string[] cc = null,
			string[] bcc = null,
			string[] replyTo = null)
			: this(from, displayName, new string[] { to }, cc, bcc, replyTo)
		{ }

		#endregion

		/// <summary>
		/// Sends the e-mail from web configuration settings.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			string subject,
			string message)
		{
			this.SendFromWebConfigSettings(
				subject: subject,
				message: message,
				attachments: null);
		}

		/// <summary>
		/// Sends the e-mail from web configuration settings.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="message">The message.</param>
		/// <param name="attachments">The attachments.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(
			string subject,
			string message,
			IEnumerable<Attachment> attachments)
		{
			using (MailMessage mailMessage = new MailMessage())
			{
				// Add from address
				mailMessage.From = new MailAddress(
					address: _from.Trim(),
					displayName: _displayName.Trim());

				// Add recipients
				if (_to != null && _to.Any())
				{
					// It's possilbe to send mails w/o recipients
					foreach (string to in _to.Where(o => string.IsNullOrWhiteSpace(o) == false))
					{
						mailMessage.To.Add(to.Trim());
					}
				}

				// Add cc
				if (_cc != null && _cc.Any())
				{
					foreach (string to in _cc.Where(o => string.IsNullOrWhiteSpace(o) == false))
					{
						mailMessage.CC.Add(to.Trim());
					}
				}

				// Add bcc
				if (_bcc != null && _bcc.Any())
				{
					foreach (string to in _bcc.Where(o => string.IsNullOrWhiteSpace(o) == false))
					{
						mailMessage.Bcc.Add(to.Trim());
					}
				}

				//TODO: Should we check if there is _to, _cc or _bcc?

				// Add reply to addresses
				if (_replyTo != null && _replyTo.Any())
				{
					foreach (string to in _replyTo.Where(o => string.IsNullOrWhiteSpace(o) == false))
					{
						mailMessage.ReplyToList.Add(to.Trim());
					}
				}

				// Add attachments
				if (attachments != null && attachments.Any())
				{
					foreach (Attachment attachment in attachments.Where(o => o != null))
					{
						mailMessage.Attachments.Add(attachment);
					}
				}

				// Set body
				mailMessage.Body = message;
				mailMessage.BodyEncoding = Encoding.UTF8;
				mailMessage.IsBodyHtml = this.IsBodyHtml;

				// Set subject
				mailMessage.Subject = subject.Trim();
				mailMessage.SubjectEncoding = Encoding.UTF8;

				this.SendFromWebConfigSettings(message: mailMessage);
			}
		}

		/// <summary>
		/// Sends the e-mail with web configuration settings.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public void SendFromWebConfigSettings(MailMessage message)
		{
			// Maybe override the settings by the default
			if (message.From == null)
			{
				message.From = new MailAddress(
					address: _from.Trim(),
					displayName: _displayName.Trim());
			}

			// Add recipients
			if (_to != null && (message.To == null || message.To.Any() == false))
			{
				foreach (string to in _to.Where(o => string.IsNullOrWhiteSpace(o) == false))
				{
					message.To.Add(to.Trim());
				}
			}

			// TODO: Add bcc and other stuff?
			using (SmtpClient client = new SmtpClient
			{
				DeliveryFormat = SmtpDeliveryFormat.International // Allow special characters like ó in the mail address
			})
			{
				client.Send(message);
			}
		}

	}
}
