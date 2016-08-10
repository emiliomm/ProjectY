namespace DialogueTree
{
	/*
	 * Clase que modifica variables del grupo indicado mientras se recorre un diálogo
	*/
	public class DialogueGrupoVariable {

		public int IDGrupo; // ID del grupo al que va dirigido el cambio de variable

		// 0 = suma el valor a la variable del grupo indicado
		// 1 = establece el valor a la variable del grupo indicado
		public int tipo;

		public int num; //posición de la variable en la lista de variables del grupo indicado
		public int valor;

		public DialogueGrupoVariable() {  }

		public int DevuelveIDGrupo()
		{
			return IDGrupo;
		}

		public int DevuelveTipo()
		{
			return tipo;
		}

		public int DevuelveNumero()
		{
			return num;
		}

		public int DevuelveValor()
		{
			return valor;
		}
	}
}
