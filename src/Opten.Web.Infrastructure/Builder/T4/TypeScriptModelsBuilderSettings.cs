namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class TypeScriptModelsBuilderSettings
	{

		public TypeScriptModelsBuilderSettings()
		{
			this.Comments = true;
		}

		public bool Comments { get; set; }

		public string PrefixExport { get; set; }

		public string PrefixEnum { get; set; } = "export enum";

		public string PrefixClass { get; set; } = "export class";

	}

#pragma warning restore 1591

}