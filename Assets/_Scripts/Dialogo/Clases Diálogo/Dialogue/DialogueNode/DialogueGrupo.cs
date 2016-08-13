namespace DialogueTree
{
	/*
	 * Clase que añade o elimina el grupo indicado mientras se recorre un diálogo
	*/
	public class DialogueGrupo
	{
		public int IDGrupo;

		//verdadero(1) = añade grupo, falso(0) = elimina el grupo
		public bool tipo;

		DialogueGrupo() { }

		public int DevuelveID()
		{
			return IDGrupo;
		}

		//verdadero(1) = añade grupo, falso(0) = elimina el grupo
		public bool DevuelveTipo()
		{
			return tipo;
		}
	}
}
