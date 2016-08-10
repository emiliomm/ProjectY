namespace DialogueTree
{
	/*
	 * Clase que modifica el nombre de un NPC
	*/
	public class DialogueNombre
	{
		//-1: ID propio, x: ID NPC
		public int IDNpc;
		public int indiceNombre; //indica la posición a la que se cambiará el indice de nombres del NPC

		public DialogueNombre() {  }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIndiceNombre()
		{
			return indiceNombre;
		}
	}
}
