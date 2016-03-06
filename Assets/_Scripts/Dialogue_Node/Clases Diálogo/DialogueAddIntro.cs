using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddIntro
	{
		public int prioridad;
		public string NombreTexto {get; set;}

		public DialogueAddIntro() { }

		public int DevuelvePrioridad()
		{
			return prioridad;
		}

		public string DevuelveNombre()
		{
			return NombreTexto;
		}
	}
}