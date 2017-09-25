using System;
using System.Drawing;

using iTextSharp.text;

/*
 * $Id: PdfPRow.cs,v 1.1.1.1 2003/02/04 02:57:38 geraldhenson Exp $
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

	/**
	 * A row in a PdfPTable.
	 *
	 * @author  Paulo Soares
	 */

	public class PdfPRow {

		protected PdfPCell[] cells;
		protected float maxHeight = 0;
		protected bool calculated = false;

		public PdfPRow(PdfPCell[] cells) {
			this.cells = cells;
		}
    
		public PdfPRow(PdfPRow row) {
			maxHeight = row.maxHeight;
			calculated = row.calculated;
			cells = new PdfPCell[row.cells.Length];
			for (int k = 0; k < cells.Length; ++k) {
				if (row.cells[k] != null)
					cells[k] = new PdfPCell(row.cells[k]);
			}
		}
    
		public bool setWidths(float[] widths) {
			if (widths.Length != cells.Length)
				return false;
			float total = 0;
			calculated = false;
			for (int k = 0; k < widths.Length; ++k) {
				PdfPCell cell = cells[k];
				cell.Left = total;
				int last = k + cell.Colspan;
				for (; k < last; ++k)
					total += widths[k];
				--k;
				cell.Right = total;
				cell.Top = 0;
			}
			return true;
		}
    
		public float calculateHeights() {
			maxHeight = 0;
			for (int k = 0; k < cells.Length; ++k) {
				PdfPCell cell = cells[k];
				if (cell == null)
					continue;
				PdfPTable table = cell.Table;
				if (table == null ) {
					float rightLimit = cell.isNoWrap() ? 20000 : cell.Right - cell.PaddingRight;
					ColumnText ct = new ColumnText(null);
					ct.setSimpleColumn(cell.Phrase,
						cell.Left + cell.PaddingLeft,
						cell.Top - cell.PaddingTop,
						rightLimit,
						-20000,
						0, cell.HorizontalAlignment);
					ct.setLeading(cell.Leading, cell.MultipliedLeading);
					ct.Indent = cell.Indent;
					ct.ExtraParagraphSpace = cell.ExtraParagraphSpace;
					ct.FollowingIndent = cell.FollowingIndent;
					ct.RightIndent = cell.RightIndent;
					ct.RunDirection = cell.RunDirection;
					try {
						ct.go(true);
					}
					catch (DocumentException e) {
						throw e;
					}
					float yLine = ct.YLine;
					cell.Bottom = yLine - cell.PaddingBottom;
				}
				else {
					table.TotalWidth = cell.Right - cell.PaddingRight - cell.PaddingLeft - cell.Left;
					cell.Bottom = cell.Top - cell.PaddingTop - cell.PaddingBottom - table.TotalHeight;
				}
				float height = cell.FixedHeight;
				if (height <= 0)
					height = cell.Height;
				if (height < cell.FixedHeight)
					height = cell.FixedHeight;
				else if (height < cell.MinimumHeight)
					height = cell.MinimumHeight;
				if (height > maxHeight)
					maxHeight = height;
			}
			calculated = true;
			return maxHeight;
		}

		public void writeBorderAndBackgroung(float xPos, float yPos, PdfPCell cell, PdfContentByte[] canvases) {
			PdfContentByte lines = canvases[PdfPTable.LINECANVAS];
			PdfContentByte backgr = canvases[PdfPTable.BACKGROUNDCANVAS];
			// the coordinates of the border are retrieved
			float x1 = cell.Left + xPos;
			float y1 = cell.Top + yPos;
			float x2 = cell.Right + xPos;
			float y2 = y1 - maxHeight;

			// the backgroundcolor is set
			Color background = cell.BackgroundColor;
			if (background != null) {
				backgr.ColorFill = background;
				backgr.rectangle(x1, y1, x2 - x1, y2 - y1);
				backgr.fill();
			}
			else if (cell.GrayFill > 0) {
				backgr.GrayFill = cell.GrayFill;
				backgr.rectangle(x1, y1, x2 - x1, y2 - y1);
				backgr.fill();
			}
			// if the element hasn't got any borders, nothing is added
			if (cell.hasBorders()) {

				// the width is set to the width of the element
				if (cell.BorderWidth != Rectangle.UNDEFINED) {
					lines.LineWidth = cell.BorderWidth;
				}

				// the color is set to the color of the element
				Color color = cell.BorderColor;
				if (color != null) {
					lines.ColorStroke = color;
				}

				// if the box is a rectangle, it is added as a rectangle
				if (cell.hasBorder(Rectangle.BOX)) {
					lines.rectangle(x1, y1, x2 - x1, y2 - y1);
				}
					// if the border isn't a rectangle, the different sides are added apart
				else {
					if (cell.hasBorder(Rectangle.RIGHT)) {
						lines.moveTo(x2, y1);
						lines.lineTo(x2, y2);
					}
					if (cell.hasBorder(Rectangle.LEFT)) {
						lines.moveTo(x1, y1);
						lines.lineTo(x1, y2);
					}
					if (cell.hasBorder(Rectangle.BOTTOM)) {
						lines.moveTo(x1, y2);
						lines.lineTo(x2, y2);
					}
					if (cell.hasBorder(Rectangle.TOP)) {
						lines.moveTo(x1, y1);
						lines.lineTo(x2, y1);
					}
				}
				lines.stroke();
				if (color != null) {
					lines.resetRGBColorStroke();
				}
			}            
		}
    
		public void writeCells(float xPos, float yPos, PdfContentByte[] canvases) {
			if (!calculated)
				calculateHeights();
			for (int k = 0; k < cells.Length; ++k) {
				PdfPCell cell = cells[k];
				if (cell == null)
					continue;
				writeBorderAndBackgroung(xPos, yPos, cell, canvases);
				PdfPTable table = cell.Table;
				float tly = 0;
				switch (cell.VerticalAlignment) {
					case Element.ALIGN_BOTTOM:
						tly = cell.Top + yPos - maxHeight + cell.Height - cell.PaddingTop;
						break;
					case Element.ALIGN_MIDDLE:
						tly = cell.Top + yPos + (cell.Height - maxHeight) / 2 - cell.PaddingTop;
						break;
					default:
						tly = cell.Top + yPos - cell.PaddingTop;
						break;
				}
				if (table == null) {
					float fixedHeight = cell.FixedHeight;
					float rightLimit = cell.Right + xPos - cell.PaddingRight;
					float leftLimit = cell.Left + xPos + cell.PaddingLeft;
					if (cell.isNoWrap()) {
						switch (cell.HorizontalAlignment) {
							case Element.ALIGN_CENTER:
								rightLimit += 10000;
								leftLimit -= 10000;
								break;
							case Element.ALIGN_RIGHT:
								leftLimit -= 20000;
								break;
							default:
								rightLimit += 20000;
								break;
						}
					}
					ColumnText ct = new ColumnText(canvases[PdfPTable.TEXTCANVAS]);
					float bry = -20000;
					if (fixedHeight > 0) {
						if (cell.Height > maxHeight) {
							tly = cell.Top + yPos - cell.PaddingTop;
							bry = cell.Top + yPos - maxHeight + cell.PaddingBottom;
						}
					}
					ct.setSimpleColumn(cell.Phrase,
						leftLimit,
						tly,
						rightLimit,
						bry,
						0, cell.HorizontalAlignment);
					ct.setLeading(cell.Leading, cell.MultipliedLeading);
					ct.Indent = cell.Indent;
					ct.ExtraParagraphSpace = cell.ExtraParagraphSpace;
					ct.FollowingIndent = cell.FollowingIndent;
					ct.RightIndent = cell.RightIndent;
					ct.SpaceCharRatio = cell.SpaceCharRatio;
					ct.RunDirection = cell.RunDirection;
					try {
						ct.go();
					}
					catch (DocumentException e) {
						throw e;
					}
				}
				else {
					float remainingHeight = 0;
					float maxLastRow = 0;
					//add by Jin-Hsia Yang, to add remaining height to last row
					if (table.Size > 0) {
						PdfPRow row = table.getRow(table.Size - 1);
						remainingHeight = maxHeight-table.TotalHeight-cell.PaddingBottom-cell.PaddingTop;
						if (remainingHeight > 0) {
							maxLastRow = row.MaxHeights;
							row.MaxHeights = row.MaxHeights+ remainingHeight;
							//table.setTotalHeight(table.TotalHeight + remainingHeight);
						}
					}
					//end add

					table.writeSelectedRows(0, -1, cell.Left + xPos + cell.PaddingLeft,
						tly, canvases);
					if (remainingHeight > 0)
						table.getRow(table.Size - 1).MaxHeights = maxLastRow;
				}
			}
		}
    
		public bool isCalculated() {
			return calculated;
		}
    
		public float MaxHeights {
			get {
				if (calculated)
					return maxHeight;
				else
					return calculateHeights();
			}

			set {
				this.maxHeight = value;
			}
		}
    
		internal float[] getEventWidth(float xPos) {
			int n = 0;
			for (int k = 0; k < cells.Length; ++k) {
				if (cells[k] != null)
					++n;
			}
			float[] width = new float[n + 1];
			n = 0;
			width[n++] = xPos;
			for (int k = 0; k < cells.Length; ++k) {
				if (cells[k] != null) {
					width[n] = width[n - 1] + cells[k].Width;
					++n;
				}
			}
			return width;
		}
	}
}