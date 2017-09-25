using System;
using System.Collections;

/*
 * $Id: IPatternConsumer.cs,v 1.1.1.1 2003/02/04 02:58:43 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This interface is used to connect the XML pattern file parser to
	 * the hyphenation tree.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public interface IPatternConsumer {

		/**
		 * Add a character class.
		 * A character class defines characters that are considered
		 * equivalent for the purpose of hyphenation (e.g. "aA"). It
		 * usually means to ignore case.
		 */
		void addClass(string chargroup);

		/**
		 * Add a hyphenation exception. An exception replaces the
		 * result obtained by the algorithm for cases for which this
		 * fails or the user wants to provide his own hyphenation.
		 * A hyphenatedword is a vector of alternating string's and
		 * {@link Hyphen Hyphen} instances
		 */
		void addException(string word, ArrayList hyphenatedword);

		/**
		 * Add hyphenation patterns.
		 * @param pattern
		 * @param values interletter values expressed as a string of
		 * digit characters.
		 */
		void addPattern(string pattern, string values);
	}
}
