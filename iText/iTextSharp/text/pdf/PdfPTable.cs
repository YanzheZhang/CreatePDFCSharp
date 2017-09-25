using System;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfPTable.cs,v 1.1.1.1 2003/02/04 02:57:40 geraldhenson Exp $
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

	/** This is a table that can be put at an absolute position but can also
	 * be added to the document as the class <CODE>Table</CODE>.
	 * In the last case when crossing pages the table always break at full rows; if a
	 * row is bigger than the page it is dropped silently to avoid infinite loops.
	 * <P>
	 * A PdfPTableEvent can be associated to the table to do custom drawing
	 * when the table is rendered.
	 * @author Paulo Soares (psoares@consiste.pt)
	 */

	public class PdfPTable : IElement{
    
		/** The index of the original <CODE>PdfcontentByte</CODE>.
		 */    
		public static int BASECANVAS = 0;
		/** The index of the duplicate <CODE>PdfContentByte</CODE> where the backgroung will be drawn.
		 */    
		public static int BACKGROUNDCANVAS = 1;
		/** The index of the duplicate <CODE>PdfContentByte</CODE> where the border lines will be drawn.
		 */    
		public static int LINECANVAS = 2;
		/** The index of the duplicate <CODE>PdfContentByte</CODE> where the text will be drawn.
		 */    
		public static int TEXTCANVAS = 3;
    
		protected ArrayList rows = new ArrayList();
		protected float totalHeight = 0;
		protected PdfPCell[] currentRow;
		protected int currentRowIdx = 0;
		protected PdfPCell defaultCell = new PdfPCell((Phrase)null);
		protected float totalWidth = 0;
		protected float[] relativeWidths;
		protected float[] absoluteWidths;
		protected IPdfPTableEvent tableEvent;
    
		/** Holds value of property headerRows. */
		protected int headerRows;
    
		/** Holds value of property widthPercentage. */
		protected float widthPercentage = 80;
    
		/** Holds value of property horizontalElement. */
		private int horizontalAlignment = Element.ALIGN_CENTER;
    
		/** Holds value of property skipFirstHeader. */
		private bool skipFirstHeader = false;

		protected bool isColspan = false;
    
		protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;

		/** Constructs a <CODE>PdfPTable</CODE> with the relative column widths.
		 * @param relativeWidths the relative column widths
		 */    
		public PdfPTable(float[] relativeWidths) {
			if (relativeWidths == null)
				throw new Exception("The widths array in PdfPTable constructor can not be null.");
			if (relativeWidths.Length == 0)
				throw new IllegalArgumentException("The widths array in PdfPTable constructor can not have zero length.");
			this.relativeWidths = new float[relativeWidths.Length];
			Array.Copy(relativeWidths, 0, this.relativeWidths, 0, relativeWidths.Length);
			absoluteWidths = new float[relativeWidths.Length];
			calculateWidths();
			currentRow = new PdfPCell[absoluteWidths.Length];
		}
    
		/** Constructs a <CODE>PdfPTable</CODE> with <CODE>numColumns</CODE> columns.
		 * @param numColumns the number of columns
		 */    
		public PdfPTable(int numColumns) {
			if (numColumns <= 0)
				throw new IllegalArgumentException("The number of columns in PdfPTable constructor must be greater than zero.");
			relativeWidths = new float[numColumns];
			for (int k = 0; k < numColumns; ++k)
				relativeWidths[k] = 1;
			absoluteWidths = new float[relativeWidths.Length];
			calculateWidths();
			currentRow = new PdfPCell[absoluteWidths.Length];
		}
    
		/** Constructs a copy of a <CODE>PdfPTable</CODE>.
		 * @param table the <CODE>PdfPTable</CODE> to be copied
		 */    
		public PdfPTable(PdfPTable table) {
			relativeWidths = new float[table.relativeWidths.Length];
			absoluteWidths = new float[table.relativeWidths.Length];
			Array.Copy(table.relativeWidths, 0, relativeWidths, 0, relativeWidths.Length);
			Array.Copy(table.absoluteWidths, 0, absoluteWidths, 0, relativeWidths.Length);
			totalWidth = table.totalWidth;
			totalHeight = table.totalHeight;
			currentRowIdx = table.currentRowIdx;
			tableEvent = table.tableEvent;
			runDirection = table.runDirection;
			defaultCell = new PdfPCell(table.defaultCell);
			currentRow = new PdfPCell[table.currentRow.Length];
			isColspan = table.isColspan;
			for (int k = 0; k < currentRow.Length; ++k) {
				if (table.currentRow[k] == null)
					break;
				currentRow[k] = new PdfPCell(table.currentRow[k]);
			}
			for (int k = 0; k < table.rows.Count; ++k) {
				rows.Add(new PdfPRow((PdfPRow)(table.rows[k])));
			}
		}
    
		/** Sets the relative widths of the table.
		 * @param relativeWidths the relative widths of the table.
		 * @throws DocumentException if the number of widths is different than tne number
		 * of columns
		 */    
		public void setWidths(float[] relativeWidths) {
			if (relativeWidths.Length != this.relativeWidths.Length)
				throw new DocumentException("Wrong number of columns.");
			this.relativeWidths = new float[relativeWidths.Length];
			Array.Copy(relativeWidths, 0, this.relativeWidths, 0, relativeWidths.Length);
			absoluteWidths = new float[relativeWidths.Length];
			totalHeight = 0;
			calculateWidths();
			calculateHeights();
		}

		/** Sets the relative widths of the table.
		 * @param relativeWidths the relative widths of the table.
		 * @throws DocumentException if the number of widths is different than tne number
		 * of columns
		 */    
		public void setWidths(int[] relativeWidths) {
			float[] tb = new float[relativeWidths.Length];
			for (int k = 0; k < relativeWidths.Length; ++k)
				tb[k] = relativeWidths[k];
			setWidths(tb);
		}

		private void calculateWidths() {
			if (totalWidth <= 0)
				return;
			float total = 0;
			for (int k = 0; k < absoluteWidths.Length; ++k) {
				total += relativeWidths[k];
			}
			for (int k = 0; k < absoluteWidths.Length; ++k) {
				absoluteWidths[k] = totalWidth * relativeWidths[k] / total;
			}
		}
    
		/** Gets the full width of the table.
		 * @return the full width of the table
		 */    
		public float TotalWidth {
			get {
				return totalWidth;
			}

			set {
				if (this.totalWidth == value)
					return;
				this.totalWidth = value;
				totalHeight = 0;
				calculateWidths();
				calculateHeights();
			}
		}

		internal void calculateHeights() {
			if (totalWidth <= 0)
				return;
			totalHeight = 0;
			for (int k = 0; k < rows.Count; ++k) {
				PdfPRow row = (PdfPRow)rows[k];
				row.setWidths(absoluteWidths);
				totalHeight += row.MaxHeights;
			}
		}
    
		/** Gets the default <CODE>PdfPCell</CODE> that will be used as
		 * reference for all the <CODE>addCell</CODE> methods except
		 * <CODE>addCell(PdfPCell)</CODE>.
		 * @return default <CODE>PdfPCell</CODE>
		 */    
		public PdfPCell DefaultCell {
			get {
				return defaultCell;
			}
		}
    
		/** Adds a cell element.
		 * @param cell the cell element
		 */    
		public void addCell(PdfPCell cell) {
			PdfPCell ncell = new PdfPCell(cell);
			int colspan = ncell.Colspan;
			colspan = Math.Max(colspan, 1);
			colspan = Math.Min(colspan, currentRow.Length - currentRowIdx);
			ncell.Colspan = colspan;
			if (colspan != 1)
				isColspan = true;
			int rdir = ncell.RunDirection;
			if (rdir == PdfWriter.RUN_DIRECTION_DEFAULT)
				ncell.RunDirection = runDirection;
			currentRow[currentRowIdx] = ncell;
			currentRowIdx += colspan;
			if (currentRowIdx >= currentRow.Length) {
				if (runDirection == PdfWriter.RUN_DIRECTION_RTL) {
					PdfPCell[] rtlRow = new PdfPCell[absoluteWidths.Length];
					int rev = currentRow.Length;
					for (int k = 0; k < currentRow.Length; ++k) {
						PdfPCell rcell = currentRow[k];
						int cspan = rcell.Colspan;
						rev -= cspan;
						rtlRow[rev] = rcell;
						k += cspan - 1;
					}
					currentRow = rtlRow;
				}
				PdfPRow row = new PdfPRow(currentRow);
				if (totalWidth > 0) {
					row.setWidths(absoluteWidths);
					totalHeight += row.MaxHeights;
				}
				rows.Add(row);
				currentRow = new PdfPCell[absoluteWidths.Length];
				currentRowIdx = 0;
			}
		}
    
		/** Adds a cell element.
		 * @param text the text for the cell
		 */    
		public void addCell(string text) {
			addCell(new Phrase(text));
		}
    
		/** Adds a cell element.
		 * @param table the table to be added to the cell
		 */    
		public void addCell(PdfPTable table) {
			defaultCell.Table = table;
			addCell(defaultCell);
			defaultCell.Table = null;
		}
    
		/** Adds a cell element.
		 * @param phrase the <CODE>Phrase</CODE> to be added to the cell
		 */    
		public void addCell(Phrase phrase) {
			defaultCell.Phrase = phrase;
			addCell(defaultCell);
			defaultCell.Phrase = null;
		}
    
		/**
		 * Writes the selected rows to the document.
		 * <P>
		 * <CODE>canvases</CODE> is obtained from <CODE>beginWrittingRows()</CODE>.
		 * @param rowStart the first row to be written, zero index
		 * @param rowEnd the last row to be written - 1. If it is -1 all the
		 * rows to the end are written
		 * @param xPos the x write coodinate
		 * @param yPos the y write coodinate
		 * @param canvases an array of 4 <CODE>PdfContentByte</CODE> obtained from
		 * <CODE>beginWrittingRows()</CODE>
		 * @return the y coordinate position of the bottom of the last row
		 * @see #beginWritingRows(iTextSharp.text.pdf.PdfContentByte)
		 */    
		public float writeSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte[] canvases) {
			if (totalWidth <= 0)
				throw new RuntimeException("The table width must be greater than zero.");
			int size = rows.Count;
			if (rowEnd < 0)
				rowEnd = size;
			if (rowStart >= size || rowStart >= rowEnd)
				return yPos;
			rowEnd = Math.Min(rowEnd, size);
			float yPosStart = yPos;
			for (int k = rowStart; k < rowEnd; ++k) {
				PdfPRow row = (PdfPRow)rows[k];
				row.writeCells(xPos, yPos, canvases);
				yPos -= row.MaxHeights;
			}
			if (tableEvent != null) {
				float[] heights = new float[rowEnd - rowStart + 1];
				heights[0] = yPosStart;
				for (int k = rowStart; k < rowEnd; ++k) {
					PdfPRow row = (PdfPRow)rows[k];
					heights[k - rowStart + 1] = heights[k - rowStart] - row.MaxHeights;
				}
				tableEvent.tableLayout(this, getEventWidths(xPos, rowStart, rowEnd, false), heights, 0, rowStart, canvases);
			}
			return yPos;
		}
    
		/**
		 * Writes the selected rows to the document.
		 * 
		 * @param rowStart the first row to be written, zero index
		 * @param rowEnd the last row to be written - 1. If it is -1 all the
		 * rows to the end are written
		 * @param xPos the x write coodinate
		 * @param yPos the y write coodinate
		 * @param canvas the <CODE>PdfContentByte</CODE> where the rows will
		 * be written to
		 * @return the y coordinate position of the bottom of the last row
		 */    
		public float writeSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte canvas) {
			PdfContentByte[] canvases = beginWritingRows(canvas);
			float y = writeSelectedRows(rowStart, rowEnd, xPos, yPos, canvases);
			endWritingRows(canvases);
			return y;
		}
    
		/** Gets and initializes the 4 layers where the table is written to. The text or graphics are added to
		 * one of the 4 <CODE>PdfContentByte</CODE> returned with the following order:<p>
		 * <ul>
		 * <li><CODE>PdfPtable.BASECANVAS</CODE> - the original <CODE>PdfContentByte</CODE>. Anything placed here
		 * will be under the table.
		 * <li><CODE>PdfPtable.BACKGROUNDCANVAS</CODE> - the layer where the background goes to.
		 * <li><CODE>PdfPtable.LINECANVAS</CODE> - the layer where the lines go to.
		 * <li><CODE>PdfPtable.TEXTCANVAS</CODE> - the layer where the text go to. Anything placed here
		 * will be over the table.
		 * </ul><p>
		 * The layers are placed in sequence on top of each other.
		 * @param canvas the <CODE>PdfContentByte</CODE> where the rows will
		 * be written to
		 * @return an array of 4 <CODE>PdfContentByte</CODE>
		 * @see #writeSelectedRows(int, int, float, float, PdfContentByte[])
		 */    
		public static PdfContentByte[] beginWritingRows(PdfContentByte canvas) {
			return new PdfContentByte[]{
										   canvas,
										   canvas.Duplicate,
										   canvas.Duplicate,
										   canvas.Duplicate,
			};
		}
    
		/** Finishes writing the table.
		 * @param canvases the array returned by <CODE>beginWritingRows()</CODE>
		 */    
		public static void endWritingRows(PdfContentByte[] canvases) {
			PdfContentByte canvas = canvases[BASECANVAS];
			canvas.saveState();
			canvas.Add(canvases[BACKGROUNDCANVAS]);
			canvas.restoreState();
			canvas.saveState();
			canvas.LineCap = 2;
			canvas.resetRGBColorStroke();
			canvas.Add(canvases[LINECANVAS]);
			canvas.restoreState();
			canvas.Add(canvases[TEXTCANVAS]);
		}
    
		/** Gets the number of rows in this table.
		 * @return the number of rows in this table
		 */    
		public int Size {
			get {
				return rows.Count;
			}
		}
    
		/** Gets the total height of the table.
		 * @return the total height of the table
		 */    
		public float TotalHeight {
			get {
				return totalHeight;
			}
		}
    
		/** Gets the height of a particular row.
		 * @param idx the row index (starts at 0)
		 * @return the height of a particular row
		 */    
		public float getRowHeight(int idx) {
			if (totalWidth <= 0 || idx < 0 || idx >= rows.Count)
				return 0;
			PdfPRow row = (PdfPRow)rows[idx];
			return row.MaxHeights;
		}
    
		/** Gets the height of the rows that constitute the header as defined by
		 * <CODE>setHeaderRows()</CODE>.
		 * @return the height of the rows that constitute the header
		 */    
		public float HeaderHeight {
			get {
				float total = 0;
				int size = Math.Min(rows.Count, headerRows);
				for (int k = 0; k < size; ++k) {
					PdfPRow row = (PdfPRow)rows[k];
					total += row.MaxHeights;
				}
				return total;
			}
		}
    
		/** Deletes a row from the table.
		 * @param rowNumber the row to be deleted
		 * @return <CODE>true</CODE> if the row was deleted
		 */    
		public bool deleteRow(int rowNumber) {
			if (rowNumber < 0 || rowNumber >= rows.Count) {
				return false;
			}
			if (totalWidth > 0) {
				PdfPRow row = (PdfPRow)rows[rowNumber];
				totalHeight -= row.MaxHeights;
			}
			rows.RemoveAt(rowNumber);
			return true;
		}
    
		/** Deletes the last row in the table.
		 * @return <CODE>true</CODE> if the last row was deleted
		 */    
		public bool deleteLastRow() {
			return deleteRow(rows.Count - 1);
		}
    
		/** Gets the number of the rows that constitute the header.
		 * @return the number of the rows that constitute the header
		 */
		public int HeaderRows {
			get {
				return headerRows;
			}

			set {
				if (value < 0)
					value = 0;
				this.headerRows = value;
			}
		}
    
		/**
		 * Gets all the chunks in this element.
		 *
		 * @return	an <CODE>ArrayList</CODE>
		 */
		public ArrayList Chunks {
			get {
				return new ArrayList();
			}
		}
    
		/**
		 * Gets the type of the text element.
		 *
		 * @return	a type
		 */
		public int Type {
			get {
				return Element.PTABLE;
			}
		}
    
		/**
		 * Processes the element by adding it (or the different parts) to an
		 * <CODE>ElementListener</CODE>.
		 *
		 * @param	listener	an <CODE>ElementListener</CODE>
		 * @return	<CODE>true</CODE> if the element was processed successfully
		 */
		public bool process(IElementListener listener) {
			try {
				return listener.Add(this);
			}
			catch(DocumentException de) {
				de.GetType();
				return false;
			}
		}
    
		/** Gets the width percentage that the table will occupy in the page.
		 * @return the width percentage that the table will occupy in the page
		 */
		public float WidthPercentage {
			get {
				return widthPercentage;
			}

			set {
				this.widthPercentage = value;
			}
		}
    
		/** Gets the horizontal alignment of the table relative to the page.
		 * @return the horizontal alignment of the table relative to the page
		 */
		public int HorizontalAlignment{
			get {
				return horizontalAlignment;
			}

			set {
				this.horizontalAlignment = value;
			}
		}
    
		//add by Jin-Hsia Yang
		internal PdfPRow getRow(int idx) {
			return (PdfPRow)rows[idx];
		}
		//end add

		/** Gets the table event for this page.
		 * @return the table event for this page
		 */    
		public IPdfPTableEvent TableEvent {
			get {
				return tableEvent;
			}

			set {
				this.tableEvent = value;
			}
		}
    
		public float[] AbsoluteWidths {
			get {
				return absoluteWidths;
			}
		}
    
		internal float [][] getEventWidths(float xPos, int firstRow, int lastRow, bool includeHeaders) {
			float[][] widths = new float[(includeHeaders ? headerRows : 0) + lastRow - firstRow][];
			if (isColspan) {
				int n = 0;
				if (includeHeaders) {
					for (int k = 0; k < headerRows; ++k)
						widths[n++] = ((PdfPRow)rows[k]).getEventWidth(xPos);
				}
				for (; firstRow < lastRow; ++firstRow)
					widths[n++] = ((PdfPRow)rows[firstRow]).getEventWidth(xPos);
			}
			else {
				float[] width = new float[absoluteWidths.Length + 1];
				width[0] = xPos;
				for (int k = 0; k < absoluteWidths.Length; ++k)
					width[k + 1] = width[k] + absoluteWidths[k];
				for (int k = 0; k < widths.Length; ++k)
					widths[k] = width;
			}
			return widths;
		}


		/** Getter for property skipFirstHeader.
		 * @return Value of property skipFirstHeader.
		 */
		public bool SkipFirstHeader {
			get {
				return skipFirstHeader;
			}

			set {
				this.skipFirstHeader = value;
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