using System;
using System.IO;
using System.Text;

using iTextSharp.text;

/**
 * $Id: RtfTOC.cs,v 1.3 2003/03/22 21:51:36 geraldhenson Exp $
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
	/// This class can be used to insert a table of contents into 
	/// the RTF document.
	/// Therefore the field TOC is used. It works great in Word 2000. 
	/// StarOffice doesn't support such fields. Other word version
	/// are not tested yet.
	/// </summary>
	public class RtfTOC : Chunk, IRtfField {


		private String      defaultText = "Klicken Sie mit der rechten Maustaste auf diesen Text, um das Inhaltsverzeichnis zu aktualisieren!";

		private bool     addTOCAsTOCEntry = false;

		private Font        entryFont = null;
		private String      entryName = null;


		/// <summary>
		/// Constructs a RtfTOC object
		/// </summary>
		/// <param name="tocName">the headline of the table of contents</param>
		/// <param name="tocFont">the font for the headline</param>
		public RtfTOC( String tocName, Font tocFont ) : base( tocName, tocFont) {}


		public void Write( RtfWriter writer, Stream str ) {

			writer.writeInitialFontSignature( str, this);
			byte[] tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(RtfWriter.filterSpecialChar(content.ToString()));
			str.Write(tmp, 0, tmp.Length);
			writer.writeFinishingFontSignature( str, this);
        
			if (addTOCAsTOCEntry) {
				RtfTOCEntry entry = new RtfTOCEntry( entryName, entryFont);
				entry.HideText();
				try {
					writer.Add( entry);
				} catch ( DocumentException de ) {
					Console.WriteLine(de.StackTrace);
					throw new RuntimeException( "underlying " + de.Message);
				}
			}

			// line break after headline
			str.WriteByte(RtfWriter.escape);
			str.Write(RtfWriter.paragraph, 0, RtfWriter.paragraph.Length);
			str.WriteByte(RtfWriter.delimiter);

			// toc field entry
			str.WriteByte(RtfWriter.openGroup);
			str.WriteByte(RtfWriter.escape);
			str.Write(RtfWriter.field, 0, RtfWriter.field.Length);
			// field initialization stuff
			str.WriteByte(RtfWriter.openGroup);        
			str.WriteByte(RtfWriter.escape);
			str.Write(RtfWriter.fieldContent, 0, RtfWriter.fieldContent.Length);
			str.WriteByte(RtfWriter.delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes("TOC"), 0, ASCIIEncoding.ASCII.GetBytes("TOC").Length);
			// create the TOC based on the 'toc entries'
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.escape);        
			str.WriteByte(RtfWriter.escape);        
			str.Write(ASCIIEncoding.ASCII.GetBytes("f"), 0, ASCIIEncoding.ASCII.GetBytes("f").Length);
			str.WriteByte(RtfWriter.delimiter);
			// create Hyperlink TOC Entrie 
			str.WriteByte(RtfWriter.escape);        
			str.WriteByte(RtfWriter.escape);        
			str.Write(ASCIIEncoding.ASCII.GetBytes("h"), 0, ASCIIEncoding.ASCII.GetBytes("h").Length);
			str.WriteByte(RtfWriter.delimiter);
			// create the TOC based on the paragraph level
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.escape);        
			str.WriteByte(RtfWriter.escape);        
			str.Write(ASCIIEncoding.ASCII.GetBytes("u"), 0, ASCIIEncoding.ASCII.GetBytes("u").Length);
			str.WriteByte(RtfWriter.delimiter);
			// create the TOC based on the paragraph headlines 1-5
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.escape);        
			str.WriteByte(RtfWriter.escape);        
			str.Write(ASCIIEncoding.ASCII.GetBytes("o"), 0, ASCIIEncoding.ASCII.GetBytes("o").Length);
			str.WriteByte(RtfWriter.delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes("\"1-5\""), 0, ASCIIEncoding.ASCII.GetBytes("\"1-5\"").Length);
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.closeGroup);

			// field default result stuff
			str.WriteByte(RtfWriter.openGroup);        
			str.WriteByte(RtfWriter.escape);
			str.Write(RtfWriter.fieldDisplay, 0, RtfWriter.fieldDisplay.Length);
			str.WriteByte(RtfWriter.delimiter);
			str.Write(ASCIIEncoding.ASCII.GetBytes(defaultText), 0, ASCIIEncoding.ASCII.GetBytes(defaultText).Length);
			str.WriteByte(RtfWriter.delimiter);
			str.WriteByte(RtfWriter.closeGroup);
			str.WriteByte(RtfWriter.closeGroup);
		}

    
		public void AddTOCAsTOCEntry( String entryName, Font entryFont ) {
			this.addTOCAsTOCEntry = true;
			this.entryFont = entryFont;
			this.entryName = entryName;        
		}

    
		public void setDefaultText( String text ) {
			this.defaultText = text;
		}
	}
}