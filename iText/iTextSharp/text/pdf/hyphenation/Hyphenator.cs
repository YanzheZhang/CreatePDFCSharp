using System;
using System.IO;
using System.Collections;

/*
 * $Id: Hyphenator.cs,v 1.1.1.1 2003/02/04 02:58:43 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This class is the main entry point to the hyphenation package.
	 * You can use only the static methods or create an instance.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public class Hyphenator {
		static Hashtable hyphenTrees = new Hashtable();

		private HyphenationTree hyphenTree = null;
		private int remainCharCount = 2;
		private int pushCharCount = 2;
		private static bool errorDump = false;
   
		/** Holds value of property hyphenDir. */
		private static string hyphenDir = "";    

		public Hyphenator(string lang, string country, int leftMin,
			int rightMin) {
			hyphenTree = getHyphenationTree(lang, country);
			remainCharCount = leftMin;
			pushCharCount = rightMin;
		}

		public static HyphenationTree getHyphenationTree(string lang,
			string country) {
			string key = lang;
			// check whether the country code has been used
			if (country != null &&!country.Equals("none"))
				key += "_" + country;
			// first try to find it in the cache
			if (hyphenTrees.ContainsKey(key))
				return (HyphenationTree)hyphenTrees[key];
			if (hyphenTrees.ContainsKey(lang))
				return (HyphenationTree)hyphenTrees[lang];

			HyphenationTree hTree = getFopHyphenationTree(key);
			if (hTree == null) {
				//string hyphenDir = "e:\\winprog2\\Fop-0.20.2\\hyph";
				//Configuration.getstringValue("hyphenation-dir");
				if (hyphenDir != null) {
					hTree = getUserHyphenationTree(key, hyphenDir);
				}
			}
			// put it into the pattern cache
			if (hTree != null) {
				hyphenTrees.Add(key, hTree);
			} else {
				Console.Error.WriteLine("Couldn't find hyphenation pattern "
					+ key);
			}
			return hTree;
		}

		private static Stream getResourceStream(string key) {
			return null;
		}

		public static HyphenationTree getFopHyphenationTree(string key) {
			HyphenationTree hTree = null;
			Stream istr = null;
			try {
				istr = getResourceStream(key);
				if (istr == null) {
					if (key.Length == 5) {
						istr = getResourceStream(key.Substring(0, 2));
						if (istr != null) {
							Console.Error.WriteLine("Couldn't find hyphenation pattern  "
								+ key
								+ "\nusing general language pattern "
								+ key.Substring(0, 2)
								+ " instead.");
						} else {
							if (errorDump) {
								Console.Error.WriteLine("Couldn't find precompiled "
									+ "hyphenation pattern "
									+ key + ".hyp");
							}
							return null;
						}
					} else {
						if (errorDump) {
							Console.Error.WriteLine("Couldn't find precompiled "
								+ "hyphenation pattern "
								+ key + ".hyp");
						}
						return null;
					}
				}
				hTree = new HyphenationTree();
				hTree.loadInternalPatterns(istr);
			} catch (Exception e) {
				Console.Error.WriteLine(e.StackTrace);
			}
			finally {
				if (istr != null) {
					try {
						istr.Close();
					} catch (IOException e) {
						e.GetType();
						Console.Error.WriteLine("can't close hyphenation stream");
					}
				}
			}
			return hTree;
		}

		/**
		 * load tree from serialized file or xml file
		 * using configuration settings
		 */
		public static HyphenationTree getUserHyphenationTree(string key,
			string hyphenDir) {
//			HyphenationTree hTree = null;
//			// I use here the following convention. The file name specified in
//			// the configuration is taken as the base name. First we try
//			// name + ".hyp" assuming a serialized HyphenationTree. If that fails
//			// we try name + ".xml", assumming a raw hyphenation pattern file.
//
//			// first try serialized object
//			FileInfo hyphenFile = new FileInfo(hyphenDir + key + ".hyp");
//			if (hyphenFile.Exists) {
//				ObjectStream ois = null;
//				try {
//					ois = new ObjectStream(new FileStream(hyphenFile));
//					hTree = (HyphenationTree)ois.readObject();
//				} catch (Exception e) {
//					Console.Error.WriteLine(e.StackTrace);
//				}
//				finally {
//					if (ois != null) {
//						try {
//							ois.Close();
//						} catch (IOException e) {}
//					}
//				}
//				return hTree;
//			} else {
//
//				// try the file
//				hyphenFile = new File(hyphenDir, key + ".xml");
//				if (hyphenFile.exists()) {
//					hTree = new HyphenationTree();
//					if (errorDump) {
//						Console.Error.WriteLine("reading " + hyphenDir + key
//							+ ".hyp");
//					}
//					try {
//						hTree.loadInternalPatterns(hyphenFile.getPath());
//						if (errorDump) {
//							Console.Error.WriteLine("Stats: ");
//							hTree.printStats();
//						}
//						return hTree;
//					} catch (HyphenationException ex) {
//						if (errorDump) {
//							Console.Error.WriteLine("Can't load user patterns "
//								+ "from file " + hyphenDir
//								+ key + ".hyp");
//						}
//						return null;
//					}
//				} else {
//					if (errorDump) {
//						Console.Error.WriteLine("Tried to load "
//							+ hyphenFile.ToString()
//							+ "\nCannot find compiled nor xml file for "
//							+ "hyphenation pattern" + key);
//					}
//					return null;
//				}
//			}
			return null;
		}

		public static Hyphenation hyphenate(string lang, string country,
			string word, int leftMin,
			int rightMin) {
			HyphenationTree hTree = getHyphenationTree(lang, country);
			if (hTree == null) {
				Console.Error.WriteLine("Error building hyphenation tree for language "
					+ lang);
				return null;
			}
			return hTree.hyphenate(word, leftMin, rightMin);
		}

		public static Hyphenation hyphenate(string lang, string country,
			char[] word, int offset, int len,
			int leftMin, int rightMin) {
			HyphenationTree hTree = getHyphenationTree(lang, country);
			if (hTree == null) {
				Console.Error.WriteLine("Error building hyphenation tree for language "
					+ lang);
				return null;
			}
			return hTree.hyphenate(word, offset, len, leftMin, rightMin);
		}

		public int MinRemainCharCount {
			set {
				remainCharCount = value;
			}
		}

		public int MinPushCharCount {
			set {
				pushCharCount = value;
			}
		}

		public void setLanguage(string lang, string country) {
			hyphenTree = getHyphenationTree(lang, country);
		}

		public Hyphenation hyphenate(char[] word, int offset, int len) {
			if (hyphenTree == null)
				return null;
			return hyphenTree.hyphenate(word, offset, len, remainCharCount,
				pushCharCount);
		}

		public Hyphenation hyphenate(string word) {
			if (hyphenTree == null)
				return null;
			return hyphenTree.hyphenate(word, remainCharCount, pushCharCount);
		}

		/** Getter for property hyphenDir.
		 * @return Value of property hyphenDir.
		 */
		public static string HyphenDir {
			get {
				return hyphenDir;
			}

			set {
				hyphenDir = value;
			}
		}
    }
}
