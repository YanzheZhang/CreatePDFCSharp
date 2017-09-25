using System;
using System.IO;
using System.Collections;

using iTextSharp.text;

/**
 * $Id: RtfTable.cs,v 1.3 2003/03/22 21:51:36 geraldhenson Exp $
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
	/// Do not use it directly, except if you want to write a DocumentListener for Rtf
	/// </remarks>
	public class RtfTable {
		/// <summary> Stores the different rows. </summary>
		private ArrayList rowsList = new ArrayList();
		/// <summary> Stores the RtfWriter, which created this RtfTable. </summary>
		private RtfWriter writer = null;
		// <!-- steffen
		/// <summary> Stores the Table, which this RtfTable is based on. </summary>
		private Table origTable = null;
		// -->
  
		/// <summary>
		/// Create a new RtfTable.
		/// </summary>
		/// <param name="writer">The RtfWriter that created this Table</param>
		public RtfTable(RtfWriter writer) : base() {
			this.writer = writer;
		}

		/// <summary>
		/// Import a Table into the RtfTable.
		/// </summary>
		/// <param name="table">A Table specifying the Table to be imported</param>
		/// <param name="pageWidth">An int specifying the page width</param>
		/// <returns></returns>
		public bool importTable(Table table, int pageWidth) {
			// <!-- steffen
			origTable = table;
			// -->        
			// All Cells are pregenerated first, so that cell and rowspanning work

			int tableWidth = (int) table.WidthPercentage;
			int cellpadding = (int) (table.Cellpadding * RtfWriter.twipsFactor);
			int cellspacing = (int) (table.Cellspacing * RtfWriter.twipsFactor);
			float[] propWidths = table.ProportionalWidths;

			int borders = table.Border;
			Color borderColor = table.BorderColor;
			float borderWidth = table.BorderWidth;

			for(int i = 0; i < table.Size; i++) {
				RtfRow rtfRow = new RtfRow(writer, this);
				rtfRow.pregenerateRows(table.Columns);
				rowsList.Add(rtfRow);
			}
			int j = 0;
			foreach (Row row in table) {
				RtfRow rtfRow = (RtfRow) rowsList[j];
				// steffen
				// <---
				rtfRow.importRow(row, propWidths, tableWidth, pageWidth, cellpadding, cellspacing, borders, borderColor, borderWidth, j);
				// -->
				j++;
			}
			return true;
		}

		/// <summary>
		/// Output the content of the RtfTable to an Stream.
		/// </summary>
		/// <param name="os">The Stream that the content of the RtfTable is to be written to</param>
		/// <returns></returns>
		public bool writeTable(MemoryStream os) {
			// <!-- steffen
			int size = rowsList.Count;
			for (int i = 0; i < size; i++) {
				RtfRow row = (RtfRow)rowsList[i];
				row.writeRow( os, i, origTable );
				os.WriteByte((byte) '\n');
			}
			// -->
			if(!writer.writingHeaderFooter()) {
				os.WriteByte(RtfWriter.escape);
				os.Write(RtfWriter.paragraphDefaults, 0, RtfWriter.paragraphDefaults.Length);
				os.WriteByte(RtfWriter.escape);
				os.Write(RtfWriter.paragraph, 0, RtfWriter.paragraph.Length);
			}
			return true;
		}

		/// <summary>
		/// RtfCells call this method to specify that a certain other cell is to be merged with it.
		/// </summary>
		/// <param name="x">The column position of the cell to be merged</param>
		/// <param name="y">The row position of the cell to be merged</param>
		/// <param name="mergeType">The merge type specifies the kind of merge to be applied (MERGE_HORIZ_PREV, MERGE_VERT_PREV, MERGE_BOTH_PREV)</param>
		/// <param name="mergeCell">The RtfCell that the cell at x and y is to be merged with</param>
		public void setMerge(int x, int y, int mergeType, RtfCell mergeCell) {
			RtfRow row = (RtfRow) rowsList[y];
			row.setMerge(x, mergeType, mergeCell);
		}
	}
}