using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Opten.Web.Infrastructure.Builder.T4;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opten.Web.Infrastructure.Test.TypeScript
{
	[TestFixture]
	public class TypeScriptModelsBuilderTests
	{

		[Test]
		public void Can_Convert_Definition_Of_Single_Model()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.PersonModel");
			result.AppendLine(" */");
			result.AppendLine("interface IPerson {");
			result.AppendLine("\tName: string;");
			result.AppendLine("\tAge: number;");
			result.AppendLine("\tEnum: any;");
			result.AppendLine("\tAddresses: Array<string>;");
			result.AppendLine("}");
			result.AppendLine();

			TypeScriptModelsBuilder builder = new TypeScriptModelsBuilder(
				models: new[] {
					new TypeScriptModel("IPerson", typeof(PersonModel))
				},
				settings: new TypeScriptModelsBuilderSettings
				{
					PrefixClass = "interface"
				});

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

		[Test]
		public void Can_Convert_Definition_Of_Single_Model_As_Class()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("class Person {");
			result.AppendLine("\tName: string;");
			result.AppendLine("\tAge: number;");
			result.AppendLine("\tEnum: any;");
			result.AppendLine("\tAddresses: Array<string>;");
			result.AppendLine("}");
			result.AppendLine();

			TypeScriptModelsBuilder builder = new TypeScriptModelsBuilder(
				models: new[] {
					new TypeScriptModel("Person", typeof(PersonModel))
				},
				settings: new TypeScriptModelsBuilderSettings
				{
					Comments = false,
					PrefixClass = "class"
				});

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

		[Test]
		public void Can_Convert_Definition_Of_Enum()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("enum MySuperEnum2 {");
			result.AppendLine("\tValue1,");
			result.AppendLine("\tHello");
			result.AppendLine("}");
			result.AppendLine();

			TypeScriptModelsBuilder builder = new TypeScriptModelsBuilder(
				models: new[] {
					new TypeScriptModel("MySuperEnum2", typeof(MySuperEnum), new[] { "Testing" })
				},
				settings: new TypeScriptModelsBuilderSettings
				{
					Comments = false,
					PrefixEnum = "enum"
				});

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

		[Test]
		public void Can_Convert_Definition_Of_Multiple_Models_With_Array()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.DoSomethingModel");
			result.AppendLine(" */");
			result.AppendLine("export class DoSomething {");
			result.AppendLine("}");
			result.AppendLine();
			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.DictionaryModel");
			result.AppendLine(" */");
			result.AppendLine("export class Dictionary {");
			result.AppendLine("}");
			result.AppendLine();
			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.MySuperEnum");
			result.AppendLine(" */");
			result.AppendLine("export enum MySuperEnum {");
			result.AppendLine("\tValue1,");
			result.AppendLine("\tTesting,");
			result.AppendLine("\tHello");
			result.AppendLine("}");
			result.AppendLine();
			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.PersonModel");
			result.AppendLine(" */");
			result.AppendLine("export class IPerson {");
			result.AppendLine("\tName: string;");
			result.AppendLine("\tAge: number;");
			result.AppendLine("\tEnum: MySuperEnum;");
			result.AppendLine("\tAddresses: Array<string>;");
			result.AppendLine("}");
			result.AppendLine();
			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.Car");
			result.AppendLine(" */");
			result.AppendLine("export class ICar {");
			result.AppendLine("\tisCabriolet?: boolean;");
			result.AppendLine("\tNumberOfWheels: number;");
			result.AppendLine("\tAgesOfPassengers: Array<number>;");
			result.AppendLine("\tNamesOfPassengers: Array<string>;");
			result.AppendLine("\tNamesOfPassengers2: Array<string>;");
			result.AppendLine("\tDriver: IPerson;");
			result.AppendLine("\tpass?: Array<string>;");
			result.AppendLine("\tiAmNotIgnoredAnymore: string;");
			result.AppendLine("\tPassengersDictionary: { [key: string]: boolean };");
			result.AppendLine("\tPassengersDictionary2: { [key: string]: boolean };");
			result.AppendLine("\tEnum: MySuperEnum;");
			result.AppendLine("\tStringEnum: string;");
			result.AppendLine("\tstringEnums: Array<string>;");
			result.AppendLine("\tSomething: DoSomething;");
			result.AppendLine("\tSomethingArray: Array<DoSomething>;");
			result.AppendLine("\tSomethingEnumerable: Array<DoSomething>;");
			result.AppendLine("\tDictionary: Array<Dictionary>;");
			result.AppendLine("}");
			result.AppendLine();

			TypeScriptModelsBuilder builder = new TypeScriptModelsBuilder(new[] {
				new TypeScriptModel("DoSomething", typeof(DoSomethingModel)),
				new TypeScriptModel("Dictionary", typeof(DictionaryModel)),
				new TypeScriptModel("MySuperEnum", typeof(MySuperEnum)),
				new TypeScriptModel("IPerson", typeof(PersonModel)),
				new TypeScriptModel("ICar", typeof(Car))
			});

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

		[Test]
		public void Can_Convert_Definition_Of_Models_Only()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("/**");
			result.AppendLine(" * Opten.Web.Infrastructure.Test.TypeScript.PersonModel");
			result.AppendLine(" */");
			result.AppendLine("export class IPerson {");
			result.AppendLine("\tName: string;");
			result.AppendLine("\tAge: number;");
			result.AppendLine("\tEnum: MySuperEnum;");
			result.AppendLine("\tAddresses: Array<string>;");
			result.AppendLine("}");
			result.AppendLine();

			TypeScriptModel[] models = new[]
			{
				new TypeScriptModel("MySuperEnum", typeof(MySuperEnum)),
				new TypeScriptModel("IPerson", typeof(PersonModel))
			};

			TypeScriptModelsBuilder builder = new TypeScriptModelsBuilder(
				declarations: models,
				models: models.Where(o => o.Type.IsEnum == false).ToArray());

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

	}

	public class Car
	{

		[JsonProperty("isCabriolet")]
		public bool? IsCabriolet { get; set; }

		public int NumberOfWheels { get; set; }

		public int[] AgesOfPassengers { get; set; }

		public IEnumerable<string> NamesOfPassengers { get; set; }

		public List<string> NamesOfPassengers2 { get; set; }

		//public IEnumerable<int?> Drivers { get; set; } FIX THIS

		public IPerson Driver { get; set; }

		[TypeScriptProperty(propertyName: "pass", propertyTypeName: "Array<string>", isRequired: false)]
		public IEnumerable<PersonModel> Passengers { get; set; }

		[JsonIgnore]
		public string Ignored { get; set; }

		[JsonIgnore, TypeScriptProperty(propertyName: "iAmNotIgnoredAnymore", propertyTypeName: "string")]
		public string NotIgnored { get; set; }

		public IDictionary<string, bool> PassengersDictionary { get; set; }

		public Dictionary<string, bool> PassengersDictionary2 { get; set; }

		public MySuperEnum Enum { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MySuperEnum StringEnum { get; set; }

		[JsonProperty("stringEnums", ItemConverterType = typeof(StringEnumConverter))]
		public MySuperEnum[] StringEnums { get; set; }

		public IDoSomething Something { get; set; }

		public IDoSomething[] SomethingArray { get; set; }

		public IEnumerable<IDoSomething> SomethingEnumerable { get; set; }

		public IEnumerable<DictionaryModel> Dictionary { get; set; }

	}

	public interface IPerson
	{

		string Name { get; }

	}

	public interface IDoSomething { }

	public class DoSomethingModel : IDoSomething { }

	public class DictionaryModel { }

	public class PersonModel : IPerson
	{

		public string Name { get; set; }

		public int Age { get; set; }

		[JsonConverter(typeof(MySuperEnum))]
		public string Enum { get; set; }

		public IList<string> Addresses { get; set; }

	}

	public enum MySuperEnum
	{
		Value1,
		Testing,
		Hello
	}

	/*public class PaginationModel<TModel>
	{

		public IEnumerable<TModel> Items { get; set; }

		public long CurrentPage { get; set; }

		public long TotalItems { get; set; }

	}*/

}