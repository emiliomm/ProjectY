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

        // parameterless constructor for serialization
        public DialogueOption() { }

        public DialogueOption(string text, int dest)
        {
            this.Text = text;
            this.DestinationNodeID = dest;
        }
    }
}
