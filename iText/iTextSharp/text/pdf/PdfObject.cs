/*
 * $Id: PdfObject.cs,v 1.1.1.1 2003/02/04 02:57:33 geraldhenson Exp $
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

using System;

namespace iTextSharp.text.pdf {
	/**
	 * <CODE>PdfObject</CODE> is the abstract baseclass of all PDF objects.
	 * <P>
	 * PDF supports seven basic types of objects: bools, numbers, strings, names,
	 * arrays, dictionaries and streams. In addition, PDF provides a null object.
	 * Objects may be labeled so that they can be referred to by other objects.<BR>
	 * All these basic PDF objects are described in the 'Portable Document Format
	 * Reference Manual version 1.3' Chapter 4 (pages 37-54).
	 *
	 * @see		PdfNull
	 * @see		Pdfbool
	 * @see		PdfNumber
	 * @see		PdfString
	 * @see		PdfName
	 * @see		PdfArray
	 * @see		PdfDictionary
	 * @see		PdfStream
	 * @see		PdfIndirectReference
	 */

	public abstract class PdfObject {
    
		// static membervariables (all the possible types of a PdfObject)
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int NULL = 0;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int BOOLEAN = 1;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int NUMBER = 2;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int STRING = 3;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int NAME = 4;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int ARRAY = 5;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int DICTIONARY = 6;
    
		/** a possible type of <CODE>PdfObject</CODE> */
		public const int STREAM = 7;

		public const int INDIRECT = 10;    

		/** This is an empty string used for the <CODE>PdfNull</CODE>-object and for an empty <CODE>PdfString</CODE>-object. */
		public const string NOTHING = "";
    
		/** This is the default encoding to be used for converting strings into bytes and vice versa.
		 * The default encoding is PdfDocEcoding.
		 */
		public const string ENCODING = "PDF";
    
		/** This is the encoding to be used to output text in Unicode. */
		public const string TEXT_UNICODE = "UnicodeBig";
    
		// membervariables
    
		/** the content of this <CODE>PdfObject</CODE> */
		protected byte[] bytes;
    
		/** the type of this <CODE>PdfObject</CODE> */
		protected int type;
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> without any <VAR>content</VAR>.
		 *
		 * @param		type			type of the new <CODE>PdfObject</CODE>
		 */
    
		protected PdfObject(int type) {
			this.type = type;
		}
    
		/**
		 * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
		 *
		 * @param		type			type of the new <CODE>PdfObject</CODE>
		 * @param		content			content of the new <CODE>PdfObject</CODE> as a <CODE>string</CODE>.
		 */
    
		protected PdfObject(int type, string content) {
			this.type = type;
			bytes = PdfEncodings.convertToBytes(content, null);
		}
    
		/**
		 * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
		 *
		 * @param		type			type of the new <CODE>PdfObject</CODE>
		 * @param		bytes			content of the new <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>.
		 */
    
		protected PdfObject(int type, byte[] bytes) {
			this.bytes = bytes;
			this.type = type;
		}
    
		// methods dealing with the content of this object
    
		/**
		 * Returns the PDF representation of this <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>s.
		 *
		 * @return		an array of <CODE>byte</CODE>
		 */
    
		public virtual byte[] toPdf(PdfWriter writer) {
			return bytes;
		}
    
		/**
		 * Returns the length of the PDF representation of the <CODE>PdfObject</CODE>.
		 * <P>
		 * In some cases, namely for <CODE>PdfString</CODE> and <CODE>PdfStream</CODE>,
		 * this method differs from the method <CODE>length</CODE> because <CODE>length</CODE>
		 * returns the length of the actual content of the <CODE>PdfObject</CODE>.</P>
		 * <P>
		 * Remark: the actual content of an object is in most cases identical to its representation.
		 * The following statement is always true: length() &gt;= pdfLength().</P>
		 *
		 * @return		a length
		 */
    
		public int PDFLength {
			get {
				return toPdf(null).Length;
			}
		}
    
		/**
		 * Returns the <CODE>string</CODE>-representation of this <CODE>PdfObject</CODE>.
		 *
		 * @return		a <CODE>string</CODE>
		 */
    
		public override string ToString() {
			return PdfEncodings.convertTostring(bytes, null);
		}
    
		/**
		 * Returns the length of the actual content of the <CODE>PdfObject</CODE>.
		 * <P>
		 * In some cases, namely for <CODE>PdfString</CODE> and <CODE>PdfStream</CODE>,
		 * this method differs from the method <CODE>pdfLength</CODE> because <CODE>pdfLength</CODE>
		 * returns the length of the PDF representation of the object, not of the actual content
		 * as does the method <CODE>length</CODE>.</P>
		 * <P>
		 * Remark: the actual content of an object is in some cases identical to its representation.
		 * The following statement is always true: length() &gt;= pdfLength().</P>
		 *
		 * @return		a length
		 */
    
		public virtual int Length {
			get {
				return this.ToString().Length;
			}

			set {}
		}
    
		/**
		 * Changes the content of this <CODE>PdfObject</CODE>.
		 *
		 * @param		content			the new content of this <CODE>PdfObject</CODE>
		 */
    
		protected string Content {
			set {
				bytes = PdfEncodings.convertToBytes(value, null);
			}
		}
    
		// methods dealing with the type of this object
    
		/**
		 * Returns the type of this <CODE>PdfObject</CODE>.
		 *
		 * @return		a type
		 */
    
		public int Type {
			get {
				return type;
			}
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfNull</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isNull() {
			return (this.Type == NULL);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>Pdfbool</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isBoolean() {
			return (this.Type == BOOLEAN);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfNumber</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isNumber() {
			return (this.type == NUMBER);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfString</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isstring() {
			return (this.type == STRING);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfName</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isName() {
			return (this.type == NAME);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfArray</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isArray() {
			return (this.type == ARRAY);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfDictionary</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isDictionary() {
			return (this.type == DICTIONARY);
		}
    
		/**
		 * Checks if this <CODE>PdfObject</CODE> is of the type <CODE>PdfStream</CODE>.
		 *
		 * @return		<CODE>true</CODE> or <CODE>false</CODE>
		 */
    
		public bool isStream() {
			return (this.type == STREAM);
		}
	}
}
