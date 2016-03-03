using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddIntro
	{
		public string NombreTexto {get; set;}

		public DialogueAddIntro() { }

		public string DevuelveNombre()
		{
			return NombreTexto;
		}
	}
}