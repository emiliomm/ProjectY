using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using System.Xml; 
using System.Text; 

using DialogueTree;

public class Lanzador{
	
	public List<DialogueAddIntro> intros;
	public List<DialogueAddMensaje> mensajes;

	public Lanzador()
	{
		intros = new List<DialogueAddIntro>();
		mensajes = new List<DialogueAddMensaje>();
	}

	public static void LoadLanzador(string path, int ID_NPC, int tipo_dialogo, ref int num_dialogo)
	{
		Lanzador lanz = Manager.Instance.DeserializeDataWithReturn<Lanzador>(path);

		lanz.AnyadirDialogueAdd(ID_NPC, tipo_dialogo, ref num_dialogo);
	}

	//JUNTAR CON LA FUNCION PARECIDA ¿?
	private void AnyadirDialogueAdd(int ID_NPC, int tipo_dialogo, ref int num_dialogo)
	{
		for(int i = 0; i < intros.Count; i++)
		{
			int prioridad = intros[i].DevuelvePrioridad();
			int ID = intros[i].DevuelveID();
			int IDNpc = intros[i].DevuelveIDNpc();

			//Buscamos en el diccionario y lo añadimos
			//si no esta en el diccionario, lo añadimos desde el xml
			GameObject gobj = Manager.Instance.GetNPC(IDNpc);

			if(gobj != null)
			{
				NPC npc = gobj.GetComponent<NPC>() as NPC;
				NPC_Dialogo diag = npc.DevuelveDialogo();

				//Si el NPC al que vamos a añadir la intro es el mismo que el del dialogo actual y estamos en una intro en el dialogo
				//Comprobamos si tenemos que cambiar el indice de dialogo
				if(ID_NPC == IDNpc && tipo_dialogo == 0)
				{
					//Una vez que sabemos que el NPC es el mismo, podemos comprobar la prioridad del intro actual
					//Si la prioridad de la intro a añadir es mayor que la actual, movemos el indice
					if(prioridad > diag.intros[num_dialogo].DevuelvePrioridad())
						num_dialogo++;
				}
				diag.AnyadirIntro(Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad));
				npc.ActualizarDialogo (diag);
			}
			else
			{
				NPC_Dialogo npc_diag;

				//Buscamos en la cola de objetos
				ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");

				if(cobj != null)
				{
					ObjetoSer objs = cobj.GetObjeto();
					npc_diag = objs as NPC_Dialogo;
				}
				else
				{
					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + ".xml");
					}
				}

				npc_diag.AnyadirIntro(Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad));
				npc_diag.AddToColaObjetos ();
			}
		}

		for(int i = 0; i < mensajes.Count; i++)
		{
			int ID = mensajes[i].DevuelveID();
			int IDNpc = mensajes[i].DevuelveIDNpc();

			//Buscamos en el diccionario y lo añadimos
			//si no esta en el diccionario, lo añadimos desde el xml
			GameObject gobj = Manager.Instance.GetNPC(IDNpc);

			if(gobj != null)
			{
				NPC npc = gobj.GetComponent<NPC>() as NPC;
				NPC_Dialogo diag = npc.DevuelveDialogo();

				diag.AnyadirMensaje(Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml"));
				npc.ActualizarDialogo (diag);
			}
			else
			{
				NPC_Dialogo npc_diag;

				//Buscamos en la cola de objetos
				ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");

				if(cobj != null)
				{
					ObjetoSer objs = cobj.GetObjeto();
					npc_diag = objs as NPC_Dialogo;
				}
				else
				{
					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + ".xml");
					}
				}

				npc_diag.AnyadirMensaje(Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml"));
				npc_diag.AddToColaObjetos ();
			}
		}
	}
}