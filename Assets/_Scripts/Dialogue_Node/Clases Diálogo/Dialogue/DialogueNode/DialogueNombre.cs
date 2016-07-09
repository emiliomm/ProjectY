using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class DialogueNombre
	{
		public int IDNpc; //-1: ID propio, x: ID NPC
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
