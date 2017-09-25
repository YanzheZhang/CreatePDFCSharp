using System;
using System.Drawing;
using System.Collections;
using System.util;

using iTextSharp.text.markup;

/*
 * $Id: Cell.cs,v 1.3 2003/03/25 01:13:44 geraldhenson Exp $
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
	/// A Cell is a Rectangle containing other Elements.
	/// </summary>
	/// <remarks>
	/// A Cell is a Rectangle containing other
	/// Elements.
	/// <p/>
	/// A Cell must be added to a Table.
	/// The Table will place the Cell in
	/// a Row.
	/// </remarks>
	/// <example>
	/// <code>
	/// Table table = new Table(3);
	/// table.setBorderWidth(1);
	/// table.setBorderColor(new Color(0, 0, 255));
	/// table.setCellpadding(5);
	/// table.setCellspacing(5);
	/// <strong>Cell cell = new Cell("header");
	/// cell.setHeader(true);
	/// cell.setColspan(3);</strong>
	/// table.addCell(cell);
	/// <strong>cell = new Cell("example cell with colspan 1 and rowspan 2");
	/// cell.setRowspan(2);
	/// cell.setBorderColor(new Color(255, 0, 0));</strong>
	/// table.addCell(cell);
	/// table.addCell("1.1");
	/// table.addCell("2.1");
	/// table.addCell("1.2");
	/// table.addCell("2.2");
	/// </code>
	/// </example>
	/// <seealso cref="T:iTextSharp.text.Rectangle"/>
	/// <seealso cref="T:iTextSharp.text.Element"/>
	/// <seealso cref="T:iTextSharp.text.Table"/>
	/// <seealso cref="T:iTextSharp.text.Row"/>
	public class Cell : Rectangle, ITextElementArray {

		// static membervariable

		///<summary> This constant can be used as empty cell. </summary>
		public static Cell EMPTY_CELL = new Cell(true);

		///<summary> This constant can be used as empty cell. </summary>
		public static Cell DUMMY_CELL = new Cell(true);
		static Cell() {
			DUMMY_CELL.Colspan = 3;
			DUMMY_CELL.Border = NO_BORDER;
		}

		// membervariables

		///<summary> This is the ArrayList of Elements. </summary>
		protected ArrayList arrayList = null;

		///<summary> This is the horizontal Element. </summary>
		protected int horizontalAlignment = Element.ALIGN_UNDEFINED;

		///<summary> This is the vertical Element. </summary>
		protected int verticalAlignment = Element.ALIGN_UNDEFINED;

		///<summary> This is the vertical Element. </summary>
		protected string width;

		///<summary> This is the colspan. </summary>
		protected int colspan = 1;

		///<summary> This is the rowspan. </summary>
		protected int rowspan = 1;

		///<summary> This is the leading. </summary>
		float leading = float.NaN;

		///<summary> Is this Cell a header? </summary>
		protected bool header;

		///<summary> Will the element have to be wrapped? </summary>
		protected bool noWrap;

		// constructors

		/**
		 * Constructs an empty Cell.
		 */
		/// <summary>
		/// Constructs an empty Cell.
		/// </summary>
		/// <overloads>
		/// Has five overloads.
		/// </overloads>
		public Cell() : base(0, 0, 0, 0) {
			// creates a Rectangle with BY DEFAULT a border of 0.5
			
			this.Border = UNDEFINED;
			this.BorderWidth = 0.5F;
			
			// initializes the arraylist and adds an element
			arrayList = new ArrayList();
		}

		/// <summary>
		/// Constructs an empty Cell (for internal use only).
		/// </summary>
		/// <param name="dummy">a dummy value</param>
		public Cell(bool dummy) : this() {
			arrayList.Add(new Paragraph(0));
		}

		/// <summary>
		/// Constructs a Cell with a certain content.
		/// </summary>
		/// <remarks>
		/// The string will be converted into a Paragraph.
		/// </remarks>
		/// <param name="content">a string</param>
		public Cell(string content) : base(0, 0, 0, 0) {
			// creates a Rectangle with BY DEFAULT a border of 0.5
			this.Border = UNDEFINED;
			this.BorderWidth = 0.5F;

			// initializes the arraylist and adds an element
			arrayList = new ArrayList();
			try {
				addElement(new Paragraph(content));
			}
			catch(BadElementException bee) {
				bee.GetType();
			}
		}

        /// <summary>
        /// 自己扩展--指定字体
        /// </summary>
        /// <param name="content"></param>
        /// <param name="_Font"></param>
        public Cell(string content, iTextSharp.text.Font _Font)
            : base(0, 0, 0, 0)
        {
            // creates a Rectangle with BY DEFAULT a border of 0.5
            this.Border = UNDEFINED;
            this.BorderWidth = 0.5F;

            // initializes the arraylist and adds an element
            arrayList = new ArrayList();
            try
            {
                addElement(new Paragraph(content, _Font));
            }
            catch (BadElementException bee)
            {
                bee.GetType();
            }
        }
		/// <summary>
		/// Constructs a Cell with a certain Element.
		/// </summary>
		/// <remarks>
		/// if the element is a ListItem, Row or
		/// Cell, an exception will be thrown.
		/// </remarks>
		/// <param name="element">the element</param>
		public Cell(IElement element) : base(0, 0, 0, 0){
			// creates a Rectangle with BY DEFAULT a border of 0.5			
			this.Border = UNDEFINED;
            this.BorderWidth = 0.5F;

			try {
				Phrase p = (Phrase)element;
				leading = p.Leading;
			}
			catch(Exception e) {
				// empty on purpose
				e.GetType();
			}

			// initializes the arraylist and adds an element
			arrayList = new ArrayList();
			addElement(element);
		}

		/// <summary>
		/// Returns a Cell that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">some attributes</param>
		public Cell(Properties attributes) : this() {
			string value;
			if ((value = attributes.Remove(ElementTags.HORIZONTALALIGN)) != null) {
				setHorizontalAlignment(value);
			}
			if ((value = attributes.Remove(ElementTags.VERTICALALIGN)) != null) {
				setVerticalAlignment(value);
			}
			if ((value = attributes.Remove(ElementTags.WIDTH)) != null) {
				this.Width = value;
			}
			if ((value = attributes.Remove(ElementTags.COLSPAN)) != null) {
				this.Colspan = int.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.ROWSPAN)) != null) {
				this.Rowspan = int.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.LEADING)) != null) {
				this.Leading = float.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.HEADER)) != null) {
				this.Header = Boolean.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.NOWRAP)) != null) {
				this.NoWrap = Boolean.Parse(value);
			}
			if ((value = attributes.Remove(ElementTags.BORDERWIDTH)) != null) {
				this.BorderWidth = float.Parse(value);
			}
			int border = 15;
			if ((value = attributes.Remove(ElementTags.LEFT)) != null) {
				if (!Boolean.Parse(value)) border -= Rectangle.LEFT;
			}
			if ((value = attributes.Remove(ElementTags.RIGHT)) != null) {
				if (!Boolean.Parse(value)) border -= Rectangle.RIGHT;				
			}
			if ((value = attributes.Remove(ElementTags.TOP)) != null) {
				if (!Boolean.Parse(value)) border -= Rectangle.TOP;
			}
			if ((value = attributes.Remove(ElementTags.BOTTOM)) != null) {
				if (!Boolean.Parse(value)) border -= Rectangle.BOTTOM;
			}
			this.Border = border;
			string r = attributes.Remove(ElementTags.RED);
			string g = attributes.Remove(ElementTags.GREEN);
			string b = attributes.Remove(ElementTags.BLUE);
			if (r != null || g != null || b != null) {
				int red = 0;
				int green = 0;
				int blue = 0;
				if (r != null) red = int.Parse(r);
				if (g != null) green = int.Parse(g);
				if (b != null) blue = int.Parse(b);
				this.BorderColor = new Color(red, green, blue);
			}
			else if ((value = attributes.Remove(ElementTags.BACKGROUNDCOLOR)) != null) {
				this.BackgroundColor = MarkupParser.decodeColor(value);
			}
			r = attributes.Remove(ElementTags.BGRED);
			g = attributes.Remove(ElementTags.BGGREEN);
			b = attributes.Remove(ElementTags.BGBLUE);
			if (r != null || g != null || b != null) {
				int red = 0;
				int green = 0;
				int blue = 0;
				if (r != null) red = int.Parse(r);
				if (g != null) green = int.Parse(g);
				if (b != null) blue = int.Parse(b);
				this.BackgroundColor = new Color(red, green, blue);
			}
			else if ((value = attributes.Remove(ElementTags.BACKGROUNDCOLOR)) != null) {
				this.BackgroundColor = MarkupParser.decodeColor(value);
			}
			if ((value = attributes.Remove(ElementTags.GRAYFILL)) != null) {
				this.GrayFill = float.Parse(value);
			}
			if (attributes.Count > 0) this.MarkupAttributes = attributes;
		}

		// implementation of the Element-methods

		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// IElementListener.
		/// </summary>
		/// <param name="listener">an IElementListener</param>
		/// <returns>true if the element was processed successfully</returns>
		public override bool process(IElementListener listener) {
			try {
				return listener.Add(this);
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
		public override int Type {
			get {
				return Element.CELL;
			}
		}

		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public override ArrayList Chunks {
			get {
				ArrayList tmp = new ArrayList();
				foreach(IElement ele in arrayList) {
					tmp.AddRange(ele.Chunks);
				}
				return tmp;
			}
		}

		// methods to set the membervariables

		/**
		 * Adds an element to this Cell.
		 * <P>
		 * Remark: you can't add ListItems, Rows, Cells,
		 * JPEGs, GIFs or PNGs to a Cell.
		 *
		 * @param element The Element to add
		 * @throws BadElementException if the method was called with a ListItem, Row or Cell
		 */
		/// <summary>
		/// Adds an element to this Cell.
		/// </summary>
		/// <remarks>
		/// You can't add ListItems, Rows, Cells,
		/// JPEGs, GIFs or PNGs to a Cell.
		/// </remarks>
		/// <param name="element">the Element to add</param>
		public void addElement(IElement element) {
			if (isTable()) {
				Table table = (Table) arrayList[0];
				Cell tmp = new Cell(element);
				tmp.Border = NO_BORDER;
				tmp.Colspan = table.Columns;
				table.addCell(tmp);
				return;
			}
			switch(element.Type) {
				case Element.LISTITEM:
				case Element.ROW:
				case Element.CELL:
					throw new BadElementException("You can't add listitems, rows or cells to a cell.");
				case Element.JPEG:
				case Element.IMGRAW:
				case Element.IMGTEMPLATE:
				case Element.GIF:
				case Element.PNG:
					arrayList.Add(element);
					break;
				case Element.LIST:
					if (float.IsNaN(this.Leading)) {
						leading = ((List) element).Leading;
					}
					if (((List) element).Size == 0) return;
					arrayList.Add(element);
					return;
				case Element.ANCHOR:
				case Element.PARAGRAPH:
				case Element.PHRASE:
					if (float.IsNaN(leading)) {
						leading = ((Phrase) element).Leading;
					}
					if (((Phrase) element).isEmpty()) return;
					arrayList.Add(element);
					return;
				case Element.CHUNK:
					if (((Chunk) element).isEmpty()) return;
					arrayList.Add(element);
					return;
				case Element.TABLE:
					Table table = new Table(3);
					float[] widths = new float[3];
					widths[1] = ((Table)element).WidthPercentage;

				switch(((Table)element).Alignment) {
					case Element.ALIGN_LEFT:
						widths[0] = 0f;
						widths[2] = 100f - widths[1];
						break;
					case Element.ALIGN_CENTER:
						widths[0] = (100f - widths[1]) / 2f;
						widths[2] = widths[0];
						break;
					case Element.ALIGN_RIGHT:
						widths[0] = 100f - widths[1];
						widths[2] = 0f;
						break;
				}
					table.Widths = widths;
					Cell tmp;
					if (arrayList.Count == 0) {
						table.addCell(DUMMY_CELL);
					}
					else {
						tmp = new Cell();
						tmp.Border = NO_BORDER;
						tmp.Colspan = 3;
						foreach(IElement ele in arrayList) {
							tmp.Add(ele);
						}
						table.addCell(tmp);
					}
					tmp = new Cell();
					tmp.Border = NO_BORDER;
					table.addCell(tmp);
					table.insertTable((Table)element);
					table.addCell(tmp);
					table.addCell(DUMMY_CELL);
					clear();
					arrayList.Add(table);
					return;
				default:
					arrayList.Add(element);
					break;
			}
		}

		/// <summary>
		/// Add an Object to this cell.
		/// </summary>
		/// <param name="o">the object to add</param>
		/// <returns>always true</returns>
		public bool Add(Object o) {
			try {
				this.addElement((IElement) o);
				return true;
			}
			catch(BadElementException bee) {
				throw new Exception(bee.Message);
			}
			catch(Exception cce) {
				cce.GetType();
				throw new Exception("You can only add objects that implement the Element interface.");
			}
		}

		/// <summary>
		/// Sets the alignment of this cell.
		/// </summary>
		/// <param name="alignment">the new alignment as a string</param>
		public void setHorizontalAlignment(string alignment) {
			alignment = alignment.ToLower();
			if (ElementTags.ALIGN_CENTER.ToLower().Equals(alignment)) {
				this.HorizontalAlignment = Element.ALIGN_CENTER;
				return;
			}
			if (ElementTags.ALIGN_RIGHT.ToLower().Equals(alignment)) {
				this.HorizontalAlignment = Element.ALIGN_RIGHT;
				return;
			}
			if (ElementTags.ALIGN_JUSTIFIED.ToLower().Equals(alignment)) {
				this.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
				return;
			}
			this.HorizontalAlignment = Element.ALIGN_LEFT;
		}

		/// <summary>
		/// Sets the alignment of this paragraph.
		/// </summary>
		/// <param name="alignment">the new alignment as a string</param>
		public void setVerticalAlignment(string alignment) {
			alignment = alignment.ToLower();
			if (ElementTags.ALIGN_MIDDLE.ToLower().Equals(alignment)) {
				this.VerticalAlignment = Element.ALIGN_MIDDLE;
				return;
			}
			if (ElementTags.ALIGN_BOTTOM.ToLower().Equals(alignment)) {
				this.VerticalAlignment = Element.ALIGN_BOTTOM;
				return;
			}
			if (ElementTags.ALIGN_BASELINE.ToLower().Equals(alignment)) {
				this.VerticalAlignment = Element.ALIGN_BASELINE;
				return;
			}
			this.VerticalAlignment = Element.ALIGN_TOP;
		}

		/// <summary>
		/// Sets the width.
		/// </summary>
		/// <value>the new value</value>
		public new string Width {
			set {
				width = value;
			}
		}

		// methods to retrieve information

		/// <summary>
		/// Gets the number of Elements in the Cell.
		/// </summary>
		/// <value>a size</value>
		public int Size {
			get {
				return arrayList.Count;
			}
		}

		/// <summary>
		/// Checks if the Cell is empty.
		/// </summary>
		/// <returns>false if there are non-empty Elements in the Cell.</returns>
		public bool isEmpty() {
			switch(this.Size) {
				case 0:
					return true;
				case 1:
					IElement element = (IElement)arrayList[0];
				switch (element.Type) {
					case Element.CHUNK:
						return ((Chunk) element).isEmpty();
					case Element.ANCHOR:
					case Element.PHRASE:
					case Element.PARAGRAPH:
						return ((Phrase) element).isEmpty();
					case Element.LIST:
						return ((List) element).Size == 0;						
				}
					return false;
				default:
					return false;
			}
		}

		/// <summary>
		/// Makes sure there is at least 1 object in the Cell.
		/// Otherwise it might not be shown in the table.
		/// </summary>
		internal void fill() {
			if (this.Size == 0) arrayList.Add(new Paragraph(0));
		}

		/// <summary>
		/// Checks if the Cell is empty.
		/// </summary>
		/// <returns>false if there are non-empty Elements in the Cell.</returns>
		public bool isTable() {
			return (this.Size == 1) && (((IElement)arrayList[0]).Type == Element.TABLE);
		}

		/// <summary>
		/// Gets Elements.
		/// </summary>
		/// <value>an ArrayList</value>
		public ArrayList Elements {
			get {
				return arrayList;
			}
		}

		/// <summary>
		/// Gets/Sets the horizontal Element.
		/// </summary>
		/// <value>a value</value>
		public int HorizontalAlignment {
			get {
				return horizontalAlignment;
			}

			set {
				horizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets/sets the vertical Element.
		/// </summary>
		/// <value>a value</value>
		public int VerticalAlignment {
			get {
				return verticalAlignment;
			}

			set {
				verticalAlignment = value;
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>a value</value>
		public string CellWidth {
			get {
				return width;
			}
		}

		/**
		 * Gets the colspan.
		 *
		 * @return	a value
		 */
		/// <summary>
		/// Gets/sets the colspan.
		/// </summary>
		/// <value>a value</value>
		public int Colspan {
			get {
				return colspan;
			}

			set {
				colspan = value;
			}
		}

		/// <summary>
		/// Gets/sets the rowspan.
		/// </summary>
		/// <value>a value</value>
		public int Rowspan {
			get {
				return rowspan;
			}

			set {
				rowspan = value;
			}
		}

		/// <summary>
		/// Gets/sets the leading.
		/// </summary>
		/// <value>a value</value>
		public float Leading {
			get {
				if (float.IsNaN(leading)) {
					return 16;
				}
				return leading;
			}

			set {
				leading = value;
			}
		}

		/// <summary>
		/// Gets/sets header
		/// </summary>
		/// <value>a value</value>
		public bool Header {
			get {
				return header;
			}

			set {
				header = value;
			}
		}

		/**
		 * Get nowrap.
		 *
		 * @return	a value
		 */
		/// <summary>
		/// Get/set nowrap.
		/// </summary>
		/// <value>a value</value>
		public bool NoWrap {
			get {
				return noWrap;
			}

			set {
				noWrap = value;
			}
		}

		/// <summary>
		/// Clears all the Elements of this Cell.
		/// </summary>
		public void clear() {
			arrayList.Clear();
		}

		/// <summary>
		/// This property throws an Exception.
		/// </summary>
		/// <value>none</value>
		public override float Top {
			get {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}

			set {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}
		}

		/// <summary>
		/// This property throws an Exception.
		/// </summary>
		/// <value>none</value>
		public override float Bottom {
			get {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}

			set {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}
		}

		/// <summary>
		/// This property throws an Exception.
		/// </summary>
		/// <value>none</value>
		public override float Left {
			get {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}

			set {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}
		}

		/// <summary>
		/// This property throws an Exception.
		/// </summary>
		/// <value>none</value>
		public override float Right {
			get {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}

			set {
				throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
			}
		}

		/// <summary>
		/// This method throws an Exception.
		/// </summary>
		/// <param name="margin">new value</param>
		/// <returns>none</returns>
		public float top(int margin) {
			throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
		}

		/// <summary>
		/// This method throws an Exception.
		/// </summary>
		/// <param name="margin">new value</param>
		/// <returns>none</returns>
		public float bottom(int margin) {
			throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
		}

		/// <summary>
		/// This method throws an Exception.
		/// </summary>
		/// <param name="margin">new value</param>
		/// <returns>none</returns>
		public float left(int margin) {
			throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
		}

		/// <summary>
		/// This method throws an Exception.
		/// </summary>
		/// <param name="margin">new value</param>
		/// <returns>none</returns>
		public float right(int margin) {
			throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
		}

		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.CELL.Equals(tag);
		}
	}
}
