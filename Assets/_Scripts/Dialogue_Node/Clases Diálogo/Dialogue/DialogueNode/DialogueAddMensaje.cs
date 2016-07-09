using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddMensaje
	{
		public int IDNpc; //NPC al que se le va a añadir el mensaje
		public int IDDialogo; //Dialogo del npc indicado arriba al que se la a añadir el mensaje

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