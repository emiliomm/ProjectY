namespace DialogueTree
{
	/*
	 * Clase que es usada para añadir un objeto al inventario mientras se recorre un diálogo
	*/
	public class DialogueAddObjeto
	{
		public int ID; //ID del objeto a añadir
		public int cantidad; //Cuantos objetos añadimos

		public DialogueAddObjeto()
		{
			
		}

		public int DevuelveIDObjeto()
		{
			return ID;
		}

		public int devuelveCantidad()
		{
			return cantidad;
		}
	}
}
