using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase con los datos de una opción en el diálogo
	*/
	public class DialogueOption
    {
        public string text; //Texto de la opción

		/* 
		 * Valor
		 * -1 : Acaba el diálogo
		 * -2 : Va a la pantalla de mensajes (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * -3: Acaba la conversación (NO SE DEBE USAR EN UN XML) - CON EL SISTEMA ACTUAL SÍ
		 * Otro: Va al nodo indicado
	     */
        public int destinationNodeID;

		//Guarda variables de grupo que determinan si mostrar una opción o no
		public List<DialogueOptionGrupo> grupos;

		public List<DialogueOptionObjeto> objetos;

        public DialogueOption()
		{
			grupos = new List<DialogueOptionGrupo>();
			objetos = new List<DialogueOptionObjeto>();
		}

		public string DevuelveTexto()
		{
			return text;
		}

		public int DevuelveDestinationNodeID()
		{
			return destinationNodeID;
		}

		public int DevuelveNumeroGrupos()
		{
			return grupos.Count;
		}

		public int DevuelveIDGrupo(int num)
		{
			return grupos[num].IDGrupo;
		}

		public List<DialogueOptionGrupoVariables> DevuelveVariables(int num)
		{
			return grupos[num].variables;
		}

		public int DevuelveNumeroObjetos()
		{
			return objetos.Count;
		}

		public int DevuelveIDObjeto(int num)
		{
			return objetos[num].IDObjeto;
		}

		public bool DevuelveObjetoPosesion(int num)
		{
			return objetos[num].enPosesion;
		}
    }
}
