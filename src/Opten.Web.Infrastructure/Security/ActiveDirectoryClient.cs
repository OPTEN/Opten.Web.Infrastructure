using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace Opten.Web.Infrastructure.Security
{
	/// <summary>
	/// The Active Directory (AD) Client.
	/// </summary>
	public class ActiveDirectoryClient
	{

		private readonly string _domainName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveDirectoryClient"/> class.
		/// </summary>
		/// <param name="domainName">Name of the domain.</param>
		public ActiveDirectoryClient(string domainName)
		{
			_domainName = domainName;
		}

		/// <summary>
		/// Gets the user by username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <returns></returns>
		public ActiveDirectoryUser Get(string username, out string errorMessage)
		{
			errorMessage = string.Empty;

			try
			{
				using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domainName))
				{
					using (UserPrincipal user = UserPrincipal.FindByIdentity(context, username))
					{
						if (user == null) return null;

						return Map(user);
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;

				return null;
			}
		}

		/// <summary>
		/// Gets all users.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <returns></returns>
		public IEnumerable<ActiveDirectoryUser> GetAll(out string errorMessage)
		{
			errorMessage = string.Empty;

			try
			{
				List<ActiveDirectoryUser> users = new List<ActiveDirectoryUser>();

				using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domainName))
				{
					using (Principal principal = new UserPrincipal(context))
					{
						using (PrincipalSearcher searcher = new PrincipalSearcher(principal))
						{
							foreach (UserPrincipal result in searcher.FindAll())
							{
								users.Add(Map(result));
							}
						}
					}
				}

				return users;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;

				return new ActiveDirectoryUser[0];
			}
		}

		#region Private helpers

		private ActiveDirectoryUser Map(UserPrincipal result)
		{
			return new ActiveDirectoryUser
			{
				Username = result.SamAccountName,
				FirstName = result.Name,
				LastName = result.Surname,
				Mail = result.EmailAddress
			};
		}

		#endregion

	}
}