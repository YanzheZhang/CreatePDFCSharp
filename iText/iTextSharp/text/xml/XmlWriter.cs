using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;

using iTextSharp.text;
using iTextSharp.text.markup;

/*
 * $Id: XmlWriter.cs,v 1.2 2003/03/23 22:56:27 geraldhenson Exp $
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE 
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml {
	/// <summary>
	/// A <CODE>DocWriter</CODE> class for XML (Remark: this class is not finished yet!).
	/// </summary>
	/// <remarks>
	/// An <CODE>XmlWriter</CODE> can be added as a <CODE>DocListener</CODE>
	/// to a certain <CODE>Document</CODE> by getting an instance.
	/// Every <CODE>Element</CODE> added to the original <CODE>Document</CODE>
	/// will be written to the <CODE>OutputStream</CODE> of this <CODE>XmlWriter</CODE>.
	/// </remarks>
	/// <example>
	/// <BLOCKQUOTE><PRE>
	/// // creation of the document with a certain size and certain margins
	/// Document document = new Document(PageSize.A4, 50, 50, 50, 50);
	/// try {
	///		// this will write XML to the Standard OutputStream
	///		<STRONG>XmlWriter.getInstance(document, Console.Out);</STRONG>
	///		// this will write XML to a file called text.html
	///		<STRONG>XmlWriter.getInstance(document, new FileStream("text.xml", FileMode.Create));</STRONG>
	///		// this will write XML to for instance the Stream of a System.Web.HttpResponse-object
	///		<STRONG>XmlWriter.getInstance(document, Response.OutputStream);</STRONG>
	///	}
	///	catch(DocumentException de) {
	///		System.err.println(de.Message);
	///	}
	///	// this will close the document and all the OutputStreams listening to it
	///	<STRONG>document.Close();</STRONG>
	///	</PRE></BLOCKQUOTE>
	/// </example>
	public class XmlWriter : DocWriter, IDocListener {
    
		// static membervariables (tags)
    
		/// <summary> This is the first line of the XML page. </summary>
		public static byte[] PROLOG = getISOBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n");
    
		/// <summary> This is the reference to the DTD. </summary>
		public static byte[] DOCTYPE = getISOBytes("<!DOCTYPE ITEXT SYSTEM \"");
    
		/// <summary> This is the place where the DTD is located. </summary>
		public static byte[] DTD = getISOBytes("http://www.lowagie.com/iText/itext.dtd");
    
		/// <summary> This is an array containing character to XML translations. </summary>
		private static string[] xmlCode = new string[256];
    
		/// <summary>
		/// Static Constructor
		/// </summary>
		static XmlWriter() {
			for (int i = 0; i < 10; i++) {
				xmlCode[i] = "&#00" + i + ";";
			}
        
			for (int i = 10; i < 32; i++) {
				xmlCode[i] = "&#0" + i + ";";
			}
        
			for (int i = 32; i < 128; i++) {
				xmlCode[i] = ((char)i).ToString();
			}
        
			// Special characters
			xmlCode['\n'] = "<" + ElementTags.NEWLINE + " />\n";
			xmlCode['\"'] = "&quot;"; // double quote
			xmlCode['\''] = "&apos;"; // single quote
			xmlCode['&'] = "&amp;"; // ampersand
			xmlCode['<'] = "&lt;"; // lower than
			xmlCode['>'] = "&gt;"; // greater than
        
			for (int i = 128; i < 256; i++) {
				xmlCode[i] = "&#" + i + ";";
			}
		}
		// membervariables
    
		/// <summary> This is the meta information of the document. </summary>
		private SortedMap itext = new SortedMap(new iTextSharp.text.StringCompare());
    
		// constructors
    
		/// <summary>
		/// Constructs an <CODE>XmlWriter</CODE>.
		/// </summary>
		/// <param name="doc">The <CODE>Document</CODE> that has to be written as XML</param>
		/// <param name="os">The <CODE>OutputStream</CODE> the writer has to write to.</param>
		protected XmlWriter(Document doc, Stream os) : base(doc, os) {
			document.addDocListener(this);
			try {
				os.Write(PROLOG, 0, PROLOG.Length);
				os.Write(DOCTYPE, 0, DOCTYPE.Length);
				os.Write(DTD, 0, DTD.Length);
				os.WriteByte(QUOTE);
				os.WriteByte(GT);
				os.WriteByte(NEWLINE);
			}
			catch(IOException ioe) {
				throw ioe;
			}
		}
    
		/// <summary>
		/// Constructs an <CODE>XmlWriter</CODE>.
		/// </summary>
		/// <param name="doc">The <CODE>Document</CODE> that has to be written as XML</param>
		/// <param name="os">The <CODE>OutputStream</CODE> the writer has to write to.</param>
		/// <param name="dtd">The DTD to use</param>
		protected XmlWriter(Document doc, Stream os, string dtd) : base(doc, os) {
        
			document.addDocListener(this);
			try {
				os.Write(PROLOG, 0, PROLOG.Length);
				os.Write(DOCTYPE, 0, DOCTYPE.Length);
				os.Write(getISOBytes(dtd), 0, getISOBytes(dtd).Length);
				os.WriteByte(QUOTE);
				os.WriteByte(GT);
				os.WriteByte(NEWLINE);
			}
			catch(IOException ioe) {
				throw ioe;
			}
		}
    
		// get an instance of the XmlWriter
    
		/// <summary>
		/// Gets an instance of the <CODE>XmlWriter</CODE>.
		/// </summary>
		/// <param name="document">The <CODE>Document</CODE> that has to be written</param>
		/// <param name="os">The <CODE>OutputStream</CODE> the writer has to write to.</param>
		/// <returns>a new <CODE>XmlWriter</CODE></returns>
		public static XmlWriter getInstance(Document document, Stream os) {
			return new XmlWriter(document, os);
		}
    
		/// <summary>
		/// Gets an instance of the <CODE>XmlWriter</CODE>.
		/// </summary>
		/// <param name="document">The <CODE>Document</CODE> that has to be written</param>
		/// <param name="os">The <CODE>OutputStream</CODE> the writer has to write to.</param>
		/// <param name="dtd">The DTD to use</param>
		/// <returns>a new <CODE>XmlWriter</CODE></returns>
		public static XmlWriter getInstance(Document document, Stream os, string dtd) {
			return new XmlWriter(document, os, dtd);
		}
    
		// implementation of the DocListener methods
    
		/// <summary>
		/// Signals that an <CODE>Element</CODE> was added to the <CODE>Document</CODE>.
		/// </summary>
		/// <param name="element">the element to add</param>
		/// <returns><CODE>true</CODE> if the element was added, <CODE>false</CODE> if not.</returns>
		public override bool Add(IElement element) {
			if (pause) {
				return false;
			}
			try {
				switch(element.Type) {
					case Element.TITLE:
						itext.Add(ElementTags.TITLE, ((Meta)element).Content);
						return true;
					case Element.SUBJECT:
						itext.Add(ElementTags.SUBJECT, ((Meta)element).Content);
						return true;
					case Element.KEYWORDS:
						itext.Add(ElementTags.KEYWORDS, ((Meta)element).Content);
						return true;
					case Element.AUTHOR:
						itext.Add(ElementTags.AUTHOR, ((Meta)element).Content);
						return true;
					default:
						write(element, 1);
						return true;
				}
			}
			catch {
				return false;
			}
		}
    
		/// <summary>
		/// Signals that the <CODE>Document</CODE> has been opened and that
		/// <CODE>Elements</CODE> can be added.
		/// </summary>
		public override void Open() {
			base.Open();
			try {
				itext.Add(ElementTags.PRODUCER, "iTextSharpXML");
				itext.Add(ElementTags.CREATIONDATE, DateTime.Now.ToString());
				writeStart(ElementTags.ITEXT);
				foreach(string key in itext.Keys) {
					write(key, (string)itext[key]);
				}
				os.WriteByte(GT);
			}
			catch(IOException ioe) {
				throw ioe;
			}
		}
    
		/// <summary>
		/// Signals that an new page has to be LTed.
		/// </summary>
		/// <returns><CODE>true</CODE> if the page was added, <CODE>false</CODE> if not.</returns>
		public override bool newPage() {
			if (pause || !open) {
				return false;
			}
			try {
				writeStart(ElementTags.NEWPAGE);
				writeEnd();
				return true;
			}
			catch {
				return false;
			}
		}
    
		/// <summary>
		/// Signals that the <CODE>Document</CODE> was closed and that no other
		/// <CODE>Elements</CODE> will be added.
		/// </summary>
		public override void Close() {
			try {
				os.WriteByte(NEWLINE);
				writeEnd(ElementTags.ITEXT);
				base.Close();
			}
			catch(IOException ioe) {
				throw ioe;
			}
		}
    
		// methods
    
		/// <summary>
		/// Writes the XML representation of an element.
		/// </summary>
		/// <param name="element">the element</param>
		/// <param name="indent">the indentation</param>
		private void write(IElement element, int indent) {
			switch(element.Type) {
				case Element.CHUNK: {
					Chunk chunk = (Chunk) element;
                
					// if the chunk contains an image, return the image representation
					try {
						Image image = chunk.Image;
						write(image, indent);
						return;
					}
					catch {
						// empty on purpose
					}
                
					addTabs(indent);
					Hashmap attributes = chunk.Attributes;
					if (chunk.Font.isStandardFont() && attributes == null && !(hasMarkupAttributes(chunk))) {
						write(encode(chunk.Content, indent));
						return;
					}
					else {
						if (attributes !=  null && attributes[Chunk.NEWPAGE] != null) {
							writeStart(ElementTags.NEWPAGE);
							writeEnd();
							return;
						}
						writeStart(ElementTags.CHUNK);
						if (! chunk.Font.isStandardFont()) {
							write(chunk.Font);
						}
						if (attributes != null) {
							foreach (string key in attributes.Keys) {
								if (key.Equals(Chunk.LOCALGOTO)
									|| key.Equals(Chunk.LOCALDESTINATION)
									|| key.Equals(Chunk.GENERICTAG)) {
									string value = (String) attributes[key];
									write(key.ToLower(), value);
								}
								if (key.Equals(Chunk.SUBSUPSCRIPT)) {
									write(key.ToLower(), ((float)attributes[key]).ToString("0.0"));
								}
							}
						}
						if (hasMarkupAttributes(chunk)) {
							writeMarkupAttributes((IMarkupAttributes)chunk);
						}
						os.WriteByte(GT);
						write(encode(chunk.Content, indent));
						writeEnd(ElementTags.CHUNK);
					}
					return;
				}
				case Element.PHRASE: {
					Phrase phrase = (Phrase) element;
                
					addTabs(indent);
					writeStart(ElementTags.PHRASE);
                
					write(ElementTags.LEADING, phrase.Leading.ToString("0.0"));
					write(phrase.Font);
					if (hasMarkupAttributes(phrase)) {
						writeMarkupAttributes((IMarkupAttributes)phrase);
					}
					os.WriteByte(GT);
					foreach (IElement ele in phrase) {
						write(ele, indent + 1);
					}
                
					addTabs(indent);
					writeEnd(ElementTags.PHRASE);
					return;
				}
				case Element.ANCHOR: {
					Anchor anchor = (Anchor) element;
                
					addTabs(indent);
					writeStart(ElementTags.ANCHOR);
                
					write(ElementTags.LEADING, anchor.Leading.ToString("0.0"));
					write(anchor.Font);
					if (anchor.Name != null) {
						write(ElementTags.NAME, anchor.Name);
					}
					if (anchor.Reference != null) {
						write(ElementTags.REFERENCE, anchor.Reference);
					}
					if (hasMarkupAttributes(anchor)) {
						writeMarkupAttributes((IMarkupAttributes)anchor);
					}
					os.WriteByte(GT);
					foreach (IElement ele in anchor) {
						write(ele, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.ANCHOR);
					return;
				}
				case Element.PARAGRAPH: {
					Paragraph paragraph = (Paragraph) element;
                
					addTabs(indent);
					writeStart(ElementTags.PARAGRAPH);
                
					write(ElementTags.LEADING, paragraph.Leading.ToString("0.0"));
					write(paragraph.Font);
					write(ElementTags.ALIGN, ElementTags.getAlignment(paragraph.Alignment));
					if (paragraph.IndentationLeft != 0) {
						write(ElementTags.INDENTATIONLEFT, paragraph.IndentationLeft.ToString("0.0"));
					}
					if (paragraph.IndentationRight != 0) {
						write(ElementTags.INDENTATIONRIGHT, paragraph.IndentationRight.ToString("0.0"));
					}
					if (hasMarkupAttributes(paragraph)) {
						writeMarkupAttributes((IMarkupAttributes)paragraph);
					}
					os.WriteByte(GT);
					foreach (IElement ele in paragraph) {
						write(ele, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.PARAGRAPH);
					return;
				}
				case Element.SECTION: {
					Section section = (Section) element;
                
					addTabs(indent);
					writeStart(ElementTags.SECTION);
					writeSection(section, indent);
					writeEnd(ElementTags.SECTION);
					return;
				}
				case Element.CHAPTER: {
					Chapter chapter = (Chapter) element;
                
					addTabs(indent);
					writeStart(ElementTags.CHAPTER);
					if (hasMarkupAttributes(chapter)) {
						writeMarkupAttributes((IMarkupAttributes)chapter);
					}
					writeSection(chapter, indent);
					writeEnd(ElementTags.CHAPTER);
					return;
                
				}
				case Element.LIST: {
					List list = (List) element;
                
					addTabs(indent);
					writeStart(ElementTags.LIST);
					write(ElementTags.NUMBERED, list.isNumbered().ToString().ToLower());
					write(ElementTags.SYMBOLINDENT, list.SymbolIndent.ToString());
					if (list.First != 1) {
						write(ElementTags.FIRST, list.First.ToString());
					}
					if (list.IndentationLeft != 0) {
						write(ElementTags.INDENTATIONLEFT, list.IndentationLeft.ToString("0.0"));
					}
					if (list.IndentationRight != 0) {
						write(ElementTags.INDENTATIONRIGHT, list.IndentationRight.ToString("0.0"));
					}
					if (!list.isNumbered()) {
						write(ElementTags.LISTSYMBOL, list.Symbol.Content);
					}
					write(list.Symbol.Font);
					if (hasMarkupAttributes(list)) {
						writeMarkupAttributes((IMarkupAttributes)list);
					}
					os.WriteByte(GT);
					foreach (IElement ele in list.Items) {
						write(ele, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.LIST);
					return;
				}
				case Element.LISTITEM: {
					ListItem listItem = (ListItem) element;
                
					addTabs(indent);
					writeStart(ElementTags.LISTITEM);
					write(ElementTags.LEADING, listItem.Leading.ToString("0.0"));
					write(listItem.Font);
					write(ElementTags.ALIGN, ElementTags.getAlignment(listItem.Alignment));
					if (listItem.IndentationLeft != 0) {
						write(ElementTags.INDENTATIONLEFT, listItem.IndentationLeft.ToString("0.0"));
					}
					if (listItem.IndentationRight != 0) {
						write(ElementTags.INDENTATIONRIGHT, listItem.IndentationRight.ToString("0.0"));
					}
					if (hasMarkupAttributes(listItem)) {
						writeMarkupAttributes((IMarkupAttributes)listItem);
					}
					os.WriteByte(GT);
					foreach (IElement ele in listItem) {
						write(ele, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.LISTITEM);
					return;
				}
				case Element.CELL: {
					Cell cell = (Cell) element;
                
					addTabs(indent);
					writeStart(ElementTags.CELL);
					write((Rectangle) cell);
					write(ElementTags.HORIZONTALALIGN, ElementTags.getAlignment(cell.HorizontalAlignment));
					write(ElementTags.VERTICALALIGN, ElementTags.getAlignment(cell.VerticalAlignment));
					if (cell.CellWidth != null) {
						write(ElementTags.WIDTH, cell.CellWidth);
					}
					if (cell.Colspan != 1) {
						write(ElementTags.COLSPAN, cell.Colspan.ToString());
					}
					if (cell.Rowspan != 1) {
						write(ElementTags.ROWSPAN, cell.Rowspan.ToString());
					}
					if (cell.Header) {
						write(ElementTags.HEADER, bool.TrueString.ToLower());
					}
					if (cell.NoWrap) {
						write(ElementTags.NOWRAP, bool.TrueString.ToLower());
					}
					if (cell.Leading != -1) {
						write(ElementTags.LEADING, cell.Leading.ToString("0.0"));
					}
					if (hasMarkupAttributes(cell)) {
						writeMarkupAttributes((IMarkupAttributes)cell);
					}
					os.WriteByte(GT);
					foreach (IElement ele in cell.Elements) {
						write(ele, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.CELL);
					return;
				}
				case Element.ROW: {
					Row row = (Row) element;
                
					addTabs(indent);
					writeStart(ElementTags.ROW);
					if (hasMarkupAttributes(row)){
						writeMarkupAttributes((IMarkupAttributes)row);
					}
					os.WriteByte(GT);
					IElement cell;
					for (int i = 0; i < row.Columns; i++) {
						if ((cell = (IElement)row.getCell(i)) != null) {
							write(cell, indent + 1);
						}
					}
					addTabs(indent);
					writeEnd(ElementTags.ROW);
					return;
				}
				case Element.TABLE: {
					Table table = (Table) element;
					table.complete();
					addTabs(indent);
					writeStart(ElementTags.TABLE);
					write(ElementTags.COLUMNS, table.Columns.ToString());
					os.WriteByte(SPACE);
					write(ElementTags.WIDTH);
					os.WriteByte(EQUALS);
					os.WriteByte(QUOTE);
					if (! "".Equals(table.AbsWidth)){
						write(table.AbsWidth);
					}
					else{
						write(table.WidthPercentage.ToString());
						write("%");
					}
					os.WriteByte(QUOTE);
					write(ElementTags.ALIGN, ElementTags.getAlignment(table.Alignment));
					write(ElementTags.CELLPADDING, table.Cellpadding.ToString("0.0"));
					write(ElementTags.CELLSPACING, table.Cellspacing.ToString("0.0"));
					os.WriteByte(SPACE);
					write(ElementTags.WIDTHS);
					os.WriteByte(EQUALS);
					os.WriteByte(QUOTE);
					float[] widths = table.ProportionalWidths;
					write(widths[0].ToString());
					for (int i = 1; i < widths.Length; i++) {
						write(";");
						write(widths[i].ToString());
					}
					os.WriteByte(QUOTE);
					write((Rectangle) table);
					if (hasMarkupAttributes(table)) {
						writeMarkupAttributes((IMarkupAttributes)table);
					}
					os.WriteByte(GT);
					foreach (Row row in table) {
						write(row, indent + 1);
					}
					addTabs(indent);
					writeEnd(ElementTags.TABLE);
					return;
				}
				case Element.ANNOTATION: {
					Annotation annotation = (Annotation) element;
                
					addTabs(indent);
					writeStart(ElementTags.ANNOTATION);
					if (annotation.Title != null) {
						write(ElementTags.TITLE, annotation.Title);
					}
					if (annotation.Content != null) {
						write(ElementTags.CONTENT, annotation.Content);
					}
					if (hasMarkupAttributes(annotation)) {
						writeMarkupAttributes((IMarkupAttributes)annotation);
					}
					writeEnd();
					return;
				}
				case Element.GIF:
				case Element.JPEG:
				case Element.PNG: {
					Image image = (Image) element;
					if (image.Url == null) {
						return;
					}
                
					addTabs(indent);
					writeStart(ElementTags.IMAGE);
					write(ElementTags.URL, image.Url.ToString());
					if ((image.Alignment & Image.LEFT) > 0) {
						write(ElementTags.ALIGN, ElementTags.ALIGN_LEFT);
					}
					else if ((image.Alignment & Image.RIGHT) > 0) {
						write(ElementTags.ALIGN, ElementTags.ALIGN_RIGHT);
					}
					else if ((image.Alignment & Image.MIDDLE) > 0) {
						write(ElementTags.ALIGN, ElementTags.ALIGN_MIDDLE);
					}
					if ((image.Alignment & Image.UNDERLYING) > 0) {
						write(ElementTags.UNDERLYING, bool.TrueString.ToLower());
					}
					if ((image.Alignment & Image.TEXTWRAP) > 0) {
						write(ElementTags.TEXTWRAP, bool.TrueString.ToLower());
					}
					if (image.Alt != null) {
						write(ElementTags.ALT, image.Alt);
					}
					if (image.hasAbsolutePosition()) {
						write(ElementTags.ABSOLUTEX, image.AbsoluteX.ToString("0.0"));
						write(ElementTags.ABSOLUTEY, image.AbsoluteY.ToString("0.0"));
					}
					write(ElementTags.PLAINWIDTH, image.PlainWidth.ToString("0.0"));
					write(ElementTags.PLAINHEIGHT, image.PlainHeight.ToString("0.0"));
					if (hasMarkupAttributes(image)) {
						writeMarkupAttributes((IMarkupAttributes)image);
					}
					writeEnd();
					return;
				}
				default:
					return;
			}
		}
    
		/// <summary>
		/// Writes the XML representation of a section.
		/// </summary>
		/// <param name="section">the section to write</param>
		/// <param name="indent">the indentation</param>
		private void writeSection(Section section, int indent) {
			write(ElementTags.NUMBERDEPTH, section.NumberDepth.ToString());
			write(ElementTags.DEPTH, section.Depth.ToString());
			write(ElementTags.INDENT, section.Indentation.ToString("0.0"));
			if (section.IndentationLeft != 0) {
				write(ElementTags.INDENTATIONLEFT, section.IndentationLeft.ToString("0.0"));
			}
			if (section.IndentationRight != 0) {
				write(ElementTags.INDENTATIONRIGHT, section.IndentationRight.ToString("0.0"));
			}
			os.WriteByte(GT);
        
			if (section.Title != null) {
				addTabs(indent + 1);
				writeStart(ElementTags.TITLE);
				write(ElementTags.LEADING, section.Title.Leading.ToString("0.0"));
				write(ElementTags.ALIGN, ElementTags.getAlignment(section.Title.Alignment));
				if (section.Title.IndentationLeft != 0) {
					write(ElementTags.INDENTATIONLEFT, section.Title.IndentationLeft.ToString("0.0"));
				}
				if (section.Title.IndentationRight != 0) {
					write(ElementTags.INDENTATIONRIGHT, section.Title.IndentationRight.ToString("0.0"));
				}
				write(section.Title.Font);
				os.WriteByte(GT);

				IEnumerator i = section.Title.GetEnumerator();
				if (i.MoveNext()) {
//					if (section.Depth > 0) {
//						i.MoveNext();
//					}

					while (i.MoveNext()) {
						write((IElement)i.Current, indent + 2);
					}
				}
				addTabs(indent + 1);
				writeEnd(ElementTags.TITLE);
			}
			foreach (IElement ele in section) {
				write(ele, indent + 1);
			}
			addTabs(indent);
		}
    
		/// <summary>
		/// Writes the XML representation of this <CODE>Rectangle</CODE>.
		/// </summary>
		/// <param name="rectangle">a <CODE>Rectangle</CODE></param>
		private void write(Rectangle rectangle) {
			if (rectangle.BorderWidth != Rectangle.UNDEFINED) {
				write(ElementTags.BORDERWIDTH, rectangle.BorderWidth.ToString("0.0"));
				if (rectangle.hasBorder(Rectangle.LEFT)) {
					write(ElementTags.LEFT, bool.TrueString.ToLower());
				}
				if (rectangle.hasBorder(Rectangle.RIGHT)) {
					write(ElementTags.RIGHT, bool.TrueString.ToLower());
				}
				if (rectangle.hasBorder(Rectangle.TOP)) {
					write(ElementTags.TOP, bool.TrueString.ToLower());
				}
				if (rectangle.hasBorder(Rectangle.BOTTOM)) {
					write(ElementTags.BOTTOM, bool.TrueString.ToLower());
				}
			}
			if (rectangle.BorderColor != null) {
				write(ElementTags.RED, rectangle.BorderColor.R.ToString());
				write(ElementTags.GREEN, rectangle.BorderColor.G.ToString());
				write(ElementTags.BLUE, rectangle.BorderColor.B.ToString());
			}
			if (rectangle.BackgroundColor != null) {
				write(ElementTags.BGRED, rectangle.BackgroundColor.R.ToString());
				write(ElementTags.BGGREEN, rectangle.BackgroundColor.G.ToString());
				write(ElementTags.BGBLUE, rectangle.BackgroundColor.B.ToString());
			}
			if (rectangle.GrayFill > 0) {
				write(ElementTags.GRAYFILL, rectangle.GrayFill.ToString());
			}
		}
    
		/// <summary>
		/// Encodes a <CODE>String</CODE>.
		/// </summary>
		/// <param name="str">the <CODE>String</CODE> to encode</param>
		/// <param name="indent"></param>
		/// <returns>the encoded <CODE>String</CODE></returns>
		static string encode(string str, int indent) {
			int n = str.Length;
			int pos = 0;
			char character;
			StringBuilder buf = new StringBuilder();
			// loop over all the characters of the String.
			for (int i = 0; i < n; i++) {
				character = str[i];
				// the Xmlcode of these characters are added to a StringBuilder one by one
				switch (character) {
					case ' ':
						if ((i - pos) > 60) {
							pos = i;
							buf.Append("\n");
							addTabs(buf, indent);
							break;
						}
						goto default;

					default:
						buf.Append(xmlCode[(int) character]);
						break;
				}
			}
			return buf.ToString();
		}
    
		/// <summary>
		/// Adds a number of tabs to a <CODE>StringBuilder</CODE>.
		/// </summary>
		/// <param name="buf">the StringBuilder</param>
		/// <param name="indent">the number of tabs to add</param>
		static void addTabs(StringBuilder buf, int indent) {
			for (int i = 0; i < indent; i++) {
				buf.Append("\t");
			}
		}
    
		/// <summary>
		/// Writes the XML representation of a <CODE>Font</CODE>.
		/// </summary>
		/// <param name="font">a <CODE>Font</CODE></param>
		private void write(Font font) {
			write(ElementTags.FONT, font.Familyname);
			if (font.Size != Font.UNDEFINED) {
				write(ElementTags.SIZE, font.Size.ToString("0.0"));
			}
			if (font.Style != Font.UNDEFINED) {
				os.WriteByte(SPACE);
				write(ElementTags.STYLE);
				os.WriteByte(EQUALS);
				os.WriteByte(QUOTE);
				switch(font.Style & Font.BOLDITALIC) {
					case Font.NORMAL:
						write(MarkupTags.CSS_NORMAL);
						break;
					case Font.BOLD:
						write(MarkupTags.CSS_BOLD);
						break;
					case Font.ITALIC:
						write(MarkupTags.CSS_ITALIC);
						break;
					case Font.BOLDITALIC:
						write(MarkupTags.CSS_BOLD);
						write(", ");
						write(MarkupTags.CSS_ITALIC);
						break;
				}
				if ((font.Style & Font.UNDERLINE) > 0) {
					write(", ");
					write(MarkupTags.CSS_UNDERLINE);
				}
				if ((font.Style & Font.STRIKETHRU) > 0) {
					write(", ");
					write(MarkupTags.CSS_LINETHROUGH);
				}
				os.WriteByte(QUOTE);
			}
			if (font.Color != null) {
				write(ElementTags.RED, font.Color.R.ToString());
				write(ElementTags.GREEN, font.Color.G.ToString());
				write(ElementTags.BLUE, font.Color.B.ToString());
			}
		}
	}
}