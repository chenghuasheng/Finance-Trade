using SmartQuant.Services;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading.Design
{
	internal class ExecutionServiceTypeEditor : ComboBoxTypeEditor
	{
		public ExecutionServiceTypeEditor() : base(true)
		{
		}

		protected override List<KeyValuePair<string, object>> GetItems()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (IService service in ServiceManager.Services)
			{
				if (service is IExecutionService)
				{
					list.Add(new KeyValuePair<string, object>(service.Name, service));
				}
			}
			return list;
		}
	}
}
