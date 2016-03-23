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
		 * -2 : Va a las respuestas (NO SE DEBE USAR EN UN XML)
		 * -3: Acaba la conversación (NO SE DEBE USAR EN UN XML)
		 * Otro: Va al nodo indicado
	     */

        // parameterless constructor for serialization
        public DialogueOption() {
		}

		public string DevuelveTexto()
		{
			return Text;
		}

		public int DevuelveDestinationNodeID()
		{
			return DestinationNodeID;
		}

		//UTIL QUIZÁ EN EL FUTURO PARA EDITAR DIALOGOS DINAMICAMENTE
//        public DialogueOption(string text, int dest)
//        {
//            this.Text = text;
//            this.DestinationNodeID = dest;
//        }
    }
}
