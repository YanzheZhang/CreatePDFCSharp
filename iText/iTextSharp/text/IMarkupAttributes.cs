using System;
using System.Collections;
using System.util;

namespace iTextSharp.text {
	/// <summary>
	/// Defines the interface for an Element with markup attributes--
	/// that is, random String-to-String properties for representation in markup
	/// languages such as HTML and XML.
	/// </summary>
	public interface IMarkupAttributes : IElement {
		/// <summary>
		/// Sets the specified attribute.
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <param name="value">attribute value</param>
		void setMarkupAttribute(string name, string value);
    
		/// <summary>
		/// Returns the value of the specified attribute.
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>attribute value</returns>
		string getMarkupAttribute(string name);
    
		/// <summary>
		/// Returns a collection of string attribute names for the
		/// IMarkupAttributes implementor.
		/// </summary>
		ICollection MarkupAttributeNames {
			get;
		}
    
		/// <summary>
		/// a Properties-object containing all the markupAttributes.
		/// </summary>
		Properties MarkupAttributes {
			get;
			set;
		}
	}
}
