using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class DialogueGrupo
	{
		public int IDGrupo;
		public bool tipo; //verdadero(1) = añade grupo, falso(0) = elimina el grupo

		DialogueGrupo() { }

		public int DevuelveID()
		{
			return IDGrupo;
		}

		public bool DevuelveTipo()
		{
			return tipo;
		}
	}
}
