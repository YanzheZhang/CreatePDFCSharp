using System;

using iTextSharp.text;

/**
 * $Id: RtfHeaderFooters.cs,v 1.2 2003/03/20 23:53:41 geraldhenson Exp $
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
 * LGPL license (the �GNU LIBRARY GENERAL PUBLIC LICENSE�), in which case the
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
 * ANY WARRANTY; without right the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.rtf {
	/// <summary>
	/// This HeaderFooter specialization contains some headers or footers for several
	/// pages. Is a list of headerFooters but also a sub class of header footer, to change
	/// as less as possible of the current API.
	/// </summary>
	/// <remarks>
	/// HeaderFooter calls to this class are throw an exception, because the underlying
	/// HeaderFooter hasn't any content.
	/// </remarks>
	public class RtfHeaderFooters : HeaderFooter {

		public const int ALL_PAGES = 0;

		public const int LEFT_PAGES = 1;

		public const int RIGHT_PAGES = 2;

		public const int FIRST_PAGE = 3;
    
		//    public int defaultHeader = ALL_PAGES;

		private HeaderFooter allPages = null;

		private HeaderFooter leftPages = null;

		private HeaderFooter rightPages = null;

		private HeaderFooter firstPage = null;

		public RtfHeaderFooters() : base(new Phrase(""), false ) {}

		public RtfHeaderFooters( Phrase before, Phrase after ) : base(before, after) {}

		public RtfHeaderFooters( Phrase before, bool numbered ) : base(before, numbered) {}

		public void Set(int type, HeaderFooter hf) {
			switch (type) {
				case ALL_PAGES:
					allPages = hf;
					break;
				case LEFT_PAGES:
					leftPages = hf;
					break;
				case RIGHT_PAGES:
					rightPages = hf;
					break;
				case FIRST_PAGE:
					firstPage = hf;
					break;
				default:
					throw new Exception( "unknown type " + type );
			}
		}
    
		public HeaderFooter get( int type ) {
			switch (type) {
				case ALL_PAGES:
					return allPages;
				case LEFT_PAGES:
					return leftPages;
				case RIGHT_PAGES:
					return rightPages;
				case FIRST_PAGE:
					return firstPage;
				default:
					throw new Exception( "unknown type " + type );
			}
		}    
	}
}