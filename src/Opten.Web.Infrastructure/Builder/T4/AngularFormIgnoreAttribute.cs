using System;

namespace Opten.Web.Infrastructure.Builder.T4
{
	/// <summary>
	/// Property to generate .d.ts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class AngularFormIgnoreAttribute : Attribute
	{
	}
}