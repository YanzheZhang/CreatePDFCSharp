using System;

namespace iTextSharp.text.pdf {

	/**
	 * $Id: PdfPatternDictionary.cs,v 1.1.1.1 2003/02/04 02:57:36 geraldhenson Exp $ 
	 * <CODE>PdfPatternDictionary</CODE> is a <CODE>PdfResource</CODE>, containing a dictionary of <CODE>PdfSpotPattern</CODE>s.
	 *	at the moment
	 *
	 * @see		PdfResource
	 * @see		PdfResources
	 */

	public class PdfPatternDictionary : PdfDictionary, IPdfResource {

		// constructors

		/**
		* Constructs a new <CODE>PdfPatternDictionary</CODE>.
		*/

		internal PdfPatternDictionary() : base(){}

		// methods

		/**
		 * Returns the name of a resource.
		 *
		 * @return		a <CODE>PdfName</CODE>.
		 */

		public PdfName Key {
			get {
				return PdfName.PATTERN;
			}
		}

		/**
		 * Returns the object that represents the resource.
		 *
		 * @return		a <CODE>PdfObject</CODE>
		 */

		public PdfObject Value {
			get {
				return this;
			}
		}

		/**
		 * Checks if the <CODE>PdfPatternDictionary</CODE> contains at least
		 * one object.
		 *
		 * @return		<CODE>true</CODE> if an object was found
		 *				<CODE>false</CODE> otherwise
		 */

		internal bool containsPattern() {
			return hashMap.Count > 0;
		}
	}
}