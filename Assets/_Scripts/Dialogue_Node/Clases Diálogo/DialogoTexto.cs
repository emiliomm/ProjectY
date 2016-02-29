using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogoTexto
	{
		public int Valor;
		private string nombreTexto {get; set;}

		public DialogoTexto() { }

//		public string DevuelveNombre()
//		{
//			return nombreTexto;
//		}
//
//		public bool ConvertirATexto()
//		{
//			string fileName = "Assets/_Data/dialoguecodes.txt";
//			// Handle any problems that might arise when reading the text
//			try {
//				string line;
//				// Create a new StreamReader, tell it which file to read and what encoding the file
//				// was saved as
//				StreamReader theReader = new StreamReader (fileName, Encoding.Default);
//
//				// Immediately clean up the reader after this block of code is done.
//				// You generally use the "using" statement for potentially memory-intensive objects
//				// instead of relying on garbage collection.
//				// (Do not confuse this with the using directive for namespace at the 
//				// beginning of a class!)
//				using (theReader) {
//					var i = 0;
//					// While there's lines left in the text file, do this:
//					do {
//						line = theReader.ReadLine ();
//
//						if (line != null && i == this.Valor) {
//							// Do whatever you need to do with the text line, it's a string now
//							nombreTexto = line;
//						}
//						i++;
//					} while (line != null);
//
//					// Done reading, close the reader and return true to broadcast success    
//					theReader.Close ();
//					return true;
//				}
//			}
//			// If anything broke in the try block, we throw an exception with information
//			// on what didn't work
//			catch (Exception e) {
//				Console.WriteLine ("{0}\n", e.Message);
//				return false;
//			}
//		}
	}
}