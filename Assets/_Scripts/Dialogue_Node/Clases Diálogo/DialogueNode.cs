using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DialogueTree
{
    public class DialogueNode
    {
        public int NodeID = -1;

        public string Text;
		public bool recorrido; //Indica si el nodo ha sido recorrido anteriormente
        public List<DialogueOption> Options;
		public List<DialogoTexto> AddDialogo;
		public List<PreguntaTexto> AddPregunta;

        // parameterless constructor for serialization
        public DialogueNode()
        {
            Options = new List<DialogueOption>();
			AddDialogo = new List<DialogoTexto>();
			AddPregunta = new List<PreguntaTexto>();
			recorrido = false;
        }

		public string DevuelveTexto()
		{
			return Text;
		}

		public int DevuelveNumeroOpciones()
		{
			return Options.Count;
		}

		public DialogueOption DevuelveNodoOpciones(int node_id)
		{
			return Options[node_id];
		}

		//UTIL QUIZÁ EN EL FUTURO PARA EDITAR DIALOGOS DINAMICAMENTE
//        public DialogueNode(string text)
//        {
//            Text = text;
//            Options = new List<DialogueOption>();
//        }
    }
}