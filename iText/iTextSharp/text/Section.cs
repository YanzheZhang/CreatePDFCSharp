using System;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: Section.cs,v 1.3 2003/04/02 02:49:21 geraldhenson Exp $
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
	/// A Section is a part of a Document containing
	/// other Sections, Paragraphs, List
	/// and/or Tables.
	/// </summary>
	/// <remarks>
	/// You can not construct a Section yourself.
	/// You will have to ask an instance of Section to the
	/// Chapter or Section to which you want to
	/// add the new Section.
	/// </remarks>
	/// <example>
	/// <code>
	/// Paragraph title2 = new Paragraph("This is Chapter 2", FontFactory.getFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255)));
	/// Chapter chapter2 = new Chapter(title2, 2);
	/// Paragraph someText = new Paragraph("This is some text");
	/// chapter2.Add(someText);
	/// Paragraph title21 = new Paragraph("This is Section 1 in Chapter 2", FontFactory.getFont(FontFactory.HELVETICA, 16, Font.BOLD, new Color(255, 0, 0)));
	/// <strong>Section section1 = chapter2.addSection(title21);</strong>
	/// Paragraph someSectionText = new Paragraph("This is some silly paragraph in a chapter and/or section. It contains some text to test the functionality of Chapters and Section.");
	/// <strong>section1.Add(someSectionText);</strong>
	/// Paragraph title211 = new Paragraph("This is SubSection 1 in Section 1 in Chapter 2", FontFactory.getFont(FontFactory.HELVETICA, 14, Font.BOLD, new Color(255, 0, 0)));
	/// <strong>Section section11 = section1.addSection(40, title211, 2);
	/// section11.Add(someSectionText);</strong>strong>
	/// </code>
	/// </example>
	public class Section : ArrayList, ITextElementArray {
    
		// membervariables
    
		///<summary> This is the title of this section. </summary>
		protected Paragraph title;
    
		///<summary> This is the number of sectionnumbers that has to be shown before the section title. </summary>
		protected int numberDepth;
    
		///<summary> The indentation of this section on the left side. </summary>
		protected float indentationLeft;
    
		///<summary> The indentation of this section on the right side. </summary>
		protected float indentationRight;
    
		///<summary> The additional indentation of the content of this section. </summary>
		protected float sectionIndent;
    
		///<summary> This is the number of subsections. </summary>
		protected int subsections = 0;
    
		///<summary> This is the complete list of sectionnumbers of this section and the parents of this section. </summary>
		protected ArrayList numbers = null;
    
		///<summary> false if the bookmark children are not visible </summary>
		protected bool bookmarkOpen = true;
    
		// constructors
    
		/// <summary>
		/// Constructs a new Section.
		/// </summary>
		/// <overloads>
		/// Has 2 overloads.
		/// </overloads>
		public Section() {
			title = new Paragraph();
			numberDepth = 1;
		}
    
		/// <summary>
		/// Constructs a new Section.
		/// </summary>
		/// <param name="title">a Paragraph</param>
		/// <param name="numberDepth">the numberDepth</param>
		public Section(Paragraph title, int numberDepth) {
			this.numberDepth = numberDepth;
			this.title = title;
		}
    
		// private methods
    
		/// <summary>
		/// Sets the number of this section.
		/// </summary>
		/// <param name="number">the number of this section</param>
		/// <param name="numbers">an ArrayList, containing the numbers of the Parent</param>
		private void setNumbers(int number, ArrayList numbers) {
			this.numbers = new ArrayList();
			this.numbers.Add(number);
			this.numbers.AddRange(numbers);
		}
    
		// implementation of the Element-methods
    
		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// IElementListener.
		/// </summary>
		/// <param name="listener">the IElementListener</param>
		/// <returns>true if the element was processed successfully</returns>
		public bool process(IElementListener listener) {
			try {
				foreach(IElement ele in this) {
					listener.Add(ele);
				}
				return true;
			}
			catch(DocumentException de) {
				de.GetType();
				return false;
			}
		}
    
		/// <summary>
		/// Gets the type of the text element.
		/// </summary>
		/// <value>a type</value>
		public virtual int Type {
			get {
				return Element.SECTION;
			}
		}
    
		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public ArrayList Chunks {
			get {
				ArrayList tmp = new ArrayList();
				foreach(IElement ele in this) {
					tmp.AddRange(ele.Chunks);
				}
				return tmp;
			}
		}
    
		// overriding some of the ArrayList-methods
    
		/// <summary>
		/// Adds a Paragraph, List or Table
		/// to this Section.
		/// </summary>
		/// <param name="index">index at which the specified element is to be inserted</param>
		/// <param name="o">an object of type Paragraph, List or Table</param>
		public void Add(int index, Object o) {
			try {
				IElement element = (IElement) o;
				if (element.Type == Element.PARAGRAPH ||
					element.Type == Element.LIST ||
					element.Type == Element.CHUNK ||
					element.Type == Element.PHRASE ||
					element.Type == Element.ANCHOR ||
					element.Type == Element.ANNOTATION ||
					element.Type == Element.TABLE ||
					element.Type == Element.PTABLE ||
					element.Type == Element.IMGTEMPLATE ||
					element.Type == Element.GIF ||
					element.Type == Element.JPEG ||
					element.Type == Element.PNG ||
					element.Type == Element.IMGRAW) {
					base.Insert(index, element);
				}
				else {
					throw new Exception(element.Type.ToString());
				}
			}
			catch(Exception cce) {
				throw new Exception("Insertion of illegal Element: " + cce.Message);
			}
		}
    
		/// <summary>
		/// Adds a Paragraph, List, Table or another Section
		/// to this Section.
		/// </summary>
		/// <param name="o">an object of type Paragraph, List, Table or another Section</param>
		/// <returns>a bool</returns>
		public new bool Add(Object o) {
			try {
				IElement element = (IElement) o;
				if (element.Type == Element.PARAGRAPH ||
					element.Type == Element.LIST ||
					element.Type == Element.CHUNK ||
					element.Type == Element.PHRASE ||
					element.Type == Element.ANCHOR ||
					element.Type == Element.ANNOTATION ||
					element.Type == Element.TABLE ||
					element.Type == Element.IMGTEMPLATE ||
					element.Type == Element.PTABLE ||
					element.Type == Element.GIF ||
					element.Type == Element.JPEG ||
					element.Type == Element.PNG ||
					element.Type == Element.IMGRAW) {
					base.Add(o);
					return true;
				}
				else if (element.Type == Element.SECTION) {
					Section section = (Section) o;
					section.setNumbers(++subsections, numbers);
					base.Add(section);
					return true;
				}
				else {
					throw new Exception(element.Type.ToString());
				}
			}
			catch(Exception cce) {
				throw new Exception("Insertion of illegal Element: " + cce.Message);
			}
		}
    
		/// <summary>
		/// Adds a collection of Elements
		/// to this Section.
		/// </summary>
		/// <param name="collection">a collection of Paragraphs, Lists and/or Tables</param>
		/// <returns>true if the action succeeded, false if not.</returns>
		public bool addAll(ICollection collection) {
			foreach(object itm in collection) {
				this.Add(itm);
			}
			return true;
		}
    
		// methods that return a Section
    
		/// <summary>
		/// Creates a Section, adds it to this Section and returns it.
		/// </summary>
		/// <param name="indentation">the indentation of the new section</param>
		/// <param name="title">the title of the new section</param>
		/// <param name="numberDepth">the numberDepth of the section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(float indentation, Paragraph title, int numberDepth) {
			Section section = new Section(title, numberDepth);
			section.Indentation = indentation;
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Creates a Section, adds it to this Section and returns it.
		/// </summary>
		/// <param name="indentation">the indentation of the new section</param>
		/// <param name="title">the title of the new section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(float indentation, Paragraph title) {
			Section section = new Section(title, 1);
			section.Indentation = indentation;
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Creates a Section, add it to this Section and returns it.
		/// </summary>
		/// <param name="title">the title of the new section</param>
		/// <param name="numberDepth">the numberDepth of the section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(Paragraph title, int numberDepth) {
			Section section = new Section(title, numberDepth);
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Creates a Section, adds it to this Section and returns it.
		/// </summary>
		/// <param name="title">the title of the new section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(Paragraph title) {
			Section section = new Section(title, 1);
			Add(section);
			return section;
		}
    
		/**
		 * Adds a Section to this Section and returns it.
		 *
		 * @param	indentation	the indentation of the new section
		 * @param	title		the title of the new section
		 * @param	numberDepth	the numberDepth of the section
		 */
		/// <summary>
		/// Adds a Section to this Section and returns it.
		/// </summary>
		/// <param name="indentation">the indentation of the new section</param>
		/// <param name="title">the title of the new section</param>
		/// <param name="numberDepth">the numberDepth of the section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(float indentation, string title, int numberDepth) {
			Section section = new Section(new Paragraph(title), numberDepth);
			section.Indentation = indentation;
			Add(section);
			return section;
		}
    
		/**
		 * Adds a Section to this Section and returns it.
		 *
		 * @param	title		the title of the new section
		 * @param	numberDepth	the numberDepth of the section
		 */
		/// <summary>
		/// Adds a Section to this Section and returns it.
		/// </summary>
		/// <param name="title">the title of the new section</param>
		/// <param name="numberDepth">the numberDepth of the section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(string title, int numberDepth) {
			Section section = new Section(new Paragraph(title), numberDepth);
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Adds a Section to this Section and returns it.
		/// </summary>
		/// <param name="indentation">the indentation of the new section</param>
		/// <param name="title">the title of the new section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(float indentation, string title) {
			Section section = new Section(new Paragraph(title), 1);
			section.Indentation = indentation;
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Adds a Section to this Section and returns it.
		/// </summary>
		/// <param name="title">the title of the new section</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(string title) {
			Section section = new Section(new Paragraph(title), 1);
			Add(section);
			return section;
		}
    
		/// <summary>
		/// Creates a given Section following a set of attributes and adds it to this one.
		/// </summary>
		/// <param name="attributes">the attributes</param>
		/// <returns>the newly added Section</returns>
		public Section addSection(Properties attributes) {
			Section section = new Section(new Paragraph(""), 1);
			string value;
			if ((value = attributes.Remove(ElementTags.NUMBER)) != null) {
				subsections = int.Parse(value) - 1;
			}
			if ((value = attributes.Remove(ElementTags.BOOKMARKOPEN)) != null) {
				this.BookmarkOpen = bool.Parse(value);
			}
			section.set(attributes);
			Add(section);
			return section;
		}
    
    
		// public methods
    
		/// <summary>
		/// Alters the attributes of this Section.
		/// </summary>
		/// <param name="attributes">the attributes</param>
		public void set(Properties attributes) {
			string value;
			if ((value = attributes.Remove(ElementTags.NUMBERDEPTH)) != null) {
				NumberDepth = int.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.INDENT)) != null) {
				Indentation = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONLEFT)) != null) {
				IndentationLeft = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.INDENTATIONRIGHT)) != null) {
				IndentationRight = float.Parse(value);
			}
		}
    
		/// <summary>
		/// Get/set the title of this section
		/// </summary>
		/// <value>a Paragraph</value>
		public Paragraph Title {
			get {
				if (title == null) {
					return null;
				}
				int depth = Math.Min(numbers.Count, numberDepth);
				if (depth < 1) {
					return title;
				}
				StringBuilder buf = new StringBuilder(" ");
				for (int i = 0; i < depth; i++) {
					buf.Insert(0, ".");
					buf.Insert(0, (int)numbers[i]);
				}
				Paragraph result = new Paragraph(title);
				result.MarkupAttributes = title.MarkupAttributes;
				result.Insert(0, new Chunk(buf.ToString(), title.Font));
				return result;
			}
			
			set {
				this.title = value;
			}
		}
    
		// methods to retrieve information
    
		/// <summary>
		/// Checks if this object is a Chapter.
		/// </summary>
		/// <returns>
		/// true if it is a Chapter,
		/// false if it is a Section
		/// </returns>
		public bool isChapter() {
			return Type == Element.CHAPTER;
		}
    
		/// <summary>
		/// Checks if this object is a Section.
		/// </summary>
		/// <returns>
		/// true if it is a Section,
		/// false if it is a Chapter.
		/// </returns>
		public bool isSection() {
			return Type == Element.SECTION;
		}
    
		/// <summary>
		/// Get/set the numberdepth of this Section.
		/// </summary>
		/// <value>a int</value>
		public int NumberDepth {
			get {
				return numberDepth;
			}

			set {
				this.numberDepth = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of this Section on the left side.
		/// </summary>
		/// <value>the indentation</value>
		public float IndentationLeft {
			get {
				return indentationLeft;
			}

			set {
				indentationLeft = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of this Section on the right side.
		/// </summary>
		/// <value>the indentation</value>
		public float IndentationRight {
			get {
				return indentationRight;
			}

			set {
				indentationRight = value;
			}
		}
    
		/// <summary>
		/// Get/set the indentation of the content of this Section.
		/// </summary>
		/// <value>the indentation</value>
		public float Indentation {
			get {
				return sectionIndent;
			}

			set {
				sectionIndent = value;
			}
		}
    
		/// <summary>
		/// Returns the depth of this section.
		/// </summary>
		/// <value>the depth</value>
		public int Depth {
			get {
				return numbers.Count;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with a title tag for this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTitle(string tag) {
			return ElementTags.TITLE.Equals(tag);
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.SECTION.Equals(tag);
		}
    
		/// <summary>
		/// Get/set the bookmark
		/// </summary>
		/// <value>a bool</value>
		public bool BookmarkOpen {
			get {
				return bookmarkOpen;
			}

			set {
				this.bookmarkOpen = value;
			}
		}
	}
}
