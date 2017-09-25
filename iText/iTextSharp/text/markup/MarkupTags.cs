using System;

/*
 * $Id: MarkupTags.cs,v 1.2 2003/04/27 02:22:03 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text.markup {
	/// <summary>
	/// A class that contains all the possible tagnames and their attributes.
	/// </summary>
	public class MarkupTags {

		/// <summary> a CSS value </summary>
		public static string CSS_NORMAL = "normal";
		/// <summary> a CSS value </summary>
		public static string CSS_NONE = "none";
    
		/// <summary> This is a possible HTML-tag. </summary>
		public static string LINK = "link";
    
		/// <summary> This is a possible HTML attribute for the LINK tag. </summary>
		public static string CSS = "text/css";

		/// <summary> This is a possible value for the language attribute (SCRIPT tag). </summary>
		public static string JAVASCRIPT = "text/javascript";

		/// <summary> attribute for specifying externally defined CSS class </summary>
		public static string CLASS = "class";
    
		/// <summary> This is a possible HTML attribute for the LINK tag. </summary>
		public static string REL = "rel";
    
		/// <summary> This is a possible HTML attribute for the TD tag. </summary>
		public static string STYLESHEET = "stylesheet";

		/// <summary> This is used for inline css style information </summary>
		public static string STYLE = "style";
    
		/// <summary> This is a possible HTML attribute for the LINK tag. </summary>
		public static string TYPE = "type";

		/// <summary> The SPAN tag. </summary>
		public static string SPAN = "span";
    
		/// <summary> The DIV tag. </summary>
		public static string DIV = "div";

		/// <summary> the CSS tag for the font size </summary>
		public static string CSS_FONT = "font";

		/// <summary> the CSS tag for the font size </summary>
		public static string CSS_FONTSIZE = "font-size";

		/// <summary> the CSS tag for the font style </summary>
		public static string CSS_FONTSTYLE = "font-style";

		/// <summary> a CSS value for text font style </summary>
		public static string CSS_ITALIC = "italic";

		/// <summary> a CSS value for text font style </summary>
		public static string CSS_OBLIQUE = "oblique";

		/// <summary> the CSS tag for the font weight </summary>
		public static string CSS_FONTWEIGHT = "font-weight";

		/// <summary> a CSS value for text font weight </summary>
		public static string CSS_BOLD = "bold";
    
		/// <summary> the CSS tag for the font family </summary>
		public static string CSS_FONTFAMILY = "font-family";

		/// <summary> the CSS tag for text decorations </summary>
		public static string CSS_TEXTDECORATION = "text-decoration";

		/// <summary> the CSS tag for text decorations </summary>
		public static string CSS_LINEHEIGHT = "line-height";
    
		/// <summary> the CSS tag for text decorations </summary>
		public static string CSS_VERTICALALIGN = "vertical-align";

		/// <summary> a CSS value for text decoration </summary>
		public static string CSS_UNDERLINE = "underline";

		/// <summary> a CSS value for text decoration </summary>
		public static string CSS_LINETHROUGH = "line-through";

		/// <summary> the CSS tag for text color </summary>
		public static string CSS_COLOR = "color";

		/// <summary> the CSS tag for background color </summary>
		public static string CSS_BGCOLOR = "background-color";
 
		/// <summary> the CSS tag for adding a page break when the document is printed </summary>
		public static string PAGE_BREAK_BEFORE = "page-break-before";
 
		/// <summary> value for the CSS tag for adding a page break when the document is printed </summary>
		public static string ALWAYS = "always";
	}
}
