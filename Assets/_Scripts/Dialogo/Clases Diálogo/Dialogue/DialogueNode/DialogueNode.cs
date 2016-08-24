using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase que contiene todos los parámetros sobre una parte de un diálogo
	*/
    public class DialogueNode
    {
		//Número que identifica la posición del nodo en el dialogo
		//El nodo debe coincidir con su posición en la lista
        public int NodeID = -1;

		public PosicionCamara posCamara; //Contiene información sobre la posición de la cámara

		//CAMBIAR ESTOS VALORES O EL LOOKAT PARA QUE COINCIDAN
		//Nombre de quien habla, representado por un entero
		// -1 --> nombre del NPC del dialogo
		// -2 --> nombre del jugador
		// >= 0 --> nombre del NPC con el número
		public int Nombre;

		//Hacia donde va el diálogo en el caso de que no haya opciones
		// 0 --> El dialogo sigue hacia el siguiente nodo (situado en +1)
		// -1 --> El dialogo va directamente al menú de mensajes
		// -2 --> El dialogo acaba
		public int siguienteNodo;

        public string Text;
		public bool recorrido; //Indica si el nodo ha sido recorrido anteriormente
		public bool destruido; //Indica si el dialogo del que forma parte el nodo va a ser destruido al acabar de leerlo

		public List<DialogueAddIntro> Intros; //Contiene intros a añadir a un diálogo. Puede estar vacío
		public List<DialogueAddMensaje> Mensajes; //Contiene mensajes a añadir a un diálogo. Puede estar vacío
		public List<DialogueGrupo> Grupos; //Contiene grupos que se añaden o eliminan. Puede estar vacío
		public List<DialogueGrupoVariable> GruposVariables; //Modifica variables de un grupo. Puede estar vacío
		public List<DialogueAddObjeto> Objetos; //Contiene objetos a añadir al inventario. Puede estar vacío
		public List<DialogueNombre> Nombres; //Modifica el nombre de los NPCs. Puede estar vacío
		public List<DialogueRutina> Rutinas; //Modifica la rutina actual del npc. Puede estar vacío
		public List<DialogueOption> Options; //Contiene las opciones del nodo. Puede estar vacío

        // parameterless constructor for serialization
        public DialogueNode()
        {
			posCamara = new PosicionCamara();
			Intros = new List<DialogueAddIntro>();
			Mensajes = new List<DialogueAddMensaje>();
			Grupos = new List<DialogueGrupo>();
			GruposVariables = new List<DialogueGrupoVariable>();
			Objetos = new List<DialogueAddObjeto>();
			Nombres = new List<DialogueNombre>();
			Rutinas = new List<DialogueRutina>();
			Options = new List<DialogueOption>();
        }

		//Devuelve el nombre de quien habla en el dialogo
		// -1 --> nombre del NPC del dialogo
		// -2 --> nombre del jugador
		// >= 0 --> nombre del NPC con el número
		public int DevuelveNombre()
		{
			return Nombre;
		}

		//Devuelve una variable que indica que se hace a continuación en un nodo sin opciones
		// 0 --> El dialogo sigue hacia el siguiente nodo (situado en +1)
		// -1 --> El dialogo va directamente al menú de mensajes, saltándose lo que venga por delante
		// -2 --> El dialogo acaba aquí
		public int DevuelveSiguienteNodo()
		{
			return siguienteNodo;
		}

		public string DevuelveTexto()
		{
			return Text;
		}

		public bool DevuelveRecorrido()
		{
			return recorrido;
		}

		//Marca la variable recorrido a true, indicando que el nodo ya ha sido recorrido
		//y no se volverán a comprobar algunas de sus funciones si se vuelve a recorrer en el futuro
		public void MarcarRecorrido()
		{
			recorrido = true;
		}

		//Devuelve un objeto dialogueoption situado en la posición indicada de la lista de opciones
		public DialogueOption DevuelveNumNodoOpciones(int pos)
		{
			return Options[pos];
		}

		public int DevuelveNumeroOpciones()
		{
			return Options.Count;
		}

		public int DevuelveNumeroIntros()
		{
			return Intros.Count;
		}

		public int DevuelveNumeroMensajes()
		{
			return Mensajes.Count;
		}

		public int DevuelveNumeroGrupos()
		{
			return Grupos.Count;
		}

		public int DevuelveNumeroGruposVariables()
		{
			return GruposVariables.Count;
		}

		public int DevuelveNumeroObjetos()
		{
			return Objetos.Count;
		}

		public int DevuelveNumeroNombres()
		{
			return Nombres.Count;
		}

		public int DevuelveNumeroRutinas()
		{
			return Rutinas.Count;
		}
    }
}