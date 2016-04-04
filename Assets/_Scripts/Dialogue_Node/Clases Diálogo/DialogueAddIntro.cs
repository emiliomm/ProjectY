using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddIntro
	{
		public int IDNpc; //-1 = NPC propio. x = NPC número x. ATENCIÓN: UTILIZAR -1 SIEMPRE QUE SEA PROPIO, NUNCA SU NÚMERO
		public int prioridad;
		public int IDIntro {get; set;}

		public DialogueAddIntro() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelvePrioridad()
		{
			return prioridad;
		}

		public int DevuelveID()
		{
			return IDIntro;
		}
	}
}