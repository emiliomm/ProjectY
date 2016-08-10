using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase con los datos de una opción en el diálogo
	*/
	public class DialogueOption
    {
        public string Text; //Texto de la opción

		/* 
		 * Valor
		 * -1 : Acaba dialogo cargado
		 * -2 : Va a las respuestas (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * -3: Acaba la conversación (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * Otro: Va al nodo indicado
	     */
        public int DestinationNodeID;

		//Guarda variables de grupo que determinan si mostrar una opción o no
		public DialogueOptionGrupo Grupo;

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

		public int DevuelveNumeroGrupo()
		{
			return Grupo.IDGrupo;
		}

		public List<DialogueOptionGrupoVariables> DevuelveVariables()
		{
			return Grupo.variables;
		}
    }
}
