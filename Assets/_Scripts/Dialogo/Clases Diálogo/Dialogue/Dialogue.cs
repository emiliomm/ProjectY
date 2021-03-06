﻿using System.Collections.Generic;
using UnityEngine;

namespace DialogueTree
{
	/*
	 * Clase que contiene un díalogo que forma pare de una conversación
	*/
	public class Dialogue
    {
        public List<DialogueNode> nodes;

		public Dialogue()
		{
			nodes = new List<DialogueNode>();
		}

		//Devuelve un objeto DialogueNode según la posición en la que se encuentre en la lista de nodos
		public DialogueNode DevuelveNodo(int pos)
		{
			return nodes[pos];
		}

		//Comprueba si existen más nodos delante de la posición indicada, comprobando si se puede continuar con el diálogo
		//Devuelve true si existen más nodos delante y false si no
		public bool AvanzaDialogue(int pos)
		{
			bool avanza = false;

			if (pos + 1 < nodes.Count)
				avanza = true;

			return avanza;
		}
    }
}

