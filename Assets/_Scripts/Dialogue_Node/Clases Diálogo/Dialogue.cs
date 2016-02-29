﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using UnityEngine;

namespace DialogueTree
{
	public class Dialogue
    {
		private string ruta;
		public bool Autodestruye; // 0 --> falso, 1 --> verdadero
        public List<DialogueNode> Nodes;

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

			//CORREGIR BUG
			Debug.Log(path);

			dia.ruta = path.Replace("Assets/_Texts", "_Data");

			//Añadir funcion para transformar los numeros en strings
//			dia.RecorrerDialogo();

			return dia;
		}

		public DialogueNode DevuelveNodo(int node_id)
		{
			return Nodes[node_id];
		}

//		public string DevolverRuta()
//		{
//			return ruta;
//		}
//
//		//Transforma los enteros de AddDialogo y AddPregunta a strings con el nombre del archivo de texto
//		private void RecorrerDialogo()
//		{
//			for(int i = 0; i < Nodes.Count; i++)
//			{
//				for(int j = 0; j < Nodes[i].AddDialogo.Count; j++)
//				{
//					Nodes[i].AddDialogo[j].ConvertirATexto();
//				}
//				for(int k = 0; k < Nodes[i].AddPregunta.Count; k++)
//				{
//					Nodes[i].AddPregunta[k].ConvertirATexto();
//				}
//			}
//		}
    }
}

