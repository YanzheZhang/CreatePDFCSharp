using System;
using System.Text;

/*
 * $Id: PdfDate.cs,v 1.1.1.1 2003/02/04 02:57:13 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.pdf {

	/**
	 * <CODE>PdfDate</CODE> is the PDF date object.
	 * <P>
	 * PDF defines a standard date format. The PDF date format closely follows the format
	 * defined by the international standard ASN.1 (Abstract Syntax Notation One, defined
	 * in CCITT X.208 or ISO/IEC 8824). A date is a <CODE>PdfString</CODE> of the form:
	 * <P><BLOCKQUOTE>
	 * (D: YYYYMMDDHHmmSSOHH'mm')
	 * </BLOCKQUOTE><P>
	 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
	 * section 7.2 (page 183-184)
	 *
	 * @see		PdfString
	 * @see		java.util.GregorianCalendar
	 */

	internal class PdfDate : PdfString {
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfDate</CODE>-object.
		 *
		 * @param		d			the date that has to be turned into a <CODE>PdfDate</CODE>-object
		 */
    
		internal PdfDate(DateTime d) : base() {
			//d = d.ToUniversalTime();
			
			StringBuilder date = new StringBuilder("D:");
			date.Append(setLength(d.Year, 4));
			date.Append(setLength(d.Month, 2));
			date.Append(setLength(d.Day, 2));
			date.Append(setLength(d.Hour, 2));
			date.Append(setLength(d.Minute, 2));
			date.Append(setLength(d.Second, 2));
			string timezone = d.ToString("zzz");
			timezone = timezone.Replace(":", "'");
			date.Append(timezone).Append("'");
			value = date.ToString();
		}
    
		/**
		 * Constructs a <CODE>PdfDate</CODE>-object, representing the current day and time.
		 */
    
		internal PdfDate() : this(DateTime.Now) {}
    
		/**
		 * Adds a number of leading zeros to a given <CODE>string</CODE> in order to get a <CODE>string</CODE>
		 * of a certain length.
		 *
		 * @param		i   		a given number
		 * @param		length		the length of the resulting <CODE>string</CODE>
		 * @return		the resulting <CODE>string</CODE>
		 */
    
		private StringBuilder setLength(int i, int length) {
			StringBuilder tmp = new StringBuilder();
			tmp.Append(i);
			while (tmp.Length < length) {
				tmp.Insert(0, "0");
			}
			tmp.Length = length;
			return tmp;
		}
	}
}