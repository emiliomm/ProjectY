﻿using System.Collections.Generic;

namespace DialogueTree
{
	/*
	 * Clase que contiene todos los parámetros sobre una parte de un diálogo
	*/
    public class DialogueNode
    {
		//Número que identifica la posición del nodo en el dialogo
		//El nodo debe coincidir con su posición en la lista
        public int nodeID = -1;

		public PosicionCamara posCamara; //Contiene información sobre la posición de la cámara

		//CAMBIAR ESTOS VALORES O EL LOOKAT PARA QUE COINCIDAN
		//Nombre de quien habla, representado por un entero
		// -1 --> nombre del NPC del dialogo
		// -2 --> nombre del jugador
		// >= 0 --> nombre del NPC con el número
		public int nombre;

		//Hacia donde va el diálogo en el caso de que no haya opciones
		// 0 --> El dialogo sigue hacia el siguiente nodo (situado en +1)
		// -1 --> El dialogo va directamente al menú de mensajes
		// -2 --> El dialogo acaba
		public int siguienteNodo;

        public string text;
		public bool leido; //Indica si el nodo ha sido recorrido anteriormente
		public bool destruido; //Indica si el dialogo del que forma parte el nodo va a ser destruido al acabar de leerlo

		public List<DialogueAddIntro> intros; //Contiene intros a añadir a un diálogo. Puede estar vacío
		public List<DialogueAddMensaje> mensajes; //Contiene mensajes a añadir a un diálogo. Puede estar vacío
		public List<DialogueGrupo> grupos; //Contiene grupos que se añaden o eliminan. Puede estar vacío
		public List<DialogueGrupoVariable> gruposVariables; //Modifica variables de un grupo. Puede estar vacío
		public List<DialogueAddObjeto> objetos; //Contiene objetos a añadir al inventario. Puede estar vacío
		public List<DialogueNombre> nombres; //Modifica el nombre de los NPCs. Puede estar vacío
		public List<DialogueRutina> rutinas; //Modifica la rutina actual del npc. Puede estar vacío
		public List<DialogueOption> options; //Contiene las opciones del nodo. Puede estar vacío

        // parameterless constructor for serialization
        public DialogueNode()
        {
			posCamara = new PosicionCamara();
			intros = new List<DialogueAddIntro>();
			mensajes = new List<DialogueAddMensaje>();
			grupos = new List<DialogueGrupo>();
			gruposVariables = new List<DialogueGrupoVariable>();
			objetos = new List<DialogueAddObjeto>();
			nombres = new List<DialogueNombre>();
			rutinas = new List<DialogueRutina>();
			options = new List<DialogueOption>();
        }

		//Devuelve el nombre de quien habla en el dialogo
		// -1 --> nombre del NPC del dialogo
		// -2 --> nombre del jugador
		// >= 0 --> nombre del NPC con el número
		public int DevuelveNombre()
		{
			return nombre;
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
			return text;
		}

		public bool DevuelveLeido()
		{
			return leido;
		}

		//Marca la variable leido a true, indicando que el nodo ya ha sido recorrido
		//y no se volverán a comprobar algunas de sus funciones si se vuelve a recorrer en el futuro
		public void MarcarLeido()
		{
			leido = true;
		}

		//Devuelve un objeto dialogueoption situado en la posición indicada de la lista de opciones
		public DialogueOption DevuelveNodoOpciones(int pos)
		{
			return options[pos];
		}

		public int DevuelveNumeroOpciones()
		{
			return options.Count;
		}

		public int DevuelveNumeroIntros()
		{
			return intros.Count;
		}

		public int DevuelveNumeroMensajes()
		{
			return mensajes.Count;
		}

		public int DevuelveNumeroGrupos()
		{
			return grupos.Count;
		}

		public int DevuelveNumeroGruposVariables()
		{
			return gruposVariables.Count;
		}

		public int DevuelveNumeroObjetos()
		{
			return objetos.Count;
		}

		public int DevuelveNumeroNombres()
		{
			return nombres.Count;
		}

		public int DevuelveNumeroRutinas()
		{
			return rutinas.Count;
		}
    }
}