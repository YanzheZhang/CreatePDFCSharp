using System;
using System.IO;
using System.Collections;

using iTextSharp.text;

/**
 * $Id: RtfCell.cs,v 1.5 2003/03/22 22:44:38 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 by Mark Hall
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
 * LGPL license (the “GNU LIBRARY GENERAL PUBLIC LICENSE”), in which case the
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

namespace iTextSharp.text.rtf {
	/// <summary>
	/// A Helper Class for the RtfWriter.
	/// </summary>
	/// <remarks>
	/// Do not use it directly
	/// </remarks>
	public class RtfCell {
		/// <summary> Constants for merging Cells </summary>
    
		/// <summary> A possible value for merging </summary>
		private const int MERGE_HORIZ_FIRST = 1;
		/// <summary> A possible value for merging </summary>
		private const int MERGE_VERT_FIRST = 2;
		/// <summary> A possible value for merging </summary>
		private const int MERGE_BOTH_FIRST = 3;
		/// <summary> A possible value for merging </summary>
		private const int MERGE_HORIZ_PREV = 4;
		/// <summary> A possible value for merging </summary>
		private const int MERGE_VERT_PREV = 5;
		/// <summary> A possible value for merging </summary>
		private const int MERGE_BOTH_PREV = 6;
    
		/**
		 * RTF Tags
		 */
    
		/// <summary> First cell to merge with - Horizontal </summary>
		private static byte[] cellMergeFirst = System.Text.ASCIIEncoding.ASCII.GetBytes("clmgf");
		/// <summary> First cell to merge with - Vertical </summary>
		private static byte[] cellVMergeFirst = System.Text.ASCIIEncoding.ASCII.GetBytes("clvmgf");
		/// <summary> Merge cell with previous horizontal cell </summary>
		private static byte[] cellMergePrev = System.Text.ASCIIEncoding.ASCII.GetBytes("clmrg");
		/// <summary> Merge cell with previous vertical cell </summary>
		private static byte[] cellVMergePrev = System.Text.ASCIIEncoding.ASCII.GetBytes("clvmrg");
		/// <summary> Cell content vertical alignment bottom </summary>
		private static byte[] cellVerticalAlignBottom = System.Text.ASCIIEncoding.ASCII.GetBytes("clvertalb");
		/// <summary> Cell content vertical alignment center </summary>
		private static byte[] cellVerticalAlignCenter = System.Text.ASCIIEncoding.ASCII.GetBytes("clvertalc");
		/// <summary> Cell content vertical alignment top </summary>
		private static byte[] cellVerticalAlignTop = System.Text.ASCIIEncoding.ASCII.GetBytes("clvertalt");
		/// <summary> Cell border left </summary>
		private static byte[] cellBorderLeft = System.Text.ASCIIEncoding.ASCII.GetBytes("clbrdrl");
		/// <summary> Cell border right </summary>
		private static byte[] cellBorderRight = System.Text.ASCIIEncoding.ASCII.GetBytes("clbrdrr");
		/// <summary> Cell border top </summary>
		private static byte[] cellBorderTop = System.Text.ASCIIEncoding.ASCII.GetBytes("clbrdrt");
		/// <summary> Cell border bottom </summary>
		private static byte[] cellBorderBottom = System.Text.ASCIIEncoding.ASCII.GetBytes("clbrdrb");
		/// <summary> Cell background color </summary>
		private static byte[] cellBackgroundColor = System.Text.ASCIIEncoding.ASCII.GetBytes("clcbpat");
		/// <summary> Cell width format </summary>
		private static byte[] cellWidthStyle = System.Text.ASCIIEncoding.ASCII.GetBytes("clftsWidth3");
		/// <summary> Cell width </summary>
		private static byte[] cellWidthTag = System.Text.ASCIIEncoding.ASCII.GetBytes("clwWidth");
		/// <summary> Cell right border position </summary>
		private static byte[] cellRightBorder = System.Text.ASCIIEncoding.ASCII.GetBytes("cellx");
		/// <summary> Cell is part of table </summary>
		internal static byte[] cellInTable = System.Text.ASCIIEncoding.ASCII.GetBytes("intbl");
		/// <summary> End of cell </summary>
		private static byte[] cellEnd = System.Text.ASCIIEncoding.ASCII.GetBytes("cell");
    
		/// <summary> padding top </summary>
		private static byte[] cellPaddingTop = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadt");
		/// <summary> padding top unit </summary>
		private static byte[] cellPaddingTopUnit = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadft3");
		/// <summary> padding bottom </summary>
		private static byte[] cellPaddingBottom = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadb");
		/// <summary> padding bottom unit </summary>
		private static byte[] cellPaddingBottomUnit = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadfb3");
		/// <summary> padding left </summary>
		private static byte[] cellPaddingLeft = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadl");
		/// <summary> padding left unit </summary>
		private static byte[] cellPaddingLeftUnit = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadfl3");
		/// <summary> padding right </summary>
		private static byte[] cellPaddingRight = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadr");
		/// <summary> padding right unit </summary>
		private static byte[] cellPaddingRightUnit = System.Text.ASCIIEncoding.ASCII.GetBytes("clpadfr3");
    
		/// <summary> The RtfWriter to which this RtfCell belongs. </summary>
		private RtfWriter writer = null;
		/// <summary> The RtfTable to which this RtfCell belongs. </summary>
		private RtfTable mainTable = null;
    
		/// <summary> Cell width </summary>
		private int cellWidth = 0;
		/// <summary> Cell right border position </summary>
		private int cellRight = 0;
		/// <summary> Cell containing the actual data </summary>
		private Cell store = null;
		/// <summary> Is this an empty cell </summary>
		private bool emptyCell = true;
		/// <summary> Type of merging to do </summary>
		private int mergeType = 0;
		/// <summary>
		/// cell padding, because the table only renders the left and right cell padding 
		/// and not the top and bottom one 
		/// </summary>
		private int cellpadding = 0;

		/// <summary>
		/// Create a new RtfCell.
		/// </summary>
		/// <param name="writer">The RtfWriter that this RtfCell belongs to</param>
		/// <param name="mainTable">
		/// The RtfTable that created the
		/// RtfRow that created the RtfCell :-)
		/// </param>
		public RtfCell(RtfWriter writer, RtfTable mainTable) : base() {
			this.writer = writer;
			this.mainTable = mainTable;
		}
    
		/// <summary>
		/// Import a Cell.
		/// </summary>
		/// <param name="cell">The Cell containing the data for this RtfCell</param>
		/// <param name="cellLeft">The position of the left border</param>
		/// <param name="cellWidth">The default width of a cell</param>
		/// <param name="x">The column index of this RtfCell</param>
		/// <param name="y">The row index of this RtfCell</param>
		/// <param name="cellpadding"></param>
		/// <returns>an integer</returns>
		public int importCell(Cell cell, int cellLeft, int cellWidth, int x, int y, int cellpadding) {
			//        System.err.println( this.getClass().getName() + "Cell: " + cell + " left: " 
			//                + cellLeft + " width: " 
			//                + cellWidth + " x: " + x + " y: " + y);
			this.cellpadding = cellpadding;

			// set this value in any case
			this.cellWidth = cellWidth;
			if(cell == null) {
				cellRight = cellLeft + cellWidth;
				return cellRight;
			}
			if(cell.CellWidth != null && !cell.CellWidth.Equals("")) {
				//            System.err.println( this.getClass().getName() + "Cell.cellWidth: " 
				//                + cell.CellWidth);

				this.cellWidth = (int) (int.Parse(cell.CellWidth) *
					RtfWriter.twipsFactor);
			}
			//        else
			//        {
			//            this.cellWidth = cellWidth;
			//        }
			cellRight = cellLeft + this.cellWidth;
			store = cell;
			emptyCell = false;
			if(cell.Colspan > 1) {
				if(cell.Rowspan > 1) {
					mergeType = MERGE_BOTH_FIRST;
					for(int i = y; i < y + cell.Rowspan; i++) {
						if(i > y) mainTable.setMerge(x, i, MERGE_VERT_PREV, this);
						for(int j = x + 1; j < x + cell.Colspan; j++) {
							mainTable.setMerge(j, i, MERGE_BOTH_PREV, this);
						}
					}
				}
				else {
					mergeType = MERGE_HORIZ_FIRST;
					for(int i = x + 1; i < x + cell.Colspan; i++) {
						mainTable.setMerge(i, y, MERGE_HORIZ_PREV, this);
					}
				}
			}
			else if(cell.Rowspan > 1) {
				mergeType = MERGE_VERT_FIRST;
				for(int i = y + 1; i < y + cell.Rowspan; i++) {
					mainTable.setMerge(x, i, MERGE_VERT_PREV, this);
				}
			}
			return cellRight;
		}
    
		/// <summary>
		/// Write the properties of the RtfCell.
		/// </summary>
		/// <param name="os">
		/// The Stream to which to write the properties of the RtfCell to.
		/// </param>
		/// <returns></returns>
		public bool writeCellSettings(MemoryStream os) {
			try {
				float lWidth, tWidth, rWidth, bWidth;
				byte[] lStyle, tStyle, rStyle, bStyle;

				if (store is RtfTableCell) {
					RtfTableCell c = (RtfTableCell)store;
					lWidth = c.LeftBorderWidth;
					tWidth = c.TopBorderWidth;
					rWidth = c.RightBorderWidth;
					bWidth = c.BottomBorderWidth;
					lStyle = RtfTableCell.getStyleControlWord(c.LeftBorderStyle);
					tStyle = RtfTableCell.getStyleControlWord(c.TopBorderStyle);
					rStyle = RtfTableCell.getStyleControlWord(c.RightBorderStyle);
					bStyle = RtfTableCell.getStyleControlWord(c.BottomBorderStyle);
				} else {
					lWidth = tWidth = rWidth = bWidth = store.BorderWidth;
					lStyle = tStyle = rStyle = bStyle = RtfRow.tableBorder;
				}

				// <!-- steffen
				if (mergeType == MERGE_HORIZ_PREV || mergeType == MERGE_BOTH_PREV) {
					return true;                
				}
				switch(mergeType) {
					case MERGE_VERT_FIRST  :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVMergeFirst, 0, cellVMergeFirst.Length);
						break;
					case MERGE_BOTH_FIRST  :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVMergeFirst, 0, cellVMergeFirst.Length);
						break;
					case MERGE_HORIZ_PREV  :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellMergePrev, 0, cellMergePrev.Length);
						break;
					case MERGE_VERT_PREV   :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVMergePrev, 0, cellVMergePrev.Length);
						break;
					case MERGE_BOTH_PREV   :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellMergeFirst, 0, cellMergeFirst.Length);
						break;
				}
				// -->
				switch(store.VerticalAlignment) {
					case Element.ALIGN_BOTTOM :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVerticalAlignBottom, 0, cellVerticalAlignBottom.Length);
						break;
					case Element.ALIGN_CENTER :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVerticalAlignCenter, 0, cellVerticalAlignCenter.Length);
						break;
					case Element.ALIGN_TOP    :
						os.WriteByte(RtfWriter.escape);
						os.Write(cellVerticalAlignTop, 0, cellVerticalAlignTop.Length);
						break;
				}

				if(((store.Border & Rectangle.LEFT) == Rectangle.LEFT) &&
					(lWidth > 0)) {
					os.WriteByte(RtfWriter.escape);
					os.Write(cellBorderLeft, 0, cellBorderLeft.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(lStyle, 0, lStyle.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderWidth, 0, RtfRow.tableBorderWidth.Length);
					writeInt(os, (int) (lWidth * RtfWriter.twipsFactor));
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderColor, 0, RtfRow.tableBorderColor.Length);
					if(store.BorderColor == null) writeInt(os, writer.addColor(new
													  Color(0,0,0))); else writeInt(os, writer.addColor(store.BorderColor));
					os.WriteByte((byte) '\n');
				}
				if(((store.Border & Rectangle.TOP) == Rectangle.TOP) && (tWidth> 0)) {
					os.WriteByte(RtfWriter.escape);
					os.Write(cellBorderTop, 0, cellBorderTop.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(tStyle, 0, tStyle.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderWidth, 0, RtfRow.tableBorderWidth.Length);
					writeInt(os, (int) (tWidth * RtfWriter.twipsFactor));
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderColor, 0, RtfRow.tableBorderColor.Length);
					if(store.BorderColor == null) writeInt(os, writer.addColor(new
													  Color(0,0,0))); else writeInt(os, writer.addColor(store.BorderColor));
					os.WriteByte((byte) '\n');
				}
				if(((store.Border & Rectangle.BOTTOM) == Rectangle.BOTTOM) &&
					(bWidth > 0)) {
					os.WriteByte(RtfWriter.escape);
					os.Write(cellBorderBottom, 0, cellBorderBottom.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(bStyle, 0, bStyle.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderWidth, 0, RtfRow.tableBorderWidth.Length);
					writeInt(os, (int) (bWidth * RtfWriter.twipsFactor));
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderColor, 0, RtfRow.tableBorderColor.Length);
					if(store.BorderColor == null) writeInt(os, writer.addColor(new
													  Color(0,0,0))); else writeInt(os, writer.addColor(store.BorderColor));
					os.WriteByte((byte) '\n');
				}
				if(((store.Border & Rectangle.RIGHT) == Rectangle.RIGHT) &&
					(rWidth > 0)) {
					os.WriteByte(RtfWriter.escape);
					os.Write(cellBorderRight, 0, cellBorderRight.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(rStyle, 0, rStyle.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderWidth, 0, RtfRow.tableBorderWidth.Length);
					writeInt(os, (int) (rWidth * RtfWriter.twipsFactor));
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfRow.tableBorderColor, 0, RtfRow.tableBorderColor.Length);
					if(store.BorderColor == null) writeInt(os, writer.addColor(new
													  Color(0,0,0))); else writeInt(os, writer.addColor(store.BorderColor));
					os.WriteByte((byte) '\n');
				}
				os.WriteByte(RtfWriter.escape);
				os.Write(cellBackgroundColor, 0, cellBackgroundColor.Length);
				if(store.BackgroundColor == null) writeInt(os, writer.addColor(new
													  Color(255,255,255))); else writeInt(os,
																					 writer.addColor(store.BackgroundColor));
				os.WriteByte((byte) '\n');
				os.WriteByte(RtfWriter.escape);
				os.Write(cellWidthStyle, 0, cellWidthStyle.Length);
				os.WriteByte((byte) '\n');
				os.WriteByte(RtfWriter.escape);
				os.Write(cellWidthTag, 0, cellWidthTag.Length);
				writeInt(os, cellWidth);
				os.WriteByte((byte) '\n');
				if (cellpadding > 0) {
					// values
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingLeft, 0, cellPaddingLeft.Length);
					writeInt( os, cellpadding / 2);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingTop, 0, cellPaddingTop.Length);
					writeInt( os, cellpadding / 2);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingRight, 0, cellPaddingRight.Length);
					writeInt( os, cellpadding / 2);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingBottom, 0, cellPaddingBottom.Length);
					writeInt( os, cellpadding / 2);
					// unit
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingLeftUnit, 0, cellPaddingLeftUnit.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingTopUnit, 0, cellPaddingTopUnit.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingRightUnit, 0, cellPaddingRightUnit.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellPaddingBottomUnit, 0, cellPaddingBottomUnit.Length);
				}
				os.WriteByte(RtfWriter.escape);
				os.Write(cellRightBorder, 0, cellRightBorder.Length);
				writeInt(os, cellRight);
			}
			catch(IOException e) {
				return false;
			}
			return true;
		}
    
		/// <summary>
		/// Write the content of the RtfCell.
		/// </summary>
		/// <param name="os">
		/// The Stream to which to write the content of the RtfCell to.
		/// </param>
		/// <returns>true is successful</returns>
		public bool writeCellContent(MemoryStream os) {
			try {
				// <!-- steffen
				if (mergeType == MERGE_HORIZ_PREV || mergeType == MERGE_BOTH_PREV) {
					return true;                
				}
				// --> 

				/*            switch(store.horizontalAlignment())
							{
								case Element.ALIGN_LEFT      : os.WriteByte(RtfWriter.escape);
								os.Write(cellHorizontalAlignLeft); break;
								case Element.ALIGN_CENTER    : os.WriteByte(RtfWriter.escape);
								os.Write(cellHorizontalAlignCenter); break;
								case Element.ALIGN_RIGHT     : os.WriteByte(RtfWriter.escape);
								os.Write(cellHorizontalAlignRight); break;
								case Element.ALIGN_JUSTIFIED : os.WriteByte(RtfWriter.escape);
								os.Write(cellHorizontalAlignJustified); break;
							}*/

				if(!emptyCell) {
					for (int i = 0; i < store.Elements.Count; i++) {
						IElement element = (IElement)store.Elements[i];

						// if horizontal alignment is undefined overwrite
						// with that of enclosing cell
						if (element is Paragraph && ((Paragraph)element).Alignment == Element.ALIGN_UNDEFINED) {
							((Paragraph)element).Alignment = store.HorizontalAlignment;
						}
						writer.addElement(element, os);
						if (element.Type == Element.PARAGRAPH && i < store.Elements.Count - 1) {
							os.WriteByte(RtfWriter.escape);
							os.Write(RtfWriter.paragraph, 0, RtfWriter.paragraph.Length);
						}
					}
				}
				else {
					os.WriteByte(RtfWriter.escape);
					os.Write(RtfWriter.paragraphDefaults, 0, RtfWriter.paragraphDefaults.Length);
					os.WriteByte(RtfWriter.escape);
					os.Write(cellInTable, 0, cellInTable.Length);
				}
				os.WriteByte(RtfWriter.escape);
				os.Write(cellEnd, 0, cellEnd.Length);
				//            os.Write((byte)'\n');
			}
			catch(IOException e) {
				return false;
			}
			return true;
		}
    
		/// <summary>
		/// Sets the merge type and the RtfCell with which this
		/// RtfCell is to be merged.
		/// </summary>
		/// <param name="mergeType">
		/// The merge type specifies the kind of merge to be applied
		/// (MERGE_HORIZ_PREV, MERGE_VERT_PREV, MERGE_BOTH_PREV)
		/// </param>
		/// <param name="mergeCell">
		/// The RtfCell that the cell at x and y is to
		/// be merged with
		/// </param>
		public void setMerge(int mergeType, RtfCell mergeCell) {
			this.mergeType = mergeType;
			store = mergeCell.Store;
			// XXX i think that this is false here, because the width must be set in importCell
			// in an colspan, not all cells have the same width
			//        cellWidth = mergeCell.getCellWidth();
		}
    
		/// <summary>
		/// Get the Cell with the actual content.
		/// </summary>
		/// <value>Cell which is contained in the RtfCell</value>
		public Cell Store {
			get {
				return store;
			}
		}
    
		/// <summary>
		/// Get/set the with of this RtfCell
		/// </summary>
		/// <value>Width of teh current RtfCell</value>
		public int CellWidth {
			get {
				return cellWidth;
			}

			set {
				cellWidth = value;
			}
		}

		/// <summary>
		/// Get/set the position of the right border of this RtfCell.
		/// </summary>
		/// <value>the position of the right border of this RtfCell</value>
		public int CellRight {
			get {
				return cellRight;
			}

			set {
				cellRight = value;
			}
		}
    
		/// <summary>
		/// Write an Integer to the Outputstream.
		/// </summary>
		/// <param name="str">the Stream to be written to</param>
		/// <param name="i">The int to be written.</param>
		private void writeInt(MemoryStream str, int i) {
			byte[] tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(i.ToString());
			str.Write(tmp, 0, tmp.Length);
		}
	}
}