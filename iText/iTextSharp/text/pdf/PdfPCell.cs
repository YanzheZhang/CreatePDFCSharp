using System;

using iTextSharp.text;

/*
 * $Id: PdfPCell.cs,v 1.1.1.1 2003/02/04 02:57:37 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 Paulo Soares
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

namespace iTextSharp.text.pdf {

	/** A cell in a PdfPTable.
	 */

	public class PdfPCell : Rectangle{
    
		/** Holds value of property horizontalElement. */
		private int horizontalAlignment = Element.ALIGN_LEFT;
    
		/** Holds value of property verticalElement. */
		private int verticalAlignment = Element.ALIGN_TOP;
    
		/** Holds value of property paddingLeft. */
		private float paddingLeft = 2;
    
		/** Holds value of property paddingLeft. */
		private float paddingRight = 2;
    
		/** Holds value of property paddingTop. */
		private float paddingTop = 2;
    
		/** Holds value of property paddingBottom. */
		private float paddingBottom = 2;
    
		/** The fixed text leading. */
		protected float fixedLeading = 0;
    
		/** The text leading that is multiplied by the biggest font size in the line.
		 */
		protected float multipliedLeading = 1;
    
		/** The extra space between paragraphs. */
		protected float extraParagraphSpace = 0;
    
		/** The first paragraph line indent. */
		protected float indent = 0;
    
		/** The following paragraph lines indent. */
		protected float followingIndent = 0;
    
		/** The right paragraph lines indent. */
		protected float rightIndent = 0;

		/** The text in the cell.
		 */    
		protected Phrase phrase;
    
		/** Holds value of property fixedHeight. */
		private float fixedHeight = 0;
    
		/** Holds value of property noWrap. */
		private bool noWrap = false;
    
		/** Holds value of property table. */
		private PdfPTable table;
    
		/** Holds value of property minimumHeight. */
		private float minimumHeight;
    
		/** Holds value of property colspan. */
		private int colspan = 1;
		private float spaceCharRatio = ColumnText.GLOBAL_SPACE_CHAR_RATIO;
		protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;
		/** Constructs a <CODE>PdfPCell</CODE> with a <CODE>Phrase</CODE>.
		 * The default padding is 2.
		 * @param phrase the text
		 */    
		public PdfPCell(Phrase phrase) : base(0, 0, 0, 0) {
			borderWidth = 0.5f;
			border = BOX;
			this.phrase = phrase;
		}
    
		/** Constructs a <CODE>PdfPCell</CODE> with an <CODE>Image</CODE>.
		 * The default padding is 0.
		 * @param image the <CODE>Image</CODE>
		 */    
		public PdfPCell(Image image) : base(0, 0, 0, 0) {
			borderWidth = 0.5f;
			border = BOX;
			phrase = new Phrase(new Chunk(image, 0, 0));
			fixedLeading = 0;
			multipliedLeading = 1;
			this.Padding = 0;
		}
    
		/** Constructs a <CODE>PdfPCell</CODE> with a <CODE>PdfPtable</CODE>.
		 * This constructor allows nested tables.
		 * The default padding is 0.
		 * @param table The <CODE>PdfPTable</CODE>
		 */    
		public PdfPCell(PdfPTable table) : base(0, 0, 0, 0) {
			borderWidth = 0.5f;
			border = BOX;
			fixedLeading = 0;
			multipliedLeading = 1;
			this.Padding = 0;
			this.table = table;
		}
    
		/** Constructs a deep copy of a <CODE>PdfPCell</CODE>.
		 * @param cell the <CODE>PdfPCell</CODE> to duplicate
		 */    
		public PdfPCell(PdfPCell cell) : base(cell.llx, cell.lly, cell.urx, cell.ury) {
			border = cell.border;
			borderWidth = cell.borderWidth;
			color = cell.color;
			background = cell.background;
			grayFill = cell.grayFill;
			horizontalAlignment = cell.HorizontalAlignment;
			verticalAlignment = cell.verticalAlignment;
			paddingLeft = cell.paddingLeft;
			paddingRight = cell.paddingRight;
			paddingTop = cell.paddingTop;
			paddingBottom = cell.paddingBottom;
			fixedLeading = cell.fixedLeading;
			multipliedLeading = cell.multipliedLeading;
			extraParagraphSpace = cell.extraParagraphSpace;
			indent = cell.indent;
			followingIndent = cell.followingIndent;
			rightIndent = cell.rightIndent;
			phrase = cell.phrase;
			fixedHeight = cell.fixedHeight;
			minimumHeight = cell.minimumHeight;
			noWrap = cell.noWrap;
			colspan = cell.colspan;
			spaceCharRatio = cell.spaceCharRatio;
			runDirection = cell.runDirection;
			if (cell.table != null)
				table = new PdfPTable(cell.table);
		}
    
		/** Gets the <CODE>Phrase</CODE> from this cell.
		 * @return the <CODE>Phrase</CODE>
		 */    
		public Phrase Phrase {
			get {
				return phrase;
			}

			set {
				this.phrase = value;
			}
		}
    
		/** Gets the horizontal alignment for the cell.
		 * @return the horizontal alignment for the cell
		 */
		public int HorizontalAlignment {
			get {
				return horizontalAlignment;
			}

			set {
				this.horizontalAlignment = value;
			}
		}
    
		/** Gets the vertical alignment for the cell.
		 * @return the vertical alignment for the cell
		 */
		public int VerticalAlignment {
			get {
				return verticalAlignment;
			}

			set {
				this.verticalAlignment = value;
			}
		}
    
		/**
		 * @return Value of property paddingLeft.
		 */
		public float PaddingLeft {
			get {
				return paddingLeft;
			}

			set {
				this.paddingLeft = value;
			}
		}
    
		/**
		 * Getter for property paddingRight.
		 * @return Value of property paddingRight.
		 */
		public float PaddingRight {
			get {
				return paddingRight;
			}

			set {
				this.paddingRight = value;
			}
		}
    
		/**
		 * Getter for property paddingTop.
		 * @return Value of property paddingTop.
		 */
		public float PaddingTop {
			get {
				return paddingTop;
			}

			set {
				this.paddingTop = value;
			}
		}
    
		/**
		 * Getter for property paddingBottom.
		 * @return Value of property paddingBottom.
		 */
		public float PaddingBottom {
			get {
				return paddingBottom;
			}

			set {
				this.paddingBottom = value;
			}
		}
    
		public float Padding {
			set {
				paddingBottom = value;
				paddingTop = value;
				paddingLeft = value;
				paddingRight = value;
			}
		}
    
		/**
		 * Sets the leading fixed and variable. The resultant leading will be
		 * fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
		 * size of the bigest font in the line.
		 * @param fixedLeading the fixed leading
		 * @param multipliedLeading the variable leading
		 */
		public void setLeading(float fixedLeading, float multipliedLeading) {
			this.fixedLeading = fixedLeading;
			this.multipliedLeading = multipliedLeading;
		}
    
		/**
		 * Gets the fixed leading
		 * @return the leading
		 */
		public float Leading {
			get {
				return fixedLeading;
			}
		}
    
		/**
		 * Gets the variable leading
		 * @return the leading
		 */
		public float MultipliedLeading {
			get {
				return multipliedLeading;
			}
		}
    
		/**
		 * Gets the first paragraph line indent.
		 * @return the indent
		 */
		public float Indent {
			get {
				return indent;
			}

			set {
				this.indent = value;
			}
		}
    
		/**
		 * Sets the extra space between paragraphs.
		 * @return the extra space between paragraphs
		 */
		public float ExtraParagraphSpace {
			get {
				return extraParagraphSpace;
			}

			set {
				this.extraParagraphSpace = value;
			}
		}
    
		/**
		 * Getter for property fixedHeight.
		 * @return Value of property fixedHeight.
		 */
		public float FixedHeight {
			get {
				return fixedHeight;
			}

			set {
				this.fixedHeight = value;
				minimumHeight = 0;
			}
		}
    
		/**
		 * Getter for property noWrap.
		 * @return Value of property noWrap.
		 */
		public bool isNoWrap() {
			return noWrap;
		}
    
		/**
		 * Setter for property noWrap.
		 * @param noWrap New value of property noWrap.
		 */
		public bool NoWrap {
			set {
				this.noWrap = noWrap;
			}
		}
    
		/**
		 * Getter for property table.
		 * @return Value of property table.
		 */
		internal PdfPTable Table {
			get {
				return table;
			}

			set {
				this.table = value;
			}
		}
    
		/** Getter for property minimumHeight.
		 * @return Value of property minimumHeight.
		 */
		public float MinimumHeight {
			get {
				return minimumHeight;
			}

			set {
				this.minimumHeight = value;
				fixedHeight = 0;
			}
		}
    
		/** Getter for property colspan.
		 * @return Value of property colspan.
		 */
		public int Colspan {
			get {
				return colspan;
			}

			set {
				this.colspan = value;
			}
		}
    
		/**
		 * Gets the following paragraph lines indent.
		 * @return the indent
		 */
		public float FollowingIndent {
			get {
				return followingIndent;
			}

			set {
				this.followingIndent = value;
            }
		}
    
		/**
		 * Gets the right paragraph lines indent.
		 * @return the indent
		 */
		public float RightIndent {
			get {
				return rightIndent;
			}

			set {
				this.rightIndent = value;
			}
		}
    
		/** Gets the space/character extra spacing ratio for
		 * fully justified text.
		 * @return the space/character extra spacing ratio
		 */    
		public float SpaceCharRatio {
			get {
				return spaceCharRatio;
			}

			set {
				this.spaceCharRatio = value;
			}
		}
    
		public int RunDirection {
			get {
				return runDirection;
			}

			set {
				if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
					throw new RuntimeException("Invalid run direction: " + value);
				this.runDirection = value;
			}
		}
	}
}