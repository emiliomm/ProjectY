using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DialogueTree
{
	public class DialogueOptionGrupo
	{
		public int IDGrupo; //-1: ningún grupo, otro: grupo
		public List<DialogueOptionGrupoVariables> variables;

		public DialogueOptionGrupo() {
			variables = new List<DialogueOptionGrupoVariables>();
		}

	}
}
