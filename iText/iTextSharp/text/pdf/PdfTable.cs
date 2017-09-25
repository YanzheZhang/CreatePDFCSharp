using System;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfTable.cs,v 1.1.1.1 2003/02/04 02:57:45 geraldhenson Exp $
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
	 * <CODE>PdfTable</CODE> is an object that contains the graphics and text of a table.
	 *
	 * @see		iTextSharp.text.Table
	 * @see		iTextSharp.text.Row
	 * @see		iTextSharp.text.Cell
	 * @see		PdfCell
	 */

	internal class PdfTable : Rectangle {
    
		// membervariables
    
		/** this is the number of columns in the table. */
		private int columns;
    
		/** this is the ArrayList with all the cell of the table header. */
		private ArrayList headercells;
    
		/** this is the ArrayList with all the cells in the table. */
		private ArrayList cells;
    
		/** this is the cellpadding of the table. */
		private float cellpadding;
    
		/** this is the cellspacing of the table. */
		private float cellspacing;
    
		// constructors
    
		/**
		 * Constructs a <CODE>PdfTable</CODE>-object.
		 *
		 * @param	table	a <CODE>Table</CODE>
		 * @param	left	the left border on the page
		 * @param	right	the right border on the page
		 * @param	top		the start position of the top of the table
		 */
    
		internal PdfTable(Table table, float left, float right, float top) : base(left, top, right, top) {        
			// copying the attributes from class Table
			Border = table.Border;
			BorderWidth = table.BorderWidth;
			BorderColor = table.BorderColor;
			BackgroundColor = table.BackgroundColor;
			GrayFill = table.GrayFill;
			this.columns = table.Columns;
			this.cellpadding = table.Cellpadding;
			this.cellspacing = table.Cellspacing;
			float[] positions = table.getWidths(left, right - left);
        
			// initialisation of some parameters
			Left = positions[0];
			Right = positions[positions.Length - 1];
        
			//Row row;
			int rowNumber = 0;
			int firstDataRow = table.firstDataRow();
			Cell cell;
			PdfCell currentCell;
			headercells = new ArrayList();
			cells = new ArrayList();
			int rows = table.Size + 1;
			float[] offsets = new float[rows];
			for (int i = 0; i < rows; i++) {
				offsets[i] = top;
			}
        
			// loop over all the rows
			foreach(Row row in table) {
				if (row.isEmpty()) {
					if (rowNumber < rows - 1 && offsets[rowNumber + 1] > offsets[rowNumber]) offsets[rowNumber + 1] = offsets[rowNumber];
				}
				else {
					for(int i = 0; i < row.Columns; i++) {
						cell = (Cell) row.getCell(i);
						if (cell != null) {
							currentCell = new PdfCell(cell, rowNumber, positions[i], positions[i + cell.Colspan], offsets[rowNumber], cellspacing, cellpadding);                        
							try {
								if (offsets[rowNumber] - currentCell.Height - cellpadding < offsets[rowNumber + currentCell.Rowspan]) {
									offsets[rowNumber + currentCell.Rowspan] = offsets[rowNumber] - currentCell.Height - cellpadding;
								}
							}
							catch(Exception aioobe) {
								aioobe.GetType();
								if (offsets[rowNumber] - currentCell.Height < offsets[rows - 1]) {
									offsets[rows - 1] = offsets[rowNumber] - currentCell.Height;
								}
							}
							if (rowNumber < firstDataRow) {
								currentCell.Header = true;
								headercells.Add(currentCell);
							}
							cells.Add(currentCell);
						}
					}
				}
				rowNumber++;
			}
        
			// loop over all the cells
			int n = cells.Count;
			for (int i = 0; i < n; i++) {
				currentCell = (PdfCell) cells[i];
				try {
					currentCell.Bottom = offsets[currentCell.Rownumber + currentCell.Rowspan];
				}
				catch(Exception aioobe) {
					aioobe.GetType();
					currentCell.Bottom = offsets[rows - 1];
				}
			}
			Bottom = offsets[rows - 1];
		}
    
		// methods
    
		/**
		 * Returns the arraylist with the cells of the table header.
		 *
		 * @return	an <CODE>ArrayList</CODE>
		 */
    
		internal ArrayList HeaderCells {
			get {
				return headercells;
			}
		}
    
		/**
		 * Checks if there is a table header.
		 *
		 * @return	an <CODE>ArrayList</CODE>
		 */
    
		internal bool hasHeader() {
			return headercells.Count > 0;
		}
    
		/**
		 * Returns the arraylist with the cells of the table.
		 *
		 * @return	an <CODE>ArrayList</CODE>
		 */
    
		internal ArrayList Cells {
			get {
				return cells;
			}
		}
    
		/**
		 * Returns the number of columns of the table.
		 *
		 * @return	the number of columns
		 */
    
		internal int Columns {
			get {
				return columns;
			}
		}
    
		/**
		 * Returns the cellpadding of the table.
		 *
		 * @return	the cellpadding
		 */
    
		internal float Cellpadding {
			get {
				return cellpadding;
			}
		}
    
		/**
		 * Returns the cellspacing of the table.
		 *
		 * @return	the cellspacing
		 */
    
		internal float Cellspacing {
			get {
				return cellspacing;
			}
		}
	}
}