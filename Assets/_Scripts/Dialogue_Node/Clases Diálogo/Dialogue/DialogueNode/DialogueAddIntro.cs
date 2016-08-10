namespace DialogueTree
{
	/*
	 * Clase que es usada para añadir una intro a un diálogo (sea este u otro) al recorrer un dialogo en una conversacion
	*/
	public class DialogueAddIntro
	{
		public int IDNpc; //NPC al que se le va a añadir la intro
		public int IDDialogo; //Dialogo del npc indicado arriba al que se la a añadir la intro

		public int prioridad; //prioridad de la intro, cuanto más alto sea el valor, antes aparece
		public int IDIntro;

		public DialogueAddIntro() { }

		public int DevuelveIDNpc()
		{
			return IDNpc;
		}

		public int DevuelveIDDialogo()
		{
			return IDDialogo;
		}

		public int DevuelvePrioridad()
		{
			return prioridad;
		}

		public int DevuelveID()
		{
			return IDIntro;
		}
	}
}