using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase con los datos de una opción en el diálogo
	*/
	public class DialogueOption
    {
        public string Text; //Texto de la opción

		/* 
		 * Valor
		 * -1 : Acaba el diálogo
		 * -2 : Va a la pantalla de mensajes (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * -3: Acaba la conversación (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * Otro: Va al nodo indicado
	     */
        public int DestinationNodeID;

		//Guarda variables de grupo que determinan si mostrar una opción o no
		public List<DialogueOptionGrupo> Grupos;

		public List<DialogueOptionObjeto> Objetos;

        public DialogueOption()
		{
			Grupos = new List<DialogueOptionGrupo>();
			Objetos = new List<DialogueOptionObjeto>();
		}

		public string DevuelveTexto()
		{
			return Text;
		}

		public int DevuelveDestinationNodeID()
		{
			return DestinationNodeID;
		}

		public int DevuelveNumeroGrupos()
		{
			return Grupos.Count;
		}

		public int DevuelveIDGrupo(int num)
		{
			return Grupos[num].IDGrupo;
		}

		public List<DialogueOptionGrupoVariables> DevuelveVariables(int num)
		{
			return Grupos[num].variables;
		}

		public int DevuelveNumeroObjetos()
		{
			return Objetos.Count;
		}

		public int DevuelveIDObjeto(int num)
		{
			return Objetos[num].IDObjeto;
		}

		public bool DevuelveObjetoPosesion(int num)
		{
			return Objetos[num].enPosesion;
		}
    }
}
