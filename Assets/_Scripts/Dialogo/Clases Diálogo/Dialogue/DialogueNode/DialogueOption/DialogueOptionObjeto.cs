using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class DialogueOptionObjeto
	{
		public int IDObjeto;

		//Si es verdadero, solo muestra la opción si en el inventario está el objeto especificado
		//Si es falso, solo muestra la opción si en el inventario no está el objeto especificado
		public bool enPosesion;

		public DialogueOptionObjeto() {  }
	}
}
