using System;
using System.Collections;

namespace System.util {
	/// <summary>
	/// Summary description for ListIterator.
	/// </summary>
	public class ListIterator {
		ArrayList col;
		ArrayList temp;
		int curPos = -1;
		object curItem;

		public ListIterator(ArrayList col) {
			this.col = col;
			temp = new ArrayList(col);
		}

		public bool hasNext {
			get {
				if (curPos == -1) {
					return (temp.Count != 0);
				} else {
					return (temp.Count > curPos + 1);
				}
			}
		}

		public object Next {
			get {
				curPos++;
				if (curPos > temp.Count) {
					throw new System.ArgumentOutOfRangeException();
				}
				curItem = this.temp[curPos];
				return curItem;
			}
		}

		public object Previous {
			get {
				if (curPos == -1) {
					throw new System.ArgumentOutOfRangeException();
				}
				return this.temp[curPos--];
			}
		}

		public void Remove() {
			this.col.Remove(curItem);
		}
	}
}
