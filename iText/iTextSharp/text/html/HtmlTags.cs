using System;

/*
 * $Id: HtmlTags.java,v 1.35 2002/09/26 08:03:44 blowagie Exp $
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

namespace iTextSharp.text.html {

	/**
	 * A class that contains all the possible tagnames and their attributes.
	 */

	public class HtmlTags {
    
		/** the root tag. */
		public static string HTML = "html";
    
		/** the head tag */
		public static string HEAD = "head";
    
		/** This is a possible HTML attribute for the HEAD tag. */
		public static string CONTENT = "content";
    
		/** the meta tag */
		public static string META = "meta";
    
		/** attribute of the root tag */
		public static string SUBJECT = "subject";
    
		/** attribute of the root tag */
		public static string KEYWORDS = "keywords";
    
		/** attribute of the root tag */
		public static string AUTHOR = "author";
    
		/** the title tag. */
		public static string TITLE = "title";
    
		/** the script tag. */
		public static string SCRIPT = "script";

		/** This is a possible HTML attribute for the SCRIPT tag. */
		public static string LANGUAGE = "language";

		/** This is a possible value for the LANGUAGE attribute. */
		public static string JAVASCRIPT = "JavaScript";

		/** the body tag. */
		public static string BODY = "body";
    
		/** This is a possible HTML attribute for the BODY tag */
		public static string JAVASCRIPT_ONLOAD = "onLoad";

		/** This is a possible HTML attribute for the BODY tag */
		public static string JAVASCRIPT_ONUNLOAD = "onUnLoad";

		/** This is a possible HTML attribute for the BODY tag. */
		public static string TOPMARGIN = "topmargin";
    
		/** This is a possible HTML attribute for the BODY tag. */
		public static string BOTTOMMARGIN = "bottommargin";
    
		/** This is a possible HTML attribute for the BODY tag. */
		public static string LEFTMARGIN = "leftmargin";
    
		/** This is a possible HTML attribute for the BODY tag. */
		public static string RIGHTMARGIN = "rightmargin";
    
		// Phrases, Anchors, Lists and Paragraphs
    
		/** the chunk tag */
		public static string CHUNK = "font";
    
		/** the phrase tag */
		public static string CODE = "code";
    
		/** the phrase tag */
		public static string VAR = "var";
    
		/** the anchor tag */
		public static string ANCHOR = "a";
    
		/** the list tag */
		public static string ORDEREDLIST = "ol";
    
		/** the list tag */
		public static string UNORDEREDLIST = "ul";
    
		/** the listitem tag */
		public static string LISTITEM = "li";
    
		/** the paragraph tag */
		public static string PARAGRAPH = "p";
    
		/** attribute of anchor tag */
		public static string NAME = "name";
    
		/** attribute of anchor tag */
		public static string REFERENCE = "href";
    
		/** attribute of anchor tag */
		public static string[] H = new string[6];
		static HtmlTags() {
			H[0] = "h1";
			H[1] = "h2";
			H[2] = "h3";
			H[3] = "h4";
			H[4] = "h5";
			H[5] = "h6";
		}
    
		// Chunks
    
		/** attribute of the chunk tag */
		public static string FONT = "face";
    
		/** attribute of the chunk tag */
		public static string SIZE = "point-size";
    
		/** attribute of the chunk/table/cell tag */
		public static string COLOR = "color";
    
		/** some phrase tag */
		public static string EM = "em";
    
		/** some phrase tag */
		public static string I = "i";
    
		/** some phrase tag */
		public static string STRONG = "strong";
    
		/** some phrase tag */
		public static string B = "b";
    
		/** some phrase tag */
		public static string S = "s";
    
		/** some phrase tag */
		public static string U = "u";
    
		/** some phrase tag */
		public static string SUB = "sub";
    
		/** some phrase tag */
		public static string SUP = "sup";
    
		/** the possible value of a tag */
		public static string HORIZONTALRULE = "hr";
    
		// tables/cells
    
		/** the table tag */
		public static string TABLE = "table";
    
		/** the cell tag */
		public static string ROW = "tr";
    
		/** the cell tag */
		public static string CELL = "td";
    
		/** attribute of the cell tag */
		public static string HEADERCELL = "th";
    
		/** attribute of the table tag */
		public static string COLUMNS = "cols";
    
		/** attribute of the table tag */
		public static string CELLPADDING = "cellpadding";
    
		/** attribute of the table tag */
		public static string CELLSPACING = "cellspacing";
    
		/** attribute of the cell tag */
		public static string COLSPAN = "colspan";
    
		/** attribute of the cell tag */
		public static string ROWSPAN = "rowspan";
    
		/** attribute of the cell tag */
		public static string NOWRAP = "nowrap";
    
		/** attribute of the table/cell tag */
		public static string BORDERWIDTH = "border";
    
		/** attribute of the table/cell tag */
		public static string WIDTH = "width";
    
		/** attribute of the table/cell tag */
		public static string BACKGROUNDCOLOR = "bgcolor";
    
		/** attribute of the table/cell tag */
		public static string BORDERCOLOR = "bordercolor";
    
		/** attribute of paragraph/image/table tag */
		public static string ALIGN = "align";
    
		/** attribute of chapter/section/paragraph/table/cell tag */
		public static string LEFT = "left";
    
		/** attribute of chapter/section/paragraph/table/cell tag */
		public static string RIGHT = "right";
    
		/** attribute of the cell tag */
		public static string HORIZONTALALIGN = "align";
    
		/** attribute of the cell tag */
		public static string VERTICALALIGN = "valign";
    
		/** attribute of the table/cell tag */
		public static string TOP = "top";
    
		/** attribute of the table/cell tag */
		public static string BOTTOM = "bottom";
    
		// Misc
    
		/** the image tag */
		public static string IMAGE = "img";
    
		/** attribute of the image tag */
		public static string URL = "src";
    
		/** attribute of the image tag */
		public static string ALT = "alt";
    
		/** attribute of the image tag */
		public static string PLAINWIDTH = "width";
    
		/** attribute of the image tag */
		public static string PLAINHEIGHT = "height";
    
		/** the newpage tag */
		public static string NEWLINE = "br";
    
		// alignment attribute values
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_LEFT = "Left";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_CENTER = "Center";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_RIGHT = "Right";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_JUSTIFIED = "Justify";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_TOP = "Top";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_MIDDLE = "Middle";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_BOTTOM = "Bottom";
    
		/** the possible value of an alignment attribute */
		public static string ALIGN_BASELINE = "Baseline";
    
		/** the possible value of an alignment attribute */
		public static string DEFAULT = "Default";

	}
}