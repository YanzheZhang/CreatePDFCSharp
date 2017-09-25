using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Runtime.CompilerServices;
using System.util;

using iTextSharp.text;

/*
 * $Id: PdfWriter.cs,v 1.2 2003/02/25 17:16:35 geraldhenson Exp $
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
 * A <CODE>DocWriter</CODE> class for PDF.
 * <P>
 * When this <CODE>PdfWriter</CODE> is added
 * to a certain <CODE>PdfDocument</CODE>, the PDF representation of every Element
 * added to this Document will be written to the Stream.</P>
 */

public class PdfWriter : DocWriter {
    
    // inner classes
    
    /**
     * This class generates the structure of a PDF document.
     * <P>
     * This class covers the third section of Chapter 5 in the 'Portable Document Format
     * Reference Manual version 1.3' (page 55-60). It contains the body of a PDF document
     * (section 5.14) and it can also generate a Cross-reference Table (section 5.15).
     *
     * @see		PdfWriter
     * @see		PdfObject
     * @see		PdfIndirectObject
     */
    
    public class PdfBody {
        
        // inner classes
        
        /**
         * <CODE>PdfCrossReference</CODE> is an entry in the PDF Cross-Reference table.
         */
        
        internal class PdfCrossReference {
            
            // membervariables
            
            /**	Byte offset in the PDF file. */
            private int offset;
            
            /**	generation of the object. */
            private int generation;
            
            // constructors
            /**
             * Constructs a cross-reference element for a PdfIndirectObject.
             *
             * @param	offset		byte offset of the object
             * @param	generation	generationnumber of the object
             */
            
            internal PdfCrossReference(int offset, int generation) {
                this.offset = offset;
                this.generation = generation;
            }
            
            /**
             * Constructs a cross-reference element for a PdfIndirectObject.
             *
             * @param	offset		byte offset of the object
             */
            
            internal PdfCrossReference(int offset) : this(offset, 0) {}
            
            /**
             * Returns the PDF representation of this <CODE>PdfObject</CODE>.
             *
             * @return		an array of <CODE>byte</CODE>s
             */
            
            internal byte[] toPdf(PdfWriter writer) {
                // This code makes it more difficult to port the lib to JDK1.1.x:
                // StringBuilder off = new StringBuilder("0000000000").Append(offset);
                // off.delete(0, off.Length - 10);
                // StringBuilder gen = new StringBuilder("00000").Append(generation);
                // gen.delete(0, gen.Length - 5);
                // so it was changed into this:
                string s = "0000000000" + offset;
                StringBuilder off = new StringBuilder(s.Substring(s.Length - 10));
                s = "00000" + generation;
                StringBuilder gen = new StringBuilder(s.Substring(s.Length - 5));
                if (generation == 65535) {
                    return getISOBytes(off.Append(' ').Append(gen).Append(" f \n").ToString());
                }
                return getISOBytes(off.Append(' ').Append(gen).Append(" n \n").ToString());
            }
        }
        
        // membervariables
        
        /**	Byte offset in the PDF file of the root object. */
        private int rootOffset;
        
        /** array containing the cross-reference table of the normal objects. */
        private ArrayList xrefs;
        
        /** the current byteposition in the body. */
        private int position;
        private PdfWriter writer;
        private bool simple;
        // constructors
        
        /**
         * Constructs a new <CODE>PdfBody</CODE>.
         *
         * @param	offset	the offset of the body
         */
        
        internal PdfBody(int offset, PdfWriter writer) : this(offset, writer, false) {}
        
        internal PdfBody(int offset, PdfWriter writer, bool simple) {
            this.simple = simple;
            xrefs = new ArrayList();
            xrefs.Add(new PdfCrossReference(0, 65535));
            if (!simple)
                xrefs.Add(new PdfCrossReference(0));
            position = offset;
            this.writer = writer;
        }
        
        // methods
        
        /**
         * Adds a <CODE>PdfObject</CODE> to the body.
         * <P>
         * This methods creates a <CODE>PdfIndirectObject</CODE> with a
         * certain number, containing the given <CODE>PdfObject</CODE>.
         * It also adds a <CODE>PdfCrossReference</CODE> for this object
         * to an <CODE>ArrayList</CODE> that will be used to build the
         * Cross-reference Table.
         *
         * @param		object			a <CODE>PdfObject</CODE>
         * @return		a <CODE>PdfIndirectObject</CODE>
         */
        
        internal PdfIndirectObject Add(PdfObject obj) {
            PdfIndirectObject indirect = new PdfIndirectObject(Size, obj, writer);
            xrefs.Add(new PdfCrossReference(position));
            position += indirect.Length;
            return indirect;
        }
        
        /**
         * Gets a PdfIndirectReference for an object that will be created in the future.
         * @return a PdfIndirectReference
         */
        
        internal PdfIndirectReference PdfIndirectReference {
			get {
				xrefs.Add(new PdfCrossReference(0));
				return new PdfIndirectReference(0, Size - 1);
			}
        }
        
        internal int IndirectReferenceNumber {
			get {
				xrefs.Add(new PdfCrossReference(0));
				return Size - 1;
			}
        }
        
        /**
         * Adds a <CODE>PdfObject</CODE> to the body given an already existing
         * PdfIndirectReference.
         * <P>
         * This methods creates a <CODE>PdfIndirectObject</CODE> with the number given by
         * <CODE>ref</CODE>, containing the given <CODE>PdfObject</CODE>.
         * It also adds a <CODE>PdfCrossReference</CODE> for this object
         * to an <CODE>ArrayList</CODE> that will be used to build the
         * Cross-reference Table.
         *
         * @param		object			a <CODE>PdfObject</CODE>
         * @param		ref		        a <CODE>PdfIndirectReference</CODE>
         * @return		a <CODE>PdfIndirectObject</CODE>
         */
        
        internal PdfIndirectObject Add(PdfObject obj, PdfIndirectReference piref) {
            PdfIndirectObject indirect = new PdfIndirectObject(piref.Number, obj, writer);
            xrefs[piref.Number] = new PdfCrossReference(position);
            position += indirect.Length;
            return indirect;
        }
        
        internal PdfIndirectObject Add(PdfObject obj, int refNumber) {
            PdfIndirectObject indirect = new PdfIndirectObject(refNumber, obj, writer);
            xrefs[refNumber] = new PdfCrossReference(position);
            position += indirect.Length;
            return indirect;
        }
        
        /**
         * Adds a <CODE>PdfResources</CODE> object to the body.
         *
         * @param		object			the <CODE>PdfResources</CODE>
         * @return		a <CODE>PdfIndirectObject</CODE>
         */
        
        internal PdfIndirectObject Add(PdfResources obj) {
            return Add(obj);
        }
        
        /**
         * Adds a <CODE>PdfPages</CODE> object to the body.
         *
         * @param		object			the root of the document
         * @return		a <CODE>PdfIndirectObject</CODE>
         */
        
        internal PdfIndirectObject Add(PdfPages obj) {
            PdfIndirectObject indirect = new PdfIndirectObject(PdfWriter.ROOT, obj, writer);
            rootOffset = position;
            position += indirect.Length;
            return indirect;
        }
        
        /**
         * Returns the offset of the Cross-Reference table.
         *
         * @return		an offset
         */
        
        internal int Offset {
			get {
				return position;
			}
        }
        
        /**
         * Returns the total number of objects contained in the CrossReferenceTable of this <CODE>Body</CODE>.
         *
         * @return	a number of objects
         */
        
        internal int Size {
			get {
				return xrefs.Count;
			}
        }
        
        /**
         * Returns the CrossReferenceTable of the <CODE>Body</CODE>.
         *
         * @return	an array of <CODE>byte</CODE>s
         */
        
        internal byte[] CrossReferenceTable {
			get {
				MemoryStream stream = new MemoryStream();
				try {
					byte[] tmp = getISOBytes("xref\n0 ");
					stream.Write(tmp, 0, tmp.Length);
					tmp = getISOBytes(this.Size.ToString());
					stream.Write(tmp, 0, tmp.Length);
					tmp = getISOBytes("\n");
					stream.Write(tmp, 0, tmp.Length);
					if (!simple) {
						// we set the ROOT object
						xrefs[PdfWriter.ROOT] = new PdfCrossReference(rootOffset);
					}
					// all the other objects
					foreach(PdfCrossReference entry in xrefs) {
						tmp = entry.toPdf(null);
						stream.Write(tmp, 0, tmp.Length);
					}
				}
				catch (IOException ioe) {
					throw ioe;
				}
				return stream.ToArray();
			}
        }
    }
    
    /**
     * <CODE>PdfTrailer</CODE> is the PDF Trailer object.
     * <P>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 5.16 (page 59-60).
     */
    
    internal class PdfTrailer {
        
        // membervariables
        
        /** content of the trailer */
        private byte[] bytes;
        
        // constructors
        
        /**
         * Constructs a PDF-Trailer.
         *
         * @param		size		the number of entries in the <CODE>PdfCrossReferenceTable</CODE>
         * @param		offset		offset of the <CODE>PdfCrossReferenceTable</CODE>
         * @param		root		an indirect reference to the root of the PDF document
         * @param		info		an indirect reference to the info object of the PDF document
         */
        
        internal PdfTrailer(int size, int offset, PdfIndirectReference root, PdfIndirectReference info, PdfIndirectReference encryption, PdfObject fileID) {
            
            MemoryStream stream = new MemoryStream();
            try {
				byte[] tmp = getISOBytes("trailer\n");
                stream.Write(tmp, 0, tmp.Length);
                
                PdfDictionary dictionary = new PdfDictionary();
                dictionary.put(PdfName.SIZE, new PdfNumber(size));
                dictionary.put(PdfName.ROOT, root);
                if (info != null) {
                    dictionary.put(PdfName.INFO, info);
                }
                if (encryption != null)
                    dictionary.put(PdfName.ENCRYPT, encryption);
                if (fileID != null)
                    dictionary.put(PdfName.ID, fileID);
				tmp = dictionary.toPdf(null);
                stream.Write(tmp, 0, tmp.Length);
				tmp = getISOBytes("\nstartxref\n");
                stream.Write(tmp, 0, tmp.Length);
				tmp = getISOBytes(offset.ToString());
                stream.Write(tmp, 0, tmp.Length);
				tmp = getISOBytes("\n%%EOF\n");
                stream.Write(tmp, 0, tmp.Length);
            }
            catch (IOException ioe) {
                throw ioe;
            }
            bytes = stream.ToArray();
        }
        
        /**
         * Returns the PDF representation of this <CODE>PdfObject</CODE>.
         *
         * @return		an array of <CODE>byte</CODE>s
         */
        
        internal byte[] toPdf(PdfWriter writer) {
            return bytes;
        }
    }
    // static membervariables
    
    /** A viewer preference */
    public static int PageLayoutSinglePage = 1;
    /** A viewer preference */
    public static int PageLayoutOneColumn = 2;
    /** A viewer preference */
    public static int PageLayoutTwoColumnLeft = 4;
    /** A viewer preference */
    public static int PageLayoutTwoColumnRight = 8;
    
    /** A viewer preference */
    public static int PageModeUseNone = 16;
    /** A viewer preference */
    public static int PageModeUseOutlines = 32;
    /** A viewer preference */
    public static int PageModeUseThumbs = 64;
    /** A viewer preference */
    public static int PageModeFullScreen = 128;
    
    /** A viewer preference */
    public static int HideToolbar = 256;
    /** A viewer preference */
    public static int HideMenubar = 512;
    /** A viewer preference */
    public static int HideWindowUI = 1024;
    /** A viewer preference */
    public static int FitWindow = 2048;
    /** A viewer preference */
    public static int CenterWindow = 4096;
    
    /** A viewer preference */
    public static int NonFullScreenPageModeUseNone = 8192;
    /** A viewer preference */
    public static int NonFullScreenPageModeUseOutlines = 16384;
    /** A viewer preference */
    public static int NonFullScreenPageModeUseThumbs = 32768;
    
    /** A viewer preference */
    public static int DirectionL2R = 65536;
    /** A viewer preference */
    public static int DirectionR2L = 131072;
    /** The mask to decide if a ViewerPreferences dictionary is needed */
    internal static int ViewerPreferencesMask = 0x3ff00;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowPrinting = 4 + 2048;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowModifyContents = 8;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowCopy = 16;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowModifyAnnotations = 32;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowFillIn = 256;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowScreenReaders = 512;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowAssembly = 1024;
    /** The operation permitted when the document is opened with the user password */
    public static int AllowDegradedPrinting = 4;
    /** Type of encryption */
    public static bool STRENGTH40BITS = false;
    /** Type of encryption */
    public static bool STRENGTH128BITS = true;
    
    public static PdfName DOCUMENT_CLOSE = PdfName.DC;
    public static PdfName WILL_SAVE = PdfName.WS;
    public static PdfName DID_SAVE = PdfName.DS;
    public static PdfName WILL_PRINT = PdfName.WP;
    public static PdfName DID_PRINT = PdfName.DP;
    
    public static int SIGNATURE_EXISTS = 1;
    public static int SIGNATURE_APPEND_ONLY = 2;
    
    public static char VERSION_1_2 = '2';
    public static char VERSION_1_3 = '3';
    public static char VERSION_1_4 = '4';
    
    private static int VPOINT = 7;
    /** this is the header of a PDF document */
    protected byte[] HEADER = getISOBytes("%PDF-1.4\n%\u00e0\u00e1\u00e2\u00e3\n");
    
    /** byte offset of the Body */
    private int OFFSET;
    
    /** This is the object number of the root. */
    private static int ROOT = 1;
    
    /** This is an indirect reference to the root. */
    private static PdfIndirectReference ROOTREFERENCE = new PdfIndirectReference(PdfObject.DICTIONARY, ROOT);
    
    /** Indirect reference to the root of the document. */
    protected PdfPages root = new PdfPages();
    
    /** Dictionary, containing all the images of the PDF document */
    protected PdfXObjectDictionary imageDictionary = new PdfXObjectDictionary();
    
    /** The form XObjects in this document. */
    protected Hashmap formXObjects = new Hashmap();
    
    /** The name counter for the form XObjects name. */
    protected int formXObjectsCounter = 1;
    
    /** The font number counter for the fonts in the document. */
    protected int fontNumber = 1;
    
    /** The color number counter for the colors in the document. */
    protected int colorNumber = 1;
    
    /** The patten number counter for the colors in the document. */
    protected int patternNumber = 1;
    
    /** The direct content in this document. */
    protected PdfContentByte directContent;
    
    /** The direct content under in this document. */
    protected PdfContentByte directContentUnder;
    
    /** The fonts of this document */
    protected Hashmap documentFonts = new Hashmap();
    
    /** The colors of this document */
    protected Hashmap documentColors = new Hashmap();
    
    /** The patterns of this document */
    protected Hashmap documentPatterns = new Hashmap();
    
    protected Hashmap documentShadings = new Hashmap();
    
    protected Hashmap documentShadingPatterns = new Hashmap();
    
    protected ColorDetails patternColorspaceRGB;
    protected ColorDetails patternColorspaceGRAY;
    protected ColorDetails patternColorspaceCMYK;
    protected Hashmap documentSpotPatterns = new Hashmap();
    
    // membervariables
    
    /** body of the PDF document */
    protected PdfBody body;
    
    /** the pdfdocument object. */
    private PdfDocument pdf;
    
    /** The <CODE>PdfPageEvent</CODE> for this document. */
    private IPdfPageEvent pageEvent;
    
    protected PdfEncryption crypto;
    
    private Hashmap importedPages = new Hashmap();
    
    private PdfReaderInstance currentPdfReaderInstance;
    
    /** The PdfIndirectReference to the pages. */
    private ArrayList pageReferences = new ArrayList();
    
    private int currentPageNumber = 1;
    
    /** The defaukt space-char ratio. */    
    public static float SPACE_CHAR_RATIO_DEFAULT = 2.5f;
    /** Disable the inter-character spacing. */    
    public static float NO_SPACE_CHAR_RATIO = 10000000f;
    
    /** Use the default run direction. */    
    public static int RUN_DIRECTION_DEFAULT = 0;
    /** Do not use bidirectional reordering. */    
    public static int RUN_DIRECTION_NO_BIDI = 1;
    /** Use bidirectional reordering with left-to-right
     * preferential run direction.
     */    
    public static int RUN_DIRECTION_LTR = 2;
    /** Use bidirectional reordering with right-to-left
     * preferential run direction.
     */    
    public static int RUN_DIRECTION_RTL = 3;
    protected int runDirection = RUN_DIRECTION_NO_BIDI;
    /**
     * The ratio between the extra word spacing and the extra character spacing.
     * Extra word spacing will grow <CODE>ratio</CODE> times more than extra character spacing.
     */
    private float spaceCharRatio = SPACE_CHAR_RATIO_DEFAULT;
    
    // constructor
    
    /**
     * Constructs a <CODE>PdfWriter</CODE>.
     * <P>
     * Remark: a PdfWriter can only be constructed by calling the method
     * <CODE>getInstance(Document document, Stream os)</CODE>.
     *
     * @param	document	The <CODE>PdfDocument</CODE> that has to be written
     * @param	os			The <CODE>Stream</CODE> the writer has to write to.
     */
    
    protected PdfWriter(PdfDocument document, Stream os) : base(document, os) {
        OFFSET = HEADER.Length;
		body = new PdfBody(OFFSET, this);
		pdf = document;
        directContent = new PdfContentByte(this);
        directContentUnder = new PdfContentByte(this);
    }
    
    // get an instance of the PdfWriter
    
    /**
     * Gets an instance of the <CODE>PdfWriter</CODE>.
     *
     * @param	document	The <CODE>Document</CODE> that has to be written
     * @param	os	The <CODE>Stream</CODE> the writer has to write to.
     * @return	a new <CODE>PdfWriter</CODE>
     *
     * @throws	DocumentException on error
     */
    
    public static PdfWriter getInstance(Document document, Stream os) {
        PdfDocument pdf = new PdfDocument();
        document.addDocListener(pdf);
        PdfWriter writer = new PdfWriter(pdf, os);
        pdf.addWriter(writer);
        return writer;
    }
    
    /** Gets an instance of the <CODE>PdfWriter</CODE>.
     *
     * @return a new <CODE>PdfWriter</CODE>
     * @param document The <CODE>Document</CODE> that has to be written
     * @param os The <CODE>Stream</CODE> the writer has to write to.
     * @param listener A <CODE>DocListener</CODE> to pass to the PdfDocument.
     * @throws DocumentException on error
     */
    
    public static PdfWriter getInstance(Document document, Stream os, IDocListener listener) {
        PdfDocument pdf = new PdfDocument();
        pdf.addDocListener(listener);
        document.addDocListener(pdf);
        PdfWriter writer = new PdfWriter(pdf, os);
        pdf.addWriter(writer);
        return writer;
    }
    
    // methods to write objects to the Stream
    
    /**
     * Adds some <CODE>PdfContents</CODE> to this Writer.
     * <P>
     * The document has to be open before you can begin to add content
     * to the body of the document.
     *
     * @return a <CODE>PdfIndirectReference</CODE>
     * @param page the <CODE>PdfPage</CODE> to add
     * @param contents the <CODE>PdfContents</CODE> of the page
     * @throws PdfException on error
     */
    
    public PdfIndirectReference Add(PdfPage page, PdfContents contents) {
        if (!open) {
            throw new PdfException("The document isn't open.");
        }
        PdfIndirectObject obj = body.Add(contents);
        try {
            obj.writeTo(os);
        }
        catch(IOException ioe) {
            throw ioe;
        }
        page.Add(obj.IndirectReference);
        page.Parent = ROOTREFERENCE;
        PdfIndirectObject pageObject = body.Add(page, getPageReference(currentPageNumber++));
        try {
            pageObject.writeTo(os);
        }
        catch(IOException ioe) {
            throw ioe;
        }
        root.Add(pageObject.IndirectReference);
        return pageObject.IndirectReference;
    }
    
    /**
     * Writes a <CODE>PdfImage</CODE> to the Stream.
     *
     * @param pdfImage the image to be added
     * @return a <CODE>PdfIndirectReference</CODE> to the encapsulated image
     * @throws PdfException when a document isn't open yet, or has been closed
     */
    
    public PdfIndirectReference Add(PdfImage pdfImage) {
        if (! imageDictionary.contains(pdfImage)) {
            PdfIndirectObject obj = body.Add(pdfImage);
            try {
                obj.writeTo(os);
            }
            catch(IOException ioe) {
                throw ioe;
            }
            imageDictionary.put(pdfImage.Name, obj.IndirectReference);
            return obj.IndirectReference;
        }
        return (PdfIndirectReference) imageDictionary.get(pdfImage.Name);
    }
    
//    protected PdfIndirectReference Add(PdfICCBased icc) {
//        PdfIndirectObject obj = body.Add(icc);
//        try {
//            obj.writeTo(os);
//        }
//        catch(IOException ioe) {
//            throw new ExceptionConverter(ioe);
//        }
//        return obj.IndirectReference;
//    }
    
    /**
     * return the <CODE>PdfIndirectReference</CODE> to the image with a given name.
     *
     * @param name the name of the image
     * @return a <CODE>PdfIndirectReference</CODE>
     */
    
    public PdfIndirectReference getImageReference(PdfName name) {
        return (PdfIndirectReference) imageDictionary.get(name);
    }
    
    // methods to open and close the writer
    
    /**
     * Signals that the <CODE>Document</CODE> has been opened and that
     * <CODE>Elements</CODE> can be added.
     * <P>
     * When this method is called, the PDF-document header is
     * written to the Stream.
     */
    
    public override void Open() {
        try {
            os.Write(HEADER, 0, HEADER.Length);
        }
        catch(IOException ioe) {
            throw ioe;
        }
    }
    
    /**
     * Signals that the <CODE>Document</CODE> was closed and that no other
     * <CODE>Elements</CODE> will be added.
     * <P>
     * The pages-tree is built and written to the Stream.
     * A Catalog is constructed, as well as an Info-object,
     * the referencetable is composed and everything is written
     * to the Stream embedded in a Trailer.
     */
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Close() {
        if (open) {
            if ((currentPageNumber - 1) != pageReferences.Count)
                throw new RuntimeException("The page " + pageReferences.Count +
                " was requested but the document has only " + (currentPageNumber - 1) + " pages.");
            pdf.Close();
            try {
                // add the fonts
				foreach(FontDetails details in documentFonts.Values) {
                    details.writeFont(this);
                }
                
                // add the form XObjects
				foreach(PdfTemplate template in formXObjects.Keys) {
                    if (template.Type == PdfTemplate.TYPE_TEMPLATE) {
                        PdfIndirectObject obj = body.Add(template.FormXObject, template.IndirectReference);
                        obj.writeTo(os);
                    }
                }
                // add all the dependencies in the imported pages
				foreach(PdfReaderInstance current in importedPages.Values) {
					currentPdfReaderInstance = current;
					current.writeAllPages();
                }
                
                // add the color
				foreach(ColorDetails color in documentColors.Values) {
                    PdfIndirectObject cobj = body.Add(color.getSpotColor(this), color.IndirectReference);
                    cobj.writeTo(os);
                }
                // add the pattern
				foreach(PdfPatternPainter pat in documentPatterns.Keys) {
                    PdfIndirectObject pobj = body.Add(pat.Pattern, pat.IndirectReference);
                    pobj.writeTo(os);
                }
                // add the shading patterns
				foreach(PdfShadingPattern shadingPattern in documentShadingPatterns.Keys) {
                    shadingPattern.addToBody();
                }
                // add the shadings
				foreach(PdfShading shading in documentShadings.Keys) {
                    shading.addToBody();
                }
                // add the root to the body
                PdfIndirectObject rootObject = body.Add(root);
                rootObject.writeTo(os);
                // make the catalog-object and add it to the body
                PdfIndirectObject indirectCatalog = body.Add(((PdfDocument)document).getCatalog(rootObject.IndirectReference));
                indirectCatalog.writeTo(os);
                // add the info-object to the body
                PdfIndirectObject info = body.Add(((PdfDocument)document).Info);
                info.writeTo(os);
                PdfIndirectReference encryption = null;
                PdfLiteral fileID = null;
                if (crypto != null) {
                    PdfIndirectObject encryptionObject = body.Add(crypto.EncryptionDictionary);
                    encryptionObject.writeTo(os);
                    encryption = encryptionObject.IndirectReference;
                    fileID = crypto.FileID;
                }
                
                // write the cross-reference table of the body
                os.Write(body.CrossReferenceTable, 0, body.CrossReferenceTable.Length);
                // make the trailer
                PdfTrailer trailer = new PdfTrailer(body.Size,
                body.Offset,
                indirectCatalog.IndirectReference,
                info.IndirectReference,
                encryption,
                fileID);
				byte[] tmp = trailer.toPdf(this);
                os.Write(tmp, 0, tmp.Length);
                base.Close();
            }
            catch(IOException ioe) {
                throw ioe;
            }
        }
    }
    
    // methods
    
    /**
     * Returns the number of the next object that can be added to the body.
     *
     * @return	the size of the body-object
     */
    
    internal int Size {
		get {
			return body.Size;
		}
    }
    
    /**
     * Sometimes it is necessary to know where the just added <CODE>Table</CODE> ends.
     *
     * For instance to avoid to add another table in a page that is ending up, because
     * the new table will be probably splitted just after the header (it is an
     * unpleasant effect, isn't it?).
     *
     * Added on September 8th, 2001
     * by Francesco De Milato
     * francesco.demilato@tiscalinet.it
     * @param table the <CODE>Table</CODE>
     * @return the bottom height of the just added table
     */
    
    public float getTableBottom(Table table) {
        return pdf.bottom(table) - pdf.IndentBottom();
    }
    
    /**
     * Checks if a <CODE>Table</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
     *
     * @param	table	the table that has to be checked
     * @param	margin	a certain margin
     * @return	<CODE>true</CODE> if the <CODE>Table</CODE> fits the page, <CODE>false</CODE> otherwise.
     */
    
    public bool fitsPage(Table table, float margin) {
        return pdf.bottom(table) > pdf.IndentBottom() + margin;
    }
    
    /**
     * Checks if a <CODE>Table</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
     *
     * @param	table	the table that has to be checked
     * @return	<CODE>true</CODE> if the <CODE>Table</CODE> fits the page, <CODE>false</CODE> otherwise.
     */
    
    public bool fitsPage(Table table) {
        return fitsPage(table, 0);
    }
    
    /**
     * Checks if a <CODE>PdfPTable</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
     *
     * @param	table	the table that has to be checked
     * @param	margin	a certain margin
     * @return	<CODE>true</CODE> if the <CODE>PdfPTable</CODE> fits the page, <CODE>false</CODE> otherwise.
     */
    public bool fitsPage(PdfPTable table, float margin) {
        return pdf.fitsPage(table, margin);
    }
    
    /**
     * Checks if a <CODE>PdfPTable</CODE> fits the current page of the <CODE>PdfDocument</CODE>.
     *
     * @param	table	the table that has to be checked
     * @return	<CODE>true</CODE> if the <CODE>PdfPTable</CODE> fits the page, <CODE>false</CODE> otherwise.
     */
    public bool fitsPage(PdfPTable table) {
        return pdf.fitsPage(table, 0);
    }
    
    /**
     * Checks if writing is paused.
     *
     * @return		<CODE>true</CODE> if writing temporarely has to be paused, <CODE>false</CODE> otherwise.
     */
    
    internal bool isPaused() {
        return pause;
    }
    
    /**
     * Gets the direct content for this document. There is only one direct content,
     * multiple calls to this method will allways retrieve the same.
     * @return the direct content
     */
    
    public PdfContentByte DirectContent {
		get {
			return directContent;
		}
    }
    
    /**
     * Gets the direct content under for this document. There is only one direct content,
     * multiple calls to this method will allways retrieve the same.
     * @return the direct content
     */
    
    public PdfContentByte DirectContentUnder {
		get {
			return directContentUnder;
		}
    }
    
    /**
     * Resets all the direct contents to empty. This happens when a new page is started.
     */
    
    internal void resetContent() {
        directContent.reset();
        directContentUnder.reset();
    }
    
    /** Gets the AcroForm object.
     * @return the <CODE>PdfAcroForm</CODE>
     */
    
    public PdfAcroForm AcroForm {
		get {
			return pdf.AcroForm;
		}
    }
    
    /** Gets the root outline.
     * @return the root outline
     */
    
    public PdfOutline RootOutline {
		get {
			return directContent.RootOutline;
		}
    }
    
    /**
     * Adds a <CODE>BaseFont</CODE> to the document and to the page resources.
     * @param bf the <CODE>BaseFont</CODE> to add
     * @return the name of the font in the document
     */
    
    internal FontDetails Add(BaseFont bf) {
        FontDetails ret = (FontDetails)addSimple(bf);
        pdf.addFont(ret.FontName, ret.IndirectReference);
        return ret;
    }
    
    /**
     * Adds a <CODE>BaseFont</CODE> to the document but not to the page resources.
     * It is used for templates.
     * @param bf the <CODE>BaseFont</CODE> to add
     * @return an <CODE>Object[]</CODE> where position 0 is a <CODE>PdfName</CODE>
     * and position 1 is an <CODE>PdfIndirectReference</CODE>
     */
    
    internal FontDetails addSimple(BaseFont bf) {
        FontDetails ret = (FontDetails)documentFonts[bf];
        if (ret == null) {
            ret = new FontDetails(new PdfName("F" + (fontNumber++)), body.PdfIndirectReference, bf);
            documentFonts.Add(bf, ret);
        }
        return ret;
    }
    
    internal void eliminateFontSubset(PdfDictionary fonts) {
		foreach(FontDetails ft in documentFonts.Values) {
            if (fonts.get(ft.FontName) != null)
                ft.Subset = false;
        }
    }
    
    /**
     * Adds a <CODE>SpotColor</CODE> to the document and to the page resources.
     * @param spc the <CODE>PdfSpotColor</CODE> to add
     * @return the name of the spotcolor in the document
     */
    
    internal ColorDetails Add(PdfSpotColor spc) {
        ColorDetails ret = (ColorDetails)addSimple(spc);
        pdf.addColor(ret.ColorName, ret.IndirectReference);
        return ret;
    }
    
    /**
     * Adds a <CODE>SpotColor</CODE> to the document but not to the page resources.
     * @param spc the <CODE>SpotColor</CODE> to add
     * @return an <CODE>Object[]</CODE> where position 0 is a <CODE>PdfName</CODE>
     * and position 1 is an <CODE>PdfIndirectReference</CODE>
     */
    
    internal ColorDetails addSimple(PdfSpotColor spc) {
        ColorDetails ret = (ColorDetails)documentColors[spc];
        if (ret == null) {
            ret = new ColorDetails(new PdfName("CS" + (colorNumber++)), body.PdfIndirectReference, spc);
            documentColors.Add(spc, ret);
        }
        return ret;
    }
    
    internal ColorDetails addSimplePatternColorspace(Color color) {
        int type = ExtendedColor.getType(color);
        if (type == ExtendedColor.TYPE_PATTERN || type == ExtendedColor.TYPE_SHADING)
            throw new RuntimeException("An uncolored tile pattern can not have another pattern or shding as color.");
        try {
            switch (type) {
                case ExtendedColor.TYPE_RGB:
                    if (patternColorspaceRGB == null) {
                        patternColorspaceRGB = new ColorDetails(new PdfName("CS" + (colorNumber++)), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICERGB);
                        PdfIndirectObject cobj = body.Add(array, patternColorspaceRGB.IndirectReference);
                        cobj.writeTo(os);
                    }
                    return patternColorspaceRGB;
                case ExtendedColor.TYPE_CMYK:
                    if (patternColorspaceCMYK == null) {
                        patternColorspaceCMYK = new ColorDetails(new PdfName("CS" + (colorNumber++)), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICECMYK);
                        PdfIndirectObject cobj = body.Add(array, patternColorspaceCMYK.IndirectReference);
                        cobj.writeTo(os);
                    }
                    return patternColorspaceCMYK;
                case ExtendedColor.TYPE_GRAY:
                    if (patternColorspaceGRAY == null) {
                        patternColorspaceGRAY = new ColorDetails(new PdfName("CS" + (colorNumber++)), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(PdfName.DEVICEGRAY);
                        PdfIndirectObject cobj = body.Add(array, patternColorspaceGRAY.IndirectReference);
                        cobj.writeTo(os);
                    }
                    return patternColorspaceGRAY;
                case ExtendedColor.TYPE_SEPARATION: {
                    ColorDetails details = addSimple(((SpotColor)color).getPdfSpotColor());
                    ColorDetails patternDetails = (ColorDetails)documentSpotPatterns[details];
                    if (patternDetails == null) {
                        patternDetails = new ColorDetails(new PdfName("CS" + (colorNumber++)), body.PdfIndirectReference, null);
                        PdfArray array = new PdfArray(PdfName.PATTERN);
                        array.Add(details.IndirectReference);
                        PdfIndirectObject cobj = body.Add(array, patternDetails.IndirectReference);
                        cobj.writeTo(os);
                        documentSpotPatterns.Add(details, patternDetails);
                    }
                    return patternDetails;
                }
                default:
                    throw new RuntimeException("Invalid color type in PdfWriter.addSimplePatternColorspace().");
            }
        }
        catch (Exception e) {
            throw new RuntimeException(e.Message);
        }
    }
    
    internal void addSimpleShadingPattern(PdfShadingPattern shading) {
        if (!documentShadingPatterns.ContainsKey(shading)) {
            shading.Name = patternNumber;
            ++patternNumber;
            documentShadingPatterns.Add(shading, null);
            addSimpleShading(shading.Shading);
        }
    }
    
    internal void addSimpleShading(PdfShading shading) {
        if (!documentShadings.ContainsKey(shading)) {
            documentShadings.Add(shading, null);
            shading.Name = documentShadings.Count;
        }
    }
    
    /**
     * Gets the <CODE>PdfDocument</CODE> associated with this writer.
     * @return the <CODE>PdfDocument</CODE>
     */
    
    internal PdfDocument PdfDocument {
		get {
			return pdf;
		}
    }
    
    /**
     * Gets a <CODE>PdfIndirectReference</CODE> for an object that
     * will be created in the future.
     * @return the <CODE>PdfIndirectReference</CODE>
     */
    
    internal PdfIndirectReference PdfIndirectReference {
		get {
			return body.PdfIndirectReference;
		}
    }
    
    internal int IndirectReferenceNumber {
		get {
			return body.IndirectReferenceNumber;
		}
    }
    
    internal PdfName addSimplePattern(PdfPatternPainter painter) {
        PdfName name = (PdfName)documentPatterns[painter];
        try {
            if ( name == null ) {
                name = new PdfName("P" + patternNumber);
                ++patternNumber;
                documentPatterns.Add(painter, name);
            }
        } catch (Exception e) {
            throw e;
        }
        return name;
    }
    
    /**
     * Adds a template to the document but not to the page resources.
     * @param template the template to add
     * @return the <CODE>PdfName</CODE> for this template
     */
    
    internal PdfName addDirectTemplateSimple(PdfTemplate template) {
        PdfName name = (PdfName)formXObjects[template];
        try {
            if (name == null) {
                name = new PdfName("Xf" + formXObjectsCounter);
                ++formXObjectsCounter;
                formXObjects.Add(template, name);
            }
        }
        catch (Exception e) {
            throw e;
        }
        return name;
    }
    
    /**
     * Gets the <CODE>PdfPageEvent</CODE> for this document or <CODE>null</CODE>
     * if none is set.
     * @return the <CODE>PdfPageEvent</CODE> for this document or <CODE>null</CODE>
     * if none is set
     */
    
    public IPdfPageEvent PageEvent {
		get {
			return pageEvent;
		}

		set {
			this.pageEvent = value;
		}
    }
    
    /**
     * Adds the local destinations to the body of the document.
     * @param dest the <CODE>Hashtable</CODE> containing the destinations
     * @throws IOException on error
     */
    
    internal void addLocalDestinations(SortedMap dest) {
		foreach(string name in dest.Keys) {
            Object[] obj = (Object[])dest[name];
            PdfDestination destination = (PdfDestination)obj[2];
            if (destination == null)
                throw new RuntimeException("The name '" + name + "' has no local destination.");
            if (obj[1] == null)
                obj[1] = PdfIndirectReference;
            PdfIndirectObject iob = body.Add(destination, (PdfIndirectReference)obj[1]);
            iob.writeTo(os);
        }
    }
    
    /**
     * Gets the current pagenumber of this document.
     *
     * @return a page number
     */
    
    public int PageNumber {
		get {
			return pdf.PageNumber;
		}
    }
    
    /**
     * Sets the viewer preferences by ORing some of these constants:<br>
     * <ul>
     * <li>The page layout to be used when the document is opened (choose one).
     *   <ul>
     *   <li><b>PageLayoutSinglePage</b> - Display one page at a time.
     *   <li><b>PageLayoutOneColumn</b> - Display the pages in one column.
     *   <li><b>PageLayoutTwoColumnLeft</b> - Display the pages in two columns, with
     *       oddnumbered pages on the left.
     *   <li><b>PageLayoutTwoColumnRight</b> - Display the pages in two columns, with
     *       oddnumbered pages on the right.
     *   </ul>
     * <li>The page mode how the document should be displayed
     *     when opened (choose one).
     *   <ul>
     *   <li><b>PageModeUseNone</b> - Neither document outline nor thumbnail images visible.
     *   <li><b>PageModeUseOutlines</b> - Document outline visible.
     *   <li><b>PageModeUseThumbs</b> - Thumbnail images visible.
     *   <li><b>PageModeFullScreen</b> - Full-screen mode, with no menu bar, window
     *       controls, or any other window visible.
     *   </ul>
     * <li><b>HideToolbar</b> - A flag specifying whether to hide the viewer application's tool
     *     bars when the document is active.
     * <li><b>HideMenubar</b> - A flag specifying whether to hide the viewer application's
     *     menu bar when the document is active.
     * <li><b>HideWindowUI</b> - A flag specifying whether to hide user interface elements in
     *     the document's window (such as scroll bars and navigation controls),
     *     leaving only the document's contents displayed.
     * <li><b>FitWindow</b> - A flag specifying whether to resize the document's window to
     *     fit the size of the first displayed page.
     * <li><b>CenterWindow</b> - A flag specifying whether to position the document's window
     *     in the center of the screen.
     * <li>The document's page mode, specifying how to display the
     *     document on exiting full-screen mode. It is meaningful only
     *     if the page mode is <b>PageModeFullScreen</b> (choose one).
     *   <ul>
     *   <li><b>NonFullScreenPageModeUseNone</b> - Neither document outline nor thumbnail images
     *       visible
     *   <li><b>NonFullScreenPageModeUseOutlines</b> - Document outline visible
     *   <li><b>NonFullScreenPageModeUseThumbs</b> - Thumbnail images visible
     *   </ul>
     * </ul>
     * @param preferences the viewer preferences
     */
    
    public void setViewerPreferences(int preferences) {
        pdf.ViewerPreferences = preferences;
    }
    
    /** Sets the encryption options for this document. The userPassword and the
     *  ownerPassword can be null or have zero length. In this case the ownerPassword
     *  is replaced by a random string. The open permissions for the document can be
     *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
     *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
     *  The permissions can be combined by ORing them.
     * @param userPassword the user password. Can be null or empty
     * @param ownerPassword the owner password. Can be null or empty
     * @param permissions the user permissions
     * @param strength128Bits true for 128 bit key length. false for 40 bit key length
     * @throws DocumentException if the document is already open
     */
    public void setEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits) {
        if (pdf.isOpen())
            throw new DocumentException("Encryption can only be added before opening the document.");
        crypto = new PdfEncryption();
        crypto.setupAllKeys(userPassword, ownerPassword, permissions, strength128Bits);
    }
    
    /**
     * Sets the encryption options for this document. The userPassword and the
     *  ownerPassword can be null or have zero length. In this case the ownerPassword
     *  is replaced by a random string. The open permissions for the document can be
     *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
     *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
     *  The permissions can be combined by ORing them.
     * @param strength true for 128 bit key length. false for 40 bit key length
     * @param userPassword the user password. Can be null or empty
     * @param ownerPassword the owner password. Can be null or empty
     * @param permissions the user permissions
     * @throws DocumentException if the document is already open
     */
    public void setEncryption(bool strength, string userPassword, string ownerPassword, int permissions) {
        setEncryption(getISOBytes(userPassword), getISOBytes(ownerPassword), permissions, strength);
    }
    
    internal PdfIndirectObject addToBody(PdfObject obj) {
        PdfIndirectObject iobj = body.Add(obj);
        iobj.writeTo(os);
        return iobj;
    }
    
    internal PdfIndirectObject addToBody(PdfObject obj, PdfIndirectReference piref) {
        PdfIndirectObject iobj = body.Add(obj, piref);
        iobj.writeTo(os);
        return iobj;
    }
    
    internal PdfIndirectObject addToBody(PdfObject obj, int refNumber) {
        PdfIndirectObject iobj = body.Add(obj, refNumber);
        iobj.writeTo(os);
        return iobj;
    }
    
    /** When the document opens it will jump to the destination with
     * this name.
     * @param name the name of the destination to jump to
     */
    public void setOpenAction(string name) {
        pdf.setOpenAction(name);
    }
    
    /** Additional-actions defining the actions to be taken in
     * response to various trigger events affecting the document
     * as a whole. The actions types allowed are: <CODE>DOCUMENT_CLOSE</CODE>,
     * <CODE>WILL_SAVE</CODE>, <CODE>DID_SAVE</CODE>, <CODE>WILL_PRINT</CODE>
     * and <CODE>DID_PRINT</CODE>.
     *
     * @param actionType the action type
     * @param action the action to execute in response to the trigger
     * @throws PdfException on invalid action type
     */
    public void setAdditionalAction(PdfName actionType, PdfAction action) {
        if (!(actionType.Equals(DOCUMENT_CLOSE) ||
        actionType.Equals(WILL_SAVE) ||
        actionType.Equals(DID_SAVE) ||
        actionType.Equals(WILL_PRINT) ||
        actionType.Equals(DID_PRINT))) {
            throw new PdfException("Invalid additional action type.");
        }
        pdf.addAdditionalAction(actionType, action);
    }
    
    /** When the document opens this <CODE>action</CODE> will be
     * invoked.
     * @param action the action to be invoked
     */
    public void setOpenAction(PdfAction action) {
        pdf.setOpenAction(action);
    }
    
    /** Sets the page labels
     * @param pageLabels the page labels
     */
    public PdfPageLabels PageLabels {
		set {
			pdf.PageLabels = value;
		}
    }
    
    internal PdfEncryption Encryption {
		get {
			return crypto;
		}
    }
    
    internal virtual RandomAccessFileOrArray getReaderFile(PdfReader reader) {
        return currentPdfReaderInstance.ReaderFile;
    }
    
    internal virtual int getNewObjectNumber(PdfReader reader, int number, int generation) {
        return currentPdfReaderInstance.getNewObjectNumber(number, generation);
    }
    
    /** Gets a page from other PDF document. The page can be used as
     * any other PdfTemplate. Note that calling this method more than
     * once with the same parameters will retrieve the same object.
     * @param reader the PDF document where the page is
     * @param pageNumber the page number. The first page is 1
     * @return the template representing the imported page
     */
    public PdfImportedPage getImportedPage(PdfReader reader, int pageNumber) {
        PdfReaderInstance inst = (PdfReaderInstance)importedPages[reader];
        if (inst == null) {
            inst = reader.getPdfReaderInstance(this);
            importedPages.Add(reader, inst);
        }
        return inst.getImportedPage(pageNumber);
    }
    
    /** Adds a JavaScript action at the document level. When the document
     * opens all this JavaScript runs.
     * @param js The JavaScrip action
     */
    public void addJavaScript(PdfAction js) {
        pdf.addJavaScript(js);
    }
    
    /** Adds a JavaScript action at the document level. When the document
     * opens all this JavaScript runs.
     * @param code the JavaScript code
     * @param unicode select JavaScript unicode. Note that the internal
     * Acrobat JavaScript engine does not support unicode,
     * so this may or may not work for you
     */
    public void addJavaScript(string code, bool unicode) {
        addJavaScript(PdfAction.javaScript(code, this, unicode));
    }
    
    /** Adds a JavaScript action at the document level. When the document
     * opens all this JavaScript runs.
     * @param code the JavaScript code
     */
    public void addJavaScript(string code) {
        addJavaScript(code, false);
    }
    
    /** Sets the crop box. The crop box should not be rotated even if the
     * page is rotated. This change only takes effect in the next
     * page.
     * @param crop the crop box
     */
    public Rectangle CropBoxSize {
		set {
			pdf.CropBoxSize = value;
		}
    }
    
    /** Gets a reference to a page existing or not. If the page does not exist
     * yet the reference will be created in advance. If on closing the document, a
     * page number greater than the total number of pages was requested, an
     * exception is thrown.
     * @param page the page number. The first page is 1
     * @return the reference to the page
     */
    internal PdfIndirectReference getPageReference(int page) {
        --page;
        if (page < 0)
            throw new Exception("The page numbers start at 1.");
        PdfIndirectReference piref;
        if (page < pageReferences.Count) {
            piref = (PdfIndirectReference)pageReferences[page];
            if (piref == null) {
                piref = body.PdfIndirectReference;
                pageReferences[page] = piref;
            }
        }
        else {
            int empty = page - pageReferences.Count;
            for (int k = 0; k < empty; ++k)
                pageReferences.Add(null);
            piref = body.PdfIndirectReference;
            pageReferences.Add(piref);
        }
        return piref;
    }
    
    internal PdfIndirectReference CurrentPage {
		get {
			return getPageReference(currentPageNumber);
		}
    }
    
    internal int CurrentPageNumber {
		get {
			return currentPageNumber;
		}
    }
    
    /** Adds the <CODE>PdfAnnotation</CODE> to the calculation order
     * array.
     * @param annot the <CODE>PdfAnnotation</CODE> to be added
     */
    public void addCalculationOrder(PdfFormField annot) {
        pdf.addCalculationOrder(annot);
    }
    
    /** Set the signature flags.
     * @param f the flags. This flags are ORed with current ones
     */
    public int SigFlags {
		set {
			pdf.SigFlags = value;
		}
    }
    
    /** Adds a <CODE>PdfAnnotation</CODE> or a <CODE>PdfFormField</CODE>
     * to the document. Only the top parent of a <CODE>PdfFormField</CODE>
     * needs to be added.
     * @param annot the <CODE>PdfAnnotation</CODE> or the <CODE>PdfFormField</CODE> to add
     */
    public void addAnnotation(PdfAnnotation annot) {
        pdf.addAnnotation(annot);
    }
    
    /** Sets the PDF version. Must be used right before the document
     * is opened. Valid options are VERSION_1_2, VERSION_1_3 and
     * VERSION_1_4. VERSION_1_4 is the default.
     * @param version the version number
     */
    public char setPdfVersion {
		set {
			HEADER[VPOINT] = (byte)value;
		}
    }
    
    /** Reorder the pages in the document. A <CODE>null</CODE> argument value
     * only returns the number of pages to process. It is
     * advisable to issue a <CODE>Document.newPage()</CODE>
     * before using this method.
     * @return the total number of pages
     * @param order an array with the new page sequence. It must have the
     * same size as the number of pages.
     * @throws DocumentException if all the pages are not present in the array
     */
    public int reorderPages(int[] order) {
        return root.reorderPages(order);
    }
    
    /** Gets the space/character extra spacing ratio for
     * fully justified text.
     * @return the space/character extra spacing ratio
     */
    public float SpaceCharRatio {
		get {
			return spaceCharRatio;
		}

		set {
			if (value < 0.001f)
				this.spaceCharRatio = 0.001f;
			else
				this.spaceCharRatio = value;
		}
    }
    
    /** Gets the run direction.
     * @return the run direction
     */    
    public int RunDirection {
		get {
			return runDirection;
		}

		set {
			if (value < RUN_DIRECTION_NO_BIDI || value > RUN_DIRECTION_RTL)
				throw new RuntimeException("Invalid run direction: " + value);
			this.runDirection = value;
		}
    }

    /**
     * Sets the display duration for the page (for presentations)
     * @param seconds   the number of seconds to display the page
     */
    public int Duration {
		set {
			pdf.Duration = value;
		}
    }
    
    /**
     * Sets the transition for the page
     * @param transition   the Transition object
     */
    public PdfTransition Transition {
		set {
			pdf.Transition = value;
		}
    }
}
}