using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using UnityEngine;

namespace DialogueTree
{
	public class Dialogue
    {
        public List<DialogueNode> Nodes;
		public bool Autodestruye; // 0 --> falso, 1 --> verdadero

		//UTIL QUIZÁ EN EL FUTURO PARA EDITAR DIALOGOS DINAMICAMENTE

//        public void AddNode(DialogueNode node)
//        {
//            // if the node is null, then it's an ExitNode and we can skip adding it.
//            if (node == null) return;
//
//            // add the node to the dialog list of nodes
//            Nodes.Add(node);
//            //give the node an ID
//            node.NodeID = Nodes.IndexOf(node);
//        }
//
//        public void AddOption(string text, DialogueNode node, DialogueNode dest)
//        {
//            // add the destination node to the dialogue if it's not already there
//            if(!Nodes.Contains(dest))
//                AddNode(dest);
//
//            // add the parent node to the dialogue if it's not already there
//            if (!Nodes.Contains(node))
//                AddNode(node);
//
//            DialogueOption opt;
//
//            // create an option object. If the destination is an ExitNode, set the index to -1
//            if (dest == null)
//                opt = new DialogueOption(text, -1);
//            else
//                opt = new DialogueOption(text, dest.NodeID);
//
//            node.Options.Add(opt);
//        }

		//Para serialización
        public Dialogue()
        {
            Nodes = new List<DialogueNode>();
        }

		public static Dialogue LoadDialogue(string path)
		{
			XmlSerializer serz = new XmlSerializer(typeof(Dialogue));
			StreamReader reader = new StreamReader(path);

			Dialogue dia = (Dialogue)serz.Deserialize(reader);

			if (path == "Assets/_Texts/text_dia.xml")
				Debug.Log(dia.Nodes[1].Options[0].AddPregunta[0]);

			//Añadir funcion para transformar los numeros en strings
			dia.RecorrerDialogo();

			return dia;
		}

		//Transforma los enteros de AddDialogo y AddPregunta a strings con el nombre del archivo de texto
		private void RecorrerDialogo()
		{
			for(int i = 0; i < Nodes.Count; i++)
			{
				for(int j = 0; j < Nodes[i].Options.Count; j++)
				{
					for(int k = 0; k < Nodes[i].Options[j].AddDialogo.Count; k++)
					{
						ConvertirATexto(Nodes[i].Options[j].AddDialogo[k].Valor);
					}
				}
			}
		}

		private void ConvertirATexto(int valor)
		{
			string fileName = "Assets/_Data/dialoguecodes.txt";
			// Handle any problems that might arise when reading the text
			try
			{
				string line;
				// Create a new StreamReader, tell it which file to read and what encoding the file
				// was saved as
				StreamReader theReader = new StreamReader(fileName, Encoding.Default);

				// Immediately clean up the reader after this block of code is done.
				// You generally use the "using" statement for potentially memory-intensive objects
				// instead of relying on garbage collection.
				// (Do not confuse this with the using directive for namespace at the 
				// beginning of a class!)
				using (theReader)
				{
					var i = 0;
					// While there's lines left in the text file, do this:
					do
					{
						line = theReader.ReadLine();

						if (line != null && i == valor)
						{
							// Do whatever you need to do with the text line, it's a string now
							//dia.
						}
						i++;
					}
					while (line != null);

					// Done reading, close the reader and return true to broadcast success    
					theReader.Close();
					return true;
				}
			}

			// If anything broke in the try block, we throw an exception with information
			// on what didn't work
			catch (Exception e)
			{
				Console.WriteLine("{0}\n", e.Message);
				return false;
			}
		}
    }
}