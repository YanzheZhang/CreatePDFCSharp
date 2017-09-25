using System;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfCell.cs,v 1.1.1.1 2003/02/04 02:57:05 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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

	/**
	 * A <CODE>PdfCell</CODE> is the PDF translation of a <CODE>Cell</CODE>.
	 * <P>
	 * A <CODE>PdfCell</CODE> is an <CODE>ArrayList</CODE> of <CODE>PdfLine</CODE>s.
	 *
	 * @see		iTextSharp.text.Rectangle
	 * @see		iTextSharp.text.Cell
	 * @see		PdfLine
	 * @see		PdfTable
	 */

	public class PdfCell : Rectangle {
    
		// membervariables
    
		/** These are the PdfLines in the Cell. */
		private ArrayList lines;
    
		/** These are the PdfLines in the Cell. */
		private PdfLine line;
    
		/** These are the Images in the Cell. */
		private ArrayList images;
    
		/** This is the leading of the lines. */
		private float leading;
    
		/** This is the number of the row the cell is in. */
		private int rownumber;
    
		/** This is the rowspan of the cell. */
		private int rowspan;
    
		/** This is the cellspacing of the cell. */
		private float cellspacing;
    
		/** This is the cellpadding of the cell. */
		private float cellpadding;
    
		/** Indicates if this cell belongs to the header of a <CODE>PdfTable</CODE> */
		private bool header = false;
    
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfCell</CODE>-object.
		 *
		 * @param	cell		the original <CODE>Cell</CODE>
		 * @param	rownumber	the number of the <CODE>Row</CODE> the <CODE>Cell</CODE> was in.
		 * @param	left		the left border of the <CODE>PdfCell</CODE>
		 * @param	right		the right border of the <CODE>PdfCell</CODE>
		 * @param	top			the top border of the <CODE>PdfCell</CODE>
		 * @param	cellspacing	the cellspacing of the <CODE>Table</CODE>
		 * @param	cellpadding	the cellpadding	of the <CODE>Table</CODE>
		 */
    
		public PdfCell(Cell cell, int rownumber, float left, float right, float top, float cellspacing, float cellpadding) : base(left, top, right, top) {
			// copying the attributes from class Cell
			this.Border = cell.Border;
			this.BorderWidth = cell.BorderWidth;
			this.BorderColor = cell.BorderColor;
			this.BackgroundColor = cell.BackgroundColor;
			this.GrayFill = cell.GrayFill;
        
			// initialisation of some parameters
			PdfChunk chunk;
			PdfChunk overflow;
			lines = new ArrayList();
			images = new ArrayList();
			leading = cell.Leading;
			int alignment = cell.HorizontalAlignment;
			left += cellspacing + cellpadding;
			right -= cellspacing + cellpadding;
        
			float height = leading + cellpadding;
			float rowSpan = (float)cell.Rowspan;
        
			switch(cell.VerticalAlignment) {
				case Element.ALIGN_BOTTOM:
					height *= rowSpan;
					break;
				case Element.ALIGN_MIDDLE:
					height *= (float)(rowSpan / 1.5);
					break;
				default:
					height -= cellpadding * 0.4f;
					break;
			}
        
			line = new PdfLine(left, right, alignment, height);
        
			ArrayList allActions;
			int aCounter;
			// we loop over all the elements of the cell
			foreach(IElement ele in cell.Elements) {
				switch(ele.Type) {
					case Element.JPEG:
					case Element.IMGRAW:
					case Element.IMGTEMPLATE:
					case Element.GIF:
					case Element.PNG:
						height = addImage((Image)ele, left, right, height, alignment);
						break;
						// if the element is a list
					case Element.LIST:
						if (line.Size > 0) {
							line.resetAlignment();
							lines.Add(line);
						}
						allActions = new ArrayList();
						processActions(ele, null, allActions);
						aCounter = 0;
						// we loop over all the listitems
						foreach(ListItem item in ((List)ele).Items) {
							line = new PdfLine(left + item.IndentationLeft, right, alignment, leading);
							line.ListItem = item;
							foreach(Chunk c in item.Chunks) {
								chunk = new PdfChunk(c, (PdfAction)(allActions[aCounter++]));
								while ((overflow = line.Add(chunk)) != null) { 
									lines.Add(line);
									line = new PdfLine(left + item.IndentationLeft, right, alignment, leading);
									chunk = overflow;
								}
								line.resetAlignment();
								lines.Add(line);
								line = new PdfLine(left + item.IndentationLeft, right, alignment, leading);
							}
						}
						line = new PdfLine(left, right, alignment, leading);
						break;
						// if the element is something else
					default:
						allActions = new ArrayList();
						processActions(ele, null, allActions);
						aCounter = 0;
						// we loop over the chunks
						foreach(Chunk c in ele.Chunks) {
							chunk = new PdfChunk(c, (PdfAction)(allActions[aCounter++]));
							while ((overflow = line.Add(chunk)) != null) {
								lines.Add(line);
								line = new PdfLine(left, right, alignment, leading);
								chunk = overflow;
							}
						}
						// if the element is a paragraph, section or chapter, we reset the alignment and add the line
						switch (ele.Type) {
							case Element.PARAGRAPH:
							case Element.SECTION:
							case Element.CHAPTER:
								line.resetAlignment();
								lines.Add(line);
								line = new PdfLine(left, right, alignment, leading);
								break;
						}
						break;
				}
			}
			if (line.Size > 0) {
				lines.Add(line);
			}
			// we set some additional parameters
			this.Bottom = top - leading * (lines.Count - 1) - cellpadding - height - 2 * cellspacing;
			this.cellpadding = cellpadding;
			this.cellspacing = cellspacing;
        
			rowspan = cell.Rowspan;
			this.rownumber = rownumber;
		}
    
		// overriding of the Rectangle methods
    
		/**
		 * Returns the lower left x-coordinaat.
		 *
		 * @return		the lower left x-coordinaat
		 */
    
		public override float Left {
			get {
				return base.left(cellspacing);
			}
		}
    
		/**
		 * Returns the upper right x-coordinate.
		 *
		 * @return		the upper right x-coordinate
		 */
    
		public override float Right {
			get {
				return base.right(cellspacing);
			}
		}
    
		/**
		 * Returns the upper right y-coordinate.
		 *
		 * @return		the upper right y-coordinate
		 */
    
		public override float Top {
			get {
				return base.top(cellspacing);
			}
		}
    
		/**
		 * Returns the lower left y-coordinate.
		 *
		 * @return		the lower left y-coordinate
		 */
    
		public override float Bottom {
			get {
				return base.bottom(cellspacing);
			}
		}
    
		// methods
    
		/**
		 * Adds an image to this Cell.
		 *
		 * @param   image   the image to add
		 * @param   left    the left border
		 * @param   right   the right border
		 */
    
		private float addImage(Image imageOrg, float left, float right, float height, int alignment) {
			Image image = Image.getInstance(imageOrg);
			if (image.ScaledWidth > right - left) {
				image.scaleToFit(right - left, float.MaxValue);
			}
			if (line.Size != 0) lines.Add(line);
			line = new PdfLine(left, right, alignment, image.ScaledHeight + 0.4f * leading);
			lines.Add(line);
			line = new PdfLine(left, right, alignment, leading);
			switch (image.Alignment & Image.MIDDLE) {
				case Image.RIGHT:
					left = right - image.ScaledWidth;
					break;
				case Image.MIDDLE:
					left = left + ((right - left - image.ScaledWidth) / 2f);
					break;
				case Image.LEFT:
				default:
					break;
			}
			image.setAbsolutePosition(left, height + (lines.Count - 2) * leading + image.ScaledHeight + 0.4f * leading);
			images.Add(image);
			return height + image.ScaledHeight + 0.4f * leading;
		}
    
		/**
		 * Gets the lines of a cell that can be drawn between certain limits.
		 * <P>
		 * Remark: all the lines that can be drawn are removed from the object!
		 *
		 * @param	top		the top of the part of the table that can be drawn
		 * @param	bottom	the bottom of the part of the table that can be drawn
		 * @return	an <CODE>ArrayList</CODE> of <CODE>PdfLine</CODE>s
		 */
    
		public ArrayList getLines(float top, float bottom) {
        
			// if the bottom of the page is higher than the top of the cell: do nothing
			if (this.Top < bottom) {
				return null;
			}
        
			// initialisations
			float lineHeight;
			float currentPosition = Math.Min(this.Top, top);
			this.Top = currentPosition + cellspacing;
			ArrayList result = new ArrayList();
        
			// we loop over the lines
			int size = lines.Count;
			bool aboveBottom = true;
			for (int i = 0; i < size && aboveBottom; i++) {
				line = (PdfLine) lines[i];
				lineHeight = line.Height;
				currentPosition -= lineHeight;
				// if the currentPosition is higher than the bottom, we add the line to the result
				if (currentPosition > (bottom + cellpadding)) { // bugfix by Tom Ring and Veerendra Namineni
					result.Add(line);
				}
				else {
					aboveBottom = false;
				}
			}
			// if the bottom of the cell is higher than the bottom of the page, the cell is written, so we can remove all lines
			float difference = 0f;
			if (!header) {
				if (aboveBottom) {
					lines = new ArrayList();
				}
				else {
					size = result.Count;
					for (int i = 0; i < size; i++) {
						line = (PdfLine)lines[0];
						lines.RemoveAt(0);
						difference += line.Height;
					}
				}
			}
			if (difference > 0) {
				foreach(Image image in images) {
					image.setAbsolutePosition(image.AbsoluteX, image.AbsoluteY - difference - leading);
				}
			}
			return result;
		}
    
		/**
		 * Gets the images of a cell that can be drawn between certain limits.
		 * <P>
		 * Remark: all the lines that can be drawn are removed from the object!
		 *
		 * @param	top		the top of the part of the table that can be drawn
		 * @param	bottom	the bottom of the part of the table that can be drawn
		 * @return	an <CODE>ArrayList</CODE> of <CODE>Image</CODE>s
		 */
    
		public ArrayList getImages(float top, float bottom) {
        
			// if the bottom of the page is higher than the top of the cell: do nothing
			if (this.Top < bottom) {
				return new ArrayList();
			}
			top = Math.Min(this.Top, top);
			// initialisations
			float height;
			ArrayList result = new ArrayList();
			// we loop over the images
			ArrayList remove = new ArrayList();
			foreach(Image image in images) {
				height = image.AbsoluteY;
				// if the currentPosition is higher than the bottom, we add the line to the result
				if (top - height > (bottom + cellpadding)) {
					image.setAbsolutePosition(image.AbsoluteX, top - height);
					result.Add(image);
					remove.Add(image);
				}
			}
			foreach(Image image in remove) {
				images.Remove(image);
			}
			return result;
		}
    
		/**
		 * Checks if this cell belongs to the header of a <CODE>PdfTable</CODE>.
		 *
		 * @return	<CODE>void</CODE>
		 */
    
		internal bool isHeader() {
			return header;
		}
    
		/**
		 * Indicates that this cell belongs to the header of a <CODE>PdfTable</CODE>.
		 */
    
		internal bool Header {
			set {
				header = value;
			}
		}
    
		/**
		 * Checks if the cell may be removed.
		 * <P>
		 * Headers may allways be removed, even if they are drawn only partially:
		 * they will be repeated on each following page anyway!
		 *
		 * @return	<CODE>true</CODE> if all the lines are allready drawn; <CODE>false</CODE> otherwise.
		 */
    
		internal bool mayBeRemoved() {
			return (header || (lines.Count == 0 && images.Count == 0));
		}
    
		/**
		 * Returns the number of lines in the cell.
		 *
		 * @return	a value
		 */
    
		public int Size {
			get {
				return lines.Count;
			}
		}
    
		/**
		 * Returns the number of lines in the cell that are not empty.
		 *
		 * @return	a value
		 */
    
		public int RemainingLines {
			get {
				if (lines.Count == 0) return 0;
				int result = 0;
				int size = lines.Count;
				PdfLine line;
				for (int i = 0; i < size; i++) {
					line = (PdfLine) lines[i];
					if (line.Size > 0) result++;
				}
				return result;
			}
		}
    
		/**
		 * Returns the height needed to draw the remaining text.
		 *
		 * @return  a height
		 */
    
		public float RemainingHeight {
			get {
				float result = 0f;
				foreach(Image image in images) {
					result += image.ScaledHeight;
				}
				return this.RemainingLines * leading + 2 * cellpadding + cellspacing + result + leading / 2.5f;
			}
		}
    
		// methods to retrieve membervariables
    
		/**
		 * Gets the leading of a cell.
		 *
		 * @return	the leading of the lines is the cell.
		 */
    
		public float Leading {
			get {
				return leading;
			}
		}
    
		/**
		 * Gets the number of the row this cell is in..
		 *
		 * @return	a number
		 */
    
		public int Rownumber {
			get {
				return rownumber;
			}
		}
    
		/**
		 * Gets the rowspan of a cell.
		 *
		 * @return	the rowspan of the cell
		 */
    
		public int Rowspan {
			get {
				return rowspan;
			}
		}
    
		/**
		 * Gets the cellspacing of a cell.
		 *
		 * @return	a value
		 */
    
		public float Cellspacing {
			get {
				return cellspacing;
			}
		}
    
		/**
		 * Gets the cellpadding of a cell..
		 *
		 * @return	a value
		 */
    
		public float Cellpadding {
			get {
				return cellpadding;
			}
		}
    
		/**
		 * Processes all actions contained in the cell.
		 */
    
		protected void processActions(IElement element, PdfAction action, ArrayList allActions) {
			if (element.Type == Element.ANCHOR) {
				string url = ((Anchor)element).Reference;
				if (url != null) {
					action = new PdfAction(url);
				}
			}
			switch (element.Type) {
				case Element.PHRASE:
				case Element.SECTION:
				case Element.ANCHOR:
				case Element.CHAPTER:
				case Element.LISTITEM:
				case Element.PARAGRAPH:
					foreach(IElement ele in ((ArrayList)element)) {
						processActions(ele, action, allActions);
					}
					break;
				case Element.CHUNK:
					allActions.Add(action);
					break;
				case Element.LIST:
					foreach(IElement ele in ((List)element).Items) {
						processActions(ele, action, allActions);
					}
					break;
				default:
					ArrayList tmp = element.Chunks;
					int n = element.Chunks.Count;
					while (n-- > 0)
						allActions.Add(action);
					break;
			}
		}
	}
}