namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class AngularFormValidator
	{

		public AngularFormValidator(string name)
		{
			this.Name = name.Trim();
		}

		public string Name { get; set; }

	}

#pragma warning restore 1591

}