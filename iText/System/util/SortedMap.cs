using System;
using System.Collections;

namespace System.util
{
	/// <summary>
	/// Summary description for SortedMap.
	/// </summary>
	public class SortedMap : SortedList
	{
		public SortedMap(IComparer c) : base(c) {}
		
		public override void Add(object key, object value) {
			// check to see if it exists
			if (this.ContainsKey(key)) {
				this[key] = value;
			} else {
				base.Add(key, value);
			}			
		}
	}
}
