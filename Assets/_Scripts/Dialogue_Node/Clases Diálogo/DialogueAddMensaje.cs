using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

namespace DialogueTree
{
	public class DialogueAddMensaje
	{
		public string Mensaje;
		public string NombreTexto {get; set;}

		public DialogueAddMensaje() { }

		public string DevuelveNombre()
		{
			return NombreTexto;
		}

		public string DevuelveMensaje()
		{
			return Mensaje;
		}
	}
}