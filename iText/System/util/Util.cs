using System;

namespace System.util
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	public class Util
	{
		public static int USR(int op1, int op2) {		
			if (op2 < 1) {
				return op1;
			} else {
				return unchecked((int)((uint)op1 >> op2));
			}
		}

		public static int USR(long op1, int op2) {		
			if (op2 < 1) {
				return (int)op1;
			} else {
				return unchecked((int)((uint)op1 >> op2));
			}
		}    
	}
}
