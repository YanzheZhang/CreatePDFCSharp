using System;
using System.IO;

using iTextSharp.text;

/**
 * $Id: RtfPageNumber.cs,v 1.3 2003/03/22 21:51:36 geraldhenson Exp $
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
	/// A rtf page number field.
	/// </summary>
	public class RtfPageNumber : Chunk, IRtfField {

		protected static String pageControl = "\\chpgn ";

		public RtfPageNumber( String content, Font contentFont ) : base( content, contentFont ) {
			/* This is a hack, because of the way multiple Chunks with the same Font are handled
			 * by iText Phrases and Paragraphs we have to add the Page Number Control to the content
			 * field. */
			this.content.Append(pageControl);
		}


		public void Write( RtfWriter writer, Stream str ) {

			writer.writeInitialFontSignature( str, this );
			byte[] tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(content.ToString());
			str.Write(tmp, 0, tmp.Length);
			/*        str.write( RtfWriter.escape );
					str.write( pageControl );*/
			writer.writeFinishingFontSignature( str, this );
			//        str.write( RtfWriter.openGroup );
			//            str.write( RtfWriter.escape );
			//            str.write( RtfWriter.field );
			//            str.write( RtfWriter.openGroup );
			//                str.write( RtfWriter.extendedEscape );
			//                str.write( RtfWriter.fieldContent );
			//                str.write( RtfWriter.openGroup );
			//                    str.write( RtfWriter.delimiter );
			//                    str.write( RtfWriter.fieldPage );
			//                    str.write( RtfWriter.delimiter );
			//                str.write( RtfWriter.closeGroup );
			//            str.write( RtfWriter.closeGroup );
			//            str.write( RtfWriter.openGroup );
			//                str.write( RtfWriter.escape );
			//                str.write( RtfWriter.fieldDisplay );
			//                str.write( RtfWriter.openGroup );
			//                str.write( RtfWriter.closeGroup );
			//            str.write( RtfWriter.closeGroup );
			//        str.write( RtfWriter.closeGroup );
		}
	}
}