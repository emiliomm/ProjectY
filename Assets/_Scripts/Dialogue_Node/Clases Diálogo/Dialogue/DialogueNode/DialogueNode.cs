using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DialogueTree
{
    public class DialogueNode
    {
        public int NodeID = -1;

		public PosicionCamara posCamara;
		// -1 --> nombre del NPC del dialogo
		// -2 --> nombre del jugador
		// >= 0 --> nombre del NPC con el número
		public int Nombre;

		//Hacia donde va el diálogo en el caso de que no haya opciones
		// 0 --> El dialogo sigue hacia el siguiente nodo (situado en +1)
		// -1 --> El dialogo va directamente al menú de mensajes, saltándose lo que venga por delante
		// -2 --> El dialogo acaba aquí
		public int siguienteNodo;
        public string Text;
		public bool recorrido; //Indica si el nodo ha sido recorrido anteriormente
		public bool destruido; //Indica si el dialogo del que forma parte el nodo va a ser destruido al acabar de leerlo
        public List<DialogueOption> Options;
		public List<DialogueAddIntro> Intros;
		public List<DialogueAddMensaje> Mensajes;
		public List<DialogueGrupo> Grupos;
		public List<DialogueGrupoVariable> GruposVariables;
		public List<DialogueAddObjeto> Objetos;
		public List<DialogueNombre> Nombres;

        // parameterless constructor for serialization
        public DialogueNode()
        {
			posCamara = new PosicionCamara();
            Options = new List<DialogueOption>();
			Intros = new List<DialogueAddIntro>();
			Mensajes = new List<DialogueAddMensaje>();
			Grupos = new List<DialogueGrupo>();
			GruposVariables = new List<DialogueGrupoVariable>();
			Objetos = new List<DialogueAddObjeto>();
			Nombres = new List<DialogueNombre>();

        }

		public int DevuelveNombre()
		{
			return Nombre;
		}

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

		public void MarcarRecorrido()
		{
			recorrido = true;
		}

		public DialogueOption DevuelveNodoOpciones(int node_id)
		{
			return Options[node_id];
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
    }
}