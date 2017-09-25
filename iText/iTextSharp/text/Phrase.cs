using System;
using System.Text;
using System.Collections;
using System.util;
using System.Runtime.CompilerServices;

using iTextSharp.text.markup;

namespace iTextSharp.text {
	/// <summary>
	/// A Phrase is a series of Chunks.
	/// </summary>
	/// <remarks>
	/// A Phrase has a main Font, but some chunks
	/// within the phrase can have a Font that differs from the
	/// main Font. All the Chunks in a Phrase
	/// have the same leading.
	/// </remarks>
	/// <example>
	/// <code>
	/// // When no parameters are passed, the default leading = 16
	/// <strong>Phrase phrase0 = new Phrase();
	/// Phrase phrase1 = new Phrase("this is a phrase");</strong>
	/// // In this example the leading is passed as a parameter
	/// <strong>Phrase phrase2 = new Phrase(16, "this is a phrase with leading 16");</strong>
	/// // When a Font is passed (explicitely or embedded in a chunk), the default leading = 1.5 * size of the font
	/// <strong>Phrase phrase3 = new Phrase("this is a phrase with a red, normal font Courier, size 12", FontFactory.getFont(FontFactory.COURIER, 12, Font.NORMAL, new Color(255, 0, 0)));
	/// Phrase phrase4 = new Phrase(new Chunk("this is a phrase"));
	/// Phrase phrase5 = new Phrase(18, new Chunk("this is a phrase", FontFactory.getFont(FontFactory.HELVETICA, 16, Font.BOLD, new Color(255, 0, 0)));</strong>
	/// </code>
	/// </example>
	public class Phrase : ArrayList, ITextElementArray, IMarkupAttributes {
    
		// membervariables
    
		/// <summary>This is the leading of this phrase.</summary>
		protected Single leading = Single.NaN;
    
		///<summary> This is the font of this phrase. </summary>
		protected Font font = new Font();

		///<summary> Contains extra markupAttributes </summary>
		protected Properties markupAttributes;
    
		// constructors
    
		/// <summary>
		/// Constructs a Phrase without specifying a leading.
		/// </summary>
		/// <overloads>
		/// Has nine overloads.
		/// </overloads>
		public Phrase() : this(16) {}
    
		/// <summary>
		/// Constructs a Phrase with a certain leading.
		/// </summary>
		/// <param name="leading">the leading</param>
		public Phrase(float leading) {
			this.leading = leading;
		}
    
		/// <summary>
		/// Constructs a Phrase with a certain Chunk.
		/// </summary>
		/// <param name="chunk">a Chunk</param>
		public Phrase(Chunk chunk) {
			base.Add(chunk);
		}
    
		/// <summary>
		/// Constructs a Phrase with a certain Chunk and a certain leading.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="chunk">a Chunk</param>
		public Phrase(float leading, Chunk chunk) : this(leading) {
			base.Add(chunk);
		}
    
		/// <summary>
		/// Constructs a Phrase with a certain string.
		/// </summary>
		/// <param name="str">a string</param>
		public Phrase(string str) : this(float.NaN, str, new Font()) {}
    
		/// <summary>
		/// Constructs a Phrase with a certain string and a certain Font.
		/// </summary>
		/// <param name="str">a string</param>
		/// <param name="font">a Font</param>
		public Phrase(string str, Font font) : this(float.NaN, str, font) {
			this.font = font;
		}
    
		/// <summary>
		/// Constructs a Phrase with a certain leading and a certain string.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="str">a string</param>
		public Phrase(float leading, string str) : this(leading, str, new Font()) {}
    
		/// <summary>
		/// Constructs a Phrase with a certain leading, a certain string
		/// and a certain Font.
		/// </summary>
		/// <param name="leading">the leading</param>
		/// <param name="str">a string</param>
		/// <param name="font">a Font</param>
		public Phrase(float leading, string str, Font font) : this(leading) {
			this.font = font;
			if (Font.Family != Font.SYMBOL && Font.Family != Font.ZAPFDINGBATS && Font.BaseFont == null) {
				int index;
				while((index = Greek.index(str)) > -1) {
					if (index > 0) {
						string firstPart = str.Substring(0, index);
						/* bugfix [ #461272 ] CODE CHANGE REQUIRED IN Phrase.java
										   by Arekh Nambiar */
						base.Add(new Chunk(firstPart, font));
						str = str.Substring(index);
					}
					Font symbol = new Font(Font.SYMBOL, Font.Size, Font.Style, Font.Color);
					StringBuilder buf = new StringBuilder();
					buf.Append(Greek.getCorrespondingSymbol(str[0]));
					str = str.Substring(1);
					while (Greek.index(str) == 0) {
						buf.Append(Greek.getCorrespondingSymbol(str[0]));
						str = str.Substring(1);
					}
					base.Add(new Chunk(buf.ToString(), symbol));
				}
			}
			if (str.Length != 0) {
				base.Add(new Chunk(str, font));
			}
		}
    
		/// <summary>
		/// Returns a Phrase that has been constructed taking in account
		/// the value of some attributes.
		/// </summary>
		/// <param name="attributes">some attributes</param>
		public Phrase(Properties attributes) : this("", FontFactory.getFont(attributes)) {
			this.Clear();
			string value;
			if ((value = attributes.Remove(ElementTags.LEADING)) != null) {
				Leading= float.Parse(value);
			}
			else if ((value = attributes.Remove(MarkupTags.CSS_LINEHEIGHT)) != null) {
				Leading = MarkupParser.parseLength(value);
			}
			if ((value = attributes.Remove(ElementTags.ITEXT)) != null) {
				Chunk chunk = new Chunk(value);
				if ((value = attributes.Remove(ElementTags.GENERICTAG)) != null) {
					chunk.setGenericTag(value);
				}
				Add(chunk);
			}
			if (attributes.Count > 0) MarkupAttributes = attributes;
		}
    
		// implementation of the Element-methods
    
		/// <summary>
		/// Processes the element by adding it (or the different parts) to an
		/// <see cref="iTextSharp.text.IElementListener"/>.
		/// </summary>
		/// <param name="listener">an IElementListener</param>
		/// <returns>true if the element was processed successfully</returns>
		public virtual bool process(IElementListener listener) {
			try {
				foreach(IElement ele in this) {
					listener.Add(ele);
				}
				return true;
			}
			catch(DocumentException de) {
				de.GetType();
				return false;
			}
		}
    
		/// <summary>
		/// Gets the type of the text element.
		/// </summary>
		/// <value>a type</value>
		public virtual int Type {
			get {
				return Element.PHRASE;
			}
		}
    
		/// <summary>
		/// Gets all the chunks in this element.
		/// </summary>
		/// <value>an ArrayList</value>
		public virtual ArrayList Chunks {
			get {
				ArrayList tmp = new ArrayList();
				foreach(IElement ele in this) {
					tmp.AddRange(ele.Chunks);
				}
				return tmp;
			}
		}
    
		// overriding some of the ArrayList-methods
    
		/// <summary>
		/// Adds a Chunk, an Anchor or another Phrase
		/// to this Phrase.
		/// </summary>
		/// <param name="index">index at which the specified element is to be inserted</param>
		/// <param name="o">an object of type Chunk, Anchor, or Phrase</param>
		public void Add(int index, Object o) {
			try {
				IElement element = (IElement) o;
				if (element.Type == Element.CHUNK) {
					Chunk chunk = (Chunk)element;
					if (!font.isStandardFont()) {
						chunk.Font = font.difference(chunk.Font);
					}
					base[index] = chunk;
				}
				else if (element.Type == Element.PHRASE ||
					element.Type == Element.ANCHOR ||
					element.Type == Element.ANNOTATION ||
					element.Type == Element.TABLE || // line added by David Freels
					element.Type == Element.GRAPHIC) {
					base.Insert(index, element);
				}
				else {
					throw new Exception(element.Type.ToString());
				}
			}
			catch(Exception cce) {
				throw new Exception("Insertion of illegal Element: " + cce.Message);
			}
		}
    
		/// <summary>
		/// Adds a Chunk, Anchor or another Phrase
		/// to this Phrase.
		/// </summary>
		/// <param name="o">an object of type Chunk, Anchor or Phrase</param>
		/// <returns>a bool</returns>
		public virtual new bool Add(Object o) {
			if (o is string) {
				base.Add(new Chunk((string) o, font));
				return true;
			}
			try {
				IElement element = (IElement) o;
				switch(element.Type) {
					case Element.CHUNK:
						return addChunk((Chunk) o);
					case Element.PHRASE:
					case Element.PARAGRAPH:
						Phrase phrase = (Phrase) o;
						bool success = true;
						foreach(IElement e in phrase) {
							if (e is Chunk) {
								success &= addChunk((Chunk)e);
							}
							else {
								success &= this.Add(e);
							}
						}
						return success;
					case Element.ANCHOR:
						base.Add((Anchor) o);
						return true;
					case Element.ANNOTATION:
						base.Add((Annotation) o);
						return true;
					case Element.TABLE: // case added by David Freels
						base.Add((Table) o);
						return true;
					case Element.LIST:
						base.Add((List) o);
						return true;
					default:
						throw new Exception(element.Type.ToString());
				}
			}
			catch(Exception cce) {
				throw new Exception("Insertion of illegal Element: " + cce.Message);
			}
		}
    
		/// <summary>
		/// Adds a Chunk.
		/// </summary>
		/// <remarks>
		/// This method is a hack to solve a problem I had with phrases that were split between chunks
		/// in the wrong place.
		/// </remarks>
		/// <param name="chunk">a Chunk</param>
		/// <returns>a bool</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		private bool addChunk(Chunk chunk) {
			if (!font.isStandardFont()) {
				chunk.Font = font.difference(chunk.Font);
			}
			if (Count > 0 && !chunk.hasAttributes()) {
				try {
					Chunk previous = (Chunk) this[Count - 1];
					if (!previous.hasAttributes() && previous.Font.CompareTo(chunk.Font) == 0 && !"".Equals(previous.Content.Trim()) && !"".Equals(chunk.Content.Trim())) {
						previous.append(chunk.Content);
						return true;
					}
				}
				catch(Exception cce) {
					cce.GetType();
				}
			}
			base.Add(chunk);
			return true;
		}
    
		/// <summary>
		/// Adds a collection of Chunks
		/// to this Phrase.
		/// </summary>
		/// <param name="collection">a collection of Chunks, Anchors and Phrases.</param>
		/// <returns>true if the action succeeded, false if not.</returns>
		public bool addAll(ICollection collection) {
			foreach(object itm in collection) {
				this.Add(itm);
			}
			return true;
		}
    
		/// <summary>
		/// Adds a Object to the Paragraph.
		/// </summary>
		/// <param name="obj">the object to add.</param>
		public void addSpecial(Object obj) {
			base.Add(obj);
		}
    
		// methods
    
		// methods to retrieve information
    
		/// <summary>
		/// Checks is this Phrase contains no or 1 empty Chunk.
		/// </summary>
		/// <returns>
		/// false if the Phrase
		/// contains more than one or more non-emptyChunks.
		/// </returns>
		public bool isEmpty() {
			switch(Count) {
				case 0:
					return true;
				case 1:
					IElement element = (IElement) this[0];
					if (element.Type == Element.CHUNK && ((Chunk) element).isEmpty()) {
						return true;
					}
					return false;
				default:
					return false;
			}
		}
    
		/// <summary>
		/// Checks you if the leading of this phrase is defined.
		/// </summary>
		/// <value>true if the leading is defined</value>
		public bool LeadingDefined {
			get {
				if (float.IsNaN(leading)) {
					return false;
				}
				return true;
			}
		}
    
		/// <summary>
		/// Gets/sets the leading of this phrase.
		/// </summary>
		/// <value>the linespacing</value>
		public float Leading {
			get {
				if (float.IsNaN(leading)) {
					return font.leading(1.5f);
				}
				return leading;
			}

			set {
				this.leading = value;
			}
		}
    
		/// <summary>
		/// Gets the font of the first Chunk that appears in this Phrase.
		/// </summary>
		/// <value>a Font</value>
		public Font Font {
			get {
				return font;
			}
		}
    
		/// <summary>
		/// Checks if a given tag corresponds with this object.
		/// </summary>
		/// <param name="tag">the given tag</param>
		/// <returns>true if the tag corresponds</returns>
		public static bool isTag(string tag) {
			return ElementTags.PHRASE.Equals(tag);
		}
    
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.setMarkupAttribute(System.String,System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <param name="value">attribute value</param>
		public void setMarkupAttribute(string name, string value) {
			markupAttributes = (markupAttributes == null) ? new Properties() : markupAttributes;
			markupAttributes.Add(name, value);
		}
    
		/// <summary>
		/// See <see cref="M:iTextSharp.text.IMarkupAttributes.getMarkupAttribute(System.String)"/>
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>attribute value</returns>
		public string getMarkupAttribute(string name) {
			return (markupAttributes == null) ? null : markupAttributes[name].ToString();
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributeNames"/>
		/// </summary>
		/// <value>a collection of string attribute names</value>
		public ICollection MarkupAttributeNames {
			get {
				return Chunk.getKeySet(markupAttributes);
			}
		}
    
		/// <summary>
		/// See <see cref="P:iTextSharp.text.IMarkupAttributes.MarkupAttributes"/>
		/// </summary>
		/// <value>a Properties-object containing all the markupAttributes.</value>
		public Properties MarkupAttributes {
			get {
				return markupAttributes;
			}

			set {
				this.markupAttributes = value;
			}
		}
	}
}
