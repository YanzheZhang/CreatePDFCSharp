using System;

/*
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
/** Supports fast encodings for winansi and PDFDocEncoding.
 *
 * @author Paulo Soares (psoares@consiste.pt)
 */
public class PdfEncodings {
    
    internal static char[] winansiByteToChar = {
        (char)0, (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10, (char)11, (char)12, (char)13, (char)14, (char)15,
        (char)16, (char)17, (char)18, (char)19, (char)20, (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30, (char)31,
        (char)32, (char)33, (char)34, (char)35, (char)36, (char)37, (char)38, (char)39, (char)40, (char)41, (char)42, (char)43, (char)44, (char)45, (char)46, (char)47,
        (char)48, (char)49, (char)50, (char)51, (char)52, (char)53, (char)54, (char)55, (char)56, (char)57, (char)58, (char)59, (char)60, (char)61, (char)62, (char)63,
        (char)64, (char)65, (char)66, (char)67, (char)68, (char)69, (char)70, (char)71, (char)72, (char)73, (char)74, (char)75, (char)76, (char)77, (char)78, (char)79,
        (char)80, (char)81, (char)82, (char)83, (char)84, (char)85, (char)86, (char)87, (char)88, (char)89, (char)90, (char)91, (char)92, (char)93, (char)94, (char)95,
        (char)96, (char)97, (char)98, (char)99, (char)100, (char)101, (char)102, (char)103, (char)104, (char)105, (char)106, (char)107, (char)108, (char)109, (char)110, (char)111,
        (char)112, (char)113, (char)114, (char)115, (char)116, (char)117, (char)118, (char)119, (char)120, (char)121, (char)122, (char)123, (char)124, (char)125, (char)126, (char)127,
        (char)8364, (char)65533, (char)8218, (char)402, (char)8222, (char)8230, (char)8224, (char)8225, (char)710, (char)8240, (char)352, (char)8249, (char)338, (char)65533, (char)381, (char)65533,
        (char)65533, (char)8216, (char)8217, (char)8220, (char)8221, (char)8226, (char)8211, (char)8212, (char)732, (char)8482, (char)353, (char)8250, (char)339, (char)65533, (char)382, (char)376,
        (char)160, (char)161, (char)162, (char)163, (char)164, (char)165, (char)166, (char)167, (char)168, (char)169, (char)170, (char)171, (char)172, (char)173, (char)174, (char)175,
        (char)176, (char)177, (char)178, (char)179, (char)180, (char)181, (char)182, (char)183, (char)184, (char)185, (char)186, (char)187, (char)188, (char)189, (char)190, (char)191,
        (char)192, (char)193, (char)194, (char)195, (char)196, (char)197, (char)198, (char)199, (char)200, (char)201, (char)202, (char)203, (char)204, (char)205, (char)206, (char)207,
        (char)208, (char)209, (char)210, (char)211, (char)212, (char)213, (char)214, (char)215, (char)216, (char)217, (char)218, (char)219, (char)220, (char)221, (char)222, (char)223,
        (char)224, (char)225, (char)226, (char)227, (char)228, (char)229, (char)230, (char)231, (char)232, (char)233, (char)234, (char)235, (char)236, (char)237, (char)238, (char)239,
        (char)240, (char)241, (char)242, (char)243, (char)244, (char)245, (char)246, (char)247, (char)248, (char)249, (char)250, (char)251, (char)252, (char)253, (char)254, (char)255};
        
    internal static char[] pdfEncodingByteToChar = {
        (char)0, (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10, (char)11, (char)12, (char)13, (char)14, (char)15,
        (char)16, (char)17, (char)18, (char)19, (char)20, (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30, (char)31,
        (char)32, (char)33, (char)34, (char)35, (char)36, (char)37, (char)38, (char)39, (char)40, (char)41, (char)42, (char)43, (char)44, (char)45, (char)46, (char)47,
        (char)48, (char)49, (char)50, (char)51, (char)52, (char)53, (char)54, (char)55, (char)56, (char)57, (char)58, (char)59, (char)60, (char)61, (char)62, (char)63,
        (char)64, (char)65, (char)66, (char)67, (char)68, (char)69, (char)70, (char)71, (char)72, (char)73, (char)74, (char)75, (char)76, (char)77, (char)78, (char)79,
        (char)80, (char)81, (char)82, (char)83, (char)84, (char)85, (char)86, (char)87, (char)88, (char)89, (char)90, (char)91, (char)92, (char)93, (char)94, (char)95,
        (char)96, (char)97, (char)98, (char)99, (char)100, (char)101, (char)102, (char)103, (char)104, (char)105, (char)106, (char)107, (char)108, (char)109, (char)110, (char)111,
        (char)112, (char)113, (char)114, (char)115, (char)116, (char)117, (char)118, (char)119, (char)120, (char)121, (char)122, (char)123, (char)124, (char)125, (char)126, (char)127,
        (char)0x2022, (char)0x2020, (char)0x2021, (char)0x2026, (char)0x2014, (char)0x2013, (char)0x0192, (char)0x2044, (char)65533, (char)65533, (char)0x2212, (char)65533, (char)65533, (char)65533, (char)65533, (char)65533,
        (char)0x2019, (char)0x201a, (char)0x2122, (char)0xfb01, (char)0xfb02, (char)0x0141, (char)0x0152, (char)0x0160, (char)0x0178, (char)0x017d, (char)0x0131, (char)0x0142, (char)0x0153, (char)0x0161, (char)0x017e, (char)65533,
        (char)0x20ac, (char)161, (char)162, (char)163, (char)164, (char)165, (char)166, (char)167, (char)168, (char)169, (char)170, (char)171, (char)172, (char)173, (char)174, (char)175,
        (char)176, (char)177, (char)178, (char)179, (char)180, (char)181, (char)182, (char)183, (char)184, (char)185, (char)186, (char)187, (char)188, (char)189, (char)190, (char)191,
        (char)192, (char)193, (char)194, (char)195, (char)196, (char)197, (char)198, (char)199, (char)200, (char)201, (char)202, (char)203, (char)204, (char)205, (char)206, (char)207,
        (char)208, (char)209, (char)210, (char)211, (char)212, (char)213, (char)214, (char)215, (char)216, (char)217, (char)218, (char)219, (char)220, (char)221, (char)222, (char)223,
        (char)224, (char)225, (char)226, (char)227, (char)228, (char)229, (char)230, (char)231, (char)232, (char)233, (char)234, (char)235, (char)236, (char)237, (char)238, (char)239,
        (char)240, (char)241, (char)242, (char)243, (char)244, (char)245, (char)246, (char)247, (char)248, (char)249, (char)250, (char)251, (char)252, (char)253, (char)254, (char)255};
        
    static internal IntHashtable winansi = new IntHashtable();
    
    internal static IntHashtable pdfEncoding = new IntHashtable();
    
    static PdfEncodings() {        
        winansi[8364] = 128;
        winansi[8218] = 130;
        winansi[402] = 131;
        winansi[8222] = 132;
        winansi[8230] = 133;
        winansi[8224] = 134;
        winansi[8225] = 135;
        winansi[710] = 136;
        winansi[8240] = 137;
        winansi[352] = 138;
        winansi[8249] = 139;
        winansi[338] = 140;
        winansi[381] = 142;
        winansi[8216] = 145;
        winansi[8217] = 146;
        winansi[8220] = 147;
        winansi[8221] = 148;
        winansi[8226] = 149;
        winansi[8211] = 150;
        winansi[8212] = 151;
        winansi[732] = 152;
        winansi[8482] = 153;
        winansi[353] = 154;
        winansi[8250] = 155;
        winansi[339] = 156;
        winansi[382] = 158;
        winansi[376] = 159;
        
        pdfEncoding[0x2022] = 128;
        pdfEncoding[0x2020] = 129;
        pdfEncoding[0x2021] = 130;
        pdfEncoding[0x2026] = 131;
        pdfEncoding[0x2014] = 132;
        pdfEncoding[0x2013] = 133;
        pdfEncoding[0x0192] = 134;
        pdfEncoding[0x2044] = 135;
        pdfEncoding[0x2212] = 138;
        pdfEncoding[0x2019] = 144;
        pdfEncoding[0x201a] = 145;
        pdfEncoding[0x2122] = 146;
        pdfEncoding[0xfb01] = 147;
        pdfEncoding[0xfb02] = 148;
        pdfEncoding[0x0141] = 149;
        pdfEncoding[0x0152] = 150;
        pdfEncoding[0x0160] = 151;
        pdfEncoding[0x0178] = 152;
        pdfEncoding[0x017d] = 153;
        pdfEncoding[0x0131] = 154;
        pdfEncoding[0x0142] = 155;
        pdfEncoding[0x0153] = 156;
        pdfEncoding[0x0161] = 157;
        pdfEncoding[0x017e] = 158;
        pdfEncoding[0x20ac] = 160;
    }

    /**
     * Converts a <CODE>string</CODE> to a </CODE>byte</CODE> array according
     * to the font's encoding.
     * @param text the <CODE>string</CODE> to be converted
     * @return an array of <CODE>byte</CODE> representing the conversion according to the font's encoding
     */
    public static byte[] convertToBytes(string text, string encoding) {
        if (encoding == null || encoding.Length == 0) {
            int len = text.Length;
            byte[] b = new byte[len];
            for (int k = 0; k < len; ++k)
                b[k] = (byte)text[k];
            return b;
        }
        IntHashtable hash = null;
        if (encoding.Equals(BaseFont.WINANSI))
            hash = winansi;
        else if (encoding.Equals(PdfObject.ENCODING))
            hash = pdfEncoding;
        if (hash != null) {
            int len = text.Length;
            byte[] b = new byte[len];
            int c = 0;
            for (int k = 0; k < len; ++k) {
                char char1 = text[k];
                if (char1 < 128 || (char1 >= 160 && char1 <= 255))
                    c = char1;
                else
                    c = hash[char1];
                b[k] = (byte)c;
            }
            return b;
        }
        if (encoding.Equals(PdfObject.TEXT_UNICODE)) {
            // workaround for jdk 1.2.2 bug
            char[] cc = text.ToCharArray();
            int len = cc.Length;
            byte[] b = new byte[cc.Length * 2 + 2];
            b[0] = unchecked((byte)-2);
            b[1] = unchecked((byte)-1);
            int bptr = 2;
            for (int k = 0; k < len; ++k) {
                char c = cc[k];
                b[bptr++] = (byte)(c >> 8);
                b[bptr++] = (byte)(c & 0xff);
            }
            return b;
        }
        try {
			return System.Text.Encoding.GetEncoding(encoding).GetBytes(text);
        }
        catch (Exception e) {
            throw e;
        }
    }
    
    public static string convertTostring(byte[] bytes, string encoding) {
        if (encoding == null || encoding.Length == 0) {
            char[] c = new char[bytes.Length];
            for (int k = 0; k < bytes.Length; ++k)
                c[k] = (char)(bytes[k] & 0xff);
            return new string(c);
        }
        char[] ch = null;
        if (encoding.Equals(BaseFont.WINANSI))
            ch = winansiByteToChar;
        else if (encoding.Equals(PdfObject.ENCODING))
            ch = pdfEncodingByteToChar;
        if (ch != null) {
            int len = bytes.Length;
            char[] c = new char[len];
            for (int k = 0; k < len; ++k) {
                c[k] = ch[bytes[k] & 0xff];
            }
            return new string(c);
        }
        try {
			return System.Text.Encoding.GetEncoding(encoding).GetString(bytes);
        }
        catch (Exception e) {
            throw e;
        }
    }
}
}