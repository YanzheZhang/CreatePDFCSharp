using System;
using System.IO;
using System.Collections;

using iTextSharp.text;

/**
 * $Id: RtfRow.cs,v 1.3 2003/03/22 22:01:24 geraldhenson Exp $
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
	public class RtfRow {
		/* Table border solid */
		public static byte[] tableBorder  = System.Text.ASCIIEncoding.ASCII.GetBytes("brdrs");
		/* Table border width */
		public static byte[] tableBorderWidth  = System.Text.ASCIIEncoding.ASCII.GetBytes("brdrw");
		/* Table border color */
		public static byte[] tableBorderColor  = System.Text.ASCIIEncoding.ASCII.GetBytes("brdrcf");
    
		/* Table row defaults */
		private static byte[] rowBegin  = System.Text.ASCIIEncoding.ASCII.GetBytes("trowd");
		/* End of table row */
		private static byte[] rowEnd  = System.Text.ASCIIEncoding.ASCII.GetBytes("row");
		/* Table row autofit */
		private static byte[] rowAutofit  = System.Text.ASCIIEncoding.ASCII.GetBytes("trautofit1");
		private static byte[] graphLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("trgraph");
		/* Row border left */
		private static byte[] rowBorderLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrl");
		/* Row border right */
		private static byte[] rowBorderRight  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrr");
		/* Row border top */
		private static byte[] rowBorderTop  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrt");
		/* Row border bottom */
		private static byte[] rowBorderBottom  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrb");
		/* Row border horiz inline */
		private static byte[] rowBorderInlineHorizontal  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrh");
		/* Row border bottom */
		private static byte[] rowBorderInlineVertical  = System.Text.ASCIIEncoding.ASCII.GetBytes("trbrdrv");
		/* Default cell spacing left */
		private static byte[] rowSpacingLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdl");
		/* Default cell spacing right */
		private static byte[] rowSpacingRight  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdr");
		/* Default cell spacing top */
		private static byte[] rowSpacingTop  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdt");
		/* Default cell spacing bottom */
		private static byte[] rowSpacingBottom  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdb");
		/* Default cell spacing format left */
		private static byte[] rowSpacingLeftStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdfl3");
		/* Default cell spacing format right */
		private static byte[] rowSpacingRightStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdfr3");
		/* Default cell spacing format top */
		private static byte[] rowSpacingTopStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdft3");
		/* Default cell spacing format bottom */
		private static byte[] rowSpacingBottomStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trspdfb3");
		/* Default cell padding left */
		private static byte[] rowPaddingLeft  = System.Text.ASCIIEncoding.ASCII.GetBytes("trpaddl");
		/* Default cell padding right */
		private static byte[] rowPaddingRight  = System.Text.ASCIIEncoding.ASCII.GetBytes("trpaddr");
		/* Default cell padding format left */
		private static byte[] rowPaddingLeftStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trpaddfl3");
		/* Default cell padding format right */
		private static byte[] rowPaddingRightStyle  = System.Text.ASCIIEncoding.ASCII.GetBytes("trpaddfr3");
		/// <summary>
		/// Table row header. This row should appear at the top of every 
		/// page the current table appears on.
		/// </summary>
		private static byte[] rowHeader  = System.Text.ASCIIEncoding.ASCII.GetBytes("trhdr");
		/// <summary>
		/// Table row keep together. This row cannot be split by a page break. 
		/// This property is assumed to be off unless the control word is 
		/// present.
		/// </summary>
		private static byte[] rowKeep  = System.Text.ASCIIEncoding.ASCII.GetBytes("trkeep");
    
		/// <summary> List of RtfCells in this RtfRow </summary>
		private ArrayList cells = new ArrayList();
		/// <summary> The RtfWriter to which this RtfRow belongs </summary>
		private RtfWriter writer = null;
		/// <summary> The RtfTable to which this RtfRow belongs </summary>
		private RtfTable mainTable = null;
    
		/// <summary> The width of this RtfRow (in percent) </summary>
		private int width = 100;
		/// <summary>
		/// The default cellpadding of RtfCells in this
		/// RtfRow
		/// </summary>
		private int cellpadding = 115;
		/// <summary>
		/// The default cellspacing of RtfCells in this
		/// RtfRow
		/// </summary>
		private int cellspacing = 14;
		/// <summary> The borders of this RtfRow </summary>
		private int borders = 0;
		/// <summary> The border color of this RtfRow </summary>
		private Color borderColor = null;
		/// <summary> The border width of this RtfRow </summary>
		private float borderWidth = 0;
    
		/// <summary>
		/// Create a new RtfRow.
		/// </summary>
		/// <param name="writer">The RtfWriter that this RtfRow belongs to</param>
		/// <param name="mainTable">
		/// The RtfTable that created this
		/// RtfRow
		/// </param>
		public RtfRow(RtfWriter writer, RtfTable mainTable) : base() {
			this.writer = writer;
			this.mainTable = mainTable;
		}
    
		/// <summary>
		/// Pregenerate the RtfCells in this RtfRow.
		/// </summary>
		/// <param name="columns">The number of RtfCells to be generated.</param>
		public void pregenerateRows(int columns) {
			for(int i = 0; i < columns; i++) {
				RtfCell rtfCell = new RtfCell(writer, mainTable);
				cells.Add(rtfCell);
			}
		}
    
		/// <summary>
		/// Import a Row.
		/// </summary>
		/// <remarks>
		/// All the parameters are taken from the RtfTable which contains
		/// this RtfRow and they do exactely what they say
		/// </remarks>
		/// <param name="row"></param>
		/// <param name="propWidths">in percent</param>
		/// <param name="tableWidth">in percent</param>
		/// <param name="pageWidth"></param>
		/// <param name="cellpadding"></param>
		/// <param name="cellspacing"></param>
		/// <param name="borders"></param>
		/// <param name="borderColor"></param>
		/// <param name="borderWidth"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool importRow(Row row, float[] propWidths, int tableWidth, int pageWidth, int cellpadding,
			int cellspacing, int borders, Color borderColor, float borderWidth,
			int y) {
			// the width of this row is the absolute witdh, calculated from the
			// proportional with of the table and the total width of the page
			this.width = pageWidth / 100 * tableWidth;
			this.cellpadding = cellpadding;
			this.cellspacing = cellspacing;
			this.borders = borders;
			this.borderColor = borderColor;
			this.borderWidth = borderWidth;
        
			if(this.borderWidth > 2) this.borderWidth = 2;
        
			int cellLeft = 0;
			//        int cellWidth = (int) (((((float) pageWidth) / 100) * width) / row.Columns);
			for(int i = 0; i < row.Columns; i++) {
				IElement cell = (IElement) row.getCell(i);

				// cellWidth is an absolute argument
				// it's based on the absolute of this row and the proportional 
				// width of this column            
				int cellWidth = (int)(width / 100 * propWidths[i]);
				//            System.err.println( this.getClass().getName() + " cellWidth: " + cellWidth + " i: " + i);
				if(cell != null) {
					if(cell.Type == Element.CELL) {
						RtfCell rtfCell = (RtfCell) cells[i];
						cellLeft = rtfCell.importCell((Cell) cell, cellLeft, cellWidth, i, y, cellpadding);
					}
				}
				else {
					RtfCell rtfCell = (RtfCell) cells[i];
					cellLeft = rtfCell.importCell(null, cellLeft, cellWidth, i, y, cellpadding);
				}
			}

			//<!-- steffen
			// recalculate the cell right border and the cumulative width 
			// on col spanning cells.
			// col + row spanning cells are also handled by this loop, because the real cell of
			// the upper left corner in such an col, row matrix is copied as first cell
			// in each row in this matrix
			int columns = row.Columns;
			for(int i = 0; i < columns; i++) {
				RtfCell firstCell = (RtfCell)cells[i];            
				Cell cell = firstCell.Store;
				//            if (elem != null && elem.type() == Element.CELL) {
				int cols = cell.Colspan;
				if (cols > 1) {
					//                    RtfCell firstCell = (RtfCell)cells.get( i);
					RtfCell lastCell = (RtfCell)cells[i + cols - 1];
					firstCell.CellRight = lastCell.CellRight;
					// sum the width of all following spanned cells
					int width = firstCell.CellWidth;
					for (int j = i + 1; j <  i + cols; j++) {
						RtfCell cCell = (RtfCell)cells[j];
						width += cCell.CellWidth;
					}
					firstCell.CellWidth = width;
					i += cols - 1;
				}

				//            }
			}
			//-->
			return true;
		}
    
		/// <summary>
		/// Write the RtfRow to the specified Stream.
		/// </summary>
		/// <param name="os">
		/// The Stream to which this RtfRow
		/// should be written to.
		/// </param>
		/// <param name="rowNum">The index of this row in the containing table.</param>
		/// <param name="table">The Table which contains the original Row.</param>
		/// <returns></returns>
		public bool writeRow(MemoryStream os, int rowNum, Table table) {
			/*        os.WriteByte(RtfWriter.escape);
					os.Write(RtfWriter.paragraphDefaults); */
			os.WriteByte(RtfWriter.escape);
			os.Write(rowBegin, 0, rowBegin.Length);
			os.WriteByte((byte) '\n');
			os.WriteByte(RtfWriter.escape);
			os.Write(rowAutofit, 0, rowAutofit.Length);
			// <!-- steffen
			os.WriteByte(RtfWriter.escape);
			os.Write(rowKeep, 0, rowKeep.Length);
			// check if this row is a header row
			if (rowNum < table.firstDataRow()) {
				//            System.err.println( this.getClass().getName() + " rowNum: " + rowNum 
				//                    + " firstDataRow: " + table.firstDataRow());
				os.WriteByte(RtfWriter.escape);
				os.Write(rowHeader, 0, rowHeader.Length);
			}        
			// -->
			os.WriteByte(RtfWriter.escape);
			os.Write(graphLeft, 0, graphLeft.Length);
			writeInt(os, 10);
			if(((borders & Rectangle.LEFT) == Rectangle.LEFT) && (borderWidth > 0)) {
				writeBorder( os, rowBorderLeft);
			}
			if(((borders & Rectangle.TOP) == Rectangle.TOP) && (borderWidth > 0)) {
				writeBorder( os, rowBorderTop);
			}
			if(((borders & Rectangle.BOTTOM) == Rectangle.BOTTOM) && (borderWidth > 0)) {
				writeBorder( os, rowBorderBottom);
			}
			if(((borders & Rectangle.RIGHT) == Rectangle.RIGHT) && (borderWidth > 0)) {
				writeBorder( os, rowBorderRight);
			}
			if(((borders & Rectangle.BOX) == Rectangle.BOX) && (borderWidth > 0)) {
				writeBorder( os, rowBorderInlineHorizontal);
				writeBorder( os, rowBorderInlineVertical);
			}

			if (cellspacing > 0) {
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingLeft, 0, rowSpacingLeft.Length);
				writeInt(os, cellspacing / 2);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingLeftStyle, 0, rowSpacingLeftStyle.Length);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingTop, 0, rowSpacingTop.Length);
				writeInt(os, cellspacing / 2);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingTopStyle, 0, rowSpacingTopStyle.Length);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingBottom, 0, rowSpacingBottom.Length);
				writeInt(os, cellspacing / 2);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingBottomStyle, 0, rowSpacingBottomStyle.Length);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingRight, 0, rowSpacingRight.Length);
				writeInt(os, cellspacing / 2);
				os.WriteByte(RtfWriter.escape);
				os.Write(rowSpacingRightStyle, 0, rowSpacingRightStyle.Length);
			}
			os.WriteByte(RtfWriter.escape);
			os.Write(rowPaddingLeft, 0, rowPaddingLeft.Length);
			writeInt(os, cellpadding / 2);
			os.WriteByte(RtfWriter.escape);
			os.Write(rowPaddingRight, 0, rowPaddingRight.Length);
			writeInt(os, cellpadding / 2);
			os.WriteByte(RtfWriter.escape);
			os.Write(rowPaddingLeftStyle, 0, rowPaddingLeftStyle.Length);
			os.WriteByte(RtfWriter.escape);
			os.Write(rowPaddingRightStyle, 0, rowPaddingRightStyle.Length);
			os.WriteByte((byte) '\n');
        
			foreach (RtfCell cell in cells) {
				cell.writeCellSettings(os);
			}

			foreach (RtfCell cell in cells) {
				cell.writeCellContent(os);
			}

			os.WriteByte(RtfWriter.delimiter);
			os.WriteByte(RtfWriter.escape);
			os.Write(rowEnd, 0, rowEnd.Length);
			return true;
		}


		private void writeBorder( MemoryStream os, byte[] borderType ) {
			// horizontal and vertical, top, left, bottom, right
			os.WriteByte(RtfWriter.escape);
			os.Write(borderType, 0, borderType.Length);
			// line style        
			os.WriteByte(RtfWriter.escape);
			os.Write(RtfRow.tableBorder, 0, RtfRow.tableBorder.Length);
			// borderwidth
			os.WriteByte(RtfWriter.escape);        
			os.Write(RtfRow.tableBorderWidth, 0, RtfRow.tableBorderWidth.Length);
			writeInt( os, (int)(borderWidth * RtfWriter.twipsFactor));
			// border color        
			os.WriteByte(RtfWriter.escape);
			os.Write(RtfRow.tableBorderColor, 0, RtfRow.tableBorderColor.Length);
			if (borderColor == null) {
				writeInt( os, writer.addColor( new Color( 0, 0, 0 ) )); 
			} else {
				writeInt( os, writer.addColor( borderColor ));
			}    
			os.WriteByte((byte) '\n');
		}


		/// <summary>
		/// RtfTables call this method from their own setMerge() to
		/// specify that a certain other cell is to be merged with it.
		/// </summary>
		/// <param name="x">The column position of the cell to be merged</param>
		/// <param name="mergeType">
		/// The merge type specifies the kind of merge to be applied
		/// (MERGE_HORIZ_PREV, MERGE_VERT_PREV, MERGE_BOTH_PREV)
		/// </param>
		/// <param name="mergeCell">
		/// The RtfCell that the cell at x and y is to
		/// be merged with
		/// </param>
		public void setMerge(int x, int mergeType, RtfCell mergeCell) {
			RtfCell cell = (RtfCell) cells[x];
			cell.setMerge(mergeType, mergeCell);
		}
    
		/// <summary>
		/// Write an Integer to the stream.
		/// </summary>
		/// <param name="str">The Stream to be written to.</param>
		/// <param name="i">The int to be written.</param>
		private void writeInt(MemoryStream str, int i) {
			byte[] tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(i.ToString());
			str.Write(tmp, 0, tmp.Length);
		}
	}
}