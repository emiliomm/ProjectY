namespace DialogueTree
{
	/*
	 * Clase que es usada para añadir un mensaje a un diálogo (sea este u otro) al recorrer un dialogo en una conversacion
	*/
	public class DialogueAddMensaje
	{
		public int IDNpc; //NPC al que se le va a añadir el mensaje
		public int IDDialogo; //Dialogo del npc indicado arriba al que se la a añadir el mensaje

		//-1: el mensaje no forma parte de ningún tema, va por separado. x = el mensaje se agrupa en el tema x
		public int IDTema;
		public int IDMensaje;

		public DialogueAddMensaje() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIDDialogo()
		{
			return IDDialogo;
		}

		public int DevuelveIDTema()
		{
			return IDTema;
		}

		public int DevuelveID()
		{
			return IDMensaje;
		}
	}
}