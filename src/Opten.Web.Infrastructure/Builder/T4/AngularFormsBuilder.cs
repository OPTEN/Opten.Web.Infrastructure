using Opten.Web.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class AngularFormsBuilder : TypeScriptModelsBuilder
	{

		/////////////////////////////////////////////////////////////////////
		//TODO: Do it like the Umbraco SqlSyntax Provider so we can to something like builder.OnParsingValidator((type, args) => { return "Validators.minLength(args)" }) or even more...
		// builder.AddFormValidator(string match, () => return "");

		public AngularFormsBuilder(TypeScriptModel[] models) : base(models, new TypeScriptModelsBuilderSettings { PrefixClass = "public static" }) { }

		//public event TypedEventHandler<AngularFormsBuilder, List<AngularFormValidator>> OnConvertingValidator;
		//public delegate void TypedEventHandler<in TSender, in TArgs>(TSender sender, TArgs args);

		protected override void Build(StringBuilder sb)
		{
			foreach (TypeScriptModel model in Models)
			{
				if (Settings.Comments)
				{
					sb.AppendLine("\t/**");
					sb.AppendLine($"\t * {this.GetDeclarationName(model)}");
					sb.AppendLine("\t */");
				}

				sb.AppendLine($"\t{Settings.PrefixClass} {model.Name}(): FormGroup {{");

				sb.AppendLine("\t\treturn new FormGroup({");

				sb.Append(this.Print(GetGroup(model), 1));

				sb.AppendLine("\t\t});");

				sb.AppendLine("\t}");
				sb.AppendLine();
			}
		}

		protected virtual string Print(AngularFormGroup group, int deep)
		{
			StringBuilder sb = new StringBuilder();

			string indents = "\t\t";
			for (int i = 0; i < deep; i++) { indents += "\t"; }

			bool isLast;
			string validators;
			IAngularControl control;
			AngularAbstractControl abstractControl;
			for (int i = 0; i < group.Controls.Count; i++)
			{
				isLast = i + 1 == group.Controls.Count;
				control = group.Controls[i];

				if (control is AngularAbstractControl)
				{
					abstractControl = control as AngularAbstractControl;
					validators = (abstractControl.Validators.Any()
						? abstractControl.Validators.Count > 1
							? $", [{string.Join(", ", abstractControl.Validators.Select(o => o.Name))}]"
							: $", {abstractControl.Validators[0].Name}"
						: string.Empty);
					sb.AppendLine($"{indents}{control.Name}: new {abstractControl.Control}({abstractControl.Value}{validators}){(isLast ? string.Empty : ",")}");
				}
				else if (control is AngularFormGroup)
				{
					sb.AppendLine($"{indents}{control.Name}: new FormGroup({{");
					sb.Append(this.Print(control as AngularFormGroup, deep + 1));
					sb.AppendLine($"{indents}}}){(isLast ? string.Empty : ",")}");
				}
			}

			return sb.ToString();
		}

		protected virtual AngularFormGroup GetGroup(TypeScriptModel model)
		{
			AngularFormGroup group = new AngularFormGroup(model.Name);

			TypeScriptPropertyDefinition[] definitions = this.GetPropertyDefinitions(model);

			foreach (TypeScriptPropertyDefinition definition in definitions)
			{
				group.Controls.Add(GetControl(definition));
			}

			return group;
		}

		protected virtual IAngularControl GetControl(TypeScriptPropertyDefinition definition)
		{
			AngularFormValidator[] validators = GetValidators(definition);

			if (IsArray(definition.PropertyInfo.PropertyType))
			{
				if (CastArrayAsFormControl(definition))
				{
					return new AngularAbstractControl(definition.Name, "FormControl", "null", validators);
				}
				else
				{
					return new AngularAbstractControl(definition.Name, "FormArray", "[]", validators);
				}
			}
			else if (IsDictionary(definition.PropertyInfo.PropertyType))
			{
				//TODO: Is there a correct type in Angular?
				return new AngularAbstractControl(definition.Name, "FormControl", "null", validators);
			}
			else
			{
				switch (definition.TypeName)
				{
					case "boolean":
						return new AngularAbstractControl(definition.Name, "FormControl", definition.IsRequired ? "false" : "null", validators);
					case "string":
					case "number":
						return new AngularAbstractControl(definition.Name, "FormControl", "null", validators);
					default:
						return GetGroup(new TypeScriptModel(
							definition.Name,
							definition.PropertyInfo.PropertyType));
				}
			}
		}

		protected virtual AngularFormValidator[] GetValidators(TypeScriptPropertyDefinition definition)
		{
			List<AngularFormValidator> validators = new List<AngularFormValidator>();

			IEnumerable<Attribute> attributes = definition.PropertyInfo.GetCustomAttributes<Attribute>(true);

			if (attributes.Any(o => o is RequiredAttribute || o is RequiredIfAttribute))
			{
				validators.Add(new AngularFormValidator("Validators.required"));
			}

			if (attributes.Any(o => o is EmailAddressAttribute))
			{
				validators.Add(new AngularFormValidator("Validators.email"));
			}

			if (attributes.SingleOrDefault(o => o is MinLengthAttribute) is MinLengthAttribute minLength)
			{
				validators.Add(new AngularFormValidator($"Validators.minLength({minLength.Length})"));
			}

			if (attributes.SingleOrDefault(o => o is MaxLengthAttribute) is MaxLengthAttribute maxLength)
			{
				validators.Add(new AngularFormValidator($"Validators.maxLength({maxLength.Length})"));
			}

			//this.OnConvertingValidator(this, validators);

			return validators.ToArray();
		}

		protected override bool IsIgnoredProperty(TypeScriptModel model, PropertyInfo property)
		{
			bool isIgnored = base.IsIgnoredProperty(model, property);

			if (isIgnored == false)
			{
				IEnumerable<Attribute> attributes = property.GetCustomAttributes<Attribute>(true);

				if (attributes.Any(o => o is AngularFormIgnoreAttribute))
				{
					isIgnored = true;
				}
			}

			if (isIgnored == false)
			{
				isIgnored = property.CanWrite == false;
			}

			return isIgnored;
		}

		private bool CastArrayAsFormControl(TypeScriptPropertyDefinition definition)
		{
			KeyValuePair<string, bool> arrayType = GetGenerericTypes(definition.PropertyInfo.PropertyType).First();

			return
				definition.TypeName == "Array<string>" ||
				definition.TypeName == "Array<number>" ||
				definition.TypeName == "Array<boolean>" ||
				arrayType.Key == "string" ||
				arrayType.Key == "number" ||
				arrayType.Key == "boolean";
		}

	}

#pragma warning restore 1591

}