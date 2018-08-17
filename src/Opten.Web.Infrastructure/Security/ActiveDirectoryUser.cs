namespace Opten.Web.Infrastructure.Security
{
	/// <summary>
	/// Represents an Active Director (AD) user.
	/// </summary>
	public class ActiveDirectoryUser
	{

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the mail.
		/// </summary>
		/// <value>
		/// The mail.
		/// </value>
		public string Mail { get; set; }

	}
}
