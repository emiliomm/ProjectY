using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using UnityEngine;

namespace DialogueTree
{
	public class Dialogue
    {
        public List<DialogueNode> Nodes;

		//Para serialización
		public Dialogue()
		{
			Nodes = new List<DialogueNode>();
		}

		public DialogueNode DevuelveNodo(int node_id)
		{
			return Nodes[node_id];
		}

		public void MarcarRecorrido(int node_id)
		{
			Nodes[node_id].MarcarRecorrido();
		}
    }
}

