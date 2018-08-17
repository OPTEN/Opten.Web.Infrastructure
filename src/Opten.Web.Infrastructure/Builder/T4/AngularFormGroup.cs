using System.Collections.Generic;

namespace Opten.Web.Infrastructure.Builder.T4
{

#pragma warning disable 1591

	public class AngularFormGroup : IAngularControl
	{

		public AngularFormGroup(string name)
		{
			this.Name = name.Trim();
			this.Controls = new List<IAngularControl>();
		}

		public string Name { get; set; }

		public List<IAngularControl> Controls { get; set; }

	}

#pragma warning restore 1591

}