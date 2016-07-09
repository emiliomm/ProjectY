using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddIntro
	{
		public int IDNpc; //NPC al que se le va a añadir la intro
		public int IDDialogo; //Dialogo del npc indicado arriba al que se la a añadir la intro

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