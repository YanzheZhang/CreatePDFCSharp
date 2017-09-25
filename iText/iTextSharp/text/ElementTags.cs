using System;

/*
 * $Id: ElementTags.cs,v 1.4 2003/04/02 02:49:21 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright (c) 2001, 2002 Bruno Lowagie.
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
	/// A class that contains all the possible tagnames and their attributes.
	/// </summary>
	public class ElementTags {
    
		/// <summary> the root tag. </summary>
		public static string ITEXT = "itext";
    
		/// <summary> attribute of the root and annotation tag (also a special tag within a chapter or section) </summary>
		public static string TITLE = "title";
    
		/// <summary> attribute of the root tag </summary>
		public static string SUBJECT = "subject";
    
		/// <summary> attribute of the root tag </summary>
		public static string KEYWORDS = "keywords";
    
		/// <summary> attribute of the root tag </summary>
		public static string AUTHOR = "author";
    
		/// <summary> attribute of the root tag </summary>
		public static string CREATIONDATE = "creationdate";
    
		/// <summary> attribute of the root tag </summary>
		public static string PRODUCER = "producer";
    
		// Chapters and Sections
    
		/// <summary> the chapter tag </summary>
		public static string CHAPTER = "chapter";
    
		/// <summary> the section tag </summary>
		public static string SECTION = "section";
    
		/// <summary> attribute of section/chapter tag </summary>
		public static string NUMBERDEPTH = "numberdepth";
    
		/// <summary> attribute of section/chapter tag </summary>
		public static string DEPTH = "depth";
    
		/// <summary> attribute of section/chapter tag </summary>
		public static string NUMBER = "number";
    
		/// <summary> attribute of section/chapter tag </summary>
		public static string INDENT = "indent";
    
		/// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
		public static string LEFT = "left";
    
		/// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
		public static string RIGHT = "right";
    
		// Phrases, Anchors, Lists and Paragraphs
    
		/// <summary> the phrase tag </summary>
		public static string PHRASE = "phrase";
    
		/// <summary> the anchor tag </summary>
		public static string ANCHOR = "anchor";
    
		/// <summary> the list tag </summary>
		public static string LIST = "list";
    
		/// <summary> the listitem tag </summary>
		public static string LISTITEM = "listitem";
    
		/// <summary> the paragraph tag </summary>
		public static string PARAGRAPH = "paragraph";
    
		/// <summary> attribute of phrase/paragraph/cell tag </summary>
		public static string LEADING = "leading";
    
		/// <summary> attribute of paragraph/image/table tag </summary>
		public static string ALIGN = "align";
    
		/// <summary> attribute of paragraph </summary>
		public static string KEEPTOGETHER = "keeptogether";
    
		/// <summary> attribute of anchor tag </summary>
		public static string NAME = "name";
    
		/// <summary> attribute of anchor tag </summary>
		public static string REFERENCE = "reference";
    
		/// <summary> attribute of list tag </summary>
		public static string LISTSYMBOL = "listsymbol";
    
		/// <summary> attribute of list tag </summary>
		public static string NUMBERED = "numbered";
    
		/// <summary> attribute of the list tag </summary>
		public static string LETTERED = "lettered";

		/// <summary> attribute of list tag </summary>
		public static string FIRST = "first";
    
		/// <summary> attribute of list tag </summary>
		public static string SYMBOLINDENT = "symbolindent";
    
		/// <summary> attribute of list tag </summary>
		public static string INDENTATIONLEFT = "indentationleft";
    
		/// <summary> attribute of list tag </summary>
		public static string INDENTATIONRIGHT = "indentationright";
    
		// Chunks
    
		/// <summary> the chunk tag </summary>
		public static string IGNORE = "ignore";
    
		/// <summary> the chunk tag </summary>
		public static string ENTITY = "entity";
    
		/// <summary> the chunk tag </summary>
		public static string ID = "id";
    
		/// <summary> the chunk tag </summary>
		public static string CHUNK = "chunk";
    
		/// <summary> attribute of the chunk tag </summary>
		public static string ENCODING = "encoding";
    
		/// <summary> attribute of the chunk tag </summary>
		public static string EMBEDDED = "embedded";
    
		/// <summary> attribute of the chunk/table/cell tag </summary>
		public static string COLOR = "color";
    
		/// <summary> attribute of the chunk/table/cell tag </summary>
		public static string RED = "red";
    
		/// <summary> attribute of the chunk/table/cell tag </summary>
		public static string GREEN = "green";
    
		/// <summary> attribute of the chunk/table/cell tag </summary>
		public static string BLUE = "blue";
    
		/// <summary> attribute of the chunk tag </summary>
		public static string SUBSUPSCRIPT = Chunk.SUBSUPSCRIPT.ToLower();
    
		/// <summary> attribute of the chunk tag </summary>
		public static string LOCALGOTO = Chunk.LOCALGOTO.ToLower();
    
		/// <summary> attribute of the chunk tag </summary>
		public static string REMOTEGOTO = Chunk.REMOTEGOTO.ToLower();
    
		/// <summary> attribute of the chunk tag </summary>
		public static string LOCALDESTINATION = Chunk.LOCALDESTINATION.ToLower();
    
		/// <summary> attribute of the chunk tag </summary>
		public static string GENERICTAG = Chunk.GENERICTAG.ToLower();
    
		// tables/cells
    
		/// <summary> the table tag </summary>
		public static string TABLE = "table";
    
		/// <summary> the cell tag </summary>
		public static string ROW = "row";
    
		/// <summary> the cell tag </summary>
		public static string CELL = "cell";
    
		/// <summary> attribute of the table tag </summary>
		public static string COLUMNS = "columns";
    
		/// <summary> attribute of the table tag </summary>
		public static string LASTHEADERROW = "lastHeaderRow";
    
		/// <summary> attribute of the table tag </summary>
		public static string CELLPADDING = "cellpadding";
    
		/// <summary> attribute of the table tag </summary>
		public static string CELLSPACING = "cellspacing";
    
		/// <summary> attribute of the table tag </summary>
		public static string OFFSET = "offset";
    
		/// <summary> attribute of the table tag </summary>
		public static string WIDTHS = "widths";
    
		/// <summary> attribute of the table tag </summary>
		public static string TABLEFITSPAGE = "tablefitspage";
    
		/// <summary> attribute of the table tag </summary>
		public static string CELLSFITPAGE = "cellsfitpage";
    
		/// <summary> attribute of the cell tag </summary>
		public static string HORIZONTALALIGN = "horizontalalign";
    
		/// <summary> attribute of the cell tag </summary>
		public static string VERTICALALIGN = "verticalalign";
    
		/// <summary> attribute of the cell tag </summary>
		public static string COLSPAN = "colspan";
    
		/// <summary> attribute of the cell tag </summary>
		public static string ROWSPAN = "rowspan";
    
		/// <summary> attribute of the cell tag </summary>
		public static string HEADER = "header";
    
		/// <summary> attribute of the cell tag </summary>
		public static string FOOTER = "footer";

		/// <summary> attribute of the cell tag </summary>
		public static string NOWRAP = "nowrap";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BORDERWIDTH = "borderwidth";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string TOP = "top";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BOTTOM = "bottom";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string WIDTH = "width";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BORDERCOLOR = "bordercolor";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BACKGROUNDCOLOR = "backgroundcolor";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BGRED = "bgred";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BGGREEN = "bggreen";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string BGBLUE = "bgblue";
    
		/// <summary> attribute of the table/cell tag </summary>
		public static string GRAYFILL = "grayfill";
    
		// Misc
    
		/// <summary> the image tag </summary>
		public static string IMAGE = "image";
    
		/// <summary> the image tag </summary>
		public static string BOOKMARKOPEN = "bookmarkopen";
    
		/// <summary> attribute of the image and annotation tag </summary>
		public static string URL = "url";
    
		/// <summary> attribute of the image tag </summary>
		public static string UNDERLYING = "underlying";
    
		/// <summary> attribute of the image tag </summary>
		public static string TEXTWRAP = "textwrap";
    
		/// <summary> attribute of the image tag </summary>
		public static string ALT = "alt";
    
		/// <summary> attribute of the image tag </summary>
		public static string ABSOLUTEX = "absolutex";
    
		/// <summary> attribute of the image tag </summary>
		public static string ABSOLUTEY = "absolutey";
    
		/// <summary> attribute of the image tag </summary>
		public static string PLAINWIDTH = "plainwidth";
    
		/// <summary> attribute of the image tag </summary>
		public static string PLAINHEIGHT = "plainheight";
    
		/// <summary> attribute of the image tag </summary>
		public static string SCALEDWIDTH = "scaledwidth";
    
		/// <summary> attribute of the image tag </summary>
		public static string SCALEDHEIGHT = "scaledheight";
    
		/// <summary> attribute of the image tag </summary>
		public static string  ROTATION = "rotation";
    
		/// <summary> the newpage tag </summary>
		public static string NEWPAGE = "newpage";
    
		/// <summary> the newpage tag </summary>
		public static string NEWLINE = "newline";
    
		/// <summary> the annotation tag </summary>
		public static string ANNOTATION = "annotation";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string FILE = "file";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string DESTINATION = "destination";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string PAGE = "page";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string NAMED = "named";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string APPLICATION = "application";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string PARAMETERS = "parameters";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string OPERATION = "operation";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string DEFAULTDIR = "defaultdir";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string LLX = "llx";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string LLY = "lly";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string URX = "urx";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string URY = "ury";
    
		/// <summary> attribute of the annotation tag </summary>
		public static string CONTENT = "content";
    
		// alignment attribute values
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_LEFT = "Left";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_CENTER = "Center";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_RIGHT = "Right";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_JUSTIFIED = "Justify";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_TOP = "Top";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_MIDDLE = "Middle";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_BOTTOM = "Bottom";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string ALIGN_BASELINE = "Baseline";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string DEFAULT = "Default";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string UNKNOWN = "unknown";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string FONT = "font";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string SIZE = "size";
    
		/// <summary> the possible value of an alignment attribute </summary>
		public static string STYLE = "fontstyle";
    
		/// <summary> the possible value of a tag </summary>
		public static string HORIZONTALRULE = "horizontalrule";
    
		// methods
    
		/// <summary>
		/// Translates the alignment value.
		/// </summary>
		/// <param name="alignment">the alignment value</param>
		/// <returns>the translated value</returns>
		public static string getAlignment(int alignment) {
			switch(alignment) {
				case Element.ALIGN_LEFT:
					return ALIGN_LEFT;
				case Element.ALIGN_CENTER:
					return ALIGN_CENTER;
				case Element.ALIGN_RIGHT:
					return ALIGN_RIGHT;
				case Element.ALIGN_JUSTIFIED:
					return ALIGN_JUSTIFIED;
				case Element.ALIGN_TOP:
					return ALIGN_TOP;
				case Element.ALIGN_MIDDLE:
					return ALIGN_MIDDLE;
				case Element.ALIGN_BOTTOM:
					return ALIGN_BOTTOM;
				case Element.ALIGN_BASELINE:
					return ALIGN_BASELINE;
				default:
					return DEFAULT;
			}
		}
	}
}
