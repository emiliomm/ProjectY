﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddMensaje
	{
		public int IDNpc; //-1, NPC propio. x, NPC número x 
		public int IDMensaje {get; set;}

		public DialogueAddMensaje() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveID()
		{
			return IDMensaje;
		}
	}
}