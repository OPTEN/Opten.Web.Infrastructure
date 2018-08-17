using System;

namespace Opten.Web.Infrastructure.Builder.T4
{

	/// <summary>
	/// A Model which will be converted to d.ts.
	/// </summary>
	public class TypeScriptModel
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeScriptModel" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="ignore">The ignore.</param>
		public TypeScriptModel(string name, Type type, string[] ignore)
		{
			this.Name = name;
			this.Type = type;
			this.Ignore = ignore;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeScriptModel" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public TypeScriptModel(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public Type Type { get; private set; }

		/// <summary>
		/// Gets the ignored properties.
		/// </summary>
		/// <value>
		/// The ignore.
		/// </value>
		public string[] Ignore { get; private set; }

	}
}