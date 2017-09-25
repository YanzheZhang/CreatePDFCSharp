using System;
using System.Collections;

/*
 * $Id: PdfPages.cs,v 1.1.1.1 2003/02/04 02:57:36 geraldhenson Exp $
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
 * <CODE>PdfPages</CODE> is the PDF Pages-object.
 * <P>
 * The Pages of a document are accessible through a tree of nodes known as the Pages tree.
 * This tree defines the ordering of the pages in the document.<BR>
 * This object is described in the 'Portable Document Format Reference Manual version 1.3'
 * section 6.3 (page 71-73)
 *
 * @see		PdfPageElement
 * @see		PdfPage
 */

public class PdfPages : PdfDictionary, IPdfPageElement {
    
    // membervariables
    
/** value of the <B>Count</B>-key */
    private PdfNumber count;
    
/** value of the <B>Kids</B>-key */
    private PdfArray kids;
    
/** array of objects of the type <CODE>PdfPageElement</CODE> (contains the actual Pages tree) */
    private ArrayList pages = new ArrayList();
    
    // constructors
    
/**
 * Constructs a <CODE>PdfPages</CODE>-object.
 */
    
    internal PdfPages() : base(PAGES) {
        count = new PdfNumber(0);
        kids = new PdfArray();
        put(PdfName.COUNT, count);
        put(PdfName.KIDS, kids);
    }
    
    // implementation of the PdfPageElement interface
    
/**
 * Set the value for the <B>Parent</B> key in the Page or Pages Dictionary.
 *
 * @param		reference			an indirect reference to a <CODE>PdfPages</CODE>-object
 */
    
    public PdfIndirectReference Parent {
		set {
			put(PdfName.PARENT, value);
		}
    }
    
/**
 * Checks if this page element is a tree of pages.
 * <P>
 * This method allways returns <CODE>true</CODE>.
 *
 * @return	<CODE>true</CODE> because this object is a tree of pages
 */
    
    public bool isParent() {
        return true;
    }
    
    // methods
    
/**
 * Adds a <CODE>PdfPages</CODE>-object.
 *
 * @param		pages		a <CODE>PdfPages</CODE>-object
 */
    
    internal void Add(PdfPages pages) {
        pages.Add(pages);
    }
    
/**
 * Adds a <CODE>PdfPage</CODE>-object.
 *
 * @param		page		a <CODE>PdfPage</CODE>-object
 */
    
    internal void Add(PdfPage page) {
        pages.Add(page);
    }
    
/**
 * Updates the array of kids.
 *
 * @param		kid			an indirect reference to a <CODE>PdfPageElement</CODE>-object
 */
    
    internal void Add(PdfIndirectReference kid) {
        count.increment();
        kids.Add(kid);
    }
    
/**
 * Returns an <CODE>Iterator</CODE> with all the leafs of this Pages-object.
 *
 * @return		an <CODE>Iterator</CODE>
 */
    
    internal IEnumerator iterator() {
        return pages.GetEnumerator();
    }
    
    internal int reorderPages(int[] order) {
        if (order == null)
            return kids.Size;
        if (order.Length != kids.Size)
            throw new DocumentException("Page reordering requires and array with the same size as the number of pages.");
        int max = kids.Size;
        bool[] temp = new bool[max];
        for (int k = 0; k < max; ++k) {
            int p = order[k];
            if (p < 1 || p > max)
                throw new DocumentException("Page reordering requires pages between 1 and " + max + ". Found " + p + ".");
            if (temp[p - 1])
                throw new DocumentException("Page reordering requires no page repetition. Page " + p + " is repeated.");
            temp[p - 1] = true;
        }
        ArrayList array = kids.ArrayList;
        Object[] copy = array.ToArray();
        for (int k = 0; k < max; ++k) {
            array[k] = copy[order[k] - 1];
        }
        return max;
    }
}
}