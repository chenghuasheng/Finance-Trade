using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class RequestList : CollectionBase
	{
		public string this[int index]
		{
			get
			{
				return base.InnerList[index] as string;
			}
		}

		public void Add(string request)
		{
			base.List.Add(request);
		}

		public void Remove(string request)
		{
			if (base.List.Contains(request))
			{
				base.List.Remove(request);
			}
		}

		public bool Contains(string request)
		{
			return base.InnerList.Contains(request);
		}

		protected override void OnInsert(int index, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "The request cannot be null.");
			}
			string text = value as string;
			if (text == null)
			{
				throw new ArgumentException("Invalid value type - " + value.GetType().ToString());
			}
			if (base.InnerList.Contains(text))
			{
				throw new ArgumentException("Duplicate request - " + text);
			}
		}
	}
}
