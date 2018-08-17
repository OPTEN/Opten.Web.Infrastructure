using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Opten.Web.Infrastructure.Builder.T4;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Opten.Web.Infrastructure.Test.TypeScript
{

	[TestFixture]
	public class AngularFormsBuilderTests
	{

		[Test]
		public void Can_Convert()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine("\t/**");
			result.AppendLine("\t * Opten.Web.Infrastructure.Test.TypeScript.Form2Inherited");
			result.AppendLine("\t */");
			result.AppendLine("\tpublic static form2Inherited(): FormGroup {");
			result.AppendLine("\t\treturn new FormGroup({");
			result.AppendLine("\t\t\tmainPerson: new FormGroup({");
			result.AppendLine("\t\t\t\tPhone: new FormControl(null, Validators.required),");
			result.AppendLine("\t\t\t\tCar: new FormGroup({");
			result.AppendLine("\t\t\t\t\tWheels: new FormControl(null)");
			result.AppendLine("\t\t\t\t})");
			result.AppendLine("\t\t\t}),");
			result.AppendLine("\t\t\tbool-nullable: new FormControl(null),");
			result.AppendLine("\t\t\tid: new FormControl(null),");
			result.AppendLine("\t\t\tPersons: new FormArray([]),");
			result.AppendLine("\t\t\tPersonIds: new FormControl(null, Validators.required),");
			result.AppendLine("\t\t\tPersonStringIds: new FormControl(null),");
			result.AppendLine("\t\t\tFormEnums: new FormArray([]),");
			result.AppendLine("\t\t\tformEnums: new FormControl(null),");
			result.AppendLine("\t\t\tDictionary: new FormControl(null)");
			result.AppendLine("\t\t});");
			result.AppendLine("\t}");
			result.AppendLine();

			TypeScriptModel[] models = new[]
			{
				new TypeScriptModel("form2Inherited", typeof(Form2Inherited))
			};

			AngularFormsBuilder builder = new AngularFormsBuilder(
				models: models);

			Assert.AreEqual(result.ToString(), builder.Build().ToString());
		}

	}

	public enum FormEnum { Submitted, Validating }

	public class Form1
	{
		[JsonProperty("firstName")]
		[Required]
		public string FirstName { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[JsonProperty("password")]
		[Required, MinLength(7), MaxLength(35)]
		public string RawPassword { get; set; }

		[JsonProperty("rememberMe")]
		public bool RememberMe { get; set; }

		[JsonProperty("boolNullable")]
		public bool? BoolNullable { get; set; }

		[JsonIgnore]
		public string IAmIgnored { get; set; }

		[AngularFormIgnore]
		public string IAmIgnored2 { get; set; }

		public TimeSpan? Time { get; set; }

		public DateTime? Date { get; set; }

		public string GetterOnly { get; }

	}

	public class Form2
	{

		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonIgnore]
		public virtual FormPerson MainPerson { get; set; }

		public IEnumerable<FormPerson> Persons { get; set; }

		[Required]
		public IEnumerable<int> PersonIds { get; set; }

		public IEnumerable<string> PersonStringIds { get; set; }

		public IEnumerable<FormEnum> FormEnums { get; set; }

		[JsonProperty("formEnums", ItemConverterType = typeof(StringEnumConverter))]
		public FormEnum[] FormEnumsArray { get; set; }

		public IDictionary<string, string> Dictionary { get; set; }

	}

	public class Form2Inherited : Form2
	{

		[JsonProperty("mainPerson")]
		public new FormPerson MainPerson { get; set; }

		[JsonProperty("bool-nullable")]
		public bool? BoolNullable { get; set; }

	}

	public class FormPerson
	{

		[Required]
		public string Phone { get; set; }

		public FormCar Car { get; set; }

	}

	public class FormCar
	{

		public int Wheels { get; set; }

	}
}