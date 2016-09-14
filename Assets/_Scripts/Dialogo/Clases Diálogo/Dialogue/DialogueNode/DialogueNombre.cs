namespace DialogueTree
{
	/*
	 * Clase que modifica el nombre de un NPC
	*/
	public class DialogueNombre
	{
		//-1: ID del NPC al cual pertence el dialogo, x: ID NPC
		public int IDInteractuable;
		public int indiceNombre; //indica la posición a la que se cambiará el indice de nombres del NPC

		public DialogueNombre() {  }

		public int DevuelveIDInteractuable()
		{
			return IDInteractuable;
		}

		public int DevuelveIndiceNombre()
		{
			return indiceNombre;
		}
	}
}
