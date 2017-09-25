using System;
using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

/*
 * $Id: PdfStream.cs,v 1.1.1.1 2003/02/04 02:57:44 geraldhenson Exp $
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
	 * <CODE>PdfStream</CODE> is the Pdf stream object.
	 * <P>
	 * A stream, like a string, is a sequence of characters. However, an application can
	 * read a small portion of a stream at a time, while a string must be read in its entirety.
	 * For this reason, objects with potentially large amounts of data, such as images and
	 * page descriptions, are represented as streams.<BR>
	 * A stream consists of a dictionary that describes a sequence of characters, followed by
	 * the keyword <B>stream</B>, followed by zero or more lines of characters, followed by
	 * the keyword <B>endstream</B>.<BR>
	 * All streams must be <CODE>PdfIndirectObject</CODE>s. The stream dictionary must be a direct
	 * object. The keyword <B>stream</B> that follows the stream dictionary should be followed by
	 * a carriage return and linefeed or just a linefeed.<BR>
	 * Remark: In this version only the FLATEDECODE-filter is supported.<BR>
	 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
	 * section 4.8 (page 41-53).<BR>
	 *
	 * @see		PdfObject
	 * @see		PdfDictionary
	 */

	public class PdfStream : PdfDictionary {
    
		// membervariables
    
		/** is the stream compressed? */
		protected bool compressed = false;
    
		protected MemoryStream streamBytes = null;
    
		protected byte[] dicBytes = null;
    
		internal static byte[] STARTSTREAM = DocWriter.getISOBytes("\nstream\n");
		internal static byte[] ENDSTREAM = DocWriter.getISOBytes("\nendstream");
		internal static int SIZESTREAM = STARTSTREAM.Length + ENDSTREAM.Length;

		// constructors
    
		/**
		 * Constructs a <CODE>PdfStream</CODE>-object.
		 *
		 * @param		bytes			content of the new <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>.
		 */
 
		public PdfStream(byte[] bytes) : base() {
			type = STREAM;
			this.bytes = bytes;
			put(PdfName.LENGTH, new PdfNumber(bytes.Length));
		}
  
		/**
		 * Constructs a <CODE>PdfStream</CODE>-object.
		 */
    
		protected PdfStream() : base() {
			type = STREAM;
		}
    
		// methods overriding some methods of PdfObject
    
		/**
		 * Returns the PDF representation of this <CODE>PdfObject</CODE> as an array of <CODE>bytes</CODE>s.
		 *
		 * @return		an array of <CODE>byte</CODE>s
		 */
    
		public override byte[] toPdf(PdfWriter writer) {
			dicBytes = base.toPdf(writer);
			return null;
		}
    
		// methods
    
		/**
		 * Compresses the stream.
		 *
		 * @throws PdfException if a filter is allready defined
		 */
    
		public void flateCompress() {
			if (!Document.compress)
				return;
			// check if the flateCompress-method has allready been
			if (compressed) {
				return;
			}
			// check if a filter allready exists
			PdfObject filter = get(PdfName.FILTER);
			if (filter != null) {
				if (filter.isName() && ((PdfName) filter).CompareTo(PdfName.FLATEDECODE) == 0) {
					return;
				}
				else if (filter.isArray() && ((PdfArray) filter).contains(PdfName.FLATEDECODE)) {
					return;
				}
				else {
					throw new PdfException("Stream could not be compressed: filter is not a name or array.");
				}
			}
			try {
				// compress
				MemoryStream stream = new MemoryStream();
				DeflaterOutputStream zip = new DeflaterOutputStream(stream);
				if (streamBytes != null)
					streamBytes.WriteTo(zip);
				else
					zip.Write(bytes, 0, bytes.Length);
				//zip.Close();
				zip.Finish();
				// update the object
				streamBytes = stream;
				bytes = null;
				put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
				if (filter == null) {
					put(PdfName.FILTER, PdfName.FLATEDECODE);
				}
				else {
					PdfArray filters = new PdfArray(filter);
					filters.Add(PdfName.FLATEDECODE);
					put(PdfName.FILTER, filters);
				}
				compressed = true;
			}
			catch(IOException ioe) {
				throw ioe;
			}
		}

		public virtual int getStreamLength(PdfWriter writer) {
			if (dicBytes == null)
				toPdf(writer);
			if (streamBytes != null)
				return (int)(streamBytes.Length + dicBytes.Length + SIZESTREAM);
			else
				return bytes.Length + dicBytes.Length + SIZESTREAM;
		}
    
		internal virtual void writeTo(Stream outstr, PdfWriter writer) {
			if (dicBytes == null)
				toPdf(writer);
			outstr.Write(dicBytes, 0, dicBytes.Length);
			outstr.Write(STARTSTREAM, 0, STARTSTREAM.Length);
			PdfEncryption crypto = writer.Encryption;
			if (crypto == null) {
				if (streamBytes != null)
					streamBytes.WriteTo(outstr);
				else
					outstr.Write(bytes, 0, bytes.Length);
			}
			else {
				crypto.prepareKey();
				byte[] b;
				if (streamBytes != null) {
					b = streamBytes.ToArray();
					crypto.encryptRC4(b);
				}
				else {
					b = new byte[bytes.Length];
					crypto.encryptRC4(bytes, b);
				}
				outstr.Write(b, 0, b.Length);
			}
			outstr.Write(ENDSTREAM, 0, ENDSTREAM.Length);
		}
	}
}
