using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
using System.Linq;


using System;

using DialogueTree;

[XmlRoot("ObjetoSer")]
public class NPC_Dialogo : ObjetoSer
{
	public int ID_NPC;
	public int ID;

	//Los intros y mensajes por defecto de los npcs no pueden tener idGrupo, ya que los
	//elementos con grupo son añadidos tras modificar un diálogo. Un dialogo con grupo no
	//sería detectado al borrar elementos de grupo ya que el dialogo no estaría en la ruta de dialogos guardados
	public List<Intro> intros;
	public List<TemaMensaje> temaMensajes;
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

	public int DevuelveNumeroTemaMensajes()
	{
		return temaMensajes.Count;
	}

	public int DevuelveNumeroMensajes()
	{
		return mensajes.Count;
	}

	public TemaMensaje DevuelveTemaMensaje(int num_tema)
	{
		return temaMensajes[num_tema];
	}

	public Dialogue DevuelveDialogoIntro(int num_intro)
	{
		return intros[num_intro].DevuelveDialogo();
	}

	public Dialogue DevuelveDialogoTemaMensajes(int num_tema, int num_mensaje)
	{
		return temaMensajes[num_tema].mensajes[num_mensaje].DevuelveDialogo();
	}

	public Dialogue DevuelveDialogoMensajes(int num_mensaje)
	{
		return mensajes[num_mensaje].DevuelveDialogo();
	}

	public string DevuelveTextoTemaMensaje(int num_tema)
	{
		return temaMensajes[num_tema].DevuelveTexto();
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

	public void AnyadirIntro(Intro d)
	{
		//Si la intro con el ID especificado no existe, la añadimos
		if(!IntroExiste (d.ID))
			intros.Add (d);

		//Ordena las intros por prioridad de mayor a menor, manteniendo el orden de los elementos iguales
		//una intro añadida con la misma prioridad será colocada abajo de los iguales, es decir, como si fuera menor.
		intros = intros.OrderByDescending(x => x.prioridad).ToList();
	}

	private bool IntroExiste(int id)
	{
		return intros.Any(x => x.ID == id);
	}

	//id_tema = -1: se añade a la lista de mensajes sin tema
	//x: se añade al tema x
	public void AnyadirMensaje(int id_tema, Mensaje m)
	{
		if(id_tema == -1)
		{
			//Si el mensaje con el ID especificado no existe, lo añadimos
			if(!MensajeExiste (id_tema, m.ID))
				mensajes.Add(m);
		}
		else
		{
			int indice = temaMensajes.FindIndex(x => x.ID == id_tema); //cogemos el indice del temamensaje buscado

			//El tema mensaje no existe, lo creamos
			if(indice == -1)
			{
				TemaMensaje tm = TemaMensaje.LoadTemaMensaje(Manager.rutaTemaMensajes + id_tema.ToString() + ".xml");
				tm.AddMensaje(m);
				this.AnyadirTemaMensaje(tm);
			}
			//El tema mensaje existe, comprobamos si el mensaje existe
			//Si no existe, lo añadimos
			else
			{
				if(!MensajeExiste(indice, m.ID))
				{
					temaMensajes[indice].mensajes.Add(m);
				}
			}
		}
	}

	//num_tema = lugar donde está el tema en el la lista temaMensajes
	//ATENCIÓN: COMPROBAR ANTES QUE EL TEMA EXISTE
	//si el num_tema = -1, es que el mensaje no tiene tema
	private bool MensajeExiste(int num_tema, int id)
	{
		bool existe = false;

		if(num_tema == -1)
			existe = mensajes.Any(x => x.ID == id);
		else
		{
			existe = temaMensajes[num_tema].mensajes.Any(x => x.ID == id);
		}

		return existe;
	}

	public void AnyadirTemaMensaje(TemaMensaje tm)
	{
		temaMensajes.Add(tm);
	}

	public void MirarSiDialogoSeAutodestruye(int tipo, ref int num_dialogo, int num_tema)
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
				num_dialogo--; // ¿? Creo que no es necesario
			}
			break;
		case 2:
			if(temaMensajes[num_tema].mensajes[num_dialogo].Autodestruye == true)
			{
				temaMensajes[num_tema].mensajes.RemoveAt(num_dialogo);
				num_dialogo--; // ¿? Creo que no es necesario

				//Si el temaMensajes se ha quedado vacío, lo destruimos
				if(temaMensajes[num_tema].mensajes.Count == 0)
				{
					this.temaMensajes.RemoveAt(num_tema);
				}
			}
			break;
		}
	}

	public void MarcaDialogueNodeComoLeido(int tipo, ref int num_dialogo, int node_id, int num_tema)
	{
		Dialogue d;
		DialogueNode dn;

		switch(tipo)
		{
		case 0:
			d = this.DevuelveDialogoIntro(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			if(dn.DevuelveRecorrido() != true)
			{
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(dn.destruido == true)
					intros [num_dialogo].Autodestruye = true;

				intros[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(tipo, ref num_dialogo, dn, num_tema);
			}
			break;
		case 1:
			d = this.DevuelveDialogoMensajes(num_dialogo);
			dn = d.DevuelveNodo(node_id);

			if(dn.DevuelveRecorrido() != true)
			{
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(dn.destruido == true)
					mensajes [num_dialogo].Autodestruye = true;

				mensajes[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(tipo, ref num_dialogo, dn, num_tema);
			}
			break;
		case 2:
			d = this.DevuelveDialogoTemaMensajes(num_tema, num_dialogo);
			dn = d.DevuelveNodo(node_id);

			if(dn.DevuelveRecorrido() != true)
			{
				//Si está marcado que el dialogo se destruye, activamos la autodestrucción de este
				if(dn.destruido == true)
					temaMensajes[num_tema].mensajes[num_dialogo].Autodestruye = true;

				temaMensajes[num_tema].mensajes[num_dialogo].MarcarRecorrido(node_id);
				AnyadirDialogueAdd(tipo, ref num_dialogo, dn, num_tema);
			}
			break;
		}
	}
		
	//SEPARAR FUNCION EN TROZOS MAS PEQUEÑOS
	private void AnyadirDialogueAdd(int tipo_dialogo, ref int num_dialogo, DialogueNode node, int num_tema)
	{
		for(int i = 0; i < node.DevuelveNumeroGrupos(); i++)
		{
			int IDGrupo = node.Grupos[i].DevuelveID();
			bool tipo = node.Grupos[i].DevuelveTipo();

			//Si el tipo es verdadero, cargamos el grupo
			if(tipo)
			{
				//Si el grupo no está activo y no está en la lista de grupos acabados, lo añadimos
				if (!Manager.Instance.GrupoActivoExiste(IDGrupo) && !Manager.Instance.GrupoAcabadoExiste(IDGrupo))
				{
					//Comprobamos si el grupo se encuentra como grupo modificado en la lista colaObjetos del Manager
					ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

					if(cobj != null)
					{
						ObjetoSer objs = cobj.GetObjeto();
						Grupo g = objs as Grupo;
						Grupo.LoadGrupo(g, ID_NPC, ID, tipo_dialogo, ref num_dialogo);

						//Borramos el grupo modificado de la cola ahora que ya ha sido añadido
						Manager.Instance.RemoveFromColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");
					}
					else
					{
						//Miramos primero en la lista de grupos modificados
						if (System.IO.File.Exists (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml"))
						{
							Grupo.LoadGrupo (Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml", ID_NPC,ID, tipo_dialogo, ref num_dialogo);
						}
						//Si no está ahí, miramos en el directorio predeterminado
						else
						{
							Grupo.LoadGrupo (Manager.rutaGrupos + IDGrupo.ToString () + ".xml", ID_NPC,ID, tipo_dialogo, ref num_dialogo);	
						}
					}
				}
			}
			//Si es falso, destruimos el grupo indicado y las intros/mensajes asignados a él
			else
			{
				//Solo se pueden eliminar grupos activos actualmente
				if(Manager.Instance.GrupoActivoExiste(IDGrupo))
				{
					//Empezamos destruyendo los intros/mensajes del dialogo actual
					switch(tipo_dialogo)
					{
					//Si estamos en una intro, comprobamos que posicionamos correctamente el indice en las intros
					case 0:
						for(int j = this.DevuelveNumeroIntros() - 1; j >= 0; j--)
						{
							if(this.intros[j].IDGrupo == IDGrupo)
							{
								//Mantenemos el indice en una posicion correcta
								if (j < num_dialogo)
								{
									this.intros.RemoveAt(j);
									num_dialogo--;
								}
								//Si la intro a destruir es el actual, lo destruimos al final (activando la autodestruccion)
								else if(j == num_dialogo)
								{
									intros [num_dialogo].Autodestruye = true;
								}
								else if(j > num_dialogo)
								{
									this.intros.RemoveAt(j);
								}
							}
						}

						//comprobamos los mensajes con tema
						for(int j = this.DevuelveNumeroTemaMensajes() - 1; j >= 0; j--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(this.temaMensajes[j].IDGrupo == -1)
							{
								for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
								{
									if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
								}
							}
							//si el tema tiene un idgrupo que coincide con el grupo eliminado, eliminamos todo el temaMensaje
							else if(this.temaMensajes[j].IDGrupo == IDGrupo)
							{
								this.temaMensajes.RemoveAt(j);
							}
						}

						//comprobamos los mensajes sin tema
						for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
						{
							if(this.mensajes[j].IDGrupo == IDGrupo)
							{
								this.mensajes.RemoveAt(j);
							}
						}
						break;
					//Si estamos en un mensaje sin tema, comprobamos que posicionamos correctamente el indice en los mensajes
					case 1:
						for(int j = this.DevuelveNumeroIntros() - 1; j >= 0; j--)
						{
							if(this.intros[j].IDGrupo == IDGrupo)
							{
								this.intros.RemoveAt(j);
							}
						}

						for(int j = this.DevuelveNumeroTemaMensajes() - 1; j >= 0; j--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(this.temaMensajes[j].IDGrupo == -1)
							{
								for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
								{
									if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
								}
							}
							else if(this.temaMensajes[j].IDGrupo == IDGrupo)
							{
								this.temaMensajes.RemoveAt(j);
							}
						}

						for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
						{
							if(this.mensajes[j].IDGrupo == IDGrupo)
							{
								//Mantenemos el indice en una posicion correcta
								if (j < num_dialogo)
								{
									this.mensajes.RemoveAt(j);
									num_dialogo--;
								}
								//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
								else if(j == num_dialogo)
								{
									mensajes [num_dialogo].Autodestruye = true;
								}
								else if(j > num_dialogo)
								{
									this.mensajes.RemoveAt(j);
								}
							}
						}
						break;
					//si estamos en un mensaje con tema, comprobamos que posicionamos bien el indice de los mensajes
					case 2:
						for(int j = this.DevuelveNumeroIntros() -1; j >= 0; j--)
						{
							if(this.intros[j].IDGrupo == IDGrupo)
							{
								this.intros.RemoveAt(j);
							}
						}

						for(int j = this.DevuelveNumeroTemaMensajes() -1; j >= 0; j--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(this.temaMensajes[j].IDGrupo == -1)
							{
								for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
								{
									if(this.temaMensajes[j].mensajes[k].IDGrupo == IDGrupo)
									{
										//Si nos encontramos en el mensaje que queremos destruir, activamos la autodestruccion
										if(j == num_tema && k == num_dialogo)
										{
											//Mantenemos el indice en una posicion correcta
											if (k < num_dialogo)
											{
												this.temaMensajes[j].mensajes.RemoveAt(k);
												num_dialogo--;

												//Si el temaMensajes se ha quedado vacío, lo destruimos
												if(temaMensajes[j].mensajes.Count == 0)
												{
													this.temaMensajes.RemoveAt(j);
												}
											}
											//Si el mensaje a destruir es el actual, lo destruimos al final (activando la autodestruccion)
											else if(k == num_dialogo)
											{
												temaMensajes[j].mensajes [num_dialogo].Autodestruye = true;
											}
											else if(k > num_dialogo)
											{
												this.temaMensajes[j].mensajes.RemoveAt(k);

												//Si el temaMensajes se ha quedado vacío, lo destruimos
												if(temaMensajes[j].mensajes.Count == 0)
												{
													this.temaMensajes.RemoveAt(j);
												}
											}
										}
										else
										{
											this.temaMensajes[j].mensajes.RemoveAt(k);

											//Si el temaMensajes se ha quedado vacío, lo destruimos
											if(temaMensajes[j].mensajes.Count == 0)
											{
												this.temaMensajes.RemoveAt(j);
											}
										}
									}
								}
							}
							//Si el tema tiene idgrupo y coincide con el que queremos destruir, comprobamos que no nos encontramos en algun mensaje que debemos destruir
							else if(this.temaMensajes[j].IDGrupo == IDGrupo)
							{
								for(int k = this.temaMensajes[j].mensajes.Count - 1; k >= 0; k--)
								{
									//Si nos encontramos en el mensaje que queremos destruir, activamos la autodestruccion
									if(j == num_tema && k == num_dialogo)
									{
										temaMensajes[j].mensajes [num_dialogo].Autodestruye = true;
									}
									else
									{
										this.temaMensajes[j].mensajes.RemoveAt(k);

										//Si el mensajetema ya no tiene mensajes, lo destruimos
										if(temaMensajes[j].mensajes.Count == 0)
										{
											this.temaMensajes.RemoveAt(j);
										}
									}
								}
							}
						}

						for(int j = this.DevuelveNumeroMensajes() - 1; j >= 0; j--)
						{
							if(this.mensajes[j].IDGrupo == IDGrupo)
							{
								this.mensajes.RemoveAt(j);
							}
						}
						break;
					}

					//Ahora comprobamos a los npc de la cola de objetos del manager
					List<ObjetoSer> listCola = new List<ObjetoSer>();
					listCola = Manager.Instance.GetColaObjetosTipo(typeof(NPC_Dialogo));

					for(var j = 0; j < listCola.Count; j++)
					{
						bool actualizado = false;

						//Buscamos en la cola de objetos
						ObjetoSer objs = listCola[j];
						NPC_Dialogo n_diag = objs as NPC_Dialogo;

						for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
						{
							if(n_diag.intros[k].IDGrupo == IDGrupo)
							{
								n_diag.intros.RemoveAt(k);
								actualizado = true;
							}
						}
						for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
						{
							//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
							//Si el temaMensaje es -1, no forma parte de ningún grupo
							if(n_diag.temaMensajes[k].IDGrupo == -1)
							{
								for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
								{
									if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
									{
										n_diag.temaMensajes[k].mensajes.RemoveAt(l);

										//Si el temaMensajes se ha quedado vacío, lo destruimos
										if(n_diag.temaMensajes[k].mensajes.Count == 0)
										{
											n_diag.temaMensajes.RemoveAt(k);
										}

										actualizado = true;
									}
								}
							}
							else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.temaMensajes.RemoveAt(k);
								actualizado = true;
							}
						}
						for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
						{
							if(n_diag.mensajes[k].IDGrupo == IDGrupo)
							{
								n_diag.mensajes.RemoveAt(k);
								actualizado = true;
							}
						}

						if (actualizado) {
							n_diag.AddToColaObjetos ();
						}
					}

					//Ahora comprobamos a los npcs de la escena actual
					List<GameObject> interactuables = Manager.Instance.GetAllInteractuables();

					//Lista de dialogos no actualizados, utilizado al comprobar los ficheros
					List<NPC_Dialogo> dialogosNoActualizados = new List<NPC_Dialogo>();

					for(int j = 0; j < interactuables.Count; j++)
					{
						GameObject gobj = interactuables[j];
						Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
						List<NPC_Dialogo> dialogos = inter.DevolverDialogos();

						for(int z = 0; z < dialogos.Count; z ++)
						{
							NPC_Dialogo n_diag = dialogos[z];

							//Si el diálogo no está en la cola de objetos, miramos si hay que borrar algo
							//Y el dialogo no es el actual
							if(!Manager.Instance.ColaObjetoExiste(Manager.rutaNPCDialogosGuardados + inter.ID.ToString() + "-" + n_diag.ID.ToString() + ".xml") && !(ID_NPC == inter.ID && ID == n_diag.ID))
							{
								bool actualizado = false;

								for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
								{
									if(n_diag.intros[k].IDGrupo == IDGrupo)
									{
										n_diag.intros.RemoveAt(k);
										actualizado = true;
									}
								}

								for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
								{
									//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
									//Si el temaMensaje es -1, no forma parte de ningún grupo
									if(n_diag.temaMensajes[k].IDGrupo == -1)
									{
										for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
										{
											if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
											{
												n_diag.temaMensajes[k].mensajes.RemoveAt(l);

												//Si el temaMensajes se ha quedado vacío, lo destruimos
												if(n_diag.temaMensajes[k].mensajes.Count == 0)
												{
													n_diag.temaMensajes.RemoveAt(k);
												}

												actualizado = true;
											}
										}
									}
									else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
									{
										n_diag.temaMensajes.RemoveAt(k);
										actualizado = true;
									}
								}

								for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
								{
									if(n_diag.mensajes[k].IDGrupo == IDGrupo)
									{
										n_diag.mensajes.RemoveAt(k);
										actualizado = true;
									}
								}

								//Si se ha actualizado, se añade a la colaObjetos
								//sino, se añade a una lista utilizada a continuación
								if(actualizado)
									n_diag.AddToColaObjetos();
								else
									dialogosNoActualizados.Add(n_diag);
							}
						}
					}

					//Ahora recorremos los ficheros guardadados
					var info = new DirectoryInfo(Manager.rutaNPCDialogosGuardados);
					var fileInfo = info.GetFiles().OrderByDescending( f => f.CreationTime).ToArray(); //los nuevos empiezan al principio de la lista

					for(var j = 0; j < fileInfo.Length; j++)
					{
						//Contiene id_npc-id_diag
						string ids = Path.GetFileNameWithoutExtension(fileInfo[j].Name);

						//Divide el string en
						//arr[0] = ID_NPC
						//arr[1] = ID_DIAG
						string[] arr = ids.Split('-');

						//Comprobamos que no es el dialogo actual
						//O uno de los dialogos que hemos comprobado anteriormente de la escena
						if(!(ID_NPC == Int32.Parse(arr[0]) && ID == Int32.Parse(arr[1])) && !(dialogosNoActualizados.Any(x => x.ID_NPC == Int32.Parse(arr[0]) && x.ID == Int32.Parse(arr[1]))))
						{
							//Buscamos en la cola de objetos
							//Si existe, no hacemos nada
							//Si no existe, comprobamos el dialogo
							if(!Manager.Instance.ColaObjetoExiste(Manager.rutaNPCDialogosGuardados + ids  + ".xml"))
							{
								NPC_Dialogo n_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + ids  + ".xml");

								bool actualizado = false;

								for(int k = n_diag.DevuelveNumeroIntros() - 1; k >= 0; k--)
								{
									if(n_diag.intros[k].IDGrupo == IDGrupo)
									{
										n_diag.intros.RemoveAt(k);
										actualizado = true;
									}
								}

								for(int k = n_diag.DevuelveNumeroTemaMensajes() - 1; k >= 0; k--)
								{
									//si el tema no tiene idgrupo, comprobamos los idgrupos de los mensajes de su interior
									//Si el temaMensaje es -1, no forma parte de ningún grupo
									if(n_diag.temaMensajes[k].IDGrupo == -1)
									{
										for(int l = n_diag.temaMensajes[k].mensajes.Count - 1; l >= 0; l--)
										{
											if(n_diag.temaMensajes[k].mensajes[l].IDGrupo == IDGrupo)
											{
												n_diag.temaMensajes[k].mensajes.RemoveAt(l);

												//Si el temaMensajes se ha quedado vacío, lo destruimos
												if(n_diag.temaMensajes[k].mensajes.Count == 0)
												{
													n_diag.temaMensajes.RemoveAt(k);
												}

												actualizado = true;
											}
										}
									}
									else if(n_diag.temaMensajes[k].IDGrupo == IDGrupo)
									{
										n_diag.temaMensajes.RemoveAt(k);
										actualizado = true;
									}
								}

								for(int k = n_diag.DevuelveNumeroMensajes() - 1; k >= 0; k--)
								{
									if(n_diag.mensajes[k].IDGrupo == IDGrupo)
									{
										n_diag.mensajes.RemoveAt(k);
										actualizado = true;
									}
								}

								if (actualizado) {
									n_diag.AddToColaObjetos ();
								}
							}
						}
					}
				}

				//Por último, eliminamos el grupo del Manager
				/*Si el grupo no está en la lista de grupos activos, se añade a la lista
				de grupos acabados y no podrá ser añadido*/
				Manager.Instance.RemoveFromGruposActivos(IDGrupo);

				//FALTA ENVIAR LOS MENSAJES/INTROS DE FINALIZACIÓN
				//ESTOS SOLO PODRÁN SER ENVIADOS SI EL GRUPO ESTABA EN GRUPOS ACTIVOS
			}
		}
		
		for(int i = 0; i < node.DevuelveNumeroIntros(); i++)
		{
			int prioridad = node.Intros[i].DevuelvePrioridad();
			int ID = node.Intros[i].DevuelveID();
			int IDNpc = node.Intros[i].DevuelveIDNpc();
			int IDDialogo = node.Intros[i].DevuelveIDDialogo();

			Intro intro = Intro.LoadIntro(Manager.rutaIntros + ID.ToString() + ".xml", prioridad);

			//Si la intro forma parte de un grupo y ese grupo ya ha acabado, no es añadida
			if(!Manager.Instance.GrupoAcabadoExiste(intro.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir una intro
				if(IDNpc == this.ID_NPC && IDDialogo == this.ID)
				{
					//Si estamos en una intro y la prioridad es mayor que la actual, cambiamos el indice de dialogo
					if(tipo_dialogo == 0 && prioridad > intros[num_dialogo].DevuelvePrioridad())
					{
						num_dialogo++;
					}

					AnyadirIntro(intro);
				}
				else
				{
					//Buscamos en el diccionario y lo añadimos
					//si no esta en el diccionario, lo añadimos desde el xml
					GameObject gobj = Manager.Instance.GetInteractuables(IDNpc);

					if(gobj != null)
					{
						Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
						NPC_Dialogo dialog = inter.DevolverDialogo(IDDialogo);

						if(dialog == null)
						{
							//Buscamos en la cola de objetos
							ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

							if(cobj != null)
							{
								ObjetoSer objs = cobj.GetObjeto();
								dialog = objs as NPC_Dialogo;
							}
							else
							{
								//Cargamos el dialogo
								//Si existe un fichero guardado, cargamos ese fichero, sino
								//cargamos el fichero por defecto
								if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
								{
									dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
								}
								else
								{
									dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
								}
							}
						}

						dialog.AnyadirIntro(intro);
						dialog.AddToColaObjetos();
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
		}
		 
		for(int i = 0; i < node.DevuelveNumeroMensajes(); i++)
		{
			int ID = node.Mensajes[i].DevuelveID();
			int IDTema = node.Mensajes[i].DevuelveIDTema();
			int IDNpc = node.Mensajes[i].DevuelveIDNpc();
			int IDDialogo = node.Mensajes[i].DevuelveIDDialogo();

			Mensaje mensaje =  Mensaje.LoadMensaje(Manager.rutaMensajes + ID.ToString() + ".xml");

			//Si el mensaje forma parte de un grupo y ese grupo ya ha acabado, no es añadido
			if(!Manager.Instance.GrupoAcabadoExiste(mensaje.DevuelveIDGrupo()))
			{
				//Si nos encontramos en el diálogo el cual queremos añadir un mensaje
				if(IDNpc == this.ID_NPC && IDDialogo == this.ID)
				{
					AnyadirMensaje(IDTema, mensaje);
				}
				else
				{
					//Buscamos en el diccionario y lo añadimos
					//si no esta en el diccionario, lo añadimos desde el xml
					GameObject gobj = Manager.Instance.GetInteractuables(IDNpc);

					if(gobj != null)
					{
						Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;
						NPC_Dialogo dialog = inter.DevolverDialogo(IDDialogo);

						if(dialog == null)
						{
							//Buscamos en la cola de objetos
							ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDialogosGuardados + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");

							if(cobj != null)
							{
								ObjetoSer objs = cobj.GetObjeto();
								dialog = objs as NPC_Dialogo;
							}
							else
							{
								//Cargamos el dialogo
								//Si existe un fichero guardado, cargamos ese fichero, sino
								//cargamos el fichero por defecto
								if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml"))
								{
									dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
								}
								else
								{
									dialog = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString()  + "-" + IDDialogo.ToString() + ".xml");
								}
							}
						}

						dialog.AnyadirMensaje(IDTema, mensaje);
						dialog.AddToColaObjetos();
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
								npc_diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + IDNpc.ToString() + "-" + IDDialogo.ToString() + ".xml");
							}
						}

						npc_diag.AnyadirMensaje(IDTema, mensaje);
						npc_diag.AddToColaObjetos ();
					}
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

					//Comprobamos si el grupo modificado existe en la colaObjetos del Manager
					//Buscamos en la cola de objetos
					ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaGruposModificados + IDGrupo.ToString () + ".xml");

					if(cobj != null)
					{
						ObjetoSer objs = cobj.GetObjeto();
						g = objs as Grupo;
					}
					else
					{
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
					g.AddToColaObjetos ();
				}
			}
		}

		for(int i = 0; i < node.DevuelveNumeroNombres(); i++)
		{
			int IDNpc = node.Nombres[i].DevuelveIDNpc();
			int Indice = node.Nombres[i].DevuelveIndiceNombre();

			GameObject gobj;

			if(IDNpc == -1)
			{
				gobj = Manager.Instance.GetInteractuables(ID);
			}
			else
			{
				gobj = Manager.Instance.GetInteractuables(IDNpc);
			}

			if(gobj != null)
			{
				Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;

				//Si el objeto es un NPC
				if(inter.GetType() == typeof(InteractuableNPC))
				{
					InteractuableNPC intern = inter as InteractuableNPC;

					int indiceActual = intern.DevuelveIndiceNombre();

					if (indiceActual < Indice)
					{
						intern.SetIndiceNombre(Indice);
					}

					intern.AddDatosToColaObjetos();

					//Actualizamos el interactuable para que muestre el nombre modificado
					intern.SetNombre(intern.DevuelveNombreActual());
				}
			}
			else
			{
				InterDatos d;

				ColaObjeto cobj = Manager.Instance.GetColaObjetos(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml");

				if(cobj != null)
				{
					ObjetoSer objs = cobj.GetObjeto();
					d = objs as InterDatos;
				}
				else
				{
					//Si existe un fichero guardado, cargamos ese fichero, sino
					//cargamos el fichero por defecto
					if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml"))
					{
						d = InterDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + IDNpc.ToString()  + ".xml");
					}
					else
					{
						d = InterDatos.LoadInterDatos(Manager.rutaNPCDatos + IDNpc.ToString()  + ".xml");
					}
				}

				if(d.GetType() == typeof(NPCDatos))
				{
					NPCDatos dnpc = d as NPCDatos;
					int indiceActual = dnpc.DevuelveIndiceNombre();

					if (indiceActual < Indice)
					{
						dnpc.SetIndiceNombre(Indice);
					}

					dnpc.AddToColaObjetos ();
				}
			}
		}
	}

	public static NPC_Dialogo LoadNPCDialogue(string path)
	{
		NPC_Dialogo npc_dialogo = Manager.Instance.DeserializeDataWithReturn<NPC_Dialogo>(path);

		return npc_dialogo;
	}

	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos (Manager.rutaNPCDialogosGuardados + ID_NPC.ToString() + "-" + ID.ToString()  + ".xml", this);
	}
}