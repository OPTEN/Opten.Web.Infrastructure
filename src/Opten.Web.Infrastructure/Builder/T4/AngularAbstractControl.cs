using System.Collections.Generic;
using System.Linq;

namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class AngularAbstractControl : IAngularControl
	{

		public AngularAbstractControl(string name, string control, string value) : this(name, control, value, new AngularFormValidator[0])
		{
		}

		public AngularAbstractControl(string name, string control, string value, AngularFormValidator[] validators)
		{
			this.Name = name.Trim();
			this.Control = control.Trim();
			this.Value = value.Trim();
			this.Validators = validators?.ToList();
		}

		public string Name { get; set; }

		public string Control { get; set; }

		public string Value { get; set; }

		public List<AngularFormValidator> Validators { get; set; }

	}

#pragma warning restore 1591

}