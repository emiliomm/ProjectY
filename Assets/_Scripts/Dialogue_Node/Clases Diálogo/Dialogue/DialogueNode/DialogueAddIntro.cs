using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddIntro
	{
		//-1 = NPC propio. x = NPC número x. ATENCIÓN: UTILIZAR -1 SIEMPRE QUE SEA PROPIO, NUNCA SU NÚMERO
		//NPC al que se le va a añadir la intro
		public int IDNpc;

		//-1 = NPC propio. x = NPC número x. ATENCIÓN: UTILIZAR -1 SIEMPRE QUE SEA PROPIO, NUNCA SU NÚMERO
		//Dialogo del npc indicado arriba al que se la a añadir la intro
		public int IDDialogo;

		public int prioridad;
		public int IDIntro {get; set;}

		public DialogueAddIntro() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIDDialogo()
		{
			return IDDialogo;
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