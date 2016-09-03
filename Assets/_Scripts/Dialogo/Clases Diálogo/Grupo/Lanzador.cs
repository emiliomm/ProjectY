using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using System.Xml; 
using System.Text; 

using DialogueTree;

/*
 * 	Clase que se encarga de añadir los intros/mensajes a los dialogos indicados
 */
public class Lanzador{
	
	public List<DialogueAddIntro> intros;
	public List<DialogueAddMensaje> mensajes;

	public Lanzador()
	{
		intros = new List<DialogueAddIntro>();
		mensajes = new List<DialogueAddMensaje>();
	}

	//Lee el lanzador en la ruta indicada pasándole parámetros del diálogo actual
	public static void LoadLanzador(string path, int ID_NPC,int ID_DiagActual, int tipo_dialogo, ref int num_dialogo)
	{
		Lanzador lanz = Manager.Instance.DeserializeData<Lanzador>(path);

		lanz.AnyadirDialogueAdd(ID_NPC,ID_DiagActual, tipo_dialogo, ref num_dialogo);
	}

	//Recorre las listas de intros y mensajes para añadirlos a los dialogos, con parámetros sobre el diálogo actual
	private void AnyadirDialogueAdd(int ID_NPC,int ID_DiagActual, int tipo_dialogo, ref int num_dialogo)
	{
		for(int i = 0; i < intros.Count; i++)
		{
			int prioridad = intros[i].DevuelvePrioridad();
			int ID = intros[i].DevuelveID();
			int IDNpc = intros[i].DevuelveIDNpc();
			int IDDialogo = intros[i].DevuelveIDDialogo();

			Intro intro = Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad);

			//Si la intro forma parte de un grupo y ese grupo ya ha acabado, no es añadida
			if(!Manager.Instance.GrupoAcabadoExiste(intro.DevuelveIDGrupo()))
			{
				NPC_Dialogo dialog = NPC_Dialogo.BuscarDialogo(IDNpc, IDDialogo);

				//Añadimos la intro
				dialog.AnyadirIntro(intro);

				//Si el NPC al que vamos a añadir la intro es el mismo que el del dialogo actual y estamos en una intro en el dialogo
				//Comprobamos si tenemos que cambiar el indice de dialogo actual
				if(ID_NPC == IDNpc && ID_DiagActual == IDDialogo)
				{
					//Una vez que sabemos que el NPC es el mismo, podemos comprobar la prioridad del intro actual
					//Si la prioridad de la intro a añadir es mayor que la actual, movemos el indice
					if(prioridad > dialog.intros[num_dialogo].DevuelvePrioridad() && tipo_dialogo == 0)
					{
						num_dialogo++;
					}
					//No añadimos el diálogo a la cola de objetos, ya que el dialogo actual se serializa al final de la conversación actual
				}
				//Si el diálogo de la intro a añadir no es el del diálogo actual
				//Añadimos el dialogo a la cola
				else
				{
					dialog.AddToColaObjetos();
				}
			}
		}

		for(int i = 0; i < mensajes.Count; i++)
		{
			int ID = mensajes[i].DevuelveID();
			int IDTema = mensajes[i].DevuelveIDTema();
			int IDNpc = mensajes[i].DevuelveIDNpc();
			int IDDialogo = mensajes[i].DevuelveIDDialogo();

			Mensaje mensaje = Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml");

			//Si el mensaje forma parte de un grupo y ese grupo ya ha acabado, no es añadido
			if(!Manager.Instance.GrupoAcabadoExiste(mensaje.DevuelveIDGrupo()))
			{
				NPC_Dialogo dialog = NPC_Dialogo.BuscarDialogo(IDNpc, IDDialogo);

				dialog.AnyadirMensaje(IDTema, mensaje);

				//Si el dialogo del cual hemos añadido el mensaje no es el actual, añadimos el dialogo a la cola
				//ya que el dialogo actual se serializa al final de la conversación
				if(!(ID_NPC == IDNpc && ID_DiagActual == IDDialogo))
				{
					dialog.AddToColaObjetos();
				}
			}
		}
	}
}