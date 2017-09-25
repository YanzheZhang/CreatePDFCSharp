using System;
using System.IO;

/*
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

/** This class takes any PDF and returns exactly the same but
 * encrypted. All the content, links, outlines, etc, are kept.
 */
public class PdfEncryptor : PdfWriter {
    
    RandomAccessFileOrArray file;
    PdfReader reader;
    int[] myXref;
    
    /** Creates new PdfEncryptor.
     * @param reader the read PDF
     * @param os the output destination
     * @throws DocumentException on error
     */
    protected PdfEncryptor(PdfReader reader, Stream os) : base(new PdfDocument(), os) {
        this.reader = reader;
        file = reader.SafeFile;
    }
    
    /** Entry point to encrypt a PDF document. The encryption parameters are the same as in
     * <code>PdfWriter</code>. The userPassword and the
     *  ownerPassword can be null or have zero length. In this case the ownerPassword
     *  is replaced by a random string. The open permissions for the document can be
     *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
     *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
     *  The permissions can be combined by ORing them.
     * @param reader the read PDF
     * @param os the output destination
     * @param userPassword the user password. Can be null or empty
     * @param ownerPassword the owner password. Can be null or empty
     * @param permissions the user permissions
     * @param strength128Bits true for 128 bit key length. false for 40 bit key length
     * @throws DocumentException on error
     * @throws IOException on error */
    public static void encrypt(PdfReader reader, Stream os, byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits) {
        PdfEncryptor enc = new PdfEncryptor(reader, os);
        enc.setEncryption(userPassword, ownerPassword, permissions, strength128Bits);
        enc.go();
    }
    
    /** Entry point to encrypt a PDF document. The encryption parameters are the same as in
     * <code>PdfWriter</code>. The userPassword and the
     *  ownerPassword can be null or have zero length. In this case the ownerPassword
     *  is replaced by a random string. The open permissions for the document can be
     *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
     *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
     *  The permissions can be combined by ORing them.
     * @param reader the read PDF
     * @param os the output destination
     * @param strength true for 128 bit key length. false for 40 bit key length
     * @param userPassword the user password. Can be null or empty
     * @param ownerPassword the owner password. Can be null or empty
     * @param permissions the user permissions
     * @throws DocumentException on error
     * @throws IOException on error */
    public static void encrypt(PdfReader reader, Stream os, bool strength, string userPassword, string ownerPassword, int permissions) {
        PdfEncryptor enc = new PdfEncryptor(reader, os);
        enc.setEncryption(strength, userPassword, ownerPassword, permissions);
        enc.go();
    }
    
    /** Does the actual document manipulation to encrypt it.
     * @throws DocumentException on error
     * @throws IOException on error
     */
    protected void go() {
        body = new PdfBody(HEADER.Length, this, true);
        os.Write(HEADER, 0, HEADER.Length);
        PdfObject[] xb = reader.xrefObj;
        myXref = new int[xb.Length];
        int idx = 1;
        for (int k = 1; k < xb.Length; ++k) {
            if (xb[k] != null)
                myXref[k] = idx++;
        }
        file.reOpen();
        for (int k = 1; k < xb.Length; ++k) {
            if (xb[k] != null)
                addToBody(xb[k]);
        }
        file.close();
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
        PRIndirectReference iRoot = (PRIndirectReference)reader.trailer.get(PdfName.ROOT);
        PdfIndirectReference root = new PdfIndirectReference(0, myXref[iRoot.Number]);
        PRIndirectReference iInfo = (PRIndirectReference)reader.trailer.get(PdfName.INFO);
        PdfIndirectReference info = null;
        if (iInfo != null)
            info = new PdfIndirectReference(0, myXref[iInfo.Number]);
        PdfTrailer trailer = new PdfTrailer(body.Size,
        body.Offset,
        root,
        info,
        encryption,
        fileID);
		byte[] tmp = trailer.toPdf(this);
        os.Write(tmp, 0, tmp.Length);
        os.Close();
    }
    
    internal override int getNewObjectNumber(PdfReader reader, int number, int generation) {
        return myXref[number];
    }
    
    internal override RandomAccessFileOrArray getReaderFile(PdfReader reader) {
        return file;
    }    
}
}