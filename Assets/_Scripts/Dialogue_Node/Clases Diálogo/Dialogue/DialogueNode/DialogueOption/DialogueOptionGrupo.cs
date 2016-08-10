using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase que contiene los datos de grupo de una opción del dialogo
	*/
	public class DialogueOptionGrupo
	{
		//-1: ningún grupo, otro: grupo
		public int IDGrupo; //indica a que grupo pertenecen las variables
		public List<DialogueOptionGrupoVariables> variables;

		public DialogueOptionGrupo() {
			variables = new List<DialogueOptionGrupoVariables>();
		}

	}
}
