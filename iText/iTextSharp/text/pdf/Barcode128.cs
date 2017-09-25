using System;
using System.Drawing;

/*
 * Copyright 2002 by Paulo Soares.
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
	/** Implements the code 128 and UCC/EAN-128. Other symbologies are allowed in raw mode.<p>
	 * The code types allowed are:<br>
	 * <ul>
	 * <li><b>CODE128</b> - plain barcode 128.
	 * <li><b>CODE128_UCC</b> - support for UCC/EAN-128.
	 * <li><b>CODE128_RAW</b> - raw mode. The code attribute has the actual codes from 0
	 *     to 105 followed by '&#92;uffff' and the human readable text.
	 * </ul>
	 * The default parameters are:
	 * <pre>
	 * x = 0.8f;
	 * font = BaseFont.createFont("Helvetica", "winansi", false);
	 * size = 8;
	 * baseline = size;
	 * barHeight = size * 3;
	 * textint= Element.ALIGN_CENTER;
	 * codeType = CODE128;
	 * </pre>
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class Barcode128 : Barcode {

    /** The bars to generate the code.
     */    
    internal static byte[][] BARS = 
    {
        new byte[] {2, 1, 2, 2, 2, 2},
        new byte[] {2, 2, 2, 1, 2, 2},
        new byte[] {2, 2, 2, 2, 2, 1},
        new byte[] {1, 2, 1, 2, 2, 3},
        new byte[] {1, 2, 1, 3, 2, 2},
        new byte[] {1, 3, 1, 2, 2, 2},
        new byte[] {1, 2, 2, 2, 1, 3},
        new byte[] {1, 2, 2, 3, 1, 2},
        new byte[] {1, 3, 2, 2, 1, 2},
        new byte[] {2, 2, 1, 2, 1, 3},
        new byte[] {2, 2, 1, 3, 1, 2},
        new byte[] {2, 3, 1, 2, 1, 2},
        new byte[] {1, 1, 2, 2, 3, 2},
        new byte[] {1, 2, 2, 1, 3, 2},
        new byte[] {1, 2, 2, 2, 3, 1},
        new byte[] {1, 1, 3, 2, 2, 2},
        new byte[] {1, 2, 3, 1, 2, 2},
        new byte[] {1, 2, 3, 2, 2, 1},
        new byte[] {2, 2, 3, 2, 1, 1},
        new byte[] {2, 2, 1, 1, 3, 2},
        new byte[] {2, 2, 1, 2, 3, 1},
        new byte[] {2, 1, 3, 2, 1, 2},
        new byte[] {2, 2, 3, 1, 1, 2},
        new byte[] {3, 1, 2, 1, 3, 1},
        new byte[] {3, 1, 1, 2, 2, 2},
        new byte[] {3, 2, 1, 1, 2, 2},
        new byte[] {3, 2, 1, 2, 2, 1},
        new byte[] {3, 1, 2, 2, 1, 2},
        new byte[] {3, 2, 2, 1, 1, 2},
        new byte[] {3, 2, 2, 2, 1, 1},
        new byte[] {2, 1, 2, 1, 2, 3},
        new byte[] {2, 1, 2, 3, 2, 1},
        new byte[] {2, 3, 2, 1, 2, 1},
        new byte[] {1, 1, 1, 3, 2, 3},
        new byte[] {1, 3, 1, 1, 2, 3},
        new byte[] {1, 3, 1, 3, 2, 1},
        new byte[] {1, 1, 2, 3, 1, 3},
        new byte[] {1, 3, 2, 1, 1, 3},
        new byte[] {1, 3, 2, 3, 1, 1},
        new byte[] {2, 1, 1, 3, 1, 3},
        new byte[] {2, 3, 1, 1, 1, 3},
        new byte[] {2, 3, 1, 3, 1, 1},
        new byte[] {1, 1, 2, 1, 3, 3},
        new byte[] {1, 1, 2, 3, 3, 1},
        new byte[] {1, 3, 2, 1, 3, 1},
        new byte[] {1, 1, 3, 1, 2, 3},
        new byte[] {1, 1, 3, 3, 2, 1},
        new byte[] {1, 3, 3, 1, 2, 1},
        new byte[] {3, 1, 3, 1, 2, 1},
        new byte[] {2, 1, 1, 3, 3, 1},
        new byte[] {2, 3, 1, 1, 3, 1},
        new byte[] {2, 1, 3, 1, 1, 3},
        new byte[] {2, 1, 3, 3, 1, 1},
        new byte[] {2, 1, 3, 1, 3, 1},
        new byte[] {3, 1, 1, 1, 2, 3},
        new byte[] {3, 1, 1, 3, 2, 1},
        new byte[] {3, 3, 1, 1, 2, 1},
        new byte[] {3, 1, 2, 1, 1, 3},
        new byte[] {3, 1, 2, 3, 1, 1},
        new byte[] {3, 3, 2, 1, 1, 1},
        new byte[] {3, 1, 4, 1, 1, 1},
        new byte[] {2, 2, 1, 4, 1, 1},
        new byte[] {4, 3, 1, 1, 1, 1},
        new byte[] {1, 1, 1, 2, 2, 4},
        new byte[] {1, 1, 1, 4, 2, 2},
        new byte[] {1, 2, 1, 1, 2, 4},
        new byte[] {1, 2, 1, 4, 2, 1},
        new byte[] {1, 4, 1, 1, 2, 2},
        new byte[] {1, 4, 1, 2, 2, 1},
        new byte[] {1, 1, 2, 2, 1, 4},
        new byte[] {1, 1, 2, 4, 1, 2},
        new byte[] {1, 2, 2, 1, 1, 4},
        new byte[] {1, 2, 2, 4, 1, 1},
        new byte[] {1, 4, 2, 1, 1, 2},
        new byte[] {1, 4, 2, 2, 1, 1},
        new byte[] {2, 4, 1, 2, 1, 1},
        new byte[] {2, 2, 1, 1, 1, 4},
        new byte[] {4, 1, 3, 1, 1, 1},
        new byte[] {2, 4, 1, 1, 1, 2},
        new byte[] {1, 3, 4, 1, 1, 1},
        new byte[] {1, 1, 1, 2, 4, 2},
        new byte[] {1, 2, 1, 1, 4, 2},
        new byte[] {1, 2, 1, 2, 4, 1},
        new byte[] {1, 1, 4, 2, 1, 2},
        new byte[] {1, 2, 4, 1, 1, 2},
        new byte[] {1, 2, 4, 2, 1, 1},
        new byte[] {4, 1, 1, 2, 1, 2},
        new byte[] {4, 2, 1, 1, 1, 2},
        new byte[] {4, 2, 1, 2, 1, 1},
        new byte[] {2, 1, 2, 1, 4, 1},
        new byte[] {2, 1, 4, 1, 2, 1},
        new byte[] {4, 1, 2, 1, 2, 1},
        new byte[] {1, 1, 1, 1, 4, 3},
        new byte[] {1, 1, 1, 3, 4, 1},
        new byte[] {1, 3, 1, 1, 4, 1},
        new byte[] {1, 1, 4, 1, 1, 3},
        new byte[] {1, 1, 4, 3, 1, 1},
        new byte[] {4, 1, 1, 1, 1, 3},
        new byte[] {4, 1, 1, 3, 1, 1},
        new byte[] {1, 1, 3, 1, 4, 1},
        new byte[] {1, 1, 4, 1, 3, 1},
        new byte[] {3, 1, 1, 1, 4, 1},
        new byte[] {4, 1, 1, 1, 3, 1},
        new byte[] {2, 1, 1, 4, 1, 2},
        new byte[] {2, 1, 1, 2, 1, 4},
        new byte[] {2, 1, 1, 2, 3, 2}
    };
    
    /** The stop bars.
     */    
    internal static byte[] BARS_STOP = {2, 3, 3, 1, 1, 1, 2};
    /** The charset code change.
     */
    public const char CODE_AB_TO_C = (char)99;
    /** The charset code change.
     */
    public const char CODE_AC_TO_B = (char)100;
    /** The charset code change.
     */
    public const char CODE_BC_TO_A = (char)101;
    /** The code for UCC/EAN-128.
     */
    public const char FNC1 = (char)102;
    /** The start code.
     */
    public const char START_A = (char)103;
    /** The start code.
     */
    public const char START_B = (char)104;
    /** The start code.
     */
    public const char START_C = (char)105;

    
    /** Creates new Barcode128 */
    public Barcode128() {
        try {
            x = 0.8f;
            font = BaseFont.createFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            textAlignment = Element.ALIGN_CENTER;
            codeType = CODE128;
        }
        catch (Exception e) {
            throw e;
        }
    }

    /** Returns <CODE>true</CODE> if the next <CODE>numDigits</CODE>
     * starting from index <CODE>textIndex</CODE> are numeric.
     * @param text the text to check
     * @param textIndex where to check from
     * @param numDigits the number of digits to check
     * @return the check result
     */    
    internal static bool isNextDigits(string text, int textIndex, int numDigits) {
        if (textIndex + numDigits > text.Length)
            return false;
        while (numDigits-- > 0) {
            char c = text[textIndex++];
            if (c < '0' || c > '9')
                return false;
        }
        return true;
    }
    
    /** Packs the digits for charset C. It assumes that all the parameters
     * are valid.
     * @param text the text to pack
     * @param textIndex where to pack from
     * @param numDigits the number of digits to pack. It is always an even number
     * @return the packed digits, two digits per character
     */    
    internal static string getPackedRawDigits(string text, int textIndex, int numDigits) {
        string ret = "";
        while (numDigits > 0) {
            numDigits -= 2;
            int c1 = text[textIndex++] - '0';
            int c2 = text[textIndex++] - '0';
            ret += (char)(c1 * 10 + c2);
        }
        return ret;
    }
    
    /** Converts the human readable text to the characters needed to
     * create a barcode. Some optimization is done to get the shortest code.
     * @param text the text to convert
     * @param ucc <CODE>true</CODE> if it is an UCC/EAN-128. In this case
     * the character FNC1 is added
     * @return the code ready to be fed to getBarsCode128Raw()
     */    
    public static string getRawText(string text, bool ucc) {
        string ret = "";
        int tLen = text.Length;
        if (tLen == 0) {
            ret += START_B;
            if (ucc)
                ret += FNC1;
            return ret;
        }
        int c = 0;
        for (int k = 0; k < tLen; ++k) {
            c = text[k];
            if (c > 127)
                throw new RuntimeException("There are illegal characters for barcode 128 in '" + text + "'.");
        }
        c = text[0];
        char currentCode = START_B;
        int index = 0;
        if (isNextDigits(text, index, 2)) {
            currentCode = START_C;
            ret += currentCode;
            if (ucc)
                ret += FNC1;
            ret += getPackedRawDigits(text, index, 2);
            index += 2;
        }
        else if (c < ' ') {
            currentCode = START_A;
            ret += currentCode;
            if (ucc)
                ret += FNC1;
            ret += (char)(c + 64);
            ++index;
        }
        else {
            ret += currentCode;
            if (ucc)
                ret += FNC1;
            ret += (char)(c - ' ');
            ++index;
        }
        while (index < tLen) {
            switch (currentCode) {
                case START_A:
                    {
                        if (isNextDigits(text, index, 4)) {
                            currentCode = START_C;
                            ret += CODE_AB_TO_C;
                            ret += getPackedRawDigits(text, index, 4);
                            index += 4;
                        }
                        else {
                            c = text[index++];
                            if (c > '_') {
                                currentCode = START_B;
                                ret += CODE_AC_TO_B;
                                ret += (char)(c - ' ');
                            }
                            else if (c < ' ')
                                ret += (char)(c + 64);
                            else
                                ret += (char)(c - ' ');
                        }
                    }
                    break;
                case START_B:
                    {
                        if (isNextDigits(text, index, 4)) {
                            currentCode = START_C;
                            ret += CODE_AB_TO_C;
                            ret += getPackedRawDigits(text, index, 4);
                            index += 4;
                        }
                        else {
                            c = text[index++];
                            if (c < ' ') {
                                currentCode = START_A;
                                ret += CODE_BC_TO_A;
                                ret += (char)(c + 64);
                            }
                            else {
                                ret += (char)(c - ' ');
                            }
                        }
                    }
                    break;
                case START_C:
                    {
                        if (isNextDigits(text, index, 2)) {
                            ret += getPackedRawDigits(text, index, 2);
                            index += 2;
                        }
                        else {
                            c = text[index++];
                            if (c < ' ') {
                                currentCode = START_A;
                                ret += CODE_BC_TO_A;
                                ret += (char)(c + 64);
                            }
                            else {
                                currentCode = START_B;
                                ret += CODE_AC_TO_B;
                                ret += (char)(c - ' ');
                            }
                        }
                    }
                    break;
            }
        }
        return ret;
    }
    
    /** Generates the bars. The input has the actual barcodes, not
     * the human readable text.
     * @param text the barcode
     * @return the bars
     */    
    public static byte[] getBarsCode128Raw(string text) {
        int k;
		int idx = text.IndexOf('\uffff');
        if (idx >= 0)
            text = text.Substring(0, idx);
        int chk = text[0];
        for (k = 1; k < text.Length; ++k)
            chk += k * text[k];
        chk = chk % 103;
        text += (char)chk;
        byte[] bars = new byte[(text.Length + 1) * 6 + 7];
        for (k = 0; k < text.Length; ++k)
            Array.Copy(BARS[text[k]], 0, bars, k * 6, 6);
        Array.Copy(BARS_STOP, 0, bars, k * 6, 7);
        return bars;
    }
    
    /** Gets the maximum area that the barcode and the text, if
     * any, will occupy. The lower left corner is always (0, 0).
     * @return the size the barcode occupies.
     */
    public override Rectangle BarcodeSize {
		get {
			float fontX = 0;
			float fontY = 0;
			string fullCode;
			if (font != null) {
				if (baseline > 0)
					fontY = baseline - font.getFontDescriptor(BaseFont.DESCENT, size);
				else
					fontY = -baseline + size;
				fullCode = code;
				if (codeType == CODE128_RAW) {
					int idx = code.IndexOf('\uffff');
					if (idx < 0)
						fullCode = "";
					else
						fullCode = code.Substring(idx + 1);
				}
				fontX = font.getWidthPoint(fullCode, size);
			}
			if (codeType == CODE128_RAW) {
				int idx = code.IndexOf('\uffff');
				if (idx >= 0)
					fullCode = code.Substring(0, idx);
				else
					fullCode = code;
			}
			else {
				fullCode = getRawText(code, codeType == CODE128_UCC);
			}
			int len = fullCode.Length;
			float fullWidth = (len + 2) * 11 * x + 2 * x;
			fullWidth = Math.Max(fullWidth, fontX);
			float fullHeight = barHeight + fontY;
			return new Rectangle(fullWidth, fullHeight);
		}
    }
    
    /** Places the barcode in a <CODE>PdfContentByte</CODE>. The
     * barcode is always placed at coodinates (0, 0). Use the
     * translation matrix to move it elsewhere.<p>
     * The bars and text are written in the following colors:<p>
     * <P><TABLE BORDER=1>
     * <TR>
     *   <TH><P><CODE>barColor</CODE></TH>
     *   <TH><P><CODE>textColor</CODE></TH>
     *   <TH><P>Result</TH>
     *   </TR>
     * <TR>
     *   <TD><P><CODE>null</CODE></TD>
     *   <TD><P><CODE>null</CODE></TD>
     *   <TD><P>bars and text painted with current fill color</TD>
     *   </TR>
     * <TR>
     *   <TD><P><CODE>barColor</CODE></TD>
     *   <TD><P><CODE>null</CODE></TD>
     *   <TD><P>bars and text painted with <CODE>barColor</CODE></TD>
     *   </TR>
     * <TR>
     *   <TD><P><CODE>null</CODE></TD>
     *   <TD><P><CODE>textColor</CODE></TD>
     *   <TD><P>bars painted with current color<br>text painted with <CODE>textColor</CODE></TD>
     *   </TR>
     * <TR>
     *   <TD><P><CODE>barColor</CODE></TD>
     *   <TD><P><CODE>textColor</CODE></TD>
     *   <TD><P>bars painted with <CODE>barColor</CODE><br>text painted with <CODE>textColor</CODE></TD>
     *   </TR>
     * </TABLE>
     * @param cb the <CODE>PdfContentByte</CODE> where the barcode will be placed
     * @param barColor the color of the bars. It can be <CODE>null</CODE>
     * @param textColor the color of the text. It can be <CODE>null</CODE>
     * @return the dimensions the barcode occupies
     */
    public override Rectangle placeBarcode(PdfContentByte cb, object barColor, object textColor) {
        string fullCode = code;
        if (codeType == CODE128_RAW) {
            int idx = code.IndexOf('\uffff');
            if (idx < 0)
                fullCode = "";
            else
                fullCode = code.Substring(idx + 1);
        }
        float fontX = 0;
        if (font != null) {
            fontX = font.getWidthPoint(fullCode, size);
        }
        string bCode;
        if (codeType == CODE128_RAW) {
            int idx = code.IndexOf('\uffff');
            if (idx >= 0)
                bCode = code.Substring(0, idx);
            else
                bCode = code;
        }
        else {
            bCode = getRawText(code, codeType == CODE128_UCC);
        }
        int len = bCode.Length;
        float fullWidth = (len + 2) * 11 * x + 2 * x;
        float barStartX = 0;
        float textStartX = 0;
        switch (textAlignment) {
            case Element.ALIGN_LEFT:
                break;
            case Element.ALIGN_RIGHT:
                if (fontX > fullWidth)
                    barStartX = fontX - fullWidth;
                else
                    textStartX = fullWidth - fontX;
                break;
            default:
                if (fontX > fullWidth)
                    barStartX = (fontX - fullWidth) / 2;
                else
                    textStartX = (fullWidth - fontX) / 2;
                break;
        }
        float barStartY = 0;
        float textStartY = 0;
        if (font != null) {
            if (baseline <= 0)
                textStartY = barHeight - baseline;
            else {
                textStartY = -font.getFontDescriptor(BaseFont.DESCENT, size);
                barStartY = textStartY + baseline;
            }
        }
        byte[] bars = getBarsCode128Raw(bCode);
        bool print = true;
        if (barColor != null)
			cb.ColorFill = (Color)barColor;
        for (int k = 0; k < bars.Length; ++k) {
            float w = bars[k] * x;
            if (print)
                cb.rectangle(barStartX, barStartY, w, barHeight);
            print = !print;
            barStartX += w;
        }
        cb.fill();
        if (font != null) {
            if (textColor != null)
				cb.ColorFill = (Color)textColor;
            cb.beginText();
            cb.setFontAndSize(font, size);
            cb.setTextMatrix(textStartX, textStartY);
            cb.showText(fullCode);
            cb.endText();
        }
        return this.BarcodeSize;
    }    
	}
}
