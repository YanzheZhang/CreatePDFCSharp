using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.util
{
	/// <summary>
	/// Summary description for Properties.
	/// </summary>
	public class Properties
	{
		private NameValueCollection _col;

		public Properties()
		{
			_col = new NameValueCollection();
		}

		public string Remove(string key) {
			string retval = (string)_col[key];
			_col.Remove(key);
			return retval;
		}

		public IEnumerator GetEnumerator() {
			return _col.GetEnumerator();
		}

		public bool ContainsKey(string key) {
			return (_col[key] != null);			
		}

		public void Add(string key, string value) {
			// check for existence
			if (this.ContainsKey(key)) {
				// already exists
				_col[key] = value;		
			} else {
				_col.Add(key, value);
			}			
		}

		public void AddAll(Properties col) {
			foreach(string itm in col) {
				this.Add(itm,col[itm]);
			}
		}

		public int Count {
			get {
				return _col.Count;
			}
		}

		public string this[int index] {
			get {
                return _col[index];
			}

			set {
				throw new Exception("not implemented");
				//_col[index] = value;
			}
		}

		public string this[string key] {
			get {
				return _col[key];
			}

			set {
				this.Add(key, value);
			}
		}

		public ICollection Keys {
			get {
				return _col.Keys;
			}
		}
	}
}
