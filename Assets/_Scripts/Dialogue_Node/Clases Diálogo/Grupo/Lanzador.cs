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

	public static void LoadLanzador(string path, int ID_NPC,int ID_DiagActual, int tipo_dialogo, ref int num_dialogo)
	{
		Lanzador lanz = Manager.Instance.DeserializeData<Lanzador>(path);

		lanz.AnyadirDialogueAdd(ID_NPC,ID_DiagActual, tipo_dialogo, ref num_dialogo);
	}

	//JUNTAR CON LA FUNCION PARECIDA ¿?
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

				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetInteractuable(IDNpc);

				if(gobj != null)
				{
					Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
					NPC_Dialogo diag = inter.DevolverDialogo(IDDialogo);

					if(diag != null)
					{
						
						//Si el NPC al que vamos a añadir la intro es el mismo que el del dialogo actual y estamos en una intro en el dialogo
						//Comprobamos si tenemos que cambiar el indice de dialogo
						if(ID_NPC == IDNpc && ID_DiagActual == IDDialogo)
						{
							//Una vez que sabemos que el NPC es el mismo, podemos comprobar la prioridad del intro actual
							//Si la prioridad de la intro a añadir es mayor que la actual, movemos el indice
							if(prioridad > diag.intros[num_dialogo].DevuelvePrioridad() && tipo_dialogo == 0)
							{
								num_dialogo++;
							}

							diag.AnyadirIntro(intro);
						}
						//Si no es el dialogo actual lo añadimos a la cola
						else
						{
							diag.AnyadirIntro(intro);
							diag.AddToColaObjetos();
						}
					}
					else
					{
						//Buscamos en la cola de objetos
						ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

						if(cobj != null)
						{
							ObjetoSer objs = cobj.GetObjeto();
							diag = objs as NPC_Dialogo;
						}
						else
						{
							//Cargamos el dialogo
							//Si existe un fichero guardado, cargamos ese fichero, sino
							//cargamos el fichero por defecto
							if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
							{
								diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
							}
							else
							{
								diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
							}
						}

						diag.AnyadirIntro(intro);
						diag.AddToColaObjetos ();
					}
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Buscamos en la cola de objetos
					ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

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
						if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
						{
							npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
						}
						else
						{
							npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
						}
					}

					npc_diag.AnyadirIntro(intro);
					npc_diag.AddToColaObjetos ();
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
				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetInteractuable(IDNpc);

				if(gobj != null)
				{
					Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
					NPC_Dialogo diag = inter.DevolverDialogo(IDDialogo);

					if(diag != null)
					{
						diag.AnyadirMensaje(IDTema, mensaje);

						//Si no es el dialogo actual lo añadimos a la cola
						if(!(ID_NPC == IDNpc && ID_DiagActual == IDDialogo))
						{
							diag.AddToColaObjetos();
						}
					}
					else
					{
						//Buscamos en la cola de objetos
						ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

						if(cobj != null)
						{
							ObjetoSer objs = cobj.GetObjeto();
							diag = objs as NPC_Dialogo;
						}
						else
						{
							//Cargamos el dialogo
							//Si existe un fichero guardado, cargamos ese fichero, sino
							//cargamos el fichero por defecto
							if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
							{
								diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
							}
							else
							{
								diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
							}
						}

						diag.AnyadirMensaje(IDTema, mensaje);
						diag.AddToColaObjetos ();
					}
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Buscamos en la cola de objetos
					ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

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
						if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
						{
							npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
						}
						else
						{
							npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
						}
					}

					npc_diag.AnyadirMensaje(IDTema, mensaje);
					npc_diag.AddToColaObjetos ();
				}
			}
		}
	}
}