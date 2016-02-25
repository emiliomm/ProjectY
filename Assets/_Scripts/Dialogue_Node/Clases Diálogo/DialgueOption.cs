using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DialogueTree
{
    public class DialogueOption
    {
        public string Text;
        public int DestinationNodeID;
		/* 
		 * Valor
		 * -1 : Acaba dialogo cargado
		 * -2 : Va a las respuestas
		 * -3: Acaba la conversación
		 * Otro: Va al nodo indicado
	     */

		public List<DialogoTexto> AddDialogo;
		public List<PreguntaTexto> AddPregunta;

        // parameterless constructor for serialization
        public DialogueOption() {
			AddDialogo = new List<DialogoTexto>();
			AddPregunta = new List<PreguntaTexto>();
		}

		//UTIL QUIZÁ EN EL FUTURO PARA EDITAR DIALOGOS DINAMICAMENTE
//        public DialogueOption(string text, int dest)
//        {
//            this.Text = text;
//            this.DestinationNodeID = dest;
//        }
    }
}
