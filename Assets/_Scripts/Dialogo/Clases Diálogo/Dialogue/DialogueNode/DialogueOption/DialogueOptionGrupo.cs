using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase que contiene los datos de grupo de una opción del dialogo
	*/
	public class DialogueOptionGrupo
	{
		public int IDGrupo; //indica a que grupo pertenecen las variables
		public List<DialogueOptionGrupoVariables> variables;

		public DialogueOptionGrupo() {
			variables = new List<DialogueOptionGrupoVariables>();
		}

	}
}
