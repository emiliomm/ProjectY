using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using DialogueTree;

/*
 * 	Clase que controla la interfaz de la conversación y su comportamiento
 *  Permanece entre escenas, aunque el objeto que la contiene está desactivada cuando no se usa
 *  También muestra los popup de la interfaz, aunque sean lanzados desde otra clase que no sea esta
 */
public class TextBox : MonoBehaviour {

	public static TextBox Instance; //Instancia propia de la clase

	//Interactuable y dialogo de la conversación
	private Interactuable inter;
	private NPC_Dialogo npc_dialogo;

	//Objetos de la interfaz
	private GameObject dialogue_window;
	private GameObject dialog_name;
	private GameObject dialog_text;
	private GameObject dialog_options;
	private GameObject iramensajesmenu;
	private GameObject exit;

	//Hay 15 botones de opciones
	private GameObject[] options;

	private int selected_option = 0; //Almacena la opción escogida

	private List<DialogoCola> dialogoCola;
	public bool comprobandoDialogos;

	//Estados de la clase
	public enum State {Ninguno, Intro_Texto, Intro_Opciones, Mensajes_Menu, Mensajes_Tema, Mensajes_Texto, Mensajes_Opciones};

	private State _state;
	private State _prevState;

	public State CurrentState {
		get { return _state; } 
	}

	public State PrevState {
		get { return _prevState; }
	}

	public void SetState(State newState) {
		_prevState = _state;
		_state = newState;
	}
		
	void Awake ()
	{
		Instance = this;
		dialogoCola = new List<DialogoCola>();

		comprobandoDialogos = false;

		dialogue_window = (GameObject)Instantiate(Resources.Load("PanelDialogoPrefab")); //Cargamos el prefab de la ventana de dialogo
		dialogue_window.transform.SetParent(Manager.Instance.canvasGlobal.transform, false); //Hacemos que la ventana sea hijo del canvas
		RectTransform dia_window_transform = (RectTransform) dialogue_window.transform;
		dia_window_transform.localPosition = new Vector3(0, 0, 0);

		//Inicializamos las variables de objetos de la interfaz
		dialog_name = dialogue_window.transform.GetChild(1).gameObject;
		dialog_text = dialogue_window.transform.GetChild(2).gameObject;
		dialog_options =  dialogue_window.transform.GetChild(3).gameObject;

		iramensajesmenu = dialogue_window.transform.GetChild(4).gameObject;
		exit = dialogue_window.transform.GetChild(5).gameObject;
		iramensajesmenu.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-2);});
		exit.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-3);});

		options = new GameObject[15];
		for(int i = 0; i < 15; i++)
		{
			options[i] = dialog_options.transform.GetChild(0).GetChild(i).gameObject;
		}
	}
		
	void Start () {
		//Establecemos el estado inicial
		SetState(State.Ninguno);

		AcabaDialogo();
	}

	public void MostrarInterfaz()
	{
		dialogue_window.SetActive(true);
	}

	public void OcultarInterfaz()
	{
		dialogue_window.SetActive(false);
	}

	public IEnumerator comprobarDialogos()
	{
		for(int i = dialogoCola.Count - 1; i >= 0; i--)
		{
			comprobandoDialogos = true;
			yield return StartCoroutine(DialogoCoroutine(dialogoCola[i].devuelveDialogo(), dialogoCola[i].devuelveIDEvento()));
			dialogoCola.RemoveAt(i);
		}
		comprobandoDialogos = false;
	}

	//Función que empieza el diálogo
	public void EmpezarDialogo(Interactuable interActual, NPC_Dialogo npcDi)
	{
		StartCoroutine(DialogoCoroutine(interActual, npcDi));
	}

	//Función que empieza el diálogo
	public void EmpezarDialogo(NPC_Dialogo npcDi, int IDEvento)
	{
		if(TP_Controller.Instance.CurrentState == TP_Controller.State.Normal)
			StartCoroutine(DialogoCoroutine(npcDi, IDEvento));
		else
			dialogoCola.Add(new DialogoCola(npcDi, IDEvento));
	}

	//Couroutine que establece valores a las variables antes de iniciar el diálogo
	//Usada también en el dialogo a distancia
	public IEnumerator DialogoCoroutine(Interactuable interActual, NPC_Dialogo npcDi)
	{
		if(!comprobandoDialogos)
			yield return StartCoroutine(comprobarDialogos());

		npc_dialogo  = npcDi;
		inter = interActual;

		TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
		TP_Camera.Instance.toDialogMode();
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();

		dialogue_window.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		//Iniciamos el dialogo en una couroutine para saber cuando ha acabado
		yield return StartCoroutine(IniciaDialogo());
	}

	//Couroutine que establece valores a las variables antes de iniciar el diálogo
	//Usada al activar un diálogo de un evento
	public IEnumerator DialogoCoroutine(NPC_Dialogo npcDi, int IDEvento)
	{
		if(!comprobandoDialogos)
			yield return StartCoroutine(comprobarDialogos());

		npc_dialogo  = npcDi;
		inter = null;

		TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
		TP_Camera.Instance.toDialogMode();
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();

		dialogue_window.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		//Iniciamos el dialogo en una couroutine para saber cuando ha acabado
		yield return StartCoroutine(IniciaDialogo());

		ManagerRutinas.Instance.guardaEvento(IDEvento);
	}

	private IEnumerator IniciaDialogo()
	{
		//Inicializamos variables locales
		int num_dialog = 0;
		int num_tema = -1; //solo usado con mensajes, -1 si no hay tema, x si hay tema
		int node_id = 0;
		bool conversacion_activa = true;
		Dialogue dialog = new Dialogue();

		SetState(State.Intro_Texto);//Estado inicial de la conversación

		//Si el diálogo tiene intros activas, asignamos un estado, sino otro
		if(npc_dialogo.DevuelveNumeroIntrosActivas(ref num_dialog) != 0)
		{
			dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
		}
		else if ((npc_dialogo.DevuelveNumeroMensajesActivos() + npc_dialogo.DevuelveNumeroTemaMensajesActivos()) != 0)
		{
			SetState(State.Mensajes_Menu);
		}
		//El dialogo está vacío, no contiene ningún elemento activo
		//Mostramos un mensaje por defecto
		else
		{
			npc_dialogo  = NPC_Dialogo.LoadNPCDialogue(Manager.rutaDialogoVacio);
			dialog = npc_dialogo.intros[0].DevuelveDialogo();
		}

		//Bucle que controla la conversación
		while(conversacion_activa)
		{
			switch(_state)
			{
			case State.Intro_Texto: //Cuando la intro muestra el texto
				DialogueNode dn = dialog.DevuelveNodo(node_id);
				selected_option = node_id;
				display_node_text (dn); //Muestra el texto del nodo

				while (selected_option == node_id) {
					yield return null;
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					RecorreDialogoNPC(ref num_dialog, num_tema, dn); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
					RecorreDialogoNPC(ref num_dialog, num_tema, dn); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogoNPC(ref num_dialog, num_tema, dn);
					EliminarDialogo(ref num_dialog, num_tema);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (npc_dialogo.AvanzaIntro(ref num_dialog))
					{
						dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
						node_id = 0;
					}
					//Sino, se muestran las respuestas
					else
					{
						SetState(State.Mensajes_Menu);
					}
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, num_tema, dn);

					if(dn.DevuelveNumeroOpciones() > 0)
					{
						SetState(State.Intro_Opciones);
					}
					//Comprobamos si se puede avanzar en el dialogo
					else if(dialog.AvanzaDialogue(node_id))
					{
						//es lo mismo que node_id++;
						node_id = selected_option;
					}
					//Si hemos llegado al final del dialogo, acabamos el dialogo actual
					else
					{
						EliminarDialogo(ref num_dialog, num_tema);
						//Si hay más dialogos, vamos al siguiente dialogo
						if (npc_dialogo.AvanzaIntro(ref num_dialog))
						{
							dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
							node_id = 0;
						}
						//Sino, se muestran las respuestas
						else
						{
							SetState(State.Mensajes_Menu);
						}
					}
					break;
				}

				//Si la lista de objetos recientes tiene algún objeto, mostramos un popup de los objetos obtenidos
				if(Manager.Instance.devuelveNumeroObjetosRecientes() != 0)
					yield return StartCoroutine(InterfazPopUpObjetos());

				break;
			case State.Intro_Opciones: //Cuando la intro muestra las opciones
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while(selected_option == node_id)
				{
					yield return null;
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref num_dialog, num_tema);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (npc_dialogo.AvanzaIntro(ref num_dialog))
					{
						dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
						node_id = 0;
						SetState(State.Intro_Texto);
					}
					//Sino, se muestran las respuestas
					else
					{
						SetState(State.Mensajes_Menu);
					}
					break;
				default: //Se sigue con la conversación, donde el nodo indique
					node_id = selected_option;
					SetState(State.Intro_Texto);
					break;
				}
				break;
			case State.Mensajes_Menu:  //Cuando se muestran el menu de mensajes
				if ((npc_dialogo.DevuelveNumeroMensajesActivos() + npc_dialogo.DevuelveNumeroTemaMensajesActivos()) != 0)
				{
					display_npc_mensajes();
					selected_option = -4;
					while (selected_option == -4) {
						yield return null;
					}

					switch(selected_option)
					{
					//Salimos del dialogo
					case -3:
					case -2:
					case -1:
						conversacion_activa = false;
						break;
					//Cargamos el dialogo escogido
					default:
						//Se ha seleccionado un mensajeTema
						//La selected_option está en el intérvalo [0-numTemaMensajes]
						if (selected_option < npc_dialogo.DevuelveNumeroTemaMensajes())
						{
							SetState(State.Mensajes_Tema);
							node_id = selected_option;
							num_tema = selected_option;
						}
						//Se ha seleccionado un mensaje sin tema
						//La selected_option está en el intérvalo [numTemaMensajes+1-numTemaMensajes+numMensajes(sueltos)]
						else
						{
							num_dialog = selected_option - npc_dialogo.DevuelveNumeroTemaMensajes();

							Mensaje men = npc_dialogo.DevuelveMensaje(num_tema, num_dialog);

							if(men.GetType() == typeof(MensajeDialogo))
							{
								SetState(State.Mensajes_Texto);

								MensajeDialogo menDi = men as MensajeDialogo;
								dialog = menDi.DevuelveDialogo();
								num_tema = -1;
								node_id = 0;
							}
							else if(men.GetType() == typeof(MensajeTienda))
							{
								MensajeTienda menTi = men as MensajeTienda;
								menTi.MostrarTienda();
							}
						}
						break;
					}
				}
				else
				{
					conversacion_activa = false;
				}
				break;
			case State.Mensajes_Tema:  //Cuando se muestran los mensajes de un tema
				display_npc_temaMensajes(npc_dialogo.DevuelveTemaMensaje(node_id));

				selected_option = -4;
				while (selected_option == -4) {
					yield return null;
				}

				switch(selected_option)
				{
				//Salimos del dialogo
				case -3:
				case -2:
				case -1:
					conversacion_activa = false;
					break;
					//Cargamos el mensaje escogido
				default:
					num_dialog = selected_option;

					Mensaje men = npc_dialogo.DevuelveMensaje(num_tema, num_dialog);

					if(men.GetType() == typeof(MensajeDialogo))
					{
						SetState(State.Mensajes_Texto);

						MensajeDialogo menDi = men as MensajeDialogo;
						dialog = menDi.DevuelveDialogo();
						node_id = 0;
					}
					else if(men.GetType() == typeof(MensajeTienda))
					{
						MensajeTienda menTi = men as MensajeTienda;
						menTi.MostrarTienda();
					}
					break;
				}
				break;
			case State.Mensajes_Texto:  //Cuando se muestra el texto del mensaje
				DialogueNode Dn = dialog.DevuelveNodo(node_id);
				selected_option = node_id;
				display_node_text(Dn);

				while(selected_option == node_id)
				{
					yield return null;
				}

				//Cambiar sistema de dialogo si sigue con botones
				//salir y cambiar_tema como ahora
				switch(selected_option)
				{
				case -3: //La conversación acaba
					RecorreDialogoNPC(ref num_dialog, num_tema, Dn); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran los mensajes
					RecorreDialogoNPC(ref num_dialog, num_tema, Dn); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogoNPC(ref num_dialog, num_tema, Dn);
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, num_tema, Dn);

					if(Dn.DevuelveNumeroOpciones() > 0)
					{
						SetState(State.Mensajes_Opciones);
					}
					//Comprobamos si se puede avanzar en el dialogo
					else if(dialog.AvanzaDialogue(node_id))
					{
						//es lo mismo que node_id++;
						node_id = selected_option;
					}
					//Si hemos llegado al final del dialogo, acabamos el dialogo actual
					else
					{
						SetState(State.Mensajes_Menu);
						EliminarDialogo(ref num_dialog, num_tema);
					}
					break;
				}

				//Si la lista de objetos recientes tiene algún objeto, mostramos un popup de los objetos obtenidos
				if(Manager.Instance.devuelveNumeroObjetosRecientes() != 0)
					yield return StartCoroutine(InterfazPopUpObjetos());

				break;
			case State.Mensajes_Opciones: //Cuando se muestran las opciones del mensaje
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = -4;

				while(selected_option == -4)
				{
					yield return null;
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					node_id = selected_option;
					SetState(State.Mensajes_Texto);
					break;
				}
				break;
			}
		}

		FinNPCDialogo();
	}

	private IEnumerator InterfazPopUpObjetos()
	{
		//Desactivamos la interfaz del diálogo y mostramos la interfaz de obtención de objetos
		dialogue_window.SetActive(false);

		yield return StartCoroutine(MostrarPopupObjetos());

		dialogue_window.SetActive(true);
	}

	//Muestra un popup de los objetos obtenidos
	public IEnumerator MostrarPopupObjetos()
	{
		GameObject panelObjeto = (GameObject)Instantiate(Resources.Load("PanelPopupObjeto"));
		panelObjeto.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

		//Recorremos los objetos obtenidos recientemente
		for(int i = 0; i < Manager.Instance.devuelveNumeroObjetosRecientes(); i++)
		{
			panelObjeto.transform.GetChild(0).GetChild(0).transform.GetComponent<Text>().text = "Has obtenido " + Manager.Instance.devuelveNombreObjetoReciente(i);

			var opcion = -4;

			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
				{ opcion = -3; }); //Listener del botón

			while (opcion == -4) {
				yield return null;
			}
		}

		Manager.Instance.vaciarObjetosRecientes();
		Destroy(panelObjeto);
	}
		
	private void FinNPCDialogo()
	{
		AcabaDialogo();
		GuardarNPCDialogo();
	}

	//Desactiva el cuadro de diálogo
	private void AcabaDialogo()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		TP_Camera.Instance.fromDialogMode();
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();

		dialogue_window.SetActive(false);
		Cursor.visible = false; //Oculta el cursor del ratón
	}

	//Serializa determinados objetos cambiados durante el diálogo
	private void GuardarNPCDialogo()
	{
		//El dialogo actual se añade al acabar la conversación, en ningún momento durante esta
		npc_dialogo.AddToColaObjetos();

		Manager.Instance.ActualizarDatos();
	}

	//Comprueba si la intro o el mensaje debe eliminarse
	private void EliminarDialogo(ref int num_dialog, int num_tema)
	{
		switch(_state)
		{
		case State.Intro_Texto:
		case State.Intro_Opciones:
			npc_dialogo.MirarSiDialogoSeAutodestruye(0, num_tema, ref num_dialog);
			break;
		case State.Mensajes_Texto:
		case State.Mensajes_Opciones:
			if (num_tema == -1)
				npc_dialogo.MirarSiDialogoSeAutodestruye(1, num_tema, ref num_dialog);
			else
				npc_dialogo.MirarSiDialogoSeAutodestruye(2, num_tema, ref num_dialog);
			break;
		}
	}

	//Ejecuta funciones del nodo actual del diálogo
	private void RecorreDialogoNPC(ref int num_dialog, int num_tema, DialogueNode dn)
	{
		switch(_state)
		{
		case State.Intro_Texto:
			npc_dialogo.MarcaDialogueNodeComoLeido(0, num_tema, ref num_dialog, dn);
			break;
		case State.Mensajes_Texto:
			if (num_tema == -1)
				npc_dialogo.MarcaDialogueNodeComoLeido(1, num_tema, ref num_dialog, dn);
			else
				npc_dialogo.MarcaDialogueNodeComoLeido(2, num_tema, ref num_dialog, dn);
			break;
		}
	}

	//Muestra texto del diálogo
	private void display_node_text(DialogueNode node)
	{
		PosicionaCamara(node.posCamara); //Posiciona la cámara

		dialog_options.SetActive(false);

		for(int i = 0; i < options.Length; i++)
		{
			options[i].SetActive(false);
		}

		iramensajesmenu.SetActive(false);
		exit.SetActive(false);

		dialog_text.SetActive(true);

		dialog_name.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());
		dialog_text.GetComponentInChildren<Text>().text = node.DevuelveTexto();

		dialog_text.GetComponent<Button>().onClick.RemoveAllListeners();

		var opcion = selected_option;

		//Dice hacia adonde continua el dialogo
		dialog_text.GetComponent<Button>().onClick.AddListener(delegate
		{
				int sigOpcion = opcion + 1;
				switch(node.DevuelveSiguienteNodo())
				{
				//El dialogo acaba
				case -2:
					sigOpcion = -3;
					break;
				//Va hacia el menú de mensajes
				case -1:
					sigOpcion = -2;
					break;
				//opción por defecto(selected_option+1)
				case 0:
				default:
					break;
				}
				SetSelectedOption(sigOpcion);
		}); //Listener del botón
	}

	//Muestra las opciones del dialogo
	private void display_node_options(DialogueNode node)
	{
		dialog_options.SetActive(true);

		//Mantiene el scroll arriba del todo al mostrar opciones
		dialog_options.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialog_text.SetActive(false);

		for(int i = 0; i < options.Length; i++)
		{
			options[i].SetActive(false);
		}

		iramensajesmenu.SetActive(false);
		exit.SetActive(false);

		dialog_name.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());

		for(int i = 0; i < node.DevuelveNumeroOpciones() && i < 14; i++)
		{
			set_option_button(options[i], node.DevuelveNumNodoOpciones(i));
		}
	}

	private void set_option_button(GameObject button, DialogueOption opt)
	{
		bool mostrar = true;

		for(int i = 0; i < opt.DevuelveNumeroGrupos(); i++)
		{
			int IDGrupo = opt.DevuelveIDGrupo(i); //Miramos si la opción está asignada a algún grupo

			Grupo gp = Manager.Instance.DevolverGrupoActivo(IDGrupo);

			if (gp != null)
			{
				List<DialogueOptionGrupoVariables> variables = opt.DevuelveVariables(i);

				for(int j = 0; i < variables.Count; j++)
				{
					//Si la variable de la opcion es mayor que la actual del grupo, no se muestra la opción
					if(variables[j].Valor > gp.variables[variables[j].num_variable])
					{
						mostrar = false;
					}
				}
			}
			//El grupo no está activo
			else
			{
				mostrar = false;
			}
		}

		if(mostrar)
		{
			//Creamos un objeto inventario
			Inventario inventario;

			//Buscamos el inventario en la colaobjetos
			ColaObjeto inventarioCola = Manager.Instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

			//Se ha encontrado en la cola de objetos
			if(inventarioCola != null)
			{
				ObjetoSerializable objs = inventarioCola.GetObjeto();
				inventario = objs as Inventario;
			}
			//No se ha encontrado en la cola de objetos
			else
			{
				//Cargamos el inventario si existe, sino lo creamos
				if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
				{
					inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
				}
				else
				{
					inventario = new Inventario();
				}
			}
				
			for(int i = 0; i < opt.DevuelveNumeroObjetos(); i++)
			{
				int IDObjeto = opt.DevuelveIDObjeto(i);
				bool enPosesion = opt.DevuelveObjetoPosesion(i);

				bool existe = inventario.ObjetoInventarioExiste(IDObjeto);

				//Mostramos la opción solo si coincide el parámetro
				if(enPosesion != existe)
				{
					mostrar = false;
				}
			}
		}

		if(mostrar)
		{
			button.SetActive(true);
			button.GetComponentInChildren<Text>().text = opt.DevuelveTexto(); //Texto del botón
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(opt.DevuelveDestinationNodeID()); }); //Listener del botón
		}
	}

	//Muestra el menu de mensajes del dialogo
	private void display_npc_mensajes()
	{
		//Mantiene el scroll arriba del todo al mostrar opciones
		dialog_options.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialog_options.SetActive(true);
		dialog_text.SetActive(false);

		for(int k = 0; k < options.Length; k++)
		{
			options[k].SetActive(false);
		}

		iramensajesmenu.SetActive(false);
		exit.SetActive(true);

		int i = 0; //indice

		for(i = 0; i < npc_dialogo.DevuelveNumeroTemaMensajes() && i < 14; i++)
		{
			if(npc_dialogo.TemaMensajeEsVisible(i))
				set_mensaje_button(options[i], npc_dialogo.DevuelveTextoTemaMensaje(i), i);
		}

		int j = i;

		for(i = j; i < npc_dialogo.DevuelveNumeroMensajes() + j && i < 14; i++)
		{
			if(npc_dialogo.MensajeEsVisible(i))
				set_mensaje_button(options[i], npc_dialogo.DevuelveTextoMensaje(i-j), i);
		}
	}

	//Crea el botón de un mensaje
	private void set_mensaje_button(GameObject button, string texto, int num)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = texto; //Texto del botón
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(num); }); //Listener del botón
	}

	//Muestra el menu de mensajes del dialogo
	private void display_npc_temaMensajes(TemaMensaje tm)
	{
		//Mantiene el scroll arriba del todo al mostrar opciones
		dialog_options.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialog_options.SetActive(true);
		dialog_text.SetActive(false);

		for(int k = 0; k < options.Length; k++)
		{
			options[k].SetActive(false);
		}

		iramensajesmenu.SetActive(true);
		exit.SetActive(true);

		for(int i = 0; i < tm.DevuelveNumeroMensajes() && i < 14; i++)
		{
			set_mensaje_button(options[i], tm.DevuelveTextoMensaje(i), i);
		}
	}

	//Guarda en una variable la opción actual
	private void SetSelectedOption(int x)
	{
		selected_option = x;
	}

	//Posiciona la cámara, si el interactuable no es nulo
	private void PosicionaCamara(PosicionCamara posC)
	{
		if(inter != null)
			TP_Camera.Instance.PosicionDialogo(posC, Manager.Instance.GetInteractuable(inter.ID));
	}

	//Devuelve el nombre de quién habla según un entero
	private string DevuelveNombre(int num)
	{
		string nombre = "";

		switch(num)
		{
		case -2:
			nombre = Manager.Instance.DevuelveNombreJugador();
			break;
		case -1:
			if(inter != null)
				nombre = inter.DevuelveNombreDialogo();
			else
				nombre = "";
			break;
		}

		return nombre;
	}
}