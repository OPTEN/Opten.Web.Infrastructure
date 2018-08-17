using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class TypeScriptModelsBuilder
	{

		/////////////////////////////////////////////////////////////////////
		//TODO: Do it like the Umbraco SqlSyntax Provider so we can to something like builder.OnConverting((type) => { return "myTypeName" }) or even more...

		protected readonly TypeScriptModel[] Declarations;
		protected readonly TypeScriptModel[] Models;
		protected readonly TypeScriptModelsBuilderSettings Settings;

		public TypeScriptModelsBuilder(TypeScriptModel[] declarations, TypeScriptModel[] models, TypeScriptModelsBuilderSettings settings)
		{
			Declarations = declarations ?? throw new ArgumentNullException(nameof(declarations));
			Models = models ?? throw new ArgumentNullException(nameof(models));
			Settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		public TypeScriptModelsBuilder(TypeScriptModel[] declarations, TypeScriptModel[] models) : this(declarations, models, new TypeScriptModelsBuilderSettings()) { }

		public TypeScriptModelsBuilder(TypeScriptModel[] models, TypeScriptModelsBuilderSettings settings) : this(models, models, settings) { }

		public TypeScriptModelsBuilder(TypeScriptModel[] models) : this(models, new TypeScriptModelsBuilderSettings()) { }

		public virtual StringBuilder Build()
		{
			StringBuilder sb = new StringBuilder();

			//TODO: Fix indents
			this.Build(sb);

			return sb;
		}

		protected virtual void Build(StringBuilder sb)
		{
			string indent = string.Empty;

			if (string.IsNullOrWhiteSpace(Settings.PrefixExport) == false)
			{
				sb.AppendLine($"{Settings.PrefixExport} {{");
				if (this.Settings.Comments) sb.AppendLine();
				indent = "\t";
			}

			foreach (TypeScriptModel model in Models)
			{
				if (Settings.Comments)
				{
					sb.AppendLine($"{indent}/**");
					sb.AppendLine($"{indent} * {this.GetDeclarationName(model)}");
					sb.AppendLine($"{indent} */");
				}

				if (model.Type.IsEnum)
				{
					sb.AppendLine($"{indent}{Settings.PrefixEnum} {model.Name} {{");

					string[] definitions = this.GetEnumDefinitions(model);

					for (int i = 0; i < definitions.Length; i++)
					{
						sb.AppendLine($"\t{indent}{definitions.GetValue(i)}{(i + 1 < definitions.Length ? "," : string.Empty)}");
					}
				}
				else
				{
					sb.AppendLine($"{indent}{Settings.PrefixClass} {model.Name} {{");

					foreach (TypeScriptPropertyDefinition definition in this.GetPropertyDefinitions(model))
					{
						sb.AppendLine($"\t{indent}{definition.Name}{(definition.IsRequired ? string.Empty : "?")}: {definition.TypeName};");
					}
				}

				sb.AppendLine($"{indent}}}");
				sb.AppendLine();
			}

			if (string.IsNullOrWhiteSpace(Settings.PrefixExport) == false)
			{
				sb.AppendLine("}");
			}
		}

		protected virtual string[] GetEnumDefinitions(TypeScriptModel model)
		{
			if (model.Type.IsEnum == false)
			{
				throw new InvalidCastException($"The type {model.Type.Name} ({model.Name}) is not an Enum.");
			}

			Array values = Enum.GetValues(model.Type);

			List<string> converted = new List<string>();

			for (int i = 0; i < values.Length; i++)
			{
				string value = values.GetValue(i).ToString();

				if (model.Ignore != null && model.Ignore.Any())
				{
					if (model.Ignore.Contains(value) == false)
					{
						converted.Add(value);
					}
				}
				else
				{
					converted.Add(value);
				}
			}

			return converted.ToArray();
		}

		protected virtual TypeScriptPropertyDefinition[] GetPropertyDefinitions(TypeScriptModel model)
		{
			if (model.Type.IsEnum)
			{
				throw new InvalidCastException($"The type {model.Type.Name} ({model.Name}) is an Enum and you tried to handle it as a class.");
			}

			List<TypeScriptPropertyDefinition> definitions = new List<TypeScriptPropertyDefinition>();
			PropertyInfo[] properties = model.Type.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				if (IsIgnoredProperty(model, property)) continue;

				definitions.Add(GetPropertyDefintion(property));
			}

			return definitions.ToArray();
		}

		protected virtual TypeScriptPropertyDefinition GetPropertyDefintion(PropertyInfo property)
		{
			string propertyName = property.Name;
			string typeName = GetTypeName(property.PropertyType, out bool isRequired);

			//HINT: This is a total hack. The problem is when we have different versions of Newtonsoft.Json installed
			// (where the T4 gets builded and here in this solutions) the JsonProperty is always null!
			//TODO: Find a way or the right solution for that or the reason?!
			IEnumerable<Attribute> attributes = property.GetCustomAttributes<Attribute>(true);

			Attribute jsonProperty = attributes.SingleOrDefault(
				o => o.GetType().FullName.Equals(typeof(JsonPropertyAttribute).FullName, StringComparison.OrdinalIgnoreCase));

			//JsonPropertyAttribute jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>(true);
			if (jsonProperty != null)
			{
				//propertyName = jsonProperty.PropertyName;
				propertyName = (string)jsonProperty.GetType().GetProperty("PropertyName").GetValue(jsonProperty);

				if (jsonProperty.GetType().GetProperty("ItemConverterType").GetValue(jsonProperty) is Type itemConverterType)
				{
					if (itemConverterType.FullName.Equals(typeof(StringEnumConverter).FullName, StringComparison.OrdinalIgnoreCase))
					{
						typeName = "Array<string>";
					}
					else
					{
						typeName = $"Array{GetTypeName(itemConverterType, out isRequired)}";
					}
				}
			}

			Attribute jsonConverter = attributes.SingleOrDefault(
				o => o.GetType().FullName.Equals(typeof(JsonConverterAttribute).FullName, StringComparison.OrdinalIgnoreCase));

			//JsonConverterAttribute jsonConverter = property.GetCustomAttribute<JsonConverterAttribute>(true);
			if (jsonConverter != null)
			{
				//Type converterType = jsonConverter.ConverterType;
				Type converterType = (Type)jsonConverter.GetType().GetProperty("ConverterType").GetValue(jsonConverter);

				if (converterType.FullName.Equals(typeof(StringEnumConverter).FullName, StringComparison.OrdinalIgnoreCase))
				{
					typeName = "string";
				}
				else
				{
					typeName = GetTypeName(converterType, out isRequired);
				}
			}

			if (attributes.SingleOrDefault(o => o is TypeScriptPropertyAttribute) is TypeScriptPropertyAttribute typeScriptProperty)
			{
				if (string.IsNullOrWhiteSpace(typeScriptProperty.PropertyName) == false)
				{
					propertyName = typeScriptProperty.PropertyName;
				}

				typeName = typeScriptProperty.PropertyTypeName;
				isRequired = typeScriptProperty.IsRequired;
			}

			return new TypeScriptPropertyDefinition
			{
				Name = propertyName,
				TypeName = typeName,
				PropertyInfo = property,
				IsRequired = isRequired
			};
		}

		protected virtual bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		protected virtual string GetTypeName(Type type, out bool isRequired)
		{
			isRequired = true;

			if (IsArray(type))
			{
				KeyValuePair<string, bool> arrayType = GetGenerericTypes(type).First();
				isRequired = arrayType.Value;

				return $"Array<{arrayType.Key}>";
			}
			else if (IsDictionary(type))
			{
				IEnumerable<KeyValuePair<string, bool>> dictionaryTypes = GetGenerericTypes(type);

				return $"{{ [key: {dictionaryTypes.First().Key}]: {dictionaryTypes.Last().Key} }}";
			}
			else if (IsNullable(type))
			{
				isRequired = false;

				// Is nullable
				return GetTypeName(type.GetGenericArguments()[0], out bool underlyingIsRequired);
			}

			string definedName = GetDefinedType(type, out bool found);

			if (found) return definedName;

			string name = type.Name.ToLowerInvariant();

			switch (name)
			{
				case "guid":
				case "datetime":
				case "timespan":
					return "string";
				case "int16":
				case "int32":
				case "int64":
				case "single":
				case "double":
				case "decimal":
					return "number";
				case "bool":
				case "boolean":
					return "boolean";
				case "void":
				case "string":
					return name;
				default:
					return "any";
			}
		}

		protected virtual bool IsIgnoredProperty(TypeScriptModel model, PropertyInfo property)
		{
			bool isIgnored = false;

			//HINT: This is a total hack. The problem is when we have different versions of Newtonsoft.Json installed
			// (where the T4 gets builded and here in this solutions) the JsonIgnoreAttribute is always null!
			//TODO: Find a way or the right solution for that or the reason?!
			IEnumerable<Attribute> inheritedAttributes = property.GetCustomAttributes<Attribute>(true);
			//IEnumerable<Attribute> ownAttributes = property.GetCustomAttributes<Attribute>(false);

			// https://stackoverflow.com/questions/28674167/how-can-i-un-jsonignore-an-attribute-in-a-derived-class
			// It seems that this is the way how JsonProperty & JsonIgnore works.
			// if you have one and inherit from it it gets ignored when posting...
			// a fix is using the 'new' keyword e.g. new string Property { get; set; } and add the JsonProperty attribute over it

			/*Attribute attribute = ownAttributes.SingleOrDefault(
				o => o.GetType().FullName.Equals(typeof(JsonIgnoreAttribute).FullName, StringComparison.OrdinalIgnoreCase));			
			
			if (attribute == null && ownAttributes.Any(
				o => o.GetType().FullName.Equals(typeof(JsonPropertyAttribute).FullName, StringComparison.OrdinalIgnoreCase)) == false)
			{
				attribute = inheritedAttributes.SingleOrDefault(
				   o => o.GetType().FullName.Equals(typeof(JsonIgnoreAttribute).FullName, StringComparison.OrdinalIgnoreCase));
			}*/

			Attribute attribute = inheritedAttributes.SingleOrDefault(
				o => o.GetType().FullName.Equals(typeof(JsonIgnoreAttribute).FullName, StringComparison.OrdinalIgnoreCase));

			//JsonIgnoreAttribute attribute = property.GetCustomAttribute<JsonIgnoreAttribute>(true);
			if (attribute != null)
			{
				isIgnored = true;
			}

			if (inheritedAttributes.Any(o => o is TypeScriptPropertyAttribute))
			{
				isIgnored = false;
			}

			// If we have an array of ignored we check that.
			if (isIgnored == false && model.Ignore != null && model.Ignore.Any())
			{
				isIgnored = model.Ignore.Contains(property.Name);
			}

			return isIgnored;
		}

		protected virtual string GetDefinedType(Type type, out bool found)
		{
			found = false;

			if (Declarations.Any(o => o.Type == type))
			{
				// find exact type
				found = true;
				return Declarations.Single(o => o.Type == type).Name;
			}
			else if (Declarations.Any(o => o.Name.Equals(type.Name, StringComparison.OrdinalIgnoreCase)))
			{
				// maybe we have something like PersonModel implements IPerson but we only want to render PersonModel
				// so we have to find the interface
				found = true;
				return type.Name;
			}
			else if (Declarations.Any(o => o.Type.GetInterface(type.FullName) != null))
			{
				// this is more like a hack because we search the first model which implements the interface
				found = true;
				return Declarations.First(o => o.Type.GetInterface(type.FullName) != null).Name;
			}

			return null;
		}

		protected IEnumerable<KeyValuePair<string, bool>> GetGenerericTypes(Type propertyType)
		{
			Type[] types = propertyType.GetGenericArguments();

			if (types == null || types.Any() == false)
			{
				types = new Type[] { propertyType.GetElementType() };
			}

			List<KeyValuePair<string, bool>> names = new List<KeyValuePair<string, bool>>();

			foreach (Type type in types)
			{
				string name = GetTypeName(type, out bool isRequired);

				// Maybe it's one we have to convert
				if (name.Equals("any"))
				{
					string definedName = GetDefinedType(type, out bool found);

					if (found)
					{
						name = definedName;
					}
				}

				names.Add(new KeyValuePair<string, bool>(name, isRequired));
			}

			return names;
		}

		protected virtual string GetDeclarationName(TypeScriptModel model)
		{
			//string[] arguments = GetGenericArguments(type);

			/*if (arguments.Any())
			{
				// Remove `1 `2 etc when generic
				name = name.Substring(0, name.IndexOf("`"));

				name += "<" + string.Join(", ", arguments)  + ">";
			}*/

			return $"{model.Type.Namespace}.{model.Type.Name}";
		}

		protected virtual bool IsArray(Type type)
		{
			//TODO: better way?
			return
				type.IsArray ||
				type.FullName.StartsWith("System.Collections.Generic.IEnumerable", StringComparison.OrdinalIgnoreCase) ||
				type.FullName.StartsWith("System.Collections.Generic.List", StringComparison.OrdinalIgnoreCase) ||
				type.FullName.StartsWith("System.Collections.Generic.IList", StringComparison.OrdinalIgnoreCase);
		}

		protected virtual bool IsDictionary(Type type)
		{
			//TODO: better way?
			return
				type.FullName.StartsWith("System.Collections.Generic.Dictionary", StringComparison.OrdinalIgnoreCase) ||
				type.FullName.StartsWith("System.Collections.Generic.IDictionary", StringComparison.OrdinalIgnoreCase);
		}

		/*private string[] GetGenericArguments(Type type)
		{
			Type[] types = type.GetGenericArguments();

			if (types == null || types.Any() == false) return new string[0];

			// Map them to the key IModel<T1, T2, T3>...

			KeyValuePair<string, Type> map = _types.Single(o => o.Value == type);

			if (map.Key.Contains("<") == false || map.Key.Contains(">") == false)
			{
				throw new ArgumentNullException("The type has generic arguments but declaration missing. Please provide a key like: IModel<T1, T2, ...>");
			}

			string mapping = map.Key.Substring(map.Key.IndexOf("<") + 1);
			mapping = mapping.Substring(0, mapping.IndexOf(">"));

			IEnumerable<string> arguments = mapping.Split(new string[] { ",", ", " }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim());

			if (arguments.Count() != types.Count())
			{
				throw new IndexOutOfRangeException(string.Format("The type has generic arguments but only found {0} of {1}", arguments.Count(), types.Count()));
			}

			return arguments.ToArray();
		}*/


	}

#pragma warning restore 1591

}