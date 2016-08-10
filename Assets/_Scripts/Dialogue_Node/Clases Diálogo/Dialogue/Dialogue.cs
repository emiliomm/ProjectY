using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase que contiene un díalogo que forma pare de una conversación
	*/
	public class Dialogue
    {
        public List<DialogueNode> Nodes;

		public Dialogue()
		{
			Nodes = new List<DialogueNode>();
		}

		//Devuelve un objeto DialogueNode según la posición de la lista indicada
		public DialogueNode DevuelveNodo(int pos)
		{
			return Nodes[pos];
		}

		//Comprueba si existen más nodos delante de la posición indicada, comprobando si se puede continuar con el diálogo
		//Devuelve true si existen más nodos delante y false si no
		public bool AvanzaDialogue(int pos)
		{
			bool avanza = false;

			if (pos + 1 < Nodes.Count)
				avanza = true;

			return avanza;
		}

		//Marca la variable recorrido a true, indicando que el nodo ya ha sido recorrido
		//y no se volverán a comprobar algunas de sus funciones si se vuelve a recorrer en el futuro
		public void MarcarRecorrido(int pos)
		{
			Nodes[pos].MarcarRecorrido();
		}
    }
}

