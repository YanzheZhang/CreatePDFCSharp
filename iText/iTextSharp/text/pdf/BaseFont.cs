using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.util;

namespace iTextSharp.text.pdf {
	/// <summary>
	/// Summary description for BaseFont.
	/// </summary>
	public abstract class BaseFont {
		/** This is a possible value of a base 14 type 1 font */
		public const string COURIER = "Courier";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string COURIER_BOLD = "Courier-Bold";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string COURIER_OBLIQUE = "Courier-Oblique";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string COURIER_BOLDOBLIQUE = "Courier-BoldOblique";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string HELVETICA = "Helvetica";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string HELVETICA_BOLD = "Helvetica-Bold";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string HELVETICA_OBLIQUE = "Helvetica-Oblique";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string SYMBOL = "Symbol";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string TIMES_ROMAN = "Times-Roman";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string TIMES_BOLD = "Times-Bold";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string TIMES_ITALIC = "Times-Italic";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string TIMES_BOLDITALIC = "Times-BoldItalic";
    
		/** This is a possible value of a base 14 type 1 font */
		public const string ZAPFDINGBATS = "ZapfDingbats";
    
		/** The maximum height above the baseline reached by glyphs in this
		 * font, excluding the height of glyphs for accented characters.
		 */    
		public const int ASCENT = 1;    
		/** The y coordinate of the top of flat capital letters, measured from
		 * the baseline.
		 */    
		public const int CAPHEIGHT = 2;
		/** The maximum depth below the baseline reached by glyphs in this
		 * font. The value is a negative number.
		 */    
		public const int DESCENT = 3;
		/** The angle, expressed in degrees counterclockwise from the vertical,
		 * of the dominant vertical strokes of the font. The value is
		 * negative for fonts that slope to the right, as almost all italic fonts do.
		 */    
		public const int ITALICANGLE = 4;
		/** The lower left x glyph coordinate.
		 */    
		public const int BBOXLLX = 5;
		/** The lower left y glyph coordinate.
		 */    
		public const int BBOXLLY = 6;
		/** The upper right x glyph coordinate.
		 */    
		public const int BBOXURX = 7;
		/** The upper right y glyph coordinate.
		 */    
		public const int BBOXURY = 8;
    
		public const int AWT_ASCENT = 9;
		public const int AWT_DESCENT = 10;
		public const int AWT_LEADING = 11;
		public const int AWT_MAXADVANCE = 12;
    
		/** The font is Type 1.
		 */    
		public const int FONT_TYPE_T1 = 0;
		/** The font is True Type with a standard encoding.
		 */    
		public const int FONT_TYPE_TT = 1;
		/** The font is CJK.
		 */    
		public const int FONT_TYPE_CJK = 2;
		/** The font is True Type with a Unicode encoding.
		 */    
		public const int FONT_TYPE_TTUNI = 3;
		/** The Unicode encoding with horizontal writing.
		 */    
		public const string IDENTITY_H = "Identity-H";
		/** The Unicode encoding with vertical writing.
		 */    
		public const string IDENTITY_V = "Identity-V";
    
		/** A possible encoding. */    
		public const string CP1250 = "1250";
    
		/** A possible encoding. */    
		public const string CP1252 = "windows-1252";
    
		/** A possible encoding. */    
		public const string CP1257 = "1257";
    
		/** A possible encoding. */    
		public const string WINANSI = "windows-1252";
    
		/** A possible encoding. */    
		public const string MACROMAN = "MacRoman";
    
    
		/** if the font has to be embedded */
		public static bool EMBEDDED = true;
    
		/** if the font doesn't have to be embedded */
		public static bool NOT_EMBEDDED = false;
		/** if the font has to be cached */
		public static bool CACHED = true;
		/** if the font doesn't have to be cached */
		public static bool NOT_CACHED = false;
    
		/** The font type.
		 */    
		int fontType;
		/** a not defined character in a custom PDF encoding */
		public const string notdef = ".notdef";
    
		/** table of characters widths for this encoding */
		protected int[] widths = new int[256];
    
		/** encoding names */
		protected string[] differences = new string[256];
		/** same as differences but with the unicode codes */
		protected char[] unicodeDifferences = new char[256];
    
		/** encoding used with this font */
		protected string encoding;
    
		/** true if the font is to be embedded in the PDF */
		protected bool embedded;
    
		/**
		 * true if the font must use it's built in encoding. In that case the
		 * <CODE>encoding</CODE> is only used to map a char to the position inside
		 * the font, not to the expected char name.
		 */
		protected bool fontSpecific = true;
    
		/** cache for the fonts already used. */
		protected static Hashmap fontCache = new Hashmap();
    
		/** list of the 14 built in fonts. */
		protected static Hashmap BuiltinFonts14 = new Hashmap();
    
		/** The subset prefix to be added to the font name when the font is embedded.
		 */    
		protected static char[] subsetPrefix = {'A', 'B', 'C', 'D', 'E', 'E', '+'};
    
		/** Forces the output of the width array. Only matters for the 14
		 * built-in fonts.
		 */
		protected bool forceWidthsOutput = false;
    
		/** Converts <CODE>char</CODE> directly to <CODE>byte</CODE>
		 * by casting.
		 */
		protected bool directTextToByte = false;
    
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document.
		 */
		protected bool subset = true;
    
		protected bool fastWinansi = false;
    
		static BaseFont() {
			BuiltinFonts14.Add(COURIER, PdfName.COURIER);
			BuiltinFonts14.Add(COURIER_BOLD, PdfName.COURIER_BOLD);
			BuiltinFonts14.Add(COURIER_BOLDOBLIQUE, PdfName.COURIER_BOLDOBLIQUE);
			BuiltinFonts14.Add(COURIER_OBLIQUE, PdfName.COURIER_OBLIQUE);
			BuiltinFonts14.Add(HELVETICA, PdfName.HELVETICA);
			BuiltinFonts14.Add(HELVETICA_BOLD, PdfName.HELVETICA_BOLD);
			BuiltinFonts14.Add(HELVETICA_BOLDOBLIQUE, PdfName.HELVETICA_BOLDOBLIQUE);
			BuiltinFonts14.Add(HELVETICA_OBLIQUE, PdfName.HELVETICA_OBLIQUE);
			BuiltinFonts14.Add(SYMBOL, PdfName.SYMBOL);
			BuiltinFonts14.Add(TIMES_ROMAN, PdfName.TIMES_ROMAN);
			BuiltinFonts14.Add(TIMES_BOLD, PdfName.TIMES_BOLD);
			BuiltinFonts14.Add(TIMES_BOLDITALIC, PdfName.TIMES_BOLDITALIC);
			BuiltinFonts14.Add(TIMES_ITALIC, PdfName.TIMES_ITALIC);
			BuiltinFonts14.Add(ZAPFDINGBATS, PdfName.ZAPFDINGBATS);
		}
    
		/** Generates the PDF stream with the Type1 and Truetype fonts returning
		 * a PdfStream.
		 */
		internal class StreamFont : PdfStream {
        
			/** Generates the PDF stream with the Type1 and Truetype fonts returning
			 * a PdfStream.
			 * @param contents the content of the stream
			 * @param lengths an array of int that describes the several lengths of each part of the font
			 * @throws DocumentException error in the stream compression
			 */
			internal StreamFont(byte[] contents, int[] lengths) {
				try {
					bytes = contents;
					put(PdfName.LENGTH, new PdfNumber(bytes.Length));
					for (int k = 0; k < lengths.Length; ++k) {
						put(new PdfName("Length" + (k + 1)), new PdfNumber(lengths[k]));
					}
					flateCompress();
				}
				catch (Exception e) {
					throw new DocumentException(e.Message);
				}
			}
        
			internal StreamFont(byte[] contents, string subType) {
				try {
					bytes = contents;
					put(PdfName.LENGTH, new PdfNumber(bytes.Length));
					if (subType != null)
						put(PdfName.SUBTYPE, new PdfName(subType));
					flateCompress();
				}
				catch (Exception e) {
					throw new DocumentException(e.Message);
				}
			}
		}
    
		/**
		 *Creates new BaseFont
		 */
		protected BaseFont() {
		}
    
		/** Creates a new font. This font can be one of the 14 built in types,
		 * a Type1 font referred by an AFM file, a TrueType font (simple or collection) or a CJK font from the
		 * Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		 * appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		 * example would be "STSong-Light,Bold". Note that this modifiers do not work if
		 * the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		 * This would get the second font (indexes start at 0), in this case "MS PGothic".
		 * <P>
		 * The fonts are cached and if they already exist they are extracted from the cache,
		 * not parsed again.
		 * <P>
		 * This method calls:<br>
		 * <PRE>
		 * createFont(name, encoding, embedded, true, null, null);
		 * </PRE>
		 * @param name the name of the font or it's location on file
		 * @param encoding the encoding to be applied to this font
		 * @param embedded true if the font is to be embedded in the PDF
		 * @return returns a new font. This font may come from the cache
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		public static BaseFont createFont(string name, string encoding, bool embedded) {
			return createFont(name, encoding, embedded, true, null, null);
		}
    
		/** Creates a new font. This font can be one of the 14 built in types,
		 * a Type1 font referred by an AFM file, a TrueType font (simple or collection) or a CJK font from the
		 * Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		 * appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		 * example would be "STSong-Light,Bold". Note that this modifiers do not work if
		 * the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		 * This would get the second font (indexes start at 0), in this case "MS PGothic".
		 * <P>
		 * The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		 * If the <CODE>byte</CODE> arrays are present the font will be
		 * read from them instead of the name. The name is still required to identify
		 * the font type.
		 * @param name the name of the font or it's location on file
		 * @param encoding the encoding to be applied to this font
		 * @param embedded true if the font is to be embedded in the PDF
		 * @param cached true if the font comes from the cache or is added to
		 * the cache if new. false if the font is always created new
		 * @param ttfAfm the true type font or the afm in a byte array
		 * @param pfb the pfb in a byte array
		 * @return returns a new font. This font may come from the cache but only if cached
		 * is true, otherwise it will always be created new
		 * @throws DocumentException the font is invalid
		 * @throws IOException the font file could not be read
		 */
		public static BaseFont createFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm, byte[] pfb) {
			string nameBase = getBaseName(name);
			encoding = normalizeEncoding(encoding);
			bool isBuiltinFonts14 = BuiltinFonts14.ContainsKey(name);
			bool isCJKFont = isBuiltinFonts14 ? false : CJKFont.isCJKFont(nameBase, encoding);
			if (isBuiltinFonts14 || isCJKFont)
				embedded = false;
			else if (encoding.Equals(IDENTITY_H) || encoding.Equals(IDENTITY_V))
				embedded = true;
			BaseFont fontFound = null;
			BaseFont fontBuilt = null;
			string key = name + "\n" + encoding + "\n" + embedded;
			if (cached) {
				lock (fontCache) {
					fontFound = (BaseFont)fontCache[key];
				}
				if (fontFound != null)
					return fontFound;
			}
			if (isBuiltinFonts14 || name.ToLower().EndsWith(".afm")) {
				fontBuilt = new Type1Font(name, encoding, embedded, ttfAfm, pfb);
				fontBuilt.fastWinansi = encoding.Equals(CP1252);
			}
			else if (nameBase.ToLower().EndsWith(".ttf") || nameBase.ToLower().IndexOf(".ttc,") > 0) {
				if (encoding.Equals(IDENTITY_H) || encoding.Equals(IDENTITY_V))
					fontBuilt = new TrueTypeFontUnicode(name, encoding, embedded, ttfAfm);
				else {
					fontBuilt = new TrueTypeFont(name, encoding, embedded, ttfAfm);
					fontBuilt.fastWinansi = encoding.Equals(CP1252);
				}
			}
			else if (nameBase.ToLower().EndsWith(".otf")) {
				fontBuilt = new TrueTypeFont(name, encoding, embedded, ttfAfm);
				fontBuilt.fastWinansi = encoding.Equals(CP1252);
			}
			else if (isCJKFont)
				fontBuilt = new CJKFont(name, encoding, embedded);
			else
				throw new DocumentException("Font '" + name + "' with '" + encoding + "' is not recognized.");
			if (cached) {
				lock (fontCache) {
					fontFound = (BaseFont)fontCache[key];
					if (fontFound != null)
						return fontFound;
					fontCache.Add(key, fontBuilt);
				}
			}
			return fontBuilt;
		}
    
		/**
		 * Gets the name without the modifiers Bold, Italic or BoldItalic.
		 * @param name the full name of the font
		 * @return the name without the modifiers Bold, Italic or BoldItalic
		 */
		protected static string getBaseName(string name) {
			if (name.EndsWith(",Bold"))
				return name.Substring(0, name.Length - 5);
			else if (name.EndsWith(",Italic"))
				return name.Substring(0, name.Length - 7);
			else if (name.EndsWith(",BoldItalic"))
				return name.Substring(0, name.Length - 11);
			else
				return name;
		}
    
		/**
		 * Normalize the encoding names. "winansi" is changed to "Cp1252" and
		 * "macroman" is changed to "MacRoman".
		 * @param enc the encoding to be normalized
		 * @return the normalized encoding
		 */
		protected static string normalizeEncoding(string enc) {
			if (enc.Equals("winansi") || enc.Equals(""))
				return CP1252;
			else if (enc.Equals("macroman"))
				return MACROMAN;
			else
				return enc;
		}
    
		/**
		 * Creates the <CODE>widths</CODE> and the <CODE>differences</CODE> arrays
		 * @throws UnsupportedEncodingException the encoding is not supported
		 */
		protected void createEncoding() {
			if (fontSpecific) {
				for (int k = 0; k < 256; ++k)
					widths[k] = getRawWidth(k, null);
			}
			else {
				string s;
				string name;
				char c;
				byte[] b = new byte[1];
				for (int k = 0; k < 256; ++k) {
					b[0] = (byte)k;
					s = System.Text.Encoding.GetEncoding(encoding).GetString(b);
					if (s.Length > 0) {
						c = s[0];
					}
					else {
						c = '?';
					}
					name = GlyphList.unicodeToName((int)c);
					if (name == null)
						name = notdef;
					differences[k] = name;
					this.UnicodeDifferences[k] = c;
					widths[k] = getRawWidth((int)c, name);
				}
			}
		}
    
		/**
		 * Gets the width from the font according to the Unicode char <CODE>c</CODE>
		 * or the <CODE>name</CODE>. If the <CODE>name</CODE> is null it's a symbolic font.
		 * @param c the unicode char
		 * @param name the glyph name
		 * @return the width of the char
		 */
		protected abstract int getRawWidth(int c, string name);
    
		/**
		 * Gets the kerning between two Unicode chars.
		 * @param char1 the first char
		 * @param char2 the second char
		 * @return the kerning to be applied
		 */
		public abstract int getKerning(char char1, char char2);
    
		/**
		 * Gets the width of a <CODE>char</CODE> in normalized 1000 units.
		 * @param char1 the unicode <CODE>char</CODE> to get the width of
		 * @return the width in normalized 1000 units
		 */
		public int getWidth(char char1) {
			if (fastWinansi) {
				if (char1 < 128 || (char1 >= 160 && char1 <= 255))
					return widths[char1];
				return widths[PdfEncodings.winansi[char1]];
			}
			return getWidth(new string(new char[]{char1}));
		}
    
		/**
		 * Gets the width of a <CODE>string</CODE> in normalized 1000 units.
		 * @param text the <CODE>string</CODE> to get the witdth of
		 * @return the width in normalized 1000 units
		 */
		public virtual int getWidth(string text) {
			int total = 0;
			if (fastWinansi) {
				int len = text.Length;
				for (int k = 0; k < len; ++k) {
					char char1 = text[k];
					if (char1 < 128 || (char1 >= 160 && char1 <= 255))
						total += widths[char1];
					else
						total += widths[PdfEncodings.winansi[char1]];
				}
				return total;
			}
			else {
				byte[] mbytes = convertToBytes(text);
				for (int k = 0; k < mbytes.Length; ++k)
					total += widths[0xff & mbytes[k]];
			}
			return total;
		}
    
		/**
		 * Gets the width of a <CODE>string</CODE> in points.
		 * @param text the <CODE>string</CODE> to get the witdth of
		 * @param fontSize the font size
		 * @return the width in points
		 */
		public float getWidthPoint(string text, float fontSize) {
			return (float)getWidth(text) * 0.001f * fontSize;
		}
    
		/**
		 * Gets the width of a <CODE>char</CODE> in points.
		 * @param char1 the <CODE>char</CODE> to get the witdth of
		 * @param fontSize the font size
		 * @return the width in points
		 */
		public float getWidthPoint(char char1, float fontSize) {
			return getWidth(char1) * 0.001f * fontSize;
		}
    
		/**
		 * Converts a <CODE>string</CODE> to a </CODE>byte</CODE> array according
		 * to the font's encoding.
		 * @param text the <CODE>string</CODE> to be converted
		 * @return an array of <CODE>byte</CODE> representing the conversion according to the font's encoding
		 */
		internal virtual byte[] convertToBytes(string text) {
			if (directTextToByte)
				return PdfEncodings.convertToBytes(text, null);
			return PdfEncodings.convertToBytes(text, encoding);
		}
    
		/** Outputs to the writer the font dictionaries and streams.
		 * @param writer the writer for this document
		 * @param ref the font indirect reference
		 * @param params several parameters that depend on the font type
		 * @throws IOException on error
		 * @throws DocumentException error in generating the object
		 */
		internal abstract void writeFont(PdfWriter writer, PdfIndirectReference piRef, Object[] oParams);
    
		/** Gets the encoding used to convert <CODE>string</CODE> into <CODE>byte[]</CODE>.
		 * @return the encoding name
		 */
		public string Encoding {
			get {
				return encoding;
			}
		}
    
		/** Gets the font parameter identified by <CODE>key</CODE>. Valid values
		 * for <CODE>key</CODE> are <CODE>ASCENT</CODE>, <CODE>CAPHEIGHT</CODE>, <CODE>DESCENT</CODE>,
		 * <CODE>ITALICANGLE</CODE>, <CODE>BBOXLLX</CODE>, <CODE>BBOXLLY</CODE>, <CODE>BBOXURX</CODE>
		 * and <CODE>BBOXURY</CODE>.
		 * @param key the parameter to be extracted
		 * @param fontSize the font size in points
		 * @return the parameter in points
		 */
		public abstract float getFontDescriptor(int key, float fontSize);
    
		/** Gets the font type. The font types can be: FONT_TYPE_T1,
		 * FONT_TYPE_TT, FONT_TYPE_CJK and FONT_TYPE_TTUNI.
		 * @return the font type
		 */
		public int FontType {
			get {
				return fontType;
			}

			set {
				fontType = value;
			}
		}
    
		/** Gets the embedded flag.
		 * @return <CODE>true</CODE> if the font is embedded.
		 */
		public bool isEmbedded() {
			return embedded;
		}
    
		/** Gets the symbolic flag of the font.
		 * @return <CODE>true</CODE> if the font is symbolic
		 */
		public bool isFontSpecific() {
			return fontSpecific;
		}
    
		/** Creates a unique subset prefix to be added to the font name when the font is embedded and subset.
		 * @return the subset prefix
		 */
		internal string createSubsetPrefix() {
			lock (subsetPrefix) {
				for (int k = 0; k < subsetPrefix.Length - 1; ++k) {
					int c = subsetPrefix[k];
					if (c == 'Z')
						subsetPrefix[k] = 'A';
					else {
						subsetPrefix[k] = (char)(c + 1);
						break;
					}
				}
				return new string(subsetPrefix);
			}
		}
    
		/** Gets the Unicode character corresponding to the byte output to the pdf stream.
		 * @param index the byte index
		 * @return the Unicode character
		 */
		internal char getUnicodeDifferences(int index) {
			return unicodeDifferences[index];
		}
    
		/** Gets the postscript font name.
		 * @return the postscript font name
		 */
		public abstract string PostscriptFontName {
			get;
		}
    
		/** Gets the full name of the font. If it is a True Type font
		 * each array element will have {Platform ID, Platform Encoding ID,
		 * Language ID, font name}. The interpretation of this values can be
		 * found in the Open Type specification, chapter 2, in the 'name' table.<br>
		 * For the other fonts the array has a single element with {"", "", "",
		 * font name}.
		 * @return the full name of the font
		 */
		public abstract string[][] FullFontName {
			get;
		}
    
		/** Gets the full name of the font. If it is a True Type font
		 * each array element will have {Platform ID, Platform Encoding ID,
		 * Language ID, font name}. The interpretation of this values can be
		 * found in the Open Type specification, chapter 2, in the 'name' table.<br>
		 * For the other fonts the array has a single element with {"", "", "",
		 * font name}.
		 * @param name the name of the font
		 * @param encoding the encoding of the font
		 * @param ttfAfm the true type font or the afm in a byte array
		 * @throws DocumentException on error
		 * @throws IOException on error
		 * @return the full name of the font
		 */    
		public static string[][] getFullFontName(string name, string encoding, byte[] ttfAfm) {
			string nameBase = getBaseName(name);
			BaseFont fontBuilt = null;
			if (nameBase.ToLower().EndsWith(".ttf") || nameBase.ToLower().EndsWith(".otf") || nameBase.ToLower().IndexOf(".ttc,") > 0)
				fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true);
			else
				fontBuilt = createFont(name, encoding, false, false, ttfAfm, null);
			return fontBuilt.FullFontName;
		}
    
		/** Gets the family name of the font. If it is a True Type font
		 * each array element will have {Platform ID, Platform Encoding ID,
		 * Language ID, font name}. The interpretation of this values can be
		 * found in the Open Type specification, chapter 2, in the 'name' table.<br>
		 * For the other fonts the array has a single element with {"", "", "",
		 * font name}.
		 * @return the family name of the font
		 */
		public abstract string[][] FamilyFontName {
			get;
		}
    
		/** Gets the code pages supported by the font. This has only meaning
		 * with True Type fonts.
		 * @return the code pages supported by the font
		 */
		public virtual string[] CodePagesSupported {
			get {
				return new string[0];
			}
		}
    
		/** Enumerates the postscript font names present inside a
		 * True Type Collection.
		 * @param ttcFile the file name of the font
		 * @throws DocumentException on error
		 * @throws IOException on error
		 * @return the postscript font names
		 */    
		public static string[] enumerateTTCNames(string ttcFile) {
			return new EnumerateTTC(ttcFile).Names;
		}

		/** Enumerates the postscript font names present inside a
		 * True Type Collection.
		 * @param ttcArray the font as a <CODE>byte</CODE> array
		 * @throws DocumentException on error
		 * @throws IOException on error
		 * @return the postscript font names
		 */    
		public static string[] enumerateTTCNames(byte[] ttcArray) {
			return new EnumerateTTC(ttcArray).Names;
		}
    
		/** Gets the font width array.
		 * @return the font width array
		 */    
		public int[] Widths {
			get {
				return widths;
			}
		}

		/** Gets the array with the names of the characters.
		 * @return the array with the names of the characters
		 */    
		public string[] Differences {
			get {
				return differences;
			}
		}

		/** Gets the array with the unicode characters.
		 * @return the array with the unicode characters
		 */    
		public char[] UnicodeDifferences {
			get {
				return unicodeDifferences;
			}
		}
    
		/** Gets the state of the property.
		 * @return value of property forceWidthsOutput
		 */
		public bool isForceWidthsOutput() {
			return forceWidthsOutput;
		}
    
		/** Set to <CODE>true</CODE> to force the generation of the
		 * widths array.
		 * @param forceWidthsOutput <CODE>true</CODE> to force the generation of the
		 * widths array
		 */
		public bool ForceWidthsOutput {
			set {
				this.forceWidthsOutput = value;
			}
		}
    
		/** Gets the direct conversion of <CODE>char</CODE> to <CODE>byte</CODE>.
		 * @return value of property directTextToByte.
		 * @see #setDirectTextToByte(bool directTextToByte)
		 */
		public bool isDirectTextToByte() {
			return directTextToByte;
		}
    
		/** Sets the conversion of <CODE>char</CODE> directly to <CODE>byte</CODE>
		 * by casting. This is a low level feature to put the bytes directly in
		 * the content stream without passing through string.getBytes().
		 * @param directTextToByte New value of property directTextToByte.
		 */
		public bool DirectTextToByte {
			set {
				this.directTextToByte = value;
			}
		}
    
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document.
		 * @return <CODE>false</CODE> to include all the glyphs and widths.
		 */
		public bool isSubset() {
			return subset;
		}
    
		/** Indicates if all the glyphs and widths for that particular
		 * encoding should be included in the document. Set to <CODE>false</CODE>
		 * to include all.
		 * @param subset new value of property subset
		 */
		public bool Subset {
			set {
				this.subset = value;
			}
		}

		/** Gets the font resources.
		 * @param key the name of the resource
		 * @return the <CODE>Stream</CODE> to get the resource or
		 * <CODE>null</CODE> if not found
		 */    
		public static Stream getResourceStream(string key) {
			Stream istr = null;
			// Try to use resource loader to load the properties file.
			try {
				Assembly assm = Assembly.GetExecutingAssembly();
				istr = assm.GetManifestResourceStream("iTextSharp.text.pdf.fonts." + key);
				//iTextSharp.text.pdf.fonts.Helvetica-Bold.afm
			}catch (Exception e) {e.GetType();}

			return istr;
		}
    
		/** Gets the Unicode equivalent to a CID.
		 * The (inexistent) CID <FF00> is translated as '\n'. 
		 * It has only meaning with CJK fonts with Identity encoding.
		 * @param c the CID code
		 * @return the Unicode equivalent
		 */    
		public virtual char getUnicodeEquivalent(char c) {
			return c;
		}
    
		/** Gets the CID code given an Unicode.
		 * It has only meaning with CJK fonts.
		 * @param c the Unicode
		 * @return the CID equivalent
		 */    
		public virtual char getCidCode(char c) {
			return c;
		}
	}
}
