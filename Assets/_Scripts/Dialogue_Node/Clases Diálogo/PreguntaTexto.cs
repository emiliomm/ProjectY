using UnityEngine;
using System.Collections;

namespace DialogueTree
{
	public class PreguntaTexto
	{
		public int Valor;
		private string nombreTexto  {get; set;} //no se deserializa, al ser privado

		public PreguntaTexto() { }
	}
}
