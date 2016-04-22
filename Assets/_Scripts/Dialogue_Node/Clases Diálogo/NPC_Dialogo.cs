using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
using System.Linq;

using DialogueTree;

public class NPC_Dialogo{
	
	public int ID;

	//Los intros y mensajes  por defecto de los npcs no pueden tener idGrupo
	public List<Intro> intros;
	public List<Mensaje> mensajes;

	public NPC_Dialogo()
	{
		intros = new List<Intro>();
		mensajes = new List<Mensaje>();
//		Prueba();
	}

	//OBSOLETA
//	private void Prueba()
//	{
//		Intro d = new Intro("text_dia.xml");
//		Intro d2 = new Intro("text_dia2.xml");
//
//		AnyadirIntro (d);
//		AnyadirIntro (d2);
//
//		for(int i = 0; i < 5; i++)
//		{
//			mensajes.Add(new Mensaje("Opcion " + i.ToString(),rutaDialogos + "text_dia3.xml"));
//		}
//	}

	public int DevuelveNumeroIntros()
	{
		return intros.Count;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public Dialogue DevuelveDialogoIntro(int num_intro)
	{
		return intros[num_intro].DevuelveDialogo();
	}

	public Dialogue DevuelveDialogoMensajes(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveDialogo();
	}

	public string DevuelveTextoMensaje(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveTexto();
	}

	public bool AvanzaIntro(int num_intro)
	{
		bool avanza = false;
		if (num_intro + 1 < this.DevuelveNumeroIntros())
			avanza = true;

		return avanza;
	}

	public void MirarSiDialogoSeAutodestruye(int tipo, ref int num_dialogo)
	{
		switch(tipo)
		{
		case 0:
			if(intros [num_dialogo].Autodestruye == true)
			{
				intros.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		case 1:
			if(mensajes [num_dialogo].Autodestruye == true)
			{
				mensajes.RemoveAt(num_dialogo);
				num_dialogo--;
			}
			break;
		}
	}

	public void MarcaDialogueNodeComoLeido(int tipo, ref int num_dialogo, int node_id)
	{
		Dialogue d;
		DialogueNode dn;

		switch(tipo)
		{
		case 0:
			d = this.DevuelveDialogoIntro(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
			if(dn.destruido == true)
				intros [num_dialogo].Autodestruye = true;

			if(dn.DevuelveRecorrido() != true)
			{
				intros[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(tipo, ref num_dialogo, dn);
			}
			break;
		case 1:
			d = this.DevuelveDialogoMensajes(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
			if(dn.destruido == true)
				mensajes [num_dialogo].Autodestruye = true;

			if(dn.DevuelveRecorrido() != true)
			{
				mensajes[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(tipo, ref num_dialogo, dn);
			}
			break;
		}
	}
		
	//SEPARAR FUNCION EN TROZOS MAS PEQUEÑOS
	private void AnyadirDialogueAdd(int tipo_dialogo, ref int num_dialogo, DialogueNode node)
	{
		for(int i = 0; i < node.DevuelveNumeroGrupos(); i++)
		{
			int ID = node.Grupos[i].DevuelveID();
			bool tipo = node.Grupos[i].DevuelveTipo();

			//Si el tipo es verdadero, cargamos el grupo
			if(tipo)
			{
				//Si el grupo no está activo y no está en la lista de grupos acabados, lo añadimos
				if (!Manager.Instance.GrupoActivoExiste(ID) && !Manager.Instance.GrupoAcabadoExiste(ID))
				{
					//Miramos primero en la lista de grupos modificados
					if (System.IO.File.Exists (Manager.rutaGruposModificados + ID.ToString () + ".xml"))
					{
						Grupo.LoadGrupo (Manager.rutaGruposModificados + ID.ToString () + ".xml", ID, tipo_dialogo, ref num_dialogo);
					}
					//Si no está ahí, miramos en el directorio predeterminado
					else
					{
						Grupo.LoadGrupo (Manager.rutaGrupos + ID.ToString () + ".xml", ID, tipo_dialogo, ref num_dialogo);	
					}
				}
			}
			//Si es falso, destruimos el grupo indicado y las intros/mensajes asignados a él
			else
			{
				//Empezamos destruyendo los intros/mensajes del dialogo actual

				//Si estamos en una intro, comprobamos que posicionamos correctamente el indice en las intros
				if(tipo_dialogo == 0)
				{
					for(int j = 0; j < this.DevuelveNumeroIntros(); j++)
					{
						if(this.intros[j].IDGrupo == ID)
						{
							this.intros.RemoveAt(j);

							//Mantenemos el indice en una posicion correcta
							if (j < num_dialogo)
							{
								num_dialogo--;
							}
							//Si la intro a destruir es el actual, lo destruimos al final (activando la autodestruccion)
							else if(j == num_dialogo)
							{
								intros [num_dialogo].Autodestruye = true;
							}
						}
					}
					for(int j = 0; j < this.DevuelveNumeroMensajes(); j++)
					{
						if(this.mensajes[j].IDGrupo == ID)
						{
							this.mensajes.RemoveAt(j);
						}
					}
				}
				//Si estamos en un mensaje, comprobamos que posicionamos correctamente el indice en los mensajes
				else
				{
					for(int j = 0; j < this.DevuelveNumeroIntros(); j++)
					{
						if(this.intros[j].IDGrupo == ID)
						{
							this.intros.RemoveAt(j);
						}
					}
					for(int j = 0; j < this.DevuelveNumeroMensajes(); j++)
					{
						if(this.mensajes[j].IDGrupo == ID)
						{
							this.mensajes.RemoveAt(j);

							//Mantenemos el indice en una posicion correcta
							if (j < num_dialogo)
							{
								num_dialogo--;
							}
							//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
							else if(j == num_dialogo)
							{
								mensajes [num_dialogo].Autodestruye = true;
							}
						}
					}
				}

				//Ahora comprobamos a los npcs de la escena actual
				List<GameObject> npcs = Manager.Instance.GetAllNPCs();

				var num_npcs = 0; //Contamos los npcs que guardamos

				for(int j = 0; j < npcs.Count; j++)
				{
					GameObject gobj = npcs[j];
					NPC npc = gobj.GetComponent<NPC>() as NPC;
					NPC_Dialogo n_diag = npc.npc_diag;

					for(int k = 0; k < n_diag.DevuelveNumeroIntros(); k++)
					{
						if(n_diag.intros[k].IDGrupo == ID)
						{
							n_diag.intros.RemoveAt(k);
						}
					}
					for(int k = 0; k < n_diag.DevuelveNumeroMensajes(); k++)
					{
						if(n_diag.mensajes[k].IDGrupo == ID)
						{
							n_diag.mensajes.RemoveAt(k);
						}
					}
					num_npcs ++;
					n_diag.Serialize();
				}
					
				//Ahora recorremos los ficheros guardadados
				var info = new DirectoryInfo(Manager.rutaNPCDialogosGuardados);
				var fileInfo = info.GetFiles().OrderByDescending( f => f.CreationTime).ToArray(); //los nuevos empiezan al principio de la lista

				for(var j = num_npcs; j < fileInfo.Length; j++)
				{
					bool actualizado = false;
					string name = Path.GetFileNameWithoutExtension(fileInfo[j].Name);
					NPC_Dialogo n_diag = NPC_Dialogo.LoadNPCDialogue(int.Parse(name), Manager.rutaNPCDialogosGuardados + name  + ".xml");

					for(int k = 0; k < n_diag.DevuelveNumeroIntros(); k++)
					{
						if(n_diag.intros[k].IDGrupo == ID)
						{
							n_diag.intros.RemoveAt(k);
							actualizado = true;
						}
					}
					for(int k = 0; k < n_diag.DevuelveNumeroMensajes(); k++)
					{
						if(n_diag.mensajes[k].IDGrupo == ID)
						{
							n_diag.mensajes.RemoveAt(k);
							actualizado = true;
						}
					}

					if(actualizado)
						n_diag.Serialize();
				}

				//Por último, eliminamos el grupo del Manager
				Manager.Instance.RemoveFromGruposActivos(ID);

				//FALTA ENVIAR LOS MENSAJES/INTROS DE FINALIZACIÓN
			}
		}
		
		for(int i = 0; i < node.DevuelveNumeroIntros(); i++)
		{
			int prioridad = node.Intros[i].DevuelvePrioridad();
			int ID = node.Intros[i].DevuelveID();
			int IDNpc = node.Intros[i].DevuelveIDNpc();

			if(IDNpc == -1)
			{
				//Si estamos en una intro y la prioridad es mayor que la actual, cambiamos el indice de dialogo
				if(tipo_dialogo == 0 && prioridad > intros[num_dialogo].DevuelvePrioridad())
				{
					num_dialogo++;
				}

				AnyadirIntro(Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad));
			}
			else
			{
				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetNPC(IDNpc);

				if(gobj != null)
				{
					NPC npc = gobj.GetComponent<NPC>() as NPC;
					npc.npc_diag.AnyadirIntro(Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad));
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, Manager.rutaNPCDialogos + IDNpc.ToString()  + ".xml");
					}

					npc_diag.AnyadirIntro(Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad));
					npc_diag.Serialize();
				}
			}
		}
		 
		for(int i = 0; i < node.DevuelveNumeroMensajes(); i++)
		{
			int ID = node.Mensajes[i].DevuelveID();
			int IDNpc = node.Mensajes[i].DevuelveIDNpc();

			if(IDNpc == -1)
			{
				AnyadirMensaje(Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml"));
			}
			else
			{
				//Buscamos en el diccionario y lo añadimos
				//si no esta en el diccionario, lo añadimos desde el xml
				GameObject gobj = Manager.Instance.GetNPC(IDNpc);

				if(gobj != null)
				{
					NPC npc = gobj.GetComponent<NPC>() as NPC;
					npc.npc_diag.AnyadirMensaje(Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml"));
				}
				else
				{
					NPC_Dialogo npc_diag;

					//Cargamos el dialogo
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml"))
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						npc_diag = NPC_Dialogo.LoadNPCDialogue(IDNpc, Manager.rutaNPCDialogos + IDNpc.ToString()  + ".xml");
					}

					npc_diag.AnyadirMensaje(Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml"));
					npc_diag.Serialize();
				}
			}
		}

		for(int i = 0; i < node.DevuelveNumeroGruposVariables(); i++)
		{
			int IDGrupo = node.GruposVariables[i].DevuelveIDGrupo();
			int tipo = node.GruposVariables[i].DevuelveTipo();
			int num = node.GruposVariables[i].DevuelveNumero();
			int valor = node.GruposVariables[i].DevuelveValor();

			//Si el grupo existe, cambiamos las variables
			if(Manager.Instance.GrupoActivoExiste(IDGrupo))
			{
				switch(tipo)
				{
				case 0: //suma
					Manager.Instance.AddVariablesGrupo(IDGrupo, num, valor);
					break;
				case 1: //establecer
					Manager.Instance.SetVariablesGrupo(IDGrupo, num, valor);
					break;
				}
			}
			//Sino existe, comprobamos que no ha sido eliminado
			else
			{
				//Tras comprobar que no ha sido eliminado, lo añadimos a lista de grupos modificados
				if (!Manager.Instance.GrupoAcabadoExiste(IDGrupo))
				{
					Grupo g;

					//Comprobamos si está en la lista de grupos modificados
					if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
					{
						g = Grupo.CreateGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
					}
					//Si no está ahí, miramos en el directorio predeterminado
					else
					{
						g = Grupo.CreateGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml");
					}

					switch (tipo)
					{
					case 0: //suma
						g.variables [num] += valor;
						break;
					case 1: //establecer
						g.variables [num] = valor;
						break;
					}

					//Guardamos el grupo en la ruta de grupos modificados
					g.Serialize();
				}
			}
		}
	}

	public void AnyadirIntro(Intro d)
	{
		//Si la intro con el ID especificado no existe, la añadimos
		if(!IntroExiste (d.ID))
			intros.Add (d);
		
		//Ordena las intros por prioridad de mayor a menor, manteniendo el orden de los elementos iguales
		intros = intros.OrderByDescending(x => x.prioridad).ToList();
	}

	private bool IntroExiste(int id)
	{
		return intros.Any(x => x.ID == id);
	}

	public void AnyadirMensaje(Mensaje m)
	{
		//Si el mensaje con el ID especificado no existe, lo añadimos
		if(!MensajeExiste (m.ID))
			mensajes.Add(m);
	}

	private bool MensajeExiste(int id)
	{
		return mensajes.Any(x => x.ID == id);
	}

	public static NPC_Dialogo LoadNPCDialogue(int id, string path)
	{
		NPC_Dialogo npc_dialogo = Manager.Instance.DeserializeDataWithReturn<NPC_Dialogo>(path);

		return npc_dialogo;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaNPCDialogosGuardados, Manager.rutaNPCDialogosGuardados + ID.ToString()  + ".xml");
	}
}
