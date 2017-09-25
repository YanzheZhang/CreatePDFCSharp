using System;
using System.IO;
using System.Text;

using iTextSharp.text;

/**
 * $Id: RtfTOCEntry.cs,v 1.2 2003/03/22 21:51:36 geraldhenson Exp $
 *
 * Copyright 2002 by 
 * <a href="http://www.smb-tec.com">SMB</a> 
 * <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a>
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
 * LGPL license (the “GNU LIBRARY GENERAL PUBLIC LICENSE”), in which case the
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

namespace iTextSharp.text.rtf {

	/// <summary>
	/// This class can be used to insert entries for a table of contents into the RTF document.
	/// </summary>
	public class RtfTOCEntry : Chunk, IRtfField {


		private bool         hideText = false;

		private bool         hidePageNumber = false;    

		private String    entryName;

		private Font      entryFont;    

		private Font      contentFont;    


		public RtfTOCEntry(String content, Font contentFont) : this(content, contentFont, content, contentFont) {}


		public RtfTOCEntry(String content, Font contentFont, String entryName, Font entryFont) : base(content, contentFont) {
			// hide the text of the entry, because it is printed  
			this.entryName = entryName;
			this.entryFont = entryFont;
			this.contentFont = contentFont;
		}


		public void Write(RtfWriter writer, Stream str) {

			if (!hideText) {
				writer.writeInitialFontSignature(str, new Chunk("", contentFont));
				str.Write(ASCIIEncoding.ASCII.GetBytes(RtfWriter.filterSpecialChar(content.ToString())), 0, 
					ASCIIEncoding.ASCII.GetBytes(RtfWriter.filterSpecialChar(content.ToString())).Length);
				writer.writeFinishingFontSignature(str, new Chunk("", contentFont));
			}

			if (!entryFont.Equals(contentFont)) {
				writer.writeInitialFontSignature(str, new Chunk("", entryFont));
				writeField(str);
				writer.writeFinishingFontSignature(str, new Chunk("", entryFont));
			} else {
				writer.writeInitialFontSignature(str, new Chunk("", contentFont));
				writeField(str);
				writer.writeFinishingFontSignature(str, new Chunk("", contentFont));
			}
		}

    
		private void writeField(Stream str) {
        
			// always hide the toc entry
			str.WriteByte(RtfWriter.openGroup);
			str.WriteByte(RtfWriter.escape);
			str.Write(ASCIIEncoding.ASCII.GetBytes("v"), 0, ASCIIEncoding.ASCII.GetBytes("v").Length);

			// tc field entry
			str.WriteByte(RtfWriter.openGroup);
			str.WriteByte(RtfWriter.escape);
			if (!hidePageNumber) {
				str.Write(ASCIIEncoding.ASCII.GetBytes("tc"), 0, ASCIIEncoding.ASCII.GetBytes("tc").Length);
			} else {
				str.Write(ASCIIEncoding.ASCII.GetBytes("tcn"), 0, ASCIIEncoding.ASCII.GetBytes("tcn").Length);
			}    
			str.WriteByte(RtfWriter.delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes(RtfWriter.filterSpecialChar(entryName)), 0, 
				ASCIIEncoding.ASCII.GetBytes(RtfWriter.filterSpecialChar(entryName)).Length);
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.closeGroup);

			str.WriteByte(RtfWriter.closeGroup);        
		}


		public void HideText() {
			hideText = true;
		}


		public void HidePageNumber() {
			hidePageNumber = true;
		}
	}
}