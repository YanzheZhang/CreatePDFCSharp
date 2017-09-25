using System;
using System.IO;
using System.Collections;
using System.util;

using iTextSharp.text;

/*
 * $Id: PdfReaderInstance.cs,v 1.1.1.1 2003/02/04 02:57:41 geraldhenson Exp $
 * $Name:  $
 *
 * Copyright 2001, 2002 Paulo Soares
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
	 * Instance of PdfReader in each output document.
	 *
	 * @author Paulo Soares (psoares@consiste.pt)
	 */
	public class PdfReaderInstance {
		internal static PdfLiteral IDENTITYMATRIX = new PdfLiteral("[1 0 0 1 0 0]");
		internal static PdfNumber ONE = new PdfNumber(1);
		PdfObject[] xrefObj;
		PdfDictionary[] pages;
		int[] myXref; 
		PdfReader reader;
		RandomAccessFileOrArray file;
		Hashmap importedPages = new Hashmap();
		PdfWriter writer;
		Hashmap visited = new Hashmap();
		ArrayList nextRound = new ArrayList();

		internal PdfReaderInstance(PdfReader reader, PdfWriter writer, PdfObject[] xrefObj, PdfDictionary[] pages) {
			this.reader = reader;
			this.xrefObj = xrefObj;
			this.pages = pages;
			this.writer = writer;
			file = reader.SafeFile;
			myXref = new int[xrefObj.Length];
		}

		internal PdfReader Reader {
			get {
				return reader;
			}
		}
    
		internal PdfImportedPage getImportedPage(int pageNumber) {
			if (pageNumber < 1 || pageNumber > pages.Length)
				throw new IllegalArgumentException("Invalid page number");
			int i = pageNumber;
			PdfImportedPage pageT = (PdfImportedPage)importedPages[i];
			if (pageT == null) {
				pageT = new PdfImportedPage(this, writer, pageNumber);
				importedPages.Add(i, pageT);
			}
			return pageT;
		}
    
		internal int getNewObjectNumber(int number, int generation) {
			if (myXref[number] == 0) {
				myXref[number] = writer.IndirectReferenceNumber;
				nextRound.Add(number);
			}
			return myXref[number];
		}
    
		internal RandomAccessFileOrArray ReaderFile {
			get {
				return file;
			}
		}
    
		internal PdfObject getResources(int pageNumber) {
			return reader.getPdfObject(pages[pageNumber - 1].get(PdfName.RESOURCES));
		}
    
		internal PdfStream getFormXObject(int pageNumber) {
			PdfDictionary page = pages[pageNumber - 1];
			PdfObject contents = reader.getPdfObject(page.get(PdfName.CONTENTS));
			int length = 0;
			int offset = 0;
			PdfDictionary dic = new PdfDictionary();
			MemoryStream bout = null;
			ArrayList filters = null;
			if (contents != null) {
				if (contents.Type == PdfObject.STREAM) {
					PRStream stream = (PRStream)contents;
					length = stream.Length;
					offset = stream.Offset;
					dic.putAll(stream);
				}
				else {
					PdfArray array = (PdfArray)contents;
					ArrayList list = array.ArrayList;
					bout = new MemoryStream();
					for (int k = 0; k < list.Count; ++k) {
						PRStream stream = (PRStream)reader.getPdfObject((PdfObject)list[k]);
						PdfObject filter = stream.get(PdfName.FILTER);
						byte[] b = new byte[stream.Length];
						file.seek(stream.Offset);
						file.readFully(b);
						filters = new ArrayList();
						if (filter != null) {
							if (filter.Type == PdfObject.NAME) {
								filters.Add(filter);
							}
							else if (filter.Type == PdfObject.ARRAY) {
								filters = ((PdfArray)filter).ArrayList;
							}
						}
						string name;
						for (int j = 0; j < filters.Count; ++j) {
							name = ((PdfName)filters[j]).ToString();
							if (name.Equals("/FlateDecode") || name.Equals("/Fl"))
								b = PdfReader.FlateDecode(b);
							else if (name.Equals("/ASCIIHexDecode") || name.Equals("/AHx"))
								b = PdfReader.ASCIIHexDecode(b);
							else if (name.Equals("/ASCII85Decode") || name.Equals("/A85"))
								b = PdfReader.ASCII85Decode(b);
							else if (name.Equals("/LZWDecode"))
								b = PdfReader.LZWDecode(b);
							else
								throw new IOException("The filter " + name + " is not supported.");
						}
						bout.Write(b, 0, b.Length);
						if (k != list.Count - 1)
							bout.WriteByte((byte)'\n');
					}
				}
			}
			dic.put(PdfName.RESOURCES, reader.getPdfObject(page.get(PdfName.RESOURCES)));
			dic.put(PdfName.TYPE, PdfName.XOBJECT);
			dic.put(PdfName.SUBTYPE, PdfName.FORM);
			dic.put(PdfName.BBOX, new PdfRectangle(((PdfImportedPage)importedPages[pageNumber]).BoundingBox));
			dic.put(PdfName.MATRIX, IDENTITYMATRIX);
			dic.put(PdfName.FORMTYPE, ONE);
			PRStream str;
			if (bout == null) {
				str = new PRStream(reader, offset);
				str.putAll(dic);
				str.Length = length;
			}
			else {
				str = new PRStream(reader, bout.ToArray());
				str.putAll(dic);
			}
			return str;
		}
    
		internal void writeAllVisited() {
			while (nextRound.Count > 0) {
				ArrayList vec = nextRound;
				nextRound = new ArrayList();
				for (int k = 0; k < vec.Count; ++k) {
					int i = (int)vec[k];
					if (!visited.ContainsKey(i)) {
						visited.Add(i, null);
						int n = i;
						writer.addToBody(xrefObj[n], myXref[n]);
					}
				}
			}
		}
    
		internal void writeAllPages() {
			try {
				file.reOpen();
				foreach(PdfImportedPage ip in importedPages.Values) {
					writer.addToBody(ip.FormXObject, ip.IndirectReference);
				}
				writeAllVisited();
			}
			finally {
				try {
					file.close();
				}
				catch (Exception e) {
					e.GetType();
					//Empty on purpose
				}
			}
		}
	}
}