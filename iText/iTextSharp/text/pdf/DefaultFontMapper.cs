using System;
using System.IO;
using System.Collections;
using System.util;

namespace iTextSharp.text.pdf {

/** Default class to map NET fonts to BaseFont.
 */

public class DefaultFontMapper : IFontMapper {
    
    /** A representation of BaseFont parameters.
     */    
    public class BaseFontParameters {
        /** The font name.
         */        
        public string fontName;
        /** The encoding for that font.
         */        
        public string encoding;
        /** The embedding for that font.
         */        
        public bool embedded;
        /** Whether the font is cached of not.
         */        
        public bool cached;
        /** The font bytes for ttf and afm.
         */        
        public byte[] ttfAfm;
        /** The font bytes for pfb.
         */        
        public byte[] pfb;
        
        /** Constructs default BaseFont parameters.
         * @param fontName the font name or location
         */        
        public BaseFontParameters(string fontName) {
            this.fontName = fontName;
            encoding = BaseFont.CP1252;
            embedded = BaseFont.EMBEDDED;
            cached = BaseFont.CACHED;
        }
    }
    
    /** Maps aliases to names.
     */    
    private Hashmap aliases = new Hashmap();
    /** Maps names to BaseFont parameters.
     */    
    private Hashmap mapper = new Hashmap();
    /**
     * Returns a BaseFont which can be used to represent the given NET Font
     *
     * @param	font		the font to be converted
     * @return	a BaseFont which has similar properties to the provided Font
     */
    
    public BaseFont netToPdf(System.Drawing.Font font) {
        try {
            BaseFontParameters p = getBaseFontParameters(font.Name);
            if (p != null)
                return BaseFont.createFont(p.fontName, p.encoding, p.embedded, p.cached, p.ttfAfm, p.pfb);
            string fontKey = null;
            string logicalName = font.Name;

            if (logicalName.Equals("DialogInput") || (logicalName.Equals("Monospaced"))) {

                if (font.Italic) {
                    if (font.Bold) {
                        fontKey = BaseFont.COURIER_BOLDOBLIQUE;

                    } else {
                        fontKey = BaseFont.COURIER_OBLIQUE;
                    }

                } else {
                    if (font.Bold) {
                        fontKey = BaseFont.COURIER_BOLD;

                    } else {
                        fontKey = BaseFont.COURIER;
                    }
                }

            } else if (logicalName.Equals("Serif")) {

                if (font.Italic) {
                    if (font.Bold) {
                        fontKey = BaseFont.TIMES_BOLDITALIC;

                    } else {
                        fontKey = BaseFont.TIMES_ITALIC;
                    }

                } else {
                    if (font.Bold) {
                        fontKey = BaseFont.TIMES_BOLD;

                    } else {
                        fontKey = BaseFont.TIMES_ROMAN;
                    }
                }

            } else {  // default, this catches Dialog and SansSerif

                if (font.Italic) {
                    if (font.Bold) {
                        fontKey = BaseFont.HELVETICA_BOLDOBLIQUE;

                    } else {
                        fontKey = BaseFont.HELVETICA_OBLIQUE;
                    }

                } else {
                    if (font.Bold) {
                        fontKey = BaseFont.HELVETICA_BOLD;
                    } else {
                        fontKey = BaseFont.HELVETICA;
                    }
                }
            }
            return BaseFont.createFont(fontKey, BaseFont.CP1252, false);
        }
        catch (Exception e) {
            throw e;
        }
    }
    
    /**
     * Returns an NET Font which can be used to represent the given BaseFont
     *
     * @param	font		the font to be converted
     * @param	size		the desired point size of the resulting font
     * @return	a Font which has similar properties to the provided BaseFont
     */
    
    public System.Drawing.Font pdfToNet(BaseFont font, int size) {
        string[][] names = font.FullFontName;
        if (names.Length == 1)
            return new System.Drawing.Font(names[0][3], (float)size, System.Drawing.FontStyle.Regular);
        string name10 = null;
        string name3x = null;
        for (int k = 0; k < names.Length; ++k) {
            string[] name = names[k];
            if (name[0].Equals("1") && name[1].Equals("0"))
                name10 = name[3];
            else if (name[2].Equals("1033")) {
                name3x = name[3];
                break;
            }
        }
        string finalName = name3x;
        if (finalName == null)
            finalName = name10;
        if (finalName == null)
            finalName = names[0][3];
        return new System.Drawing.Font(finalName, (float)size, System.Drawing.FontStyle.Regular);
    }
    
    /** Maps a name to a BaseFont parameter.
     * @param netName the name
     * @param parameters the BaseFont parameter
     */    
    public void putName(string netName, BaseFontParameters parameters) {
        mapper.Add(netName, parameters);
    }
    
    /** Maps an alias to a name.
     * @param alias the alias
     * @param netName the name
     */    
    public void putAlias(string alias, string netName) {
        aliases.Add(alias, netName);
    }
    
    /** Looks for a BaseFont parameter associated with a name.
     * @param name the name
     * @return the BaseFont parameter or <CODE>null</CODE> if not found.
     */    
    public BaseFontParameters getBaseFontParameters(string name) {
        string alias = (string)aliases[name];
        if (alias == null)
            return (BaseFontParameters)mapper[name];
        BaseFontParameters p = (BaseFontParameters)mapper[alias];
        if (p == null)
            return (BaseFontParameters)mapper[name];
        else
            return p;
    }
    
    protected void insertNames(string[][] names, string path) {
        string main = null;
        for (int k = 0; k < names.Length; ++k) {
            string[] name = names[k];
            if (name[2].Equals("1033")) {
                main = name[3];
                break;
            }
        }
        if (main == null)
            main = names[0][3];
        BaseFontParameters p = new BaseFontParameters(path);
        mapper.Add(main, p);
        for (int k = 0; k < names.Length; ++k) {
            aliases.Add(names[k][3], main);
        }
    }
    
    /** Inserts all the fonts recognized by iText in the
     * <CODE>directory</CODE> into the map. The encoding
     * will be <CODE>BaseFont.CP1252</CODE> but can be
     * changed later.
     * @param dir the directory to scan
     * @return the number of files processed
     */    
    public int insertDirectory(string dir) {
        DirectoryInfo di = new DirectoryInfo(dir);
        if (!di.Exists)
            return 0;
        FileInfo[] files = di.GetFiles();
        int count = 0;
		foreach(FileInfo file in files) {
            string name = file.FullName.ToLower();
            try {
                if (name.EndsWith(".ttf") || name.EndsWith(".otf") || name.EndsWith(".afm")) {
                    string[][] names = BaseFont.getFullFontName(file.FullName, BaseFont.CP1252, null);
                    insertNames(names, file.FullName);
                    ++count;
                }
                else if (name.EndsWith(".ttc")) {
                    string[] ttcs = BaseFont.enumerateTTCNames(file.FullName);
                    for (int j = 0; j < ttcs.Length; ++j) {
                        string nt = file.FullName + "," + (j + 1);
                        string[][] names = BaseFont.getFullFontName(nt, BaseFont.CP1252, null);
                        insertNames(names, nt);
                    }
                    ++count;
                }
            }
            catch (Exception e) {
				e.GetType();
            }
        }
        return count;
    }
    
    public Hashmap Mapper {
		get {
			return mapper;
		}
    }
    
    public Hashmap Aliases {
		get {
			return aliases;
		}
    }
}
}