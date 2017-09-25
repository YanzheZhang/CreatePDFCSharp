using System;
using System.Collections;

namespace System.util
{
	/// <summary>
	/// Extends a Hashtable to mimic the functionality of the HashMap java class.
	/// </summary>
	public class Hashmap : Hashtable
	{
		/// <summary>
		/// Mimics a HashMap java object such that if a key
		/// already exists in the table, it's value will be
		/// updated instead of added.
		/// </summary>
		/// <param name="key">the key</param>
		/// <param name="value">new value</param>
		/// <returns>the old value or null</returns>
		public new object Add(object key, object value) {
			object retVal;
			if (this.ContainsKey(key)) {
				retVal = this[key];
				this[key] = value;
				return retVal;
			} else {
				base.Add(key, value);
				return null;
			}		
		}

		/// <summary>
		/// Mimics a HashMap java object such that if 
		/// the key is found, the value is removed 
		/// and returned. If not found a null
		/// is returned
		/// </summary>
		/// <param name="key">the key</param>
		/// <returns>the value or null</returns>
		public new object Remove(object key) {
			object retVal;
			if (this.ContainsKey(key)) {
				retVal = this[key];
				this.Remove(key);
				return retVal;
			} else {
				return null;
			}
		}
	}
}
