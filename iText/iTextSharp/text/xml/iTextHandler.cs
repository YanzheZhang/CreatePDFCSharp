using System;
using System.Collections;
using System.util;
using System.Text;
using System.Xml;

using iTextSharp.text;

/*
 * $Id: iTextHandler.cs,v 1.6 2003/05/15 01:52:36 geraldhenson Exp $
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

namespace iTextSharp.text.xml {

	/// <summary>
	/// The <CODE>iTextHandler</CODE>-class maps several XHTML-tags to iText-objects.
	/// </summary>
	public class iTextHandler : ParserBase {
    
		/// <summary> This is the resulting document. </summary>
		protected IDocListener document;
    
		/// <summary> This is a <CODE>Stack</CODE> of objects, waiting to be added to the document. </summary>
		protected Stack stack;
    
		/// <summary> Counts the number of chapters in this document. </summary>
		protected int chapters = 0;
    
		/// <summary> This is the current chunk to which characters can be added. </summary>
		protected Chunk currentChunk = null;
    
		/// <summary> This is the current chunk to which characters can be added. </summary>
		protected bool ignore = false;

		/// <summary> This is a flag that can be set, if you want to open and close the Document-object yourself. </summary>
		protected bool controlOpenClose = true;

		/// <summary>
		/// Constructs a new iTextHandler that will translate all the events
		/// triggered by the parser to actions on the <CODE>Document</CODE>-object.
		/// </summary>
		/// <param name="document">this is the document on which events must be triggered</param>
		public iTextHandler(IDocListener document) : base() {
			this.document = document;
			stack = new Stack();
		}

		/// <summary>
		/// Sets the parameter that allows you to enable/disable the control over the Document.open() and Document.close() method.
		/// </summary>
		/// <remarks>
		/// If you set this parameter to true (= default), the parser will open the Document object when the start-root-tag is encountered
		/// and close it when the end-root-tag is met. If you set it to false, you have to open and close the Document object
		/// yourself.
		/// </remarks>
		/// <param name="controlOpenClose">set this to false if you plan to open/close the Document yourself</param>
		public void setControlOpenClose(bool controlOpenClose) {
			this.controlOpenClose = controlOpenClose;
		}

		/// <summary>
		/// This method gets called when a start tag is encountered.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="lname"></param>
		/// <param name="name">the name of the tag that is encountered</param>
		/// <param name="attrs">the list of attributes</param>
		public override void startElement(String uri, String lname, String name, Hashtable attrs) {
        
			Properties attributes = new Properties();
			if (attrs != null) {
				foreach (string key in attrs.Keys) {
					attributes.Add(key, (string)attrs[key]);
				}
			}
			handleStartingTags(name, attributes);
		}
    
		/// <summary>
		/// This method deals with the starting tags.
		/// </summary>
		/// <param name="name">the name of the tag</param>
		/// <param name="attributes">the list of attributes</param>
		public void handleStartingTags(String name, Properties attributes) {
			//System.err.println("Start: " + name);
			if (ignore || ElementTags.IGNORE.Equals(name)) {
				ignore = true;
				return;
			}
        
			// maybe there is some meaningful data that wasn't between tags
			if (currentChunk != null) {
				ITextElementArray current;
				try {
					current = (ITextElementArray) stack.Pop();
				}
				catch {
					current = new Paragraph();
				}
				current.Add(currentChunk);
				stack.Push(current);
				currentChunk = null;
			}

			// registerfont
			if (name.Equals("registerfont")) {
				FontFactory.register(attributes);
			}

			// header
			if (ElementTags.HEADER.Equals(name)) {
				stack.Push(new HeaderFooter(attributes));
				return;
			}

			// footer
			if (ElementTags.FOOTER.Equals(name)) {
				stack.Push(new HeaderFooter(attributes));
				return;
			}

			// before
			if (name.Equals("before")) {
				HeaderFooter tmp = (HeaderFooter)stack.Pop();

				tmp.Before = new Phrase(attributes);
				stack.Push(tmp);
				return;
			}
        
			// after
			if (name.Equals("after")) {
				HeaderFooter tmp = (HeaderFooter)stack.Pop();

				tmp.After = new Phrase(attributes);
				stack.Push(tmp);
				return;
			}
        
			// chunks
			if (Chunk.isTag(name)) {
				currentChunk = new Chunk(attributes);
				return;
			}
        
			// symbols
			if (Entities.isTag(name)) {
				Font f = new Font();
				if (currentChunk != null) {
					handleEndingTags(ElementTags.CHUNK);
					f = currentChunk.Font;
				}
				currentChunk = Entities.get(attributes[ElementTags.ID], f);
				return;
			}

			// phrases
			if (Phrase.isTag(name)) {
				stack.Push(new Phrase(attributes));
				return;
			}
        
			// anchors
			if (Anchor.isTag(name)) {
				stack.Push(new Anchor(attributes));
				return;
			}
        
			// paragraphs and titles
			if (Paragraph.isTag(name) || Section.isTitle(name)) {
				stack.Push(new Paragraph(attributes));
				return;
			}
        
			// lists
			if (List.isTag(name)) {
				stack.Push(new List(attributes));
				return;
			}
        
			// listitems
			if (ListItem.isTag(name)) {
				stack.Push(new ListItem(attributes));
				return;
			}
        
			// cells
			if (Cell.isTag(name)) {
				stack.Push(new Cell(attributes));
				return;
			}
        
			// tables
			if (Table.isTag(name)) {
				Table table = new Table(attributes);
				float[] widths = table.ProportionalWidths;
				for (int i = 0; i < widths.Length; i++) {
					if (widths[i] == 0) {
						widths[i] = 100.0f / (float)widths.Length;
					}
				}
				try {
					table.Widths = widths;
				}
				catch(BadElementException bee) {
					// this shouldn't happen
					throw new Exception("", bee);
				}
				stack.Push(table);
				return;
			}
        
			// sections
			if (Section.isTag(name)) {
				IElement previous = (IElement) stack.Pop();
				Section section;
				try {
					section = ((Section)previous).addSection(attributes);
				}
				catch(Exception cce) {
					throw cce;
				}
				stack.Push(previous);
				stack.Push(section);
				return;
			}
        
			// chapters
			if (Chapter.isTag(name)) {
				String value; // changed after a suggestion by Serge S. Vasiljev
				if ((value = (String)attributes.Remove(ElementTags.NUMBER)) != null){
					chapters = int.Parse(value);
				}
				else {
					chapters++;
				}
				Chapter chapter = new Chapter(attributes,chapters);
				stack.Push(chapter);
				return;
			}
        
			// images
			if (Image.isTag(name)) {
				try {
					Image img = Image.getInstance(attributes);
					Object current;
					try {
						// if there is an element on the stack...
						current = stack.Pop();
						// ...and it's a Chapter or a Section, the Image can be added directly
						if (current is Chapter || current is Section || current is Cell) {
							((ITextElementArray)current).Add(img);
							stack.Push(current);
							return;
						}
							// ...if not, the Image is wrapped in a Chunk before it's added
						else {
							Stack newStack = new Stack();
							try {
								while (! (current is Chapter || current is Section || current is Cell)) {
									newStack.Push(current);
									if (current is Anchor) {
										img.Annotation = new Annotation(0, 0, 0, 0, ((Anchor)current).Reference);
									}
									current = stack.Pop();
								}
								((ITextElementArray)current).Add(img);
								stack.Push(current);
							}
							catch {
								document.Add(img);
							}
							while (!(newStack.Count == 0)) {
								stack.Push(newStack.Pop());
							}
							return;
						}
					}
					catch {
						// if there is no element on the stack, the Image is added to the document
						try {
							document.Add(img);
						}
						catch(DocumentException de) {
							throw new Exception("", de);
						}
						return;
					}
				}
				catch(Exception e) {
					throw new Exception("", e);
				}
			}
        
			// annotations
			if (Annotation.isTag(name)) {
				Annotation annotation = new Annotation(attributes);
				ITextElementArray current;
				try {
					try {
						current = (ITextElementArray) stack.Pop();
						try {
							current.Add(annotation);
						}
						catch {
							document.Add(annotation);
						}
						stack.Push(current);
					}
					catch {
						document.Add(annotation);
					}
					return;
				}
				catch(DocumentException de) {
					throw new Exception("", de);
				}
			}
        
			// newlines
			if (isNewline(name)) {
				ITextElementArray current;
				try {
					current = (ITextElementArray) stack.Pop();
					current.Add(Chunk.NEWLINE);
					stack.Push(current);
				}
				catch {
					if (currentChunk == null) {
						try {
							document.Add(Chunk.NEWLINE);
						}
						catch(DocumentException de) {
							throw new Exception("", de);
						}
					}
					else {
						currentChunk.append("\n");
					}
				}
				return;
			}
        
			// newpage
			if (isNewpage(name)) {
				ITextElementArray current;
				try {
					current = (ITextElementArray) stack.Pop();
					Chunk newPage = new Chunk("");
					newPage.setNewPage();
					current.Add(newPage);
					stack.Push(current);
				}
				catch {
					try {
						document.newPage();
					}
					catch(DocumentException de) {
						throw new Exception("", de);
					}
				}
				return;
			}
        
			// newpage
			if (ElementTags.HORIZONTALRULE.Equals(name)) {
				ITextElementArray current;
				Graphic hr = new Graphic();
				hr.setHorizontalLine(1.0f, 100.0f);
				try {
					current = (ITextElementArray) stack.Pop();
					current.Add(hr);
					stack.Push(current);
				}
				catch {
					try {
						document.Add(hr);
					}
					catch(DocumentException de) {
						throw new Exception("", de);
					}
				}
				return;
			}
        
			// documentroot
			if (isDocumentRoot(name)) {
				String value;
				foreach (string key in attributes.Keys) {
					value = attributes[key];
					try {
						document.Add(new Meta(key, value));
					}
					catch(DocumentException de) {
						throw new Exception("", de);
					}
				}
				if (controlOpenClose) document.Open();
			}
		}
    
		/// <summary>
		/// This method gets called when ignorable white space encountered.
		/// </summary>
		/// <param name="ch">an array of characters</param>
		/// <param name="start">the start position in the array</param>
		/// <param name="length">the number of characters to read from the array</param>
		public void ignorableWhitespace(char[] ch, int start, int length) {
			// do nothing: we handle white space ourselves in the characters method
		}
    
		/// <summary>
		/// This method gets called when characters are encountered.
		/// </summary>
		/// <param name="content">an array of characters</param>
		/// <param name="start">the start position in the array</param>
		/// <param name="length">the number of characters to read from the array</param>
		public override void characters(string content, int start, int length) {
        
			if (ignore) return;
        
			if (content.Trim().Length == 0) {
				return;
			}
        
			StringBuilder buf = new StringBuilder();
			int len = content.Length;
			char character;
			bool newline = false;
			for (int i = 0; i < len; i++) {
				switch (character = content[i]) {
					case ' ':
						if (!newline) {
							buf.Append(character);
						}
						break;
					case '\n':
						if (i > 0) {
							newline = true;
							buf.Append(' ');
						}
						break;
					case '\r':
						break;
					case '\t':
						break;
					default:
						newline = false;
						buf.Append(character);
						break;
				}
			}

			string tmp = buf.ToString();
			string rline = new String('\r', 1);
			string nline = new String('\n', 1);
			string tline = new String('\t', 1);
			tmp = tmp.Replace("\\n", nline);
			tmp = tmp.Replace("\\t", tline);
			tmp = tmp.Replace("\\r", rline);

			if (currentChunk == null) {
				currentChunk = new Chunk(tmp);
			}
			else {
				currentChunk.append(tmp);
			}
		}
    
		/// <summary>
		/// This method gets called when an end tag is encountered.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="lname"></param>
		/// <param name="name">the name of the tag that ends</param>
		public override void endElement(String uri, String lname, String name) {
			handleEndingTags(name);
		}
    
		/// <summary>
		/// This method deals with the starting tags.
		/// </summary>
		/// <param name="name">the name of the tag</param>
		public void handleEndingTags(String name) {
        
			//System.err.println("Stop: " + name);
        
			if (ElementTags.IGNORE.Equals(name)) {
				ignore = false;
				return;
			}
			if (ignore) return;
			// tags that don't have any content
			if (isNewpage(name) || Annotation.isTag(name) || Image.isTag(name) || isNewline(name)) {
				return;
			}
        
			try {
				// titles of sections and chapters
				if (Section.isTitle(name)) {
					Paragraph current = (Paragraph) stack.Pop();
					if (currentChunk != null) {
						current.Add(currentChunk);
						currentChunk = null;
					}
					Section previous = (Section) stack.Pop();
					previous.Title = current;
					stack.Push(previous);
					return;
				}
            
				// all other endtags
				if (currentChunk != null) {
					ITextElementArray current;
					try {
						current = (ITextElementArray) stack.Pop();
					}
					catch {
						current = new Paragraph();
					}
					current.Add(currentChunk);
					stack.Push(current);
					currentChunk = null;
				}
            
				// chunks
				if (Chunk.isTag(name)) {
					return;
				}
            
				// phrases, anchors, lists, tables
				if (Phrase.isTag(name) || Anchor.isTag(name) || List.isTag(name) || Paragraph.isTag(name)) {
					IElement current = (IElement) stack.Pop();
					try {
						ITextElementArray previous = (ITextElementArray) stack.Pop();
						previous.Add(current);
						stack.Push(previous);
					}
					catch {
						document.Add(current);
					}
					return;
				}
            
				// listitems
				if (ListItem.isTag(name)) {
					ListItem listItem = (ListItem) stack.Pop();
					List list = (List) stack.Pop();
					list.Add(listItem);
					stack.Push(list);
				}
            
				// tables
				if (Table.isTag(name)) {
					Table table = (Table) stack.Pop();           
					try {
						ITextElementArray previous = (ITextElementArray) stack.Pop(); 
						previous.Add(table);
						stack.Push(previous);
					}
					catch {
						document.Add(table);
					}
					return;
				}
            
				// rows
				if (Row.isTag(name)) {
					ArrayList cells = new ArrayList();
					int columns = 0;
					Table table;
					Cell cell;
					while (true) {
						IElement element = (IElement) stack.Pop();
						if (element.Type == Element.CELL) {
							cell = (Cell) element;
							columns += cell.Colspan;
							cells.Add(cell);
						}
						else {
							table = (Table) element;
							break;
						}
					}
					if (table.Columns < columns) {
						table.addColumns(columns - table.Columns);
					}
					cells.Reverse(0, cells.Count);
					String width;
					float[] cellWidths = new float[columns];
					bool[] cellNulls = new bool[columns];
					for (int i = 0; i < columns; i++) {
						cellWidths[i] = 0;
						cellNulls[i] = true;
					}
					float total = 0;
					int j = 0;
					foreach (Cell c in cells) {
						cell = c;
						if ((width = cell.CellWidth) == null) {
							if (cell.Colspan == 1 && cellWidths[j] == 0) {
								try {
									cellWidths[j] = 100f / columns;
									total += cellWidths[j];
								}
								catch {
									// empty on purpose
								}
							}
							else if (cell.Colspan == 1) {
								cellNulls[j] = false;
							}
						}
						else if (cell.Colspan == 1 && width.EndsWith("%")) {
							try {
								cellWidths[j] = float.Parse(width.Substring(0, width.Length - 1));
								total += cellWidths[j];
							}
							catch {
								// empty on purpose
							}
						}
						j += cell.Colspan;
						table.addCell(cell);
					}
					float[] widths = table.ProportionalWidths;
					if (widths.Length == columns) {
						float left = 0.0f;
						for (int i = 0; i < columns; i++) {
							if (cellNulls[i] && widths[i] != 0) {
								left += widths[i];
								cellWidths[i] = widths[i];
							}
						}
						if (100.0 >= total) {
							for (int i = 0; i < widths.Length; i++) {
								if (cellWidths[i] == 0 && widths[i] != 0) {
									cellWidths[i] = (widths[i] / left) * (100.0f - total);
								}
							}
						}
						table.Widths = cellWidths;
					}
					stack.Push(table);
				}

				// registerfont
				if (name.Equals("registerfont")) {
					return;
				}

				// header
				if (ElementTags.HEADER.Equals(name)) {
					document.Header = (HeaderFooter)stack.Pop();
					return;
				}

				// footer
				if (ElementTags.FOOTER.Equals(name)) {
					document.Footer = (HeaderFooter)stack.Pop();
					return;
				}

				// before
				if (name.Equals("before")) {
					return;
				}

				// after
				if (name.Equals("after")) {
					return;
				}
            
				// cells
				if (Cell.isTag(name)) {
					return;
				}
            
				// sections
				if (Section.isTag(name)) {
					stack.Pop();
					return;
				}
            
				// chapters
				if (Chapter.isTag(name)) {
					document.Add((IElement) stack.Pop());
					return;
				}
            
				// the documentroot
				if (isDocumentRoot(name)) {
					try {
						while (true) {
							IElement element = (IElement) stack.Pop();
							try {
								ITextElementArray previous = (ITextElementArray) stack.Pop();
								previous.Add(element);
								stack.Push(previous);
							}
							catch {
								document.Add(element);
							}
						}
					}
					catch {
						// empty on purpose
					}
					if (controlOpenClose) document.Close();
					return;
				}
			}
			catch(DocumentException de) {
				throw new Exception("", de);
			}
		}
    
		/// <summary>
		/// Checks if a certain tag corresponds with the newpage-tag.
		/// </summary>
		/// <param name="tag">a presumed tagname</param>
		/// <returns><CODE>true</CODE> or <CODE>false</CODE></returns>
		private bool isNewpage(String tag) {
			return ElementTags.NEWPAGE.Equals(tag);
		}
    
		/// <summary>
		/// Checks if a certain tag corresponds with the newpage-tag.
		/// </summary>
		/// <param name="tag">a presumed tagname</param>
		/// <returns><CODE>true</CODE> or <CODE>false</CODE></returns>
		private bool isNewline(String tag) {
			return ElementTags.NEWLINE.Equals(tag);
		}
    
		/// <summary>
		/// Checks if a certain tag corresponds with the roottag.
		/// </summary>
		/// <param name="tag">a presumed tagname</param>
		/// <returns><CODE>true</CODE> if <VAR>tag</VAR> equals <CODE>itext</CODE>, <CODE>false</CODE> otherwise.</returns>
		protected bool isDocumentRoot(String tag) {
			return ElementTags.ITEXT.Equals(tag);
		}
	}
}