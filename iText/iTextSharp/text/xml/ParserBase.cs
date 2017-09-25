using System;
using System.Xml;
using System.Collections;

namespace iTextSharp.text.xml
{
	/// <summary>
	/// The <CODE>ParserBase</CODE>-class provides XML document parsing.
	/// </summary>
	public abstract class ParserBase
	{
		/// <summary>
		/// Begins the process of processing an XML document
		/// </summary>
		/// <param name="url">the XML document to parse</param>
		public void Parse(string url) {
			XmlTextReader reader = null;
			try {
				reader = new XmlTextReader(url);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							string namespaceURI = reader.NamespaceURI;
							string name = reader.Name;
							bool isEmpty = reader.IsEmptyElement;
							Hashtable attributes = new Hashtable();
							if (reader.HasAttributes) {
								for (int i = 0; i < reader.AttributeCount; i++) {
									reader.MoveToAttribute(i);
									attributes.Add(reader.Name,reader.Value);
								}
							}
							this.startElement(namespaceURI, name, name, attributes);
							if (isEmpty) {
								endElement(namespaceURI,
									name, name);
							}
							break;
						case XmlNodeType.EndElement:
							endElement(reader.NamespaceURI,
								reader.Name, reader.Name);
							break;
						case XmlNodeType.Text:
							characters(reader.Value, 0, reader.Value.Length);
							break;
							// There are many other types of nodes, but
							// we are not interested in them
					}
				}
			} catch (XmlException e) {
				Console.WriteLine(e.Message);
			} finally {
				if (reader != null) {
					reader.Close();
				}
			}
		}

		/// <summary>
		/// This method gets called when a start tag is encountered.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="lname"></param>
		/// <param name="name">the name of the tag that is encountered</param>
		/// <param name="attrs">the list of attributes</param>
		public abstract void startElement(String uri, String lname, String name, Hashtable attrs);

		/// <summary>
		/// This method gets called when an end tag is encountered.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="lname"></param>
		/// <param name="name">the name of the tag that ends</param>
		public abstract void endElement(String uri, String lname, String name);

		/// <summary>
		/// This method gets called when characters are encountered.
		/// </summary>
		/// <param name="content">an array of characters</param>
		/// <param name="start">the start position in the array</param>
		/// <param name="length">the number of characters to read from the array</param>
		public abstract void characters(string content, int start, int length);
	}
}
