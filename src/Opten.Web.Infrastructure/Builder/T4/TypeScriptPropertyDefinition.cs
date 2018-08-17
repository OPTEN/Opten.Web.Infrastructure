using System.Reflection;

namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class TypeScriptPropertyDefinition
	{

		public string Name { get; set; }

		public string TypeName { get; set; }

		public PropertyInfo PropertyInfo { get; set; }

		public bool IsRequired { get; set; }

	}

#pragma warning restore 1591

}