using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddMensaje
	{
		public int IDNpc; //-1 = NPC propio. x = NPC número x. ATENCIÓN: UTILIZAR -1 SIEMPRE QUE SEA EL MENSAJE VAYA DIRIGIDO AL NPC PROPIO NUNCA SU NÚMERO

		//-1 = NPC propio. x = NPC número x. ATENCIÓN: UTILIZAR -1 SIEMPRE QUE SEA PROPIO, NUNCA SU NÚMERO
		//Dialogo del npc indicado arriba al que se la a añadir la intro
		public int IDDialogo;

		public int IDTema; //-1: el mensaje no forma parte de ningún tema, va por separado. x = el mensaje se agrupa en el tema x
		public int IDMensaje {get; set;}

		public DialogueAddMensaje() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIDDialogo()
		{
			return IDDialogo;
		}

		public int DevuelveIDTema()
		{
			return IDTema;
		}

		public int DevuelveID()
		{
			return IDMensaje;
		}
	}
}