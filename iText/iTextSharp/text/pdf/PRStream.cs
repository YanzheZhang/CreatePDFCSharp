using System;
using System.IO;

using iTextSharp.text;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;


/*
 * $Id: PRStream.cs,v 1.2 2003/02/24 20:27:56 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Paulo Soares.
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
	 * a Literal
	 */

	public class PRStream : PdfStream {
    
		protected PdfReader reader;
		protected int offset;
		protected int length;
    
		public PRStream(PdfReader reader, int offset) {
			this.reader = reader;
			this.offset = offset;
		}
    
		public PRStream(PdfReader reader, byte[] conts) {
			this.reader = reader;
			this.offset = -1;
			if (Document.compress) {
				try {
					MemoryStream stream = new MemoryStream();
					DeflaterOutputStream zip = new DeflaterOutputStream(stream);
					zip.Write(conts, 0, conts.Length);
					zip.Close();
					bytes = stream.ToArray();
				}
				catch(IOException ioe) {
					throw ioe;
				}
				put(PdfName.FILTER, PdfName.FLATEDECODE);
			}
			else
				bytes = conts;
			Length = bytes.Length;
		}
    
		public int Offset {
			get {
				return offset;
			}
		}
    
		public override int Length {
			get {
				return length;
			}

			set {
				this.length = value;
				put(PdfName.LENGTH, new PdfNumber(value));
			}
		}
    
		public override int getStreamLength(PdfWriter writer) {
			if (dicBytes == null)
				toPdf(writer);
			return length + dicBytes.Length + SIZESTREAM;
		}
    
		internal override void writeTo(Stream ostr, PdfWriter writer) {
			if (dicBytes == null)
				toPdf(writer);
			ostr.Write(dicBytes, 0, dicBytes.Length);
			ostr.Write(STARTSTREAM, 0, STARTSTREAM.Length);
			if (length > 0) {
				PdfEncryption crypto = writer.Encryption;
				if (offset < 0) {
					if (crypto == null)
						ostr.Write(bytes, 0, bytes.Length);
					else {
						crypto.prepareKey();
						byte[] buf = new byte[length];
						Array.Copy(bytes, 0, buf, 0, length);
						crypto.encryptRC4(buf);
						ostr.Write(buf, 0, buf.Length);
					}
				}
				else {
					byte[] buf = new byte[Math.Min(length, 4092)];
					RandomAccessFileOrArray file = writer.getReaderFile(reader);
					file.seek(offset);
					int size = length;
					if (crypto != null)
						crypto.prepareKey();
					while (size > 0) {
						int r = file.read(buf, 0, Math.Min(size, buf.Length));
						size -= r;
						if (crypto != null)
							crypto.encryptRC4(buf, 0, r);
						ostr.Write(buf, 0, r);
					}
				}
			}
			ostr.Write(ENDSTREAM, 0, ENDSTREAM.Length);
		}
	}
}