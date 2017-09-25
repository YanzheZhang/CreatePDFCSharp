using System;
using System.IO;

using iTextSharp.text;

/*
 * $Id: PdfIndirectObject.cs,v 1.1.1.1 2003/02/04 02:57:29 geraldhenson Exp $
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
	 * <CODE>PdfIndirectObject</CODE> is the Pdf indirect object.
	 * <P>
	 * An <I>indirect object</I> is an object that has been labeled so that it can be referenced by
	 * other objects. Any type of <CODE>PdfObject</CODE> may be labeled as an indirect object.<BR>
	 * An indirect object consists of an object identifier, a direct object, and the <B>endobj</B>
	 * keyword. The <I>object identifier</I> consists of an int <I>object number</I>, an int
	 * <I>generation number</I>, and the <B>obj</B> keyword.<BR>
	 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
	 * section 4.10 (page 53).
	 *
	 * @see		PdfObject
	 * @see		PdfIndirectReference
	 */

	internal class PdfIndirectObject {
    
		// membervariables
    
		/** The object number */
		protected int number;
    
		/** the generation number */
		protected int generation = 0;
    
		/** The object type */
		protected int type;
    
		/** The object ready to stream out */
		protected MemoryStream bytes;
    
		internal static byte[] STARTOBJ = DocWriter.getISOBytes(" obj\n");
		internal static byte[] ENDOBJ = DocWriter.getISOBytes("\nendobj\n");
		internal static int SIZEOBJ = STARTOBJ.Length + ENDOBJ.Length;
		bool isStream = false;
		PdfStream stream;
		PdfWriter writer;
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfIndirectObject</CODE>.
		 *
		 * @param		number			the object number
		 * @param		object			the direct object
		 */
    
		internal PdfIndirectObject(int number, PdfObject obj, PdfWriter writer) : this(number, 0, obj, writer) {}
    
		/**
		 * Constructs a <CODE>PdfIndirectObject</CODE>.
		 *
		 * @param		number			the object number
		 * @param		generation		the generation number
		 * @param		object			the direct object
		 */
    
		internal PdfIndirectObject(int number, int generation, PdfObject obj, PdfWriter writer) {
			this.writer = writer;
			this.number = number;
			this.generation = generation;
			type = obj.Type;
			isStream = (obj.Type == PdfObject.STREAM);
			PdfEncryption crypto = writer.Encryption;
			if (crypto != null) {
				crypto.setHashKey(number, generation);
			}
			try {
				bytes = new MemoryStream();
				byte[] tmp = DocWriter.getISOBytes(number.ToString());
				bytes.Write(tmp, 0, tmp.Length);
				bytes.WriteByte((byte)32);
				tmp = DocWriter.getISOBytes(generation.ToString());
				bytes.Write(tmp, 0, tmp.Length);
				if (!isStream) {
					bytes.Write(STARTOBJ, 0, STARTOBJ.Length);
					tmp = obj.toPdf(writer); 
					bytes.Write(tmp, 0, tmp.Length);
					bytes.Write(ENDOBJ, 0, ENDOBJ.Length);
				}
				else
					stream = (PdfStream)obj;
			}
			catch (IOException ioe) {
				throw ioe;
			}
		}
    
		// methods
    
		/**
		 * Return the length of this <CODE>PdfIndirectObject</CODE>.
		 *
		 * @return		the length of the PDF-representation of this indirect object.
		 */
    
		public int Length {
			get {
				if (isStream)
					return (int)(bytes.Length + SIZEOBJ + stream.getStreamLength(writer));
				else
					return (int)bytes.Length;
			}
		}
    
    
		/**
		 * Returns a <CODE>PdfIndirectReference</CODE> to this <CODE>PdfIndirectObject</CODE>.
		 *
		 * @return		a <CODE>PdfIndirectReference</CODE>
		 */
    
		internal PdfIndirectReference IndirectReference {
			get {
				return new PdfIndirectReference(type, number, generation);
			}
		}
    
		/**
		 * Writes eficiently to a stream
		 *
		 * @param out the stream to write to
		 * @throws IOException on write error
		 */
		internal void writeTo(Stream ostr) {
			bytes.WriteTo(ostr);
			if (isStream) {
				ostr.Write(STARTOBJ, 0, STARTOBJ.Length);
				stream.writeTo(ostr, writer);
				ostr.Write(ENDOBJ, 0, ENDOBJ.Length);
			}
		}
	}
}