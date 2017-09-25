using System;

/*
 * $Id: PageSize.cs,v 1.2 2003/03/20 03:19:22 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {
	/// <summary>
	/// The PageSize-object contains a number of rectangles representing the most common papersizes.
	/// </summary>
	/// <seealso cref="T:iTextSharp.text.Rectangle"/>
	public class PageSize {
    
		// membervariables
    
		///<summary> This is the letter format </summary>
		public static Rectangle LETTER = new Rectangle(612,792);
    
		///<summary> This is the note format </summary>
		public static Rectangle NOTE = new Rectangle(540,720);
    
		///<summary> This is the legal format </summary>
		public static Rectangle LEGAL = new Rectangle(612,1008);
    
		///<summary> This is the a0 format </summary>
		public static Rectangle A0 = new Rectangle(2380,3368);
    
		///<summary> This is the a1 format </summary>
		public static Rectangle A1 = new Rectangle(1684,2380);
    
		///<summary> This is the a2 format </summary>
		public static Rectangle A2 = new Rectangle(1190,1684);
    
		///<summary> This is the a3 format </summary>
		public static Rectangle A3 = new Rectangle(842,1190);
    
		///<summary> This is the a4 format </summary>
		public static Rectangle A4 = new Rectangle(595,842);
    
		///<summary> This is the a5 format </summary>
		public static Rectangle A5 = new Rectangle(421,595);
    
		///<summary> This is the a6 format </summary>
		public static Rectangle A6 = new Rectangle(297,421);
    
		///<summary> This is the a7 format </summary>
		public static Rectangle A7 = new Rectangle(210,297);
    
		///<summary> This is the a8 format </summary>
		public static Rectangle A8 = new Rectangle(148,210);
    
		///<summary> This is the a9 format </summary>
		public static Rectangle A9 = new Rectangle(105,148);
    
		///<summary> This is the a10 format </summary>
		public static Rectangle A10 = new Rectangle(74,105);
    
		///<summary> This is the b0 format </summary>
		public static Rectangle B0 = new Rectangle(2836,4008);
    
		///<summary> This is the b1 format </summary>
		public static Rectangle B1 = new Rectangle(2004,2836);
    
		///<summary> This is the b2 format </summary>
		public static Rectangle B2 = new Rectangle(1418,2004);
    
		///<summary> This is the b3 format </summary>
		public static Rectangle B3 = new Rectangle(1002,1418);
    
		///<summary> This is the b4 format </summary>
		public static Rectangle B4 = new Rectangle(709,1002);
    
		///<summary> This is the b5 format </summary>
		public static Rectangle B5 = new Rectangle(501,709);
    
		///<summary> This is the archE format </summary>
		public static Rectangle ARCH_E = new Rectangle(2592,3456);
    
		///<summary> This is the archD format </summary>
		public static Rectangle ARCH_D = new Rectangle(1728,2592);
    
		///<summary> This is the archC format </summary>
		public static Rectangle ARCH_C = new Rectangle(1296,1728);
    
		///<summary> This is the archB format </summary>
		public static Rectangle ARCH_B = new Rectangle(864,1296);
    
		///<summary> This is the archA format </summary>
		public static Rectangle ARCH_A = new Rectangle(648,864);
    
		///<summary> This is the flsa format </summary>
		public static Rectangle FLSA = new Rectangle(612,936);
    
		///<summary> This is the flse format </summary>
		public static Rectangle FLSE = new Rectangle(612,936);
    
		///<summary> This is the halfletter format </summary>
		public static Rectangle HALFLETTER = new Rectangle(396,612);
    
		///<summary> This is the 11x17 format </summary>
		public static Rectangle _11X17 = new Rectangle(792,1224);
    
		///<summary> This is the ledger format </summary>
		public static Rectangle LEDGER = new Rectangle(1224,792);
	}
}
