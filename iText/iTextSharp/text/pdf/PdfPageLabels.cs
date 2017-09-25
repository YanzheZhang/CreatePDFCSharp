using System;
using System.Collections;
using System.util;

/*
 * $Id: PdfPageLabels.cs,v 1.1.1.1 2003/02/04 02:57:36 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 Paulo Soares
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

	/** Page labels are used to identify each
	 * page visually on the screen or in print.
	 * @author  Paulo Soares (psoares@consiste.pt)
	 */
	public class PdfPageLabels : IComparer {

		/** Logical pages will have the form 1,2,3,...
		 */    
		public static int DECIMAL_ARABIC_NUMERALS = 0;
		/** Logical pages will have the form I,II,III,IV,...
		 */    
		public static int UPPERCASE_ROMAN_NUMERALS = 1;
		/** Logical pages will have the form i,ii,iii,iv,...
		 */    
		public static int LOWERCASE_ROMAN_NUMERALS = 2;
		/** Logical pages will have the form of uppercase letters
		 * (A to Z for the first 26 pages, AA to ZZ for the next 26, and so on)
		 */    
		public static int UPPERCASE_LETTERS = 3;
		/** Logical pages will have the form of uppercase letters
		 * (a to z for the first 26 pages, aa to zz for the next 26, and so on)
		 */    
		public static int LOWERCASE_LETTERS = 4;
		/** No logical page numbers are generated but fixed text may
		 * still exist
		 */    
		public static int EMPTY = 5;
		/** Dictionary values to set the logical page styles
		 */    
		internal static PdfName[] numberingStyle;
		/** The sequence of logical pages. Will contain at least a value for page 1
		 */    
		internal SortedMap map;
    
		static PdfPageLabels() {
			try {
				numberingStyle = new PdfName[]{new PdfName("D"), new PdfName("R"),
												  new PdfName("r"), new PdfName("A"), new PdfName("a")};
			}
			catch (Exception e) {
				e.GetType();
				// empty on purpose
			}
		}
		/** Creates a new PdfPageLabel with a default logical page 1
		 */
		public PdfPageLabels() {
			map = new SortedMap(this);
			addPageLabel(1, DECIMAL_ARABIC_NUMERALS, null, 1);
		}

		/** Compares two <CODE>int</CODE>.
		 * @param obj the first <CODE>int</CODE>
		 * @param obj1 the second <CODE>int</CODE>
		 * @return a negative int, zero, or a positive int as the first argument is less than, equal to, or greater than the second
		 */    
		public int Compare(Object obj, Object obj1) {
			int v1 = (int)obj;
			int v2 = (int)obj1;
			if (v1 < v2)
				return -1;
			if (v1 == v2)
				return 0;
			return 1;
		}
    
		/** Not used
		 * @param obj not used
		 * @return always <CODE>true</CODE>
		 */    
		public new bool Equals(Object obj) {
			return true;
		}
    
		/** Adds or replaces a page label.
		 * @param page the real page to start the numbering. First page is 1
		 * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
		 * @param text the text to prefix the number. Can be <CODE>null</CODE> or empty
		 * @param firstPage the first logical page number
		 */    
		public void addPageLabel(int page, int numberStyle, string text, int firstPage) {
			if (page < 1 || firstPage < 1)
				throw new IllegalArgumentException("In a page label the page numbers must be greater or equal to 1.");
			PdfName pdfName = null;
			if (numberStyle >= 0 && numberStyle < numberingStyle.Length)
				pdfName = numberingStyle[numberStyle];
			int iPage = page;
			Object obj = new Object[]{iPage, pdfName, text, firstPage};
			map.Add(iPage, obj);
		}

		/** Adds or replaces a page label. The first logical page has the default
		 * of 1.
		 * @param page the real page to start the numbering. First page is 1
		 * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
		 * @param text the text to prefix the number. Can be <CODE>null</CODE> or empty
		 */    
		public void addPageLabel(int page, int numberStyle, string text) {
			addPageLabel(page, numberStyle, text, 1);
		}
    
		/** Adds or replaces a page label. There is no text prefix and the first
		 * logical page has the default of 1.
		 * @param page the real page to start the numbering. First page is 1
		 * @param numberStyle the numbering style such as LOWERCASE_ROMAN_NUMERALS
		 */    
		public void addPageLabel(int page, int numberStyle) {
			addPageLabel(page, numberStyle, null, 1);
		}
    
		/** Removes a page label. The first page lagel can not be removed, only changed.
		 * @param page the real page to remove
		 */    
		public void removePageLabel(int page) {
			if (page <= 1)
				return;
			map.RemoveAt(page);
		}

		/** Gets the page label dictionary to insert into the document.
		 * @return the page label dictionary
		 */    
		internal PdfDictionary Dictionary {
			get {
				PdfDictionary dic = new PdfDictionary();
				PdfArray array = new PdfArray();
				foreach(object[] obj in map.Values) {
					PdfDictionary subDic = new PdfDictionary();
					PdfName pName = (PdfName)obj[1];
					if (pName != null)
						subDic.put(PdfName.S, pName);
					string text = (string)obj[2];
					if (text != null)
						subDic.put(PdfName.P, new PdfString(text, PdfObject.TEXT_UNICODE));
					int st = (int)obj[3];
					if (st != 1)
						subDic.put(new PdfName("St"), new PdfNumber(st));
					array.Add(new PdfNumber((int)obj[0] - 1));
					array.Add(subDic);
				}
				dic.put(new PdfName("Nums"), array);
				return dic;
			}
		}
	}
}