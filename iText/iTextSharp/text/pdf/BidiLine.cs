using System;
using System.Collections;
using System.Text;

/*
 *
 * Copyright 2002 Paulo Soares
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

	/** Does all the line bidirectional processing with PdfChunk assembly.
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class BidiLine {
		private const int pieceSizeStart = 2048;
    
		protected int runDirection;
		protected int pieceSize = pieceSizeStart;
		protected char[] text = new char[pieceSizeStart];
		protected PdfChunk[] detailChunks = new PdfChunk[pieceSizeStart];
		protected int totalTextLength = 0;
    
		protected byte[] orderLevels = new byte[pieceSizeStart];
		protected int[] indexChars = new int[pieceSizeStart];
    
		protected ArrayList chunks = new ArrayList();
		protected int indexChunk = 0;
		protected int indexChunkChar = 0;
		protected int currentChar = 0;
    
		protected int storedRunDirection;
		protected char[] storedText = new char[0];
		protected PdfChunk[] storedDetailChunks = new PdfChunk[0];
		protected int storedTotalTextLength = 0;
    
		protected byte[] storedOrderLevels = new byte[0];
		protected int[] storedIndexChars = new int[0];
    
		protected int storedIndexChunk = 0;
		protected int storedIndexChunkChar = 0;
		protected int storedCurrentChar = 0;
    
		protected bool shortStore;
		protected ArabicShaping arabic = new ArabicShaping(ArabicShaping.LETTERS_SHAPE | ArabicShaping.LENGTH_GROW_SHRINK | ArabicShaping.TEXT_DIRECTION_LOGICAL);
		protected static IntHashtable mirrorChars = new IntHashtable();

		/** Creates new BidiLine */
		public BidiLine() {
		}
    
		public bool isEmpty() {
			return (currentChar >= totalTextLength && indexChunk >= chunks.Count);
		}
    
		public void clearChunks() {
			chunks.Clear();
			totalTextLength = 0;
			currentChar = 0;
		}

		public bool getParagraph(int runDirection) {
			this.runDirection = runDirection;
			currentChar = 0;
			totalTextLength = 0;
			bool hasText = false;
			char c;
			for (; indexChunk < chunks.Count; ++indexChunk) {
				PdfChunk ck = (PdfChunk)chunks[indexChunk];
				string s = ck.ToString();
				int len = s.Length;
				for (; indexChunkChar < len; ++indexChunkChar) {
					c = s[indexChunkChar];
					if (c == '\r' || c == '\n') {
						if (c == '\r' && indexChunkChar + 1 < len && s[indexChunkChar + 1] == '\n')
							++indexChunkChar;
						++indexChunkChar;
						if (indexChunkChar >= len) {
							indexChunkChar = 0;
							++indexChunk;
						}
						hasText = true;
						if (totalTextLength == 0)
							detailChunks[0] = ck;
						break;
					}
					addPiece(c, ck);
				}
				if (hasText)
					break;
				indexChunkChar = 0;
			}
			if (totalTextLength == 0)
				return hasText;

			// remove trailing WS
			totalTextLength = trimRight(0, totalTextLength - 1) + 1;
			if (totalTextLength == 0)
				return true;
        
//			if (runDirection == PdfWriter.RUN_DIRECTION_LTR || runDirection == PdfWriter.RUN_DIRECTION_RTL) {
//				if (orderLevels.Length < totalTextLength) {
//					orderLevels = new byte[pieceSize];
//					indexChars = new int[pieceSize];
//				}
//
//				Hashtable attr = new Hashtable();
//				if (runDirection == PdfWriter.RUN_DIRECTION_LTR)
//					attr.Add(TextAttribute.RUN_DIRECTION, TextAttribute.RUN_DIRECTION_LTR);
//				else
//					attr.Add(TextAttribute.RUN_DIRECTION, TextAttribute.RUN_DIRECTION_RTL);
//            
//				TextLayout t = new TextLayout(new string(text, 0, totalTextLength), attr, new FontRenderContext(new AffineTransform(), false, true));
//				for (int k = 0; k < totalTextLength; ++k) {
//					orderLevels[k] = t.getCharacterLevel(k);
//					indexChars[k] = k;
//				}
//				doArabicShapping();
//				mirrorGlyphs();
//			}
			totalTextLength = trimRightEx(0, totalTextLength - 1) + 1;
			return true;
		}
    
		public void addChunk(PdfChunk chunk) {
			chunks.Add(chunk);
		}
    
		public void addChunks(ArrayList chunks) {
			this.chunks.AddRange(chunks);
		}
    
		public void addPiece(char c, PdfChunk chunk) {
			if (totalTextLength >= pieceSize) {
				char[] tempText = text;
				PdfChunk[] tempDetailChunks = detailChunks;
				pieceSize *= 2;
				text = new char[pieceSize];
				detailChunks = new PdfChunk[pieceSize];
				Array.Copy(tempText, 0, text, 0, totalTextLength);
				Array.Copy(tempDetailChunks, 0, detailChunks, 0, totalTextLength);
			}
			text[totalTextLength] = c;
			detailChunks[totalTextLength++] = chunk;
		}
    
		public void save() {
			if (indexChunk > 0) {
				if (indexChunk >= chunks.Count)
					chunks.Clear();
				else {
					for (--indexChunk; indexChunk >= 0; --indexChunk)
						chunks.RemoveAt(indexChunk);
				}
				indexChunk = 0;
			}
			storedRunDirection = runDirection;
			storedTotalTextLength = totalTextLength;
			storedIndexChunk = indexChunk;
			storedIndexChunkChar = indexChunkChar;
			storedCurrentChar = currentChar;
			shortStore = (currentChar < totalTextLength);
			if (!shortStore) {
				// long save
				if (storedText.Length < totalTextLength) {
					storedText = new char[totalTextLength];
					storedDetailChunks = new PdfChunk[totalTextLength];
				}
				Array.Copy(text, 0, storedText, 0, totalTextLength);
				Array.Copy(detailChunks, 0, storedDetailChunks, 0, totalTextLength);
			}
			if (runDirection == PdfWriter.RUN_DIRECTION_LTR || runDirection == PdfWriter.RUN_DIRECTION_RTL) {
				if (storedOrderLevels.Length < totalTextLength) {
					storedOrderLevels = new byte[totalTextLength];
					storedIndexChars = new int[totalTextLength];
				}
				Array.Copy(orderLevels, currentChar, storedOrderLevels, currentChar, totalTextLength - currentChar);
				Array.Copy(indexChars, currentChar, storedIndexChars, currentChar, totalTextLength - currentChar);
			}
		}
    
		public void restore() {
			runDirection = storedRunDirection;
			totalTextLength = storedTotalTextLength;
			indexChunk = storedIndexChunk;
			indexChunkChar = storedIndexChunkChar;
			currentChar = storedCurrentChar;
			if (!shortStore) {
				// long restore
				Array.Copy(storedText, 0, text, 0, totalTextLength);
				Array.Copy(storedDetailChunks, 0, detailChunks, 0, totalTextLength);
			}
			if (runDirection == PdfWriter.RUN_DIRECTION_LTR || runDirection == PdfWriter.RUN_DIRECTION_RTL) {
				Array.Copy(storedOrderLevels, currentChar, orderLevels, currentChar, totalTextLength - currentChar);
				Array.Copy(storedIndexChars, currentChar, indexChars, currentChar, totalTextLength - currentChar);
			}
		}
    
		public void mirrorGlyphs() {
			for (int k = 0; k < totalTextLength; ++k) {
				if ((orderLevels[k] & 1) == 1) {
					int mirror = mirrorChars[text[k]];
					if (mirror != 0)
						text[k] = (char)mirror;
				}
			}
		}
    
		public void doArabicShapping() {
			int src = 0;
			int dest = 0;
			for (;;) {
				while (src < totalTextLength) {
					char c = text[src];
					if (c >= 0x0600 && c <= 0x06ff)
						break;
					if (src != dest) {
						text[dest] = text[src];
						detailChunks[dest] = detailChunks[src];
						orderLevels[dest] = orderLevels[src];
					}
					++src;
					++dest;
				}
				if (src >= totalTextLength) {
					totalTextLength = dest;
					return;
				}
				int startArabicIdx = src;
				++src;
				while (src < totalTextLength) {
					char c = text[src];
					if (c < 0x0600 || c > 0x06ff)
						break;
					++src;
				}
				int arabicWordSize = src - startArabicIdx;
				int size = arabic.shape(text, startArabicIdx, arabicWordSize, text, dest, arabicWordSize);
				if (startArabicIdx != dest) {
					for (int k = 0; k < size; ++k) {
						detailChunks[dest] = detailChunks[startArabicIdx];
						orderLevels[dest++] = orderLevels[startArabicIdx++];
					}
				}
				else
					dest += size;
			}
		}
    
		public string processLineTT(float width) {
			reorder(0, totalTextLength);
			string res = "";
			for (int k = 0; k < totalTextLength; ++k) {
				char c = text[indexChars[k]];
				if (!PdfChunk.noPrint(c))
					res += c;
			}
			return res;
			/*        while (currentStartLine < totalTextLength) {
						if (text[currentStartLine] != ' ')
							break;
						currentStartLine++;
					}
					if (currentStartLine >= totalTextLength)
						return null;
					return null;*/
		}
    
		public PdfLine processLine(float width, int alignment, int runDirection) {
			PdfChunk ck;

			save();
			if (currentChar >= totalTextLength) {
				bool hasText = getParagraph(runDirection);
				if (!hasText)
					return null;
				if (totalTextLength == 0) {
					ArrayList ar = new ArrayList();
					ck = new PdfChunk("", detailChunks[0]);
					ar.Add(ck);
					return new PdfLine(0, 0, alignment, true, ar, (runDirection == PdfWriter.RUN_DIRECTION_RTL));
				}
			}
			float originalWidth = width;
			int lastSplit = -1;
			if (currentChar != 0)
				currentChar = trimLeftEx(currentChar, totalTextLength - 1);
			int oldCurrentChar = currentChar;
			char c = (char)0;
			ck = null;
			float charWidth = 0;
			PdfChunk lastValidChunk = null;
			for (; currentChar < totalTextLength; ++currentChar) {
				c = text[currentChar];
				if (PdfChunk.noPrint(c))
					continue;
				ck = detailChunks[currentChar];
				charWidth = ck.getCharWidth(c);
				if (ck.isExtSplitCharacter(c))
					lastSplit = currentChar;
				if (width - charWidth < 0)
					break;
				width -= charWidth;
				lastValidChunk = ck;
			}
			bool isRTL = (runDirection == PdfWriter.RUN_DIRECTION_RTL);
			if (lastValidChunk == null) {
				// not even a single char fit; must output the first char
				++currentChar;
				return new PdfLine(0, 0, alignment, false, createArrayOfPdfChunks(currentChar - 1, currentChar - 1), isRTL);
			}
			if (currentChar >= totalTextLength) {
				// there was more line than text
				return new PdfLine(0, width, alignment, true, createArrayOfPdfChunks(oldCurrentChar, totalTextLength - 1), isRTL);
			}
			int newCurrentChar = trimRightEx(oldCurrentChar, currentChar - 1);
			if (newCurrentChar < oldCurrentChar) {
				// only WS
				return new PdfLine(0, width, alignment, false, createArrayOfPdfChunks(oldCurrentChar, currentChar - 1), isRTL);
			}
			if (lastSplit == -1 || lastSplit >= newCurrentChar) {
				// no split point or split point ahead of end
				return new PdfLine(0, width + getWidth(newCurrentChar + 1, currentChar - 1), alignment, false, createArrayOfPdfChunks(oldCurrentChar, newCurrentChar), isRTL);
			}
			// standard split
			currentChar = lastSplit + 1;
			newCurrentChar = trimRightEx(oldCurrentChar, lastSplit);
			if (newCurrentChar < oldCurrentChar) {
				// only WS again
				newCurrentChar = currentChar - 1;
			}
			return new PdfLine(0, originalWidth - getWidth(oldCurrentChar, newCurrentChar), alignment, false, createArrayOfPdfChunks(oldCurrentChar, newCurrentChar), isRTL);
		}
    
		/** Gets the width of a range of characters.
		 * @param startIdx the first index to calculate
		 * @param lastIdx the last inclusive index to calculate
		 * @return the sum of all widths
		 */    
		public float getWidth(int startIdx, int lastIdx) {
			char c = (char)0;
			float width = 0;
			for (; startIdx <= lastIdx; ++startIdx) {
				c = text[startIdx];
				if (PdfChunk.noPrint(c))
					continue;
				width += detailChunks[startIdx].getCharWidth(c);
			}
			return width;
		}
    
		public ArrayList createArrayOfPdfChunks(int startIdx, int endIdx) {
			bool bidi = (runDirection == PdfWriter.RUN_DIRECTION_LTR || runDirection == PdfWriter.RUN_DIRECTION_RTL);
			if (bidi)
				reorder(startIdx, endIdx);
			ArrayList ar = new ArrayList();
			PdfChunk refCk = detailChunks[startIdx];
			PdfChunk ck = null;
			StringBuilder buf = new StringBuilder();
			char c;
			int idx = 0;
			for (; startIdx <= endIdx; ++startIdx) {
				idx = bidi ? indexChars[startIdx] : startIdx;
				c = text[idx];
				if (PdfChunk.noPrint(c))
					continue;
				ck = detailChunks[idx];
				if (ck.isImage()) {
					if (buf.Length > 0) {
						ar.Add(new PdfChunk(buf.ToString(), refCk));
						buf = new StringBuilder();
					}
					ar.Add(ck);
				}
				else if (ck == refCk) {
					buf.Append(c);
				}
				else {
					if (buf.Length > 0) {
						ar.Add(new PdfChunk(buf.ToString(), refCk));
						buf = new StringBuilder();
					}
					if (!ck.isImage())
						buf.Append(c);
					refCk = ck;
				}
			}
			if (buf.Length > 0) {
				ar.Add(new PdfChunk(buf.ToString(), refCk));
			}
			return ar;
		}
    
		public int trimRight(int startIdx, int endIdx) {
			int idx = endIdx;
			for (; idx >= startIdx; --idx) {
				if (!isWS(text[idx]))
					break;
			}
			return idx;
		}
    
		public int trimLeft(int startIdx, int endIdx) {
			int idx = startIdx;
			for (; idx <= endIdx; ++idx) {
				if (!isWS(text[idx]))
					break;
			}
			return idx;
		}
    
		public int trimRightEx(int startIdx, int endIdx) {
			int idx = endIdx;
			char c = (char)0;
			for (; idx >= startIdx; --idx) {
				c = text[idx];
				if (!isWS(c) && !PdfChunk.noPrint(c))
					break;
			}
			return idx;
		}
    
		public int trimLeftEx(int startIdx, int endIdx) {
			int idx = startIdx;
			char c = (char)0;
			for (; idx <= endIdx; ++idx) {
				c = text[idx];
				if (!isWS(c) && !PdfChunk.noPrint(c))
					break;
			}
			return idx;
		}
    
		public void reorder(int start, int end) {
			byte maxLevel = orderLevels[start];
			byte minLevel = maxLevel;
			byte onlyOddLevels = maxLevel;
			byte onlyEvenLevels = maxLevel;
			for (int k = start + 1; k <= end; ++k) {
				byte b = orderLevels[k];
				if (b > maxLevel)
					maxLevel = b;
				else if (b < minLevel)
					minLevel = b;
				onlyOddLevels &= b;
				onlyEvenLevels |= b;
			}
			if ((onlyEvenLevels & 1) == 0) // nothing to do
				return;
			if ((onlyOddLevels & 1) == 1) { // single inversion
				flip(start, end + 1);
				return;
			}
			minLevel |= 1;
			for (; maxLevel >= minLevel; --maxLevel) {
				int pstart = start;
				for (;;) {
					for (;pstart <= end; ++pstart) {
						if (orderLevels[pstart] >= maxLevel)
							break;
					}
					if (pstart > end)
						break;
					int pend = pstart + 1;
					for (; pend <= end; ++pend) {
						if (orderLevels[pend] < maxLevel)
							break;
					}
					flip(pstart, pend);
					pstart = pend + 1;
				}
			}
		}
    
		public void flip(int start, int end) {
			int mid = (start + end) / 2;
			--end;
			for (; start < mid; ++start, --end) {
				int temp = indexChars[start];
				indexChars[start] = indexChars[end];
				indexChars[end] = temp;
			}
		}
    
		public static bool isWS(char c) {
			return (c <= ' ');
		}

		static BidiLine() {
			mirrorChars[0x0028] = 0x0029; // LEFT PARENTHESIS
			mirrorChars[0x0029] = 0x0028; // RIGHT PARENTHESIS
			mirrorChars[0x003C] = 0x003E; // LESS-THAN SIGN
			mirrorChars[0x003E] = 0x003C; // GREATER-THAN SIGN
			mirrorChars[0x005B] = 0x005D; // LEFT SQUARE BRACKET
			mirrorChars[0x005D] = 0x005B; // RIGHT SQUARE BRACKET
			mirrorChars[0x007B] = 0x007D; // LEFT CURLY BRACKET
			mirrorChars[0x007D] = 0x007B; // RIGHT CURLY BRACKET
			mirrorChars[0x00AB] = 0x00BB; // LEFT-POINTING DOUBLE ANGLE QUOTATION MARK
			mirrorChars[0x00BB] = 0x00AB; // RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK
			mirrorChars[0x2039] = 0x203A; // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
			mirrorChars[0x203A] = 0x2039; // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
			mirrorChars[0x2045] = 0x2046; // LEFT SQUARE BRACKET WITH QUILL
			mirrorChars[0x2046] = 0x2045; // RIGHT SQUARE BRACKET WITH QUILL
			mirrorChars[0x207D] = 0x207E; // SUPERSCRIPT LEFT PARENTHESIS
			mirrorChars[0x207E] = 0x207D; // SUPERSCRIPT RIGHT PARENTHESIS
			mirrorChars[0x208D] = 0x208E; // SUBSCRIPT LEFT PARENTHESIS
			mirrorChars[0x208E] = 0x208D; // SUBSCRIPT RIGHT PARENTHESIS
			mirrorChars[0x2208] = 0x220B; // ELEMENT OF
			mirrorChars[0x2209] = 0x220C; // NOT AN ELEMENT OF
			mirrorChars[0x220A] = 0x220D; // SMALL ELEMENT OF
			mirrorChars[0x220B] = 0x2208; // CONTAINS AS MEMBER
			mirrorChars[0x220C] = 0x2209; // DOES NOT CONTAIN AS MEMBER
			mirrorChars[0x220D] = 0x220A; // SMALL CONTAINS AS MEMBER
			mirrorChars[0x2215] = 0x29F5; // DIVISION SLASH
			mirrorChars[0x223C] = 0x223D; // TILDE OPERATOR
			mirrorChars[0x223D] = 0x223C; // REVERSED TILDE
			mirrorChars[0x2243] = 0x22CD; // ASYMPTOTICALLY EQUAL TO
			mirrorChars[0x2252] = 0x2253; // APPROXIMATELY EQUAL TO OR THE IMAGE OF
			mirrorChars[0x2253] = 0x2252; // IMAGE OF OR APPROXIMATELY EQUAL TO
			mirrorChars[0x2254] = 0x2255; // COLON EQUALS
			mirrorChars[0x2255] = 0x2254; // EQUALS COLON
			mirrorChars[0x2264] = 0x2265; // LESS-THAN OR EQUAL TO
			mirrorChars[0x2265] = 0x2264; // GREATER-THAN OR EQUAL TO
			mirrorChars[0x2266] = 0x2267; // LESS-THAN OVER EQUAL TO
			mirrorChars[0x2267] = 0x2266; // GREATER-THAN OVER EQUAL TO
			mirrorChars[0x2268] = 0x2269; // [BEST FIT] LESS-THAN BUT NOT EQUAL TO
			mirrorChars[0x2269] = 0x2268; // [BEST FIT] GREATER-THAN BUT NOT EQUAL TO
			mirrorChars[0x226A] = 0x226B; // MUCH LESS-THAN
			mirrorChars[0x226B] = 0x226A; // MUCH GREATER-THAN
			mirrorChars[0x226E] = 0x226F; // [BEST FIT] NOT LESS-THAN
			mirrorChars[0x226F] = 0x226E; // [BEST FIT] NOT GREATER-THAN
			mirrorChars[0x2270] = 0x2271; // [BEST FIT] NEITHER LESS-THAN NOR EQUAL TO
			mirrorChars[0x2271] = 0x2270; // [BEST FIT] NEITHER GREATER-THAN NOR EQUAL TO
			mirrorChars[0x2272] = 0x2273; // [BEST FIT] LESS-THAN OR EQUIVALENT TO
			mirrorChars[0x2273] = 0x2272; // [BEST FIT] GREATER-THAN OR EQUIVALENT TO
			mirrorChars[0x2274] = 0x2275; // [BEST FIT] NEITHER LESS-THAN NOR EQUIVALENT TO
			mirrorChars[0x2275] = 0x2274; // [BEST FIT] NEITHER GREATER-THAN NOR EQUIVALENT TO
			mirrorChars[0x2276] = 0x2277; // LESS-THAN OR GREATER-THAN
			mirrorChars[0x2277] = 0x2276; // GREATER-THAN OR LESS-THAN
			mirrorChars[0x2278] = 0x2279; // NEITHER LESS-THAN NOR GREATER-THAN
			mirrorChars[0x2279] = 0x2278; // NEITHER GREATER-THAN NOR LESS-THAN
			mirrorChars[0x227A] = 0x227B; // PRECEDES
			mirrorChars[0x227B] = 0x227A; // SUCCEEDS
			mirrorChars[0x227C] = 0x227D; // PRECEDES OR EQUAL TO
			mirrorChars[0x227D] = 0x227C; // SUCCEEDS OR EQUAL TO
			mirrorChars[0x227E] = 0x227F; // [BEST FIT] PRECEDES OR EQUIVALENT TO
			mirrorChars[0x227F] = 0x227E; // [BEST FIT] SUCCEEDS OR EQUIVALENT TO
			mirrorChars[0x2280] = 0x2281; // [BEST FIT] DOES NOT PRECEDE
			mirrorChars[0x2281] = 0x2280; // [BEST FIT] DOES NOT SUCCEED
			mirrorChars[0x2282] = 0x2283; // SUBSET OF
			mirrorChars[0x2283] = 0x2282; // SUPERSET OF
			mirrorChars[0x2284] = 0x2285; // [BEST FIT] NOT A SUBSET OF
			mirrorChars[0x2285] = 0x2284; // [BEST FIT] NOT A SUPERSET OF
			mirrorChars[0x2286] = 0x2287; // SUBSET OF OR EQUAL TO
			mirrorChars[0x2287] = 0x2286; // SUPERSET OF OR EQUAL TO
			mirrorChars[0x2288] = 0x2289; // [BEST FIT] NEITHER A SUBSET OF NOR EQUAL TO
			mirrorChars[0x2289] = 0x2288; // [BEST FIT] NEITHER A SUPERSET OF NOR EQUAL TO
			mirrorChars[0x228A] = 0x228B; // [BEST FIT] SUBSET OF WITH NOT EQUAL TO
			mirrorChars[0x228B] = 0x228A; // [BEST FIT] SUPERSET OF WITH NOT EQUAL TO
			mirrorChars[0x228F] = 0x2290; // SQUARE IMAGE OF
			mirrorChars[0x2290] = 0x228F; // SQUARE ORIGINAL OF
			mirrorChars[0x2291] = 0x2292; // SQUARE IMAGE OF OR EQUAL TO
			mirrorChars[0x2292] = 0x2291; // SQUARE ORIGINAL OF OR EQUAL TO
			mirrorChars[0x2298] = 0x29B8; // CIRCLED DIVISION SLASH
			mirrorChars[0x22A2] = 0x22A3; // RIGHT TACK
			mirrorChars[0x22A3] = 0x22A2; // LEFT TACK
			mirrorChars[0x22A6] = 0x2ADE; // ASSERTION
			mirrorChars[0x22A8] = 0x2AE4; // TRUE
			mirrorChars[0x22A9] = 0x2AE3; // FORCES
			mirrorChars[0x22AB] = 0x2AE5; // DOUBLE VERTICAL BAR DOUBLE RIGHT TURNSTILE
			mirrorChars[0x22B0] = 0x22B1; // PRECEDES UNDER RELATION
			mirrorChars[0x22B1] = 0x22B0; // SUCCEEDS UNDER RELATION
			mirrorChars[0x22B2] = 0x22B3; // NORMAL SUBGROUP OF
			mirrorChars[0x22B3] = 0x22B2; // CONTAINS AS NORMAL SUBGROUP
			mirrorChars[0x22B4] = 0x22B5; // NORMAL SUBGROUP OF OR EQUAL TO
			mirrorChars[0x22B5] = 0x22B4; // CONTAINS AS NORMAL SUBGROUP OR EQUAL TO
			mirrorChars[0x22B6] = 0x22B7; // ORIGINAL OF
			mirrorChars[0x22B7] = 0x22B6; // IMAGE OF
			mirrorChars[0x22C9] = 0x22CA; // LEFT NORMAL FACTOR SEMIDIRECT PRODUCT
			mirrorChars[0x22CA] = 0x22C9; // RIGHT NORMAL FACTOR SEMIDIRECT PRODUCT
			mirrorChars[0x22CB] = 0x22CC; // LEFT SEMIDIRECT PRODUCT
			mirrorChars[0x22CC] = 0x22CB; // RIGHT SEMIDIRECT PRODUCT
			mirrorChars[0x22CD] = 0x2243; // REVERSED TILDE EQUALS
			mirrorChars[0x22D0] = 0x22D1; // DOUBLE SUBSET
			mirrorChars[0x22D1] = 0x22D0; // DOUBLE SUPERSET
			mirrorChars[0x22D6] = 0x22D7; // LESS-THAN WITH DOT
			mirrorChars[0x22D7] = 0x22D6; // GREATER-THAN WITH DOT
			mirrorChars[0x22D8] = 0x22D9; // VERY MUCH LESS-THAN
			mirrorChars[0x22D9] = 0x22D8; // VERY MUCH GREATER-THAN
			mirrorChars[0x22DA] = 0x22DB; // LESS-THAN EQUAL TO OR GREATER-THAN
			mirrorChars[0x22DB] = 0x22DA; // GREATER-THAN EQUAL TO OR LESS-THAN
			mirrorChars[0x22DC] = 0x22DD; // EQUAL TO OR LESS-THAN
			mirrorChars[0x22DD] = 0x22DC; // EQUAL TO OR GREATER-THAN
			mirrorChars[0x22DE] = 0x22DF; // EQUAL TO OR PRECEDES
			mirrorChars[0x22DF] = 0x22DE; // EQUAL TO OR SUCCEEDS
			mirrorChars[0x22E0] = 0x22E1; // [BEST FIT] DOES NOT PRECEDE OR EQUAL
			mirrorChars[0x22E1] = 0x22E0; // [BEST FIT] DOES NOT SUCCEED OR EQUAL
			mirrorChars[0x22E2] = 0x22E3; // [BEST FIT] NOT SQUARE IMAGE OF OR EQUAL TO
			mirrorChars[0x22E3] = 0x22E2; // [BEST FIT] NOT SQUARE ORIGINAL OF OR EQUAL TO
			mirrorChars[0x22E4] = 0x22E5; // [BEST FIT] SQUARE IMAGE OF OR NOT EQUAL TO
			mirrorChars[0x22E5] = 0x22E4; // [BEST FIT] SQUARE ORIGINAL OF OR NOT EQUAL TO
			mirrorChars[0x22E6] = 0x22E7; // [BEST FIT] LESS-THAN BUT NOT EQUIVALENT TO
			mirrorChars[0x22E7] = 0x22E6; // [BEST FIT] GREATER-THAN BUT NOT EQUIVALENT TO
			mirrorChars[0x22E8] = 0x22E9; // [BEST FIT] PRECEDES BUT NOT EQUIVALENT TO
			mirrorChars[0x22E9] = 0x22E8; // [BEST FIT] SUCCEEDS BUT NOT EQUIVALENT TO
			mirrorChars[0x22EA] = 0x22EB; // [BEST FIT] NOT NORMAL SUBGROUP OF
			mirrorChars[0x22EB] = 0x22EA; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP
			mirrorChars[0x22EC] = 0x22ED; // [BEST FIT] NOT NORMAL SUBGROUP OF OR EQUAL TO
			mirrorChars[0x22ED] = 0x22EC; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP OR EQUAL
			mirrorChars[0x22F0] = 0x22F1; // UP RIGHT DIAGONAL ELLIPSIS
			mirrorChars[0x22F1] = 0x22F0; // DOWN RIGHT DIAGONAL ELLIPSIS
			mirrorChars[0x22F2] = 0x22FA; // ELEMENT OF WITH LONG HORIZONTAL STROKE
			mirrorChars[0x22F3] = 0x22FB; // ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
			mirrorChars[0x22F4] = 0x22FC; // SMALL ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
			mirrorChars[0x22F6] = 0x22FD; // ELEMENT OF WITH OVERBAR
			mirrorChars[0x22F7] = 0x22FE; // SMALL ELEMENT OF WITH OVERBAR
			mirrorChars[0x22FA] = 0x22F2; // CONTAINS WITH LONG HORIZONTAL STROKE
			mirrorChars[0x22FB] = 0x22F3; // CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
			mirrorChars[0x22FC] = 0x22F4; // SMALL CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
			mirrorChars[0x22FD] = 0x22F6; // CONTAINS WITH OVERBAR
			mirrorChars[0x22FE] = 0x22F7; // SMALL CONTAINS WITH OVERBAR
			mirrorChars[0x2308] = 0x2309; // LEFT CEILING
			mirrorChars[0x2309] = 0x2308; // RIGHT CEILING
			mirrorChars[0x230A] = 0x230B; // LEFT FLOOR
			mirrorChars[0x230B] = 0x230A; // RIGHT FLOOR
			mirrorChars[0x2329] = 0x232A; // LEFT-POINTING ANGLE BRACKET
			mirrorChars[0x232A] = 0x2329; // RIGHT-POINTING ANGLE BRACKET
			mirrorChars[0x2768] = 0x2769; // MEDIUM LEFT PARENTHESIS ORNAMENT
			mirrorChars[0x2769] = 0x2768; // MEDIUM RIGHT PARENTHESIS ORNAMENT
			mirrorChars[0x276A] = 0x276B; // MEDIUM FLATTENED LEFT PARENTHESIS ORNAMENT
			mirrorChars[0x276B] = 0x276A; // MEDIUM FLATTENED RIGHT PARENTHESIS ORNAMENT
			mirrorChars[0x276C] = 0x276D; // MEDIUM LEFT-POINTING ANGLE BRACKET ORNAMENT
			mirrorChars[0x276D] = 0x276C; // MEDIUM RIGHT-POINTING ANGLE BRACKET ORNAMENT
			mirrorChars[0x276E] = 0x276F; // HEAVY LEFT-POINTING ANGLE QUOTATION MARK ORNAMENT
			mirrorChars[0x276F] = 0x276E; // HEAVY RIGHT-POINTING ANGLE QUOTATION MARK ORNAMENT
			mirrorChars[0x2770] = 0x2771; // HEAVY LEFT-POINTING ANGLE BRACKET ORNAMENT
			mirrorChars[0x2771] = 0x2770; // HEAVY RIGHT-POINTING ANGLE BRACKET ORNAMENT
			mirrorChars[0x2772] = 0x2773; // LIGHT LEFT TORTOISE SHELL BRACKET
			mirrorChars[0x2773] = 0x2772; // LIGHT RIGHT TORTOISE SHELL BRACKET
			mirrorChars[0x2774] = 0x2775; // MEDIUM LEFT CURLY BRACKET ORNAMENT
			mirrorChars[0x2775] = 0x2774; // MEDIUM RIGHT CURLY BRACKET ORNAMENT
			mirrorChars[0x27D5] = 0x27D6; // LEFT OUTER JOIN
			mirrorChars[0x27D6] = 0x27D5; // RIGHT OUTER JOIN
			mirrorChars[0x27DD] = 0x27DE; // LONG RIGHT TACK
			mirrorChars[0x27DE] = 0x27DD; // LONG LEFT TACK
			mirrorChars[0x27E2] = 0x27E3; // WHITE CONCAVE-SIDED DIAMOND WITH LEFTWARDS TICK
			mirrorChars[0x27E3] = 0x27E2; // WHITE CONCAVE-SIDED DIAMOND WITH RIGHTWARDS TICK
			mirrorChars[0x27E4] = 0x27E5; // WHITE SQUARE WITH LEFTWARDS TICK
			mirrorChars[0x27E5] = 0x27E4; // WHITE SQUARE WITH RIGHTWARDS TICK
			mirrorChars[0x27E6] = 0x27E7; // MATHEMATICAL LEFT WHITE SQUARE BRACKET
			mirrorChars[0x27E7] = 0x27E6; // MATHEMATICAL RIGHT WHITE SQUARE BRACKET
			mirrorChars[0x27E8] = 0x27E9; // MATHEMATICAL LEFT ANGLE BRACKET
			mirrorChars[0x27E9] = 0x27E8; // MATHEMATICAL RIGHT ANGLE BRACKET
			mirrorChars[0x27EA] = 0x27EB; // MATHEMATICAL LEFT DOUBLE ANGLE BRACKET
			mirrorChars[0x27EB] = 0x27EA; // MATHEMATICAL RIGHT DOUBLE ANGLE BRACKET
			mirrorChars[0x2983] = 0x2984; // LEFT WHITE CURLY BRACKET
			mirrorChars[0x2984] = 0x2983; // RIGHT WHITE CURLY BRACKET
			mirrorChars[0x2985] = 0x2986; // LEFT WHITE PARENTHESIS
			mirrorChars[0x2986] = 0x2985; // RIGHT WHITE PARENTHESIS
			mirrorChars[0x2987] = 0x2988; // Z NOTATION LEFT IMAGE BRACKET
			mirrorChars[0x2988] = 0x2987; // Z NOTATION RIGHT IMAGE BRACKET
			mirrorChars[0x2989] = 0x298A; // Z NOTATION LEFT BINDING BRACKET
			mirrorChars[0x298A] = 0x2989; // Z NOTATION RIGHT BINDING BRACKET
			mirrorChars[0x298B] = 0x298C; // LEFT SQUARE BRACKET WITH UNDERBAR
			mirrorChars[0x298C] = 0x298B; // RIGHT SQUARE BRACKET WITH UNDERBAR
			mirrorChars[0x298D] = 0x2990; // LEFT SQUARE BRACKET WITH TICK IN TOP CORNER
			mirrorChars[0x298E] = 0x298F; // RIGHT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
			mirrorChars[0x298F] = 0x298E; // LEFT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
			mirrorChars[0x2990] = 0x298D; // RIGHT SQUARE BRACKET WITH TICK IN TOP CORNER
			mirrorChars[0x2991] = 0x2992; // LEFT ANGLE BRACKET WITH DOT
			mirrorChars[0x2992] = 0x2991; // RIGHT ANGLE BRACKET WITH DOT
			mirrorChars[0x2993] = 0x2994; // LEFT ARC LESS-THAN BRACKET
			mirrorChars[0x2994] = 0x2993; // RIGHT ARC GREATER-THAN BRACKET
			mirrorChars[0x2995] = 0x2996; // DOUBLE LEFT ARC GREATER-THAN BRACKET
			mirrorChars[0x2996] = 0x2995; // DOUBLE RIGHT ARC LESS-THAN BRACKET
			mirrorChars[0x2997] = 0x2998; // LEFT BLACK TORTOISE SHELL BRACKET
			mirrorChars[0x2998] = 0x2997; // RIGHT BLACK TORTOISE SHELL BRACKET
			mirrorChars[0x29B8] = 0x2298; // CIRCLED REVERSE SOLIDUS
			mirrorChars[0x29C0] = 0x29C1; // CIRCLED LESS-THAN
			mirrorChars[0x29C1] = 0x29C0; // CIRCLED GREATER-THAN
			mirrorChars[0x29C4] = 0x29C5; // SQUARED RISING DIAGONAL SLASH
			mirrorChars[0x29C5] = 0x29C4; // SQUARED FALLING DIAGONAL SLASH
			mirrorChars[0x29CF] = 0x29D0; // LEFT TRIANGLE BESIDE VERTICAL BAR
			mirrorChars[0x29D0] = 0x29CF; // VERTICAL BAR BESIDE RIGHT TRIANGLE
			mirrorChars[0x29D1] = 0x29D2; // BOWTIE WITH LEFT HALF BLACK
			mirrorChars[0x29D2] = 0x29D1; // BOWTIE WITH RIGHT HALF BLACK
			mirrorChars[0x29D4] = 0x29D5; // TIMES WITH LEFT HALF BLACK
			mirrorChars[0x29D5] = 0x29D4; // TIMES WITH RIGHT HALF BLACK
			mirrorChars[0x29D8] = 0x29D9; // LEFT WIGGLY FENCE
			mirrorChars[0x29D9] = 0x29D8; // RIGHT WIGGLY FENCE
			mirrorChars[0x29DA] = 0x29DB; // LEFT DOUBLE WIGGLY FENCE
			mirrorChars[0x29DB] = 0x29DA; // RIGHT DOUBLE WIGGLY FENCE
			mirrorChars[0x29F5] = 0x2215; // REVERSE SOLIDUS OPERATOR
			mirrorChars[0x29F8] = 0x29F9; // BIG SOLIDUS
			mirrorChars[0x29F9] = 0x29F8; // BIG REVERSE SOLIDUS
			mirrorChars[0x29FC] = 0x29FD; // LEFT-POINTING CURVED ANGLE BRACKET
			mirrorChars[0x29FD] = 0x29FC; // RIGHT-POINTING CURVED ANGLE BRACKET
			mirrorChars[0x2A2B] = 0x2A2C; // MINUS SIGN WITH FALLING DOTS
			mirrorChars[0x2A2C] = 0x2A2B; // MINUS SIGN WITH RISING DOTS
			mirrorChars[0x2A2D] = 0x2A2C; // PLUS SIGN IN LEFT HALF CIRCLE
			mirrorChars[0x2A2E] = 0x2A2D; // PLUS SIGN IN RIGHT HALF CIRCLE
			mirrorChars[0x2A34] = 0x2A35; // MULTIPLICATION SIGN IN LEFT HALF CIRCLE
			mirrorChars[0x2A35] = 0x2A34; // MULTIPLICATION SIGN IN RIGHT HALF CIRCLE
			mirrorChars[0x2A3C] = 0x2A3D; // INTERIOR PRODUCT
			mirrorChars[0x2A3D] = 0x2A3C; // RIGHTHAND INTERIOR PRODUCT
			mirrorChars[0x2A64] = 0x2A65; // Z NOTATION DOMAIN ANTIRESTRICTION
			mirrorChars[0x2A65] = 0x2A64; // Z NOTATION RANGE ANTIRESTRICTION
			mirrorChars[0x2A79] = 0x2A7A; // LESS-THAN WITH CIRCLE INSIDE
			mirrorChars[0x2A7A] = 0x2A79; // GREATER-THAN WITH CIRCLE INSIDE
			mirrorChars[0x2A7D] = 0x2A7E; // LESS-THAN OR SLANTED EQUAL TO
			mirrorChars[0x2A7E] = 0x2A7D; // GREATER-THAN OR SLANTED EQUAL TO
			mirrorChars[0x2A7F] = 0x2A80; // LESS-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
			mirrorChars[0x2A80] = 0x2A7F; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
			mirrorChars[0x2A81] = 0x2A82; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
			mirrorChars[0x2A82] = 0x2A81; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
			mirrorChars[0x2A83] = 0x2A84; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE RIGHT
			mirrorChars[0x2A84] = 0x2A83; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE LEFT
			mirrorChars[0x2A8B] = 0x2A8C; // LESS-THAN ABOVE DOUBLE-LINE EQUAL ABOVE GREATER-THAN
			mirrorChars[0x2A8C] = 0x2A8B; // GREATER-THAN ABOVE DOUBLE-LINE EQUAL ABOVE LESS-THAN
			mirrorChars[0x2A91] = 0x2A92; // LESS-THAN ABOVE GREATER-THAN ABOVE DOUBLE-LINE EQUAL
			mirrorChars[0x2A92] = 0x2A91; // GREATER-THAN ABOVE LESS-THAN ABOVE DOUBLE-LINE EQUAL
			mirrorChars[0x2A93] = 0x2A94; // LESS-THAN ABOVE SLANTED EQUAL ABOVE GREATER-THAN ABOVE SLANTED EQUAL
			mirrorChars[0x2A94] = 0x2A93; // GREATER-THAN ABOVE SLANTED EQUAL ABOVE LESS-THAN ABOVE SLANTED EQUAL
			mirrorChars[0x2A95] = 0x2A96; // SLANTED EQUAL TO OR LESS-THAN
			mirrorChars[0x2A96] = 0x2A95; // SLANTED EQUAL TO OR GREATER-THAN
			mirrorChars[0x2A97] = 0x2A98; // SLANTED EQUAL TO OR LESS-THAN WITH DOT INSIDE
			mirrorChars[0x2A98] = 0x2A97; // SLANTED EQUAL TO OR GREATER-THAN WITH DOT INSIDE
			mirrorChars[0x2A99] = 0x2A9A; // DOUBLE-LINE EQUAL TO OR LESS-THAN
			mirrorChars[0x2A9A] = 0x2A99; // DOUBLE-LINE EQUAL TO OR GREATER-THAN
			mirrorChars[0x2A9B] = 0x2A9C; // DOUBLE-LINE SLANTED EQUAL TO OR LESS-THAN
			mirrorChars[0x2A9C] = 0x2A9B; // DOUBLE-LINE SLANTED EQUAL TO OR GREATER-THAN
			mirrorChars[0x2AA1] = 0x2AA2; // DOUBLE NESTED LESS-THAN
			mirrorChars[0x2AA2] = 0x2AA1; // DOUBLE NESTED GREATER-THAN
			mirrorChars[0x2AA6] = 0x2AA7; // LESS-THAN CLOSED BY CURVE
			mirrorChars[0x2AA7] = 0x2AA6; // GREATER-THAN CLOSED BY CURVE
			mirrorChars[0x2AA8] = 0x2AA9; // LESS-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
			mirrorChars[0x2AA9] = 0x2AA8; // GREATER-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
			mirrorChars[0x2AAA] = 0x2AAB; // SMALLER THAN
			mirrorChars[0x2AAB] = 0x2AAA; // LARGER THAN
			mirrorChars[0x2AAC] = 0x2AAD; // SMALLER THAN OR EQUAL TO
			mirrorChars[0x2AAD] = 0x2AAC; // LARGER THAN OR EQUAL TO
			mirrorChars[0x2AAF] = 0x2AB0; // PRECEDES ABOVE SINGLE-LINE EQUALS SIGN
			mirrorChars[0x2AB0] = 0x2AAF; // SUCCEEDS ABOVE SINGLE-LINE EQUALS SIGN
			mirrorChars[0x2AB3] = 0x2AB4; // PRECEDES ABOVE EQUALS SIGN
			mirrorChars[0x2AB4] = 0x2AB3; // SUCCEEDS ABOVE EQUALS SIGN
			mirrorChars[0x2ABB] = 0x2ABC; // DOUBLE PRECEDES
			mirrorChars[0x2ABC] = 0x2ABB; // DOUBLE SUCCEEDS
			mirrorChars[0x2ABD] = 0x2ABE; // SUBSET WITH DOT
			mirrorChars[0x2ABE] = 0x2ABD; // SUPERSET WITH DOT
			mirrorChars[0x2ABF] = 0x2AC0; // SUBSET WITH PLUS SIGN BELOW
			mirrorChars[0x2AC0] = 0x2ABF; // SUPERSET WITH PLUS SIGN BELOW
			mirrorChars[0x2AC1] = 0x2AC2; // SUBSET WITH MULTIPLICATION SIGN BELOW
			mirrorChars[0x2AC2] = 0x2AC1; // SUPERSET WITH MULTIPLICATION SIGN BELOW
			mirrorChars[0x2AC3] = 0x2AC4; // SUBSET OF OR EQUAL TO WITH DOT ABOVE
			mirrorChars[0x2AC4] = 0x2AC3; // SUPERSET OF OR EQUAL TO WITH DOT ABOVE
			mirrorChars[0x2AC5] = 0x2AC6; // SUBSET OF ABOVE EQUALS SIGN
			mirrorChars[0x2AC6] = 0x2AC5; // SUPERSET OF ABOVE EQUALS SIGN
			mirrorChars[0x2ACD] = 0x2ACE; // SQUARE LEFT OPEN BOX OPERATOR
			mirrorChars[0x2ACE] = 0x2ACD; // SQUARE RIGHT OPEN BOX OPERATOR
			mirrorChars[0x2ACF] = 0x2AD0; // CLOSED SUBSET
			mirrorChars[0x2AD0] = 0x2ACF; // CLOSED SUPERSET
			mirrorChars[0x2AD1] = 0x2AD2; // CLOSED SUBSET OR EQUAL TO
			mirrorChars[0x2AD2] = 0x2AD1; // CLOSED SUPERSET OR EQUAL TO
			mirrorChars[0x2AD3] = 0x2AD4; // SUBSET ABOVE SUPERSET
			mirrorChars[0x2AD4] = 0x2AD3; // SUPERSET ABOVE SUBSET
			mirrorChars[0x2AD5] = 0x2AD6; // SUBSET ABOVE SUBSET
			mirrorChars[0x2AD6] = 0x2AD5; // SUPERSET ABOVE SUPERSET
			mirrorChars[0x2ADE] = 0x22A6; // SHORT LEFT TACK
			mirrorChars[0x2AE3] = 0x22A9; // DOUBLE VERTICAL BAR LEFT TURNSTILE
			mirrorChars[0x2AE4] = 0x22A8; // VERTICAL BAR DOUBLE LEFT TURNSTILE
			mirrorChars[0x2AE5] = 0x22AB; // DOUBLE VERTICAL BAR DOUBLE LEFT TURNSTILE
			mirrorChars[0x2AEC] = 0x2AED; // DOUBLE STROKE NOT SIGN
			mirrorChars[0x2AED] = 0x2AEC; // REVERSED DOUBLE STROKE NOT SIGN
			mirrorChars[0x2AF7] = 0x2AF8; // TRIPLE NESTED LESS-THAN
			mirrorChars[0x2AF8] = 0x2AF7; // TRIPLE NESTED GREATER-THAN
			mirrorChars[0x2AF9] = 0x2AFA; // DOUBLE-LINE SLANTED LESS-THAN OR EQUAL TO
			mirrorChars[0x2AFA] = 0x2AF9; // DOUBLE-LINE SLANTED GREATER-THAN OR EQUAL TO
			mirrorChars[0x3008] = 0x3009; // LEFT ANGLE BRACKET
			mirrorChars[0x3009] = 0x3008; // RIGHT ANGLE BRACKET
			mirrorChars[0x300A] = 0x300B; // LEFT DOUBLE ANGLE BRACKET
			mirrorChars[0x300B] = 0x300A; // RIGHT DOUBLE ANGLE BRACKET
			mirrorChars[0x300C] = 0x300D; // [BEST FIT] LEFT CORNER BRACKET
			mirrorChars[0x300D] = 0x300C; // [BEST FIT] RIGHT CORNER BRACKET
			mirrorChars[0x300E] = 0x300F; // [BEST FIT] LEFT WHITE CORNER BRACKET
			mirrorChars[0x300F] = 0x300E; // [BEST FIT] RIGHT WHITE CORNER BRACKET
			mirrorChars[0x3010] = 0x3011; // LEFT BLACK LENTICULAR BRACKET
			mirrorChars[0x3011] = 0x3010; // RIGHT BLACK LENTICULAR BRACKET
			mirrorChars[0x3014] = 0x3015; // LEFT TORTOISE SHELL BRACKET
			mirrorChars[0x3015] = 0x3014; // RIGHT TORTOISE SHELL BRACKET
			mirrorChars[0x3016] = 0x3017; // LEFT WHITE LENTICULAR BRACKET
			mirrorChars[0x3017] = 0x3016; // RIGHT WHITE LENTICULAR BRACKET
			mirrorChars[0x3018] = 0x3019; // LEFT WHITE TORTOISE SHELL BRACKET
			mirrorChars[0x3019] = 0x3018; // RIGHT WHITE TORTOISE SHELL BRACKET
			mirrorChars[0x301A] = 0x301B; // LEFT WHITE SQUARE BRACKET
			mirrorChars[0x301B] = 0x301A; // RIGHT WHITE SQUARE BRACKET
			mirrorChars[0xFF08] = 0xFF09; // FULLWIDTH LEFT PARENTHESIS
			mirrorChars[0xFF09] = 0xFF08; // FULLWIDTH RIGHT PARENTHESIS
			mirrorChars[0xFF1C] = 0xFF1E; // FULLWIDTH LESS-THAN SIGN
			mirrorChars[0xFF1E] = 0xFF1C; // FULLWIDTH GREATER-THAN SIGN
			mirrorChars[0xFF3B] = 0xFF3D; // FULLWIDTH LEFT SQUARE BRACKET
			mirrorChars[0xFF3D] = 0xFF3B; // FULLWIDTH RIGHT SQUARE BRACKET
			mirrorChars[0xFF5B] = 0xFF5D; // FULLWIDTH LEFT CURLY BRACKET
			mirrorChars[0xFF5D] = 0xFF5B; // FULLWIDTH RIGHT CURLY BRACKET
			mirrorChars[0xFF5F] = 0xFF60; // FULLWIDTH LEFT WHITE PARENTHESIS
			mirrorChars[0xFF60] = 0xFF5F; // FULLWIDTH RIGHT WHITE PARENTHESIS
			mirrorChars[0xFF62] = 0xFF63; // [BEST FIT] HALFWIDTH LEFT CORNER BRACKET
			mirrorChars[0xFF63] = 0xFF62; // [BEST FIT] HALFWIDTH RIGHT CORNER BRACKET
		}
	}
}