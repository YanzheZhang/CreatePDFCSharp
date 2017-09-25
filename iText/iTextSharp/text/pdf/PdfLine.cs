using System;
using System.Text;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfLine.cs,v 1.1.1.1 2003/02/04 02:57:30 geraldhenson Exp $
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
	 * <CODE>PdfLine</CODE> defines an array with <CODE>PdfChunk</CODE>-objects
	 * that fit into 1 line.
	 */

	public class PdfLine {
    
		// membervariables
    
		/** The arraylist containing the chunks. */
		protected ArrayList line;
    
		/** The left indentation of the line. */
		protected float left;
    
		/** The width of the line. */
		protected float width;
    
		/** The alignment of the line. */
		protected int alignment;
    
		/** The heigth of the line. */
		protected float height;
    
		/** The listsymbol (if necessary). */
		protected PdfChunk listSymbol = null;
    
		/** The listsymbol (if necessary). */
		protected float symbolIndent;
    
		/** <CODE>true</CODE> if the chunk splitting was caused by a newline. */
		protected bool newlineSplit = false;
    
		/** The original width. */
		protected float originalWidth;
    
		protected bool isRTL = false;
    
		// constructors
    
		/**
		 * Constructs a new <CODE>PdfLine</CODE>-object.
		 *
		 * @param	left		the limit of the line at the left
		 * @param	right		the limit of the line at the right
		 * @param	alignment	the alignment of the line
		 * @param	height		the height of the line
		 */
    
		internal PdfLine(float left, float right, int alignment, float height) {
			this.left = left;
			this.width = right - left;
			this.originalWidth = this.width;
			this.alignment = alignment;
			this.height = height;
			this.line = new ArrayList();
		}
    
		internal PdfLine(float left, float remainingWidth, int alignment, bool newlineSplit, ArrayList line, bool isRTL) {
			this.left = left;
			this.width = remainingWidth;
			this.alignment = alignment;
			this.line = line;
			this.newlineSplit = newlineSplit;
			this.isRTL = isRTL;
		}
    
		// methods
    
		/**
		 * Adds a <CODE>PdfChunk</CODE> to the <CODE>PdfLine</CODE>.
		 *
		 * @param		chunk		the <CODE>PdfChunk</CODE> to add
		 * @return		<CODE>null</CODE> if the chunk could be added completely; if not
		 *				a <CODE>PdfChunk</CODE> containing the part of the chunk that could
		 *				not be added is returned
		 */
    
		internal PdfChunk Add(PdfChunk chunk) {
			// nothing happens if the chunk is null.
			if (chunk == null || chunk.ToString().Equals("")) {
				return null;
			}
        
			// we split the chunk to be added
			PdfChunk overflow = chunk.split(width);
			newlineSplit = (chunk.isNewlineSplit() || overflow == null);
			//        if (chunk.isNewlineSplit() && alignment == Element.ALIGN_JUSTIFIED)
			//            alignment = Element.ALIGN_LEFT;
        
        
			// if the length of the chunk > 0 we add it to the line
			if (chunk.Length > 0) {
				if (overflow != null)
					chunk.trimLastSpace();
				width -= chunk.Width;
				line.Add(chunk);
			}
        
				// if the length == 0 and there were no other chunks added to the line yet,
				// we risk to end up in an endless loop trying endlessly to add the same chunk
			else if (line.Count < 1) {
				chunk = overflow;
				overflow = chunk.truncate(width);
				width -= chunk.Width;
				if (chunk.Length > 0) {
					line.Add(chunk);
					return overflow;
				}
					// if the chunck couldn't even be truncated, we add everything, so be it
				else {
					line.Add(overflow);
					return null;
				}
			}
			else {
				width += ((PdfChunk)(line[line.Count - 1])).trimLastSpace();
			}
			return overflow;
		}
    
		// methods to retrieve information
    
		/**
		 * Returns the number of chunks in the line.
		 *
		 * @return	a value
		 */
    
		public int Size {
			get {
				return line.Count;
			}
		}
    
		/**
		 * Returns an iterator of <CODE>PdfChunk</CODE>s.
		 *
		 * @return	an <CODE>Iterator</CODE>
		 */
    
		public IEnumerator GetEnumerator() {
			return line.GetEnumerator();
		}
    
		/**
		 * Returns the height of the line.
		 *
		 * @return	a value
		 */
    
		internal float Height {
			get {
				return height;
			}
		}
    
		/**
		 * Returns the left indentation of the line taking the alignment of the line into account.
		 *
		 * @return	a value
		 */
    
		internal  float IndentLeft {
			get {
				if (isRTL) {
					switch (alignment) {
						case Element.ALIGN_LEFT:
							return left + width;
						case Element.ALIGN_CENTER:
							return left + (width / 2f);
						default:
							return left;
					}
				}
				else {
					switch (alignment) {
						case Element.ALIGN_RIGHT:
							return left + width;
						case Element.ALIGN_CENTER:
							return left + (width / 2f);
						default:
							return left;
					}
				}
			}
		}
    
		/**
		 * Checks if this line has to be justified.
		 *
		 * @return	<CODE>true</CODE> if the alignment equals <VAR>ALIGN_JUSTIFIED</VAR> and there is some width left.
		 */
    
		public bool hasToBeJustified() {
			return (alignment == Element.ALIGN_JUSTIFIED && width != 0);
		}
    
		/**
		 * Resets the alignment of this line.
		 * <P>
		 * The alignment of the last line of for instance a <CODE>Paragraph</CODE>
		 * that has to be justified, has to be reset to <VAR>ALIGN_LEFT</VAR>.
		 */
    
		public void resetAlignment() {
			if (alignment == Element.ALIGN_JUSTIFIED) {
				alignment = Element.ALIGN_LEFT;
			}
		}
    
		/**
		 * Returns the width that is left, after a maximum of characters is added to the line.
		 *
		 * @return	a value
		 */
    
		internal float WidthLeft {
			get {
				return width;
			}
		}
    
		/**
		 * Returns the number of space-characters in this line.
		 *
		 * @return	a value
		 */
    
		internal int NumberOfSpaces {
			get {
				string str = ToString();
				int length = str.Length;
				int numberOfSpaces = 0;
				for (int i = 0; i < length; i++) {
					if (str[i] == ' ') {
						numberOfSpaces++;
					}
				}
				return numberOfSpaces;
			}
		}
    
		/**
		 * Sets the listsymbol of this line.
		 * <P>
		 * This is only necessary for the first line of a <CODE>ListItem</CODE>.
		 *
		 * @param listItem the list symbol
		 */
    
		public ListItem ListItem {
			set {
				this.listSymbol = new PdfChunk(value.ListSymbol, null);
				this.symbolIndent = value.IndentationLeft;
			}
		}
    
		/**
		 * Returns the listsymbol of this line.
		 *
		 * @return	a <CODE>PdfChunk</CODE> if the line has a listsymbol; <CODE>null</CODE> otherwise
		 */
    
		public PdfChunk ListSymbol {
			get {
				return listSymbol;
			}
		}
    
		/**
		 * Return the indentation needed to show the listsymbol.
		 *
		 * @return	a value
		 */
    
		public float ListIndent {
			get {
				return symbolIndent;
			}
		}
    
		/**
		 * Get the string representation of what is in this line.
		 *
		 * @return	a <CODE>string</CODE>
		 */
    
		public override string ToString() {
			StringBuilder tmp = new StringBuilder();
			foreach(PdfChunk c in line) {
				tmp.Append(c.ToString());
			}
			return tmp.ToString();
		}
    
		/**
		 * Checks if a newline caused the line split.
		 * @return <CODE>true</CODE> if a newline caused the line split
		 */
		public bool isNewlineSplit() {
			return newlineSplit;
		}
    
		/**
		 * Gets the index of the last <CODE>PdfChunk</CODE> with metric attributes
		 * @return the last <CODE>PdfChunk</CODE> with metric attributes
		 */
		public int LastStrokeChunk {
			get {
				int lastIdx = line.Count - 1;
				for (; lastIdx >= 0; --lastIdx) {
					PdfChunk chunk = (PdfChunk)line[lastIdx];
					if (chunk.isStroked())
						break;
				}
				return lastIdx;
			}
		}
    
		/**
		 * Gets a <CODE>PdfChunk</CODE> by index.
		 * @param idx the index
		 * @return the <CODE>PdfChunk</CODE> or null if beyond the array
		 */
		public PdfChunk getChunk(int idx) {
			if (idx < 0 || idx >= line.Count)
				return null;
			return (PdfChunk)line[idx];
		}
    
		/**
		 * Gets the original width of the line.
		 * @return the original width of the line
		 */
		public float OriginalWidth {
			get {
				return originalWidth;
			}
		}
    
		/**
		 * Gets the maximum size of all the fonts used in this line
		 * including images (if there are images in the line and if
		 * the leading has to be changed).
		 * @return maximum size of all the fonts used in this line
		 */
		internal float MaxSize {
			get {
				float maxSize = 0;
				for (int k = 0; k < line.Count; ++k) {
					PdfChunk chunk = (PdfChunk)line[k];
					if (!chunk.isImage() || !chunk.ChangeLeading) {
						maxSize = Math.Max(chunk.Font.Size, maxSize);
					}
					else {
						maxSize = Math.Max(chunk.Image.ScaledHeight + chunk.ImageOffsetY , maxSize);
					}
				}
				return maxSize;
			}
		}
    
		/**
		 * Gets the maximum size of all the fonts used in this line
		 * including images.
		 * @return maximum size of all the fonts used in this line
		 */
		internal float MaxSizeSimple {
			get {
				float maxSize = 0;
				for (int k = 0; k < line.Count; ++k) {
					PdfChunk chunk = (PdfChunk)line[k];
					if (!chunk.isImage()) {
						maxSize = Math.Max(chunk.Font.Size, maxSize);
					}
					else {
						maxSize = Math.Max(chunk.Image.ScaledHeight + chunk.ImageOffsetY , maxSize);
					}
				}
				return maxSize;
			}
		}
    
		internal bool IsRTL {
			get {
				return isRTL;
			}
		}
    
		public float getWidthCorrected(float charSpacing, float wordSpacing) {
			float total = 0;
			for (int k = 0; k < line.Count; ++k) {
				PdfChunk ck = (PdfChunk)line[k];
				total += ck.getWidthCorrected(charSpacing, wordSpacing);
			}
			return total;
		}
	}
}