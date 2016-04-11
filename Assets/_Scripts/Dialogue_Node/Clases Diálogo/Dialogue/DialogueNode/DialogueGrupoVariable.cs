using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class DialogueGrupoVariable {

		public int IDGrupo; // Grupo al que va dirigido el cambio de variable
		public int tipo; // 0 = suma el valor a la variable del grupo indicado
						 // 1 = establece el valor a la variable del grupo indicado
		public int num; //numero de variable
		public int valor;

		public DialogueGrupoVariable() {  }

		public int DevuelveIDGrupo()
		{
			return IDGrupo;
		}

		public int DevuelveTipo()
		{
			return tipo;
		}

		public int DevuelveNumero()
		{
			return num;
		}

		public int DevuelveValor()
		{
			return valor;
		}
	}
}
