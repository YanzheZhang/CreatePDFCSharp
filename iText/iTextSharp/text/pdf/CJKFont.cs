using System;
using System.IO;
using System.Text;
using System.Collections;
using System.util;

/*
 * $Id: CJKFont.cs,v 1.1.1.1 2003/02/04 02:56:46 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2000, 2001, 2002 by Paulo Soares.
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
 * Creates a CJK font compatible with the fonts in the Adobe Asian font Pack.
 *
 * @author  Paulo Soares (psoares@consiste.pt)
 */

internal class CJKFont : BaseFont {
    /** The encoding used in the PDF document for CJK fonts
     */
    internal static string CJK_ENCODING = "UTF-16BE";
    private const int FIRST = 0;
    private const int BRACKET = 1;
    private const int SERIAL = 2;
    private const int V1Y = 880;
        
    internal static Properties cjkFonts = new Properties();
    internal static Properties cjkEncodings = new Properties();
    internal static Hashtable allCMaps = new Hashtable();
    internal static Hashtable allFonts = new Hashtable();
    
    static CJKFont() {
        try {
            Stream istr = getResourceStream("cjkfonts.properties");
            //cjkFonts.load(istr);
            //istr.Close();
            //istr = getResourceStream("cjkencodings.properties");
            //cjkEncodings.load(istr);
            //istr.Close();
        }
        catch (Exception e) {
			e.GetType();
            cjkFonts = new Properties();
            cjkEncodings = new Properties();
            Console.Error.WriteLine("Missing configuration files for CJK fonts");
        }
    }
    
    /** The font name */
    private string fontName;
    /** The style modifier */
    private string style = "";
    /** The CMap name associated with this font */
    private string CMap;
    
    private bool cidDirect = false;
    
    private char[] translationMap;
    private IntHashtable vMetrics;
    private IntHashtable hMetrics;
    private Hashmap fontDesc;
    private bool vertical = false;
    
    /** Creates a CJK font.
     * @param fontName the name of the font
     * @param enc the encoding of the font
     * @param emb always <CODE>false</CODE>. CJK font and not embedded
     * @throws DocumentException on error
     * @throws IOException on error
     */
    internal CJKFont(string fontName, string enc, bool emb) {
        this.FontType = FONT_TYPE_CJK;
        string nameBase = getBaseName(fontName);
        if (!isCJKFont(nameBase, enc))
            throw new DocumentException("Font '" + fontName + "' with '" + enc + "' encoding is not a CJK font.");
        if (nameBase.Length < fontName.Length) {
            style = fontName.Substring(nameBase.Length);
            fontName = nameBase;
        }
        this.fontName = fontName;
        encoding = CJK_ENCODING;
        vertical = enc.EndsWith("V");
        CMap = enc;
        if (enc.StartsWith("Identity-")) {
            cidDirect = true;
            string s = cjkFonts[fontName];
            s = s.Substring(0, s.IndexOf('_'));
            char[] c = (char[])allCMaps[s];
            if (c == null) {
                c = readCMap(s);
                if (c == null)
                    throw new DocumentException("The cmap " + s + " does not exist as a resource.");
                c[0xff00] = '\n';
                allCMaps.Add(s, c);
            }
            translationMap = c;
        }
        else {
            char[] c = (char[])allCMaps[enc];
            if (c == null) {
                string s = cjkEncodings[enc];
                if (s == null)
                    throw new DocumentException("The resource cjkencodings.properties does not contain the encoding " + enc);
                StringTokenizer tk = new StringTokenizer(s);
                string nt = tk.nextToken();
                c = (char[])allCMaps[nt];
                if (c == null) {
                    c = readCMap(nt);
                    allCMaps.Add(nt, c);
                }
                if (tk.hasMoreTokens()) {
                    string nt2 = tk.nextToken();
                    char[] m2 = readCMap(nt2);
                    for (int k = 0; k < 0x10000; ++k) {
                        if (m2[k] == 0)
                            m2[k] = c[k];
                    }
                    allCMaps.Add(enc, m2);
                    c = m2;
                }
            }
            translationMap = c;
        }
        fontDesc = (Hashmap)allFonts[fontName];
        if (fontDesc == null) {
            fontDesc = readFontProperties(fontName);
            allFonts.Add(fontName, fontDesc);
        }
        hMetrics = (IntHashtable)fontDesc["W"];
        vMetrics = (IntHashtable)fontDesc["W2"];
    }
    
    /** Checks if its a valid CJK font.
     * @param fontName the font name
     * @param enc the encoding
     * @return <CODE>true</CODE> if it is CJK font
     */
    public static bool isCJKFont(string fontName, string enc) {
        string encodings = cjkFonts[fontName];
        return (encodings != null && (enc.Equals("Identity-H") || enc.Equals("Identity-V") || encodings.IndexOf("_" + enc + "_") >= 0));
    }
        
    public override int getWidth(string text) {
        int total = 0;
        for (int k = 0; k < text.Length; ++k) {
            int c = text[k];
            if (!cidDirect)
                c = translationMap[c];
            int v;
            if (vertical)
                v = vMetrics[c];
            else
                v = hMetrics[c];
            if (v > 0)
                total += v;
            else
                total += 1000;
        }
        return total;
    }
    
    protected override int getRawWidth(int c, string name) {
        return 0;
    }
    public override int getKerning(char char1, char char2) {
        return 0;
    }

    private PdfDictionary getFontDescriptor() {
        PdfDictionary dic = new PdfDictionary(new PdfName("FontDescriptor"));
        dic.put(new PdfName("Ascent"), new PdfLiteral((string)fontDesc["Ascent"]));
        dic.put(new PdfName("CapHeight"), new PdfLiteral((string)fontDesc["CapHeight"]));
        dic.put(new PdfName("Descent"), new PdfLiteral((string)fontDesc["Descent"]));
        dic.put(new PdfName("Flags"), new PdfLiteral((string)fontDesc["Flags"]));
        dic.put(new PdfName("FontBBox"), new PdfLiteral((string)fontDesc["FontBBox"]));
        dic.put(new PdfName("FontName"), new PdfName(fontName + style));
        dic.put(new PdfName("ItalicAngle"), new PdfLiteral((string)fontDesc["ItalicAngle"]));
        dic.put(new PdfName("StemV"), new PdfLiteral((string)fontDesc["StemV"]));
        PdfDictionary pdic = new PdfDictionary();
        pdic.put(PdfName.PANOSE, new PdfString((string)fontDesc["Panose"], null));
        dic.put(new PdfName("Style"), pdic);
        return dic;
    }
    
    private PdfDictionary getCIDFont(PdfIndirectReference fontDescriptor, IntHashtable cjkTag) {
        PdfDictionary dic = new PdfDictionary(PdfName.FONT);
        dic.put(PdfName.SUBTYPE, new PdfName("CIDFontType0"));
        dic.put(new PdfName("BaseFont"), new PdfName(fontName + style));
        dic.put(new PdfName("FontDescriptor"), fontDescriptor);
        int[] keys = cjkTag.toOrderedKeys();
        string w = convertToHCIDMetrics(keys, hMetrics);
        if (w != null)
            dic.put(new PdfName("W"), new PdfLiteral(w));
        if (vertical) {
            w = convertToVCIDMetrics(keys, vMetrics, hMetrics);;
            if (w != null)
                dic.put(new PdfName("W2"), new PdfLiteral(w));
        }
        PdfDictionary cdic = new PdfDictionary();
        cdic.put(PdfName.REGISTRY, new PdfString((string)fontDesc["Registry"], null));
        cdic.put(PdfName.ORDERING, new PdfString((string)fontDesc["Ordering"], null));
        cdic.put(PdfName.SUPPLEMENT, new PdfLiteral((string)fontDesc["Supplement"]));
        dic.put(new PdfName("CIDSystemInfo"), cdic);
        return dic;
    }
    
    private PdfDictionary getFontBaseType(PdfIndirectReference CIDFont) {
        PdfDictionary dic = new PdfDictionary(PdfName.FONT);
        dic.put(PdfName.SUBTYPE, new PdfName("Type0"));
        string name = fontName;
        if (style.Length > 0)
            name += "-" + style.Substring(1);
        name += "-" + CMap;
        dic.put(new PdfName("BaseFont"), new PdfName(name));
        dic.put(new PdfName("Encoding"), new PdfName(CMap));
        dic.put(new PdfName("DescendantFonts"), new PdfArray(CIDFont));
        return dic;
    }
    
    internal override void writeFont(PdfWriter writer, PdfIndirectReference piref, Object[] parms) {
        IntHashtable cjkTag = (IntHashtable)parms[0];
        PdfIndirectReference ind_font = null;
        PdfObject pobj = null;
        PdfIndirectObject obj = null;
        pobj = getFontDescriptor();
        if (pobj != null){
            obj = writer.addToBody(pobj);
            ind_font = obj.IndirectReference;
        }
        pobj = getCIDFont(ind_font, cjkTag);
        if (pobj != null){
            obj = writer.addToBody(pobj);
            ind_font = obj.IndirectReference;
        }
        pobj = getFontBaseType(ind_font);
        writer.addToBody(pobj, piref);
    }
    
    private float getDescNumber(string name) {
        return int.Parse((string)fontDesc[name]);
    }
    
    private float getBBox(int idx) {
        string s = (string)fontDesc["FontBBox"];
        StringTokenizer tk = new StringTokenizer(s, " []\r\n\t\f");
        string ret = tk.nextToken();
        for (int k = 0; k < idx; ++k)
            ret = tk.nextToken();
        return int.Parse(ret);
    }
    
    /** Gets the font parameter identified by <CODE>key</CODE>. Valid values
     * for <CODE>key</CODE> are <CODE>ASCENT</CODE>, <CODE>CAPHEIGHT</CODE>, <CODE>DESCENT</CODE>
     * and <CODE>ITALICANGLE</CODE>.
     * @param key the parameter to be extracted
     * @param fontSize the font size in points
     * @return the parameter in points
     */
    public override float getFontDescriptor(int key, float fontSize) {
        switch (key) {
            case AWT_ASCENT:
            case ASCENT:
                return getDescNumber("Ascent") * fontSize / 1000;
            case CAPHEIGHT:
                return getDescNumber("CapHeight") * fontSize / 1000;
            case AWT_DESCENT:
            case DESCENT:
                return getDescNumber("Descent") * fontSize / 1000;
            case ITALICANGLE:
                return getDescNumber("ItalicAngle");
            case BBOXLLX:
                return fontSize * getBBox(0) / 1000;
            case BBOXLLY:
                return fontSize * getBBox(1) / 1000;
            case BBOXURX:
                return fontSize * getBBox(2) / 1000;
            case BBOXURY:
                return fontSize * getBBox(3) / 1000;
            case AWT_LEADING:
                return 0;
            case AWT_MAXADVANCE:
                return fontSize * (getBBox(2) - getBBox(0)) / 1000;
        }
        return 0;
    }
    
    public override string PostscriptFontName {
		get {
			return fontName;
		}
    }
    
    /** Gets the full name of the font. If it is a True Type font
     * each array element will have {Platform ID, Platform Encoding ID,
     * Language ID, font name}. The interpretation of this values can be
     * found in the Open Type specification, chapter 2, in the 'name' table.<br>
     * For the other fonts the array has a single element with {"", "", "",
     * font name}.
     * @return the full name of the font
     */
    public override string[][] FullFontName {
		get {
			return new string[][]{new string[] {"", "", "", fontName}};
		}
    }
    
    /** Gets the family name of the font. If it is a True Type font
     * each array element will have {Platform ID, Platform Encoding ID,
     * Language ID, font name}. The interpretation of this values can be
     * found in the Open Type specification, chapter 2, in the 'name' table.<br>
     * For the other fonts the array has a single element with {"", "", "",
     * font name}.
     * @return the family name of the font
     */
    public override string[][] FamilyFontName {
		get {
			return this.FullFontName;
		}
    }
    
    internal static char[] readCMap(string name) {
        try {
            name = name + ".cmap";
            Stream istr = getResourceStream(name);
            char[] c = new char[0x10000];
            for (int k = 0; k < 0x10000; ++k)
                c[k] = (char)((istr.ReadByte() << 8) + istr.ReadByte());
            return c;
        }
        catch (Exception e) {
            Console.Error.WriteLine(e.StackTrace);
        }
        return null;
    }
    
    internal static IntHashtable createMetric(string s) {
        IntHashtable h = new IntHashtable();
        StringTokenizer tk = new StringTokenizer(s);
        while (tk.hasMoreTokens()) {
            int n1 = int.Parse(tk.nextToken());
            h[n1] = int.Parse(tk.nextToken());
        }
        return h;
    }
    
    internal static string convertToHCIDMetrics(int[] keys, IntHashtable h) {
        if (keys.Length == 0)
            return null;
        int lastCid = 0;
        int lastValue = 0;
        int start;
        for (start = 0; start < keys.Length; ++start) {
            lastCid = keys[start];
            lastValue = h[lastCid];
            if (lastValue != 0) {
                ++start;
                break;
            }
        }
        if (lastValue == 0)
            return null;
        StringBuilder buf = new StringBuilder();
        buf.Append('[');
        buf.Append(lastCid);
        int state = FIRST;
        for (int k = start; k < keys.Length; ++k) {
            int cid = keys[k];
            int value = h[cid];
            if (value == 0)
                continue;
            switch (state) {
                case FIRST: {
                    if (cid == lastCid + 1 && value == lastValue) {
                        state = SERIAL;
                    }
                    else if (cid == lastCid + 1) {
                        state = BRACKET;
                        buf.Append('[').Append(lastValue);
                    }
                    else {
                        buf.Append('[').Append(lastValue).Append(']').Append(cid);
                    }
                    break;
                }
                case BRACKET: {
                    if (cid == lastCid + 1 && value == lastValue) {
                        state = SERIAL;
                        buf.Append(']').Append(lastCid);
                    }
                    else if (cid == lastCid + 1) {
                        buf.Append(' ').Append(lastValue);
                    }
                    else {
                        state = FIRST;
                        buf.Append(' ').Append(lastValue).Append(']').Append(cid);
                    }
                    break;
                }
                case SERIAL: {
                    if (cid != lastCid + 1 || value != lastValue) {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(lastValue).Append(' ').Append(cid);
                        state = FIRST;
                    }
                    break;
                }
            }
            lastValue = value;
            lastCid = cid;
        }
        switch (state) {
            case FIRST: {
                buf.Append('[').Append(lastValue).Append("]]");
                break;
            }
            case BRACKET: {
                buf.Append(' ').Append(lastValue).Append("]]");
                break;
            }
            case SERIAL: {
                buf.Append(' ').Append(lastCid).Append(' ').Append(lastValue).Append(']');
                break;
            }
        }
        return buf.ToString();
    }
    
    internal static string convertToVCIDMetrics(int[] keys, IntHashtable v, IntHashtable h) {
        if (keys.Length == 0)
            return null;
        int lastCid = 0;
        int lastValue = 0;
        int lastHValue = 0;
        int start;
        for (start = 0; start < keys.Length; ++start) {
            lastCid = keys[start];
            lastValue = v[lastCid];
            if (lastValue != 0) {
                ++start;
                break;
            }
            else
                lastHValue = h[lastCid];
        }
        if (lastValue == 0)
            return null;
        if (lastHValue == 0)
            lastHValue = 1000;
        StringBuilder buf = new StringBuilder();
        buf.Append('[');
        buf.Append(lastCid);
        int state = FIRST;
        for (int k = start; k < keys.Length; ++k) {
            int cid = keys[k];
            int value = v[cid];
            if (value == 0)
                continue;
            int hValue = h[lastCid];
            if (hValue == 0)
                hValue = 1000;
            switch (state) {
                case FIRST: {
                    if (cid == lastCid + 1 && value == lastValue && hValue == lastHValue) {
                        state = SERIAL;
                    }
                    else {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ').Append(lastHValue / 2).Append(' ').Append(V1Y).Append(' ').Append(cid);
                    }
                    break;
                }
                case SERIAL: {
                    if (cid != lastCid + 1 || value != lastValue || hValue != lastHValue) {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ').Append(lastHValue / 2).Append(' ').Append(V1Y).Append(' ').Append(cid);
                        state = FIRST;
                    }
                    break;
                }
            }
            lastValue = value;
            lastCid = cid;
            lastHValue = hValue;
        }
        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ').Append(lastHValue / 2).Append(' ').Append(V1Y).Append(" ]");
        return buf.ToString();
    }
    
    internal static Hashmap readFontProperties(string name) {
        try {
//            name += ".properties";
//            Stream istr = getResourceStream(name);
//            Properties p = new Properties();
//            p.load(istr);
//            istr.Close();
//            IntHashtable W = createMetric(p["W"]);
//            p.Remove("W");
//            IntHashtable W2 = createMetric(p["W2"]);
//            p.Remove("W2");
//            Hashtable map = new Hashtable();
//            for (Enumeration em = p.keys(); em.hasMoreElements();) {
//                Object obj = em.nextElement();
//                map.Add(obj, p[(string)obj]);
//            }
//            map.Add("W", W);
//            map.Add("W2", W2);
//            return map;
        }
        catch (Exception e) {
            Console.Error.WriteLine(e.StackTrace);
        }
        return null;
    }

    public override char getUnicodeEquivalent(char c) {
        if (cidDirect)
            return translationMap[c];
        return c;
    }
    
    public override char getCidCode(char c) {
        if (cidDirect)
            return c;
        return translationMap[c];
    }
}
}