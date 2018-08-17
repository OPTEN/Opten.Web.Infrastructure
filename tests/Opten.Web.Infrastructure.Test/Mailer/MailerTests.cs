using NUnit.Framework;
using System;
using System.IO;

namespace Opten.Web.Infrastructure.Test.Mailer
{
	[TestFixture]
	public class MailerTests
	{

		[Test]
		public void Can_Send_Error_Mail()
		{
			// This only works atm @local Workstation but not on the optweb01...
			bool isTestable = Directory.Exists("D:\\Mails");
			bool canSend = true;

			if (isTestable)
			{
				canSend = new Mail.ErrorMailer(
					from: "test@opten.ch",
					displayName: "Test Error Mailer",
					to: "notifications@opten.ch").TrySendFromWebConfigSettings(new Exception("Test"));
			}

			Assert.That(canSend, Is.True);
		}

	}
}