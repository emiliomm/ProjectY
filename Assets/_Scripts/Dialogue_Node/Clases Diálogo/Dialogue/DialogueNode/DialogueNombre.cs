using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class DialogueNombre
	{
		public int IDNpc;
		public int indiceNombre;

		public DialogueNombre() {  }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIndiceNombre()
		{
			return indiceNombre;
		}
	}
}
