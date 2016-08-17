using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using DialogueTree;

/*
 * 	Clase que controla la interfaz de la conversación y su comportamiento
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

	private GameObject option_1;
	private GameObject option_2;
	private GameObject option_3;
	private GameObject option_4;
	private GameObject option_5;
	private GameObject option_6;
	private GameObject option_7;
	private GameObject option_8;
	private GameObject option_9;
	private GameObject option_10;
	private GameObject option_11;
	private GameObject option_12;
	private GameObject option_13;
	private GameObject option_14;
	private GameObject option_15;

	private int selected_option = 0; //Almacena la opción escogida

	//Estados de la clase
	public enum State {Ninguno, Intro_Texto, Intro_Opciones, Mensajes_Menu, Mensajes_Tema, Mensajes_Texto, Mensajes_Opciones};

	State _state = State.Ninguno;
	State _prevState;

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
	}
		
	void Start () {
		//Establecemos el estado inicial
		SetState(State.Ninguno);

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

		option_1 = dialog_options.transform.GetChild(0).GetChild(0).gameObject;
		option_2 = dialog_options.transform.GetChild(0).GetChild(1).gameObject;
		option_3 = dialog_options.transform.GetChild(0).GetChild(2).gameObject;
		option_4 = dialog_options.transform.GetChild(0).GetChild(3).gameObject;
		option_5 = dialog_options.transform.GetChild(0).GetChild(4).gameObject;
		option_6 = dialog_options.transform.GetChild(0).GetChild(5).gameObject;
		option_7 = dialog_options.transform.GetChild(0).GetChild(6).gameObject;
		option_8 = dialog_options.transform.GetChild(0).GetChild(7).gameObject;
		option_9 = dialog_options.transform.GetChild(0).GetChild(8).gameObject;
		option_10 = dialog_options.transform.GetChild(0).GetChild(9).gameObject;
		option_11 = dialog_options.transform.GetChild(0).GetChild(10).gameObject;
		option_12 = dialog_options.transform.GetChild(0).GetChild(11).gameObject;
		option_13 = dialog_options.transform.GetChild(0).GetChild(12).gameObject;
		option_14 = dialog_options.transform.GetChild(0).GetChild(13).gameObject;
		option_15 = dialog_options.transform.GetChild(0).GetChild(14).gameObject;

		DisableTextBox();
	}

	//Función que empieza el diálogo
	public void EmpezarDialogo(Interactuable interActual, NPC_Dialogo npcDi)
	{
		StartCoroutine(DialogoCoroutine(interActual, npcDi));
	}

	//Couroutine que establece valores a las variables antes de iniciar el diálogo
	public IEnumerator DialogoCoroutine(Interactuable interActual, NPC_Dialogo npcDi)
	{
		npc_dialogo  = npcDi;
		inter = interActual;

		EnableTextBox();

		//Iniciamos el dialogo en una couroutine para saber cuando ha acabado
		yield return StartCoroutine(IniciaDialogo());
	}

	//Activa el cuadro de diálogo
	private void EnableTextBox()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
		TP_Camera.Instance.toDialogMode();
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();

		dialogue_window.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón
	}

	//Desactiva el cuadro de diálogo
	private void DisableTextBox()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		TP_Camera.Instance.fromDialogMode();
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();

		dialogue_window.SetActive(false);
		Cursor.visible = false; //Oculta el cursor del ratón
	}

	private IEnumerator IniciaDialogo()
	{
		//Inicializamos variables locales
		int num_dialog = 0;
		int num_tema = -1; //solo usado con mensajes, -1 si no hay tema, x si hay tema
		int node_id = 0;
		bool conversacion_activa = true;
		Dialogue dialog = new Dialogue();

		//Si el diálogo tiene intros, asignamos un estado, sino otro
		if (npc_dialogo.DevuelveNumeroIntros() == 0)
		{
			SetState(State.Mensajes_Menu);
		}
		else
		{
			dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
			SetState(State.Intro_Texto);
		}

		//Bucle que controla la conversación
		while(conversacion_activa)
		{
			switch(_state)
			{
			case State.Intro_Texto: //Cuando la intro muestra el texto
				display_node_text (dialog.DevuelveNodo(node_id)); //Muestra el texto del nodo
				selected_option = node_id;

				while (selected_option == node_id) {
					yield return new WaitForSeconds (0.25f);
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					EliminarDialogo(ref num_dialog, num_tema);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (npc_dialogo.AvanzaIntro(num_dialog))
					{
						num_dialog++;
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
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					DialogueNode dn = dialog.DevuelveNodo(node_id);
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
						if (npc_dialogo.AvanzaIntro(num_dialog))
						{
							num_dialog++;
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
				break;
			case State.Intro_Opciones: //Cuando la intro muestra las opciones
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while(selected_option == node_id)
				{
					yield return new WaitForSeconds(0.25f);
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
					if (npc_dialogo.AvanzaIntro(num_dialog))
					{
						num_dialog++;
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
				if ((npc_dialogo.DevuelveNumeroMensajes() + npc_dialogo.DevuelveNumeroTemaMensajes()) != 0)
				{
					display_npc_mensajes();
					selected_option = -4;
					while (selected_option == -4) {
						yield return new WaitForSeconds (0.25f);
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

							SetState(State.Mensajes_Texto);
							dialog = npc_dialogo.DevuelveDialogoMensajes(num_dialog);
							num_tema = -1;
							node_id = 0;
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
					yield return new WaitForSeconds (0.25f);
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
					num_dialog = selected_option;

					SetState(State.Mensajes_Texto);
					dialog = npc_dialogo.DevuelveDialogoTemaMensajes(node_id, num_dialog);
					node_id = 0;
					break;
				}
				break;
			case State.Mensajes_Texto:  //Cuando se muestra el texto del mensaje
				display_node_text(dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while(selected_option == node_id)
				{
					yield return new WaitForSeconds(0.25f);
				}

				//Cambiar sistema de dialogo si sigue con botones
				//salir y cambiar_tema como ahora
				switch(selected_option)
				{
				case -3: //La conversación acaba
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					conversacion_activa = false;
					break;
				case -2: //Se muestran los mensajes
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					EliminarDialogo(ref num_dialog, num_tema);
					SetState(State.Mensajes_Menu);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					DialogueNode dn = dialog.DevuelveNodo(node_id);
					if(dn.DevuelveNumeroOpciones() > 0)
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
				break;
			case State.Mensajes_Opciones: //Cuando se muestran las opciones del mensaje
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = -4;

				while(selected_option == -4)
				{
					yield return new WaitForSeconds(0.25f);
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
				
			//Si la lista de objetos recientes tiene algún objeto, mostramos un popup de los objetos obtenidos
			if(Manager.Instance.devuelveNumeroObjetosRecientes() != 0)
				yield return StartCoroutine(MostrarPopupObjetos());
		}

		FinNPCDialogo();
	}

	//Muestra un popup de los objetos obtenidos durante el dialogo
	private IEnumerator MostrarPopupObjetos()
	{
		//Desactivamos la interfaz del diálogo y mostramos la interfaz de obtención de objetos
		dialogue_window.SetActive(false);
		GameObject panelObjeto = (GameObject)Instantiate(Resources.Load("PanelPopupObjeto"));
		panelObjeto.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

		//Recorremos los objetos obtenidos recientemente
		for(int i = 0; i < Manager.Instance.devuelveNumeroObjetosRecientes(); i++)
		{
			panelObjeto.transform.GetChild(0).GetChild(0).transform.GetComponent<Text>().text = "Has obtenido " + Manager.Instance.devuelveNombreObjetoReciente(i);

			selected_option = -4;

			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
				{ SetSelectedOption(selected_option + 1); }); //Listener del botón

			while (selected_option == -4) {
				yield return new WaitForSeconds (0.25f);
			}
		}

		Manager.Instance.vaciarObjetosRecientes();
		Destroy(panelObjeto);

		dialogue_window.SetActive(true);
	}
		
	private void FinNPCDialogo()
	{
		DisableTextBox();
		GuardarNPCDialogo();
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
	private void RecorreDialogoNPC(ref int num_dialog, int node_id, int num_tema)
	{
		switch(_state)
		{
		case State.Intro_Texto:
			npc_dialogo.MarcaDialogueNodeComoLeido(0, num_tema, ref num_dialog, node_id);
			break;
		case State.Mensajes_Texto:
			if (num_tema == -1)
				npc_dialogo.MarcaDialogueNodeComoLeido(1, num_tema, ref num_dialog, node_id);
			else
				npc_dialogo.MarcaDialogueNodeComoLeido(2, num_tema, ref num_dialog, node_id);
			break;
		}
	}

	//Guarda en una variable la opción actual
	private void SetSelectedOption(int x)
	{
		selected_option = x;
	}

	//Muestra texto del diálogo
	private void display_node_text(DialogueNode node)
	{
		PosicionaCamara(node.posCamara); //Posiciona la cámara

		dialog_options.SetActive(false);
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		iramensajesmenu.SetActive(false);
		exit.SetActive(false);

		dialog_text.SetActive(true);

		dialog_name.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());
		dialog_text.GetComponentInChildren<Text>().text = node.DevuelveTexto();
		dialog_text.GetComponent<Button>().onClick.RemoveAllListeners();
		//Dice hacia adonde continua el dialogo
		dialog_text.GetComponent<Button>().onClick.AddListener(delegate
		{
				int sigOpcion = selected_option + 1;
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
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		iramensajesmenu.SetActive(false);
		exit.SetActive(false);

		dialog_name.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());

		for(int i = 0; i < node.DevuelveNumeroOpciones() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_option_button(option_1, node.DevuelveNumNodoOpciones(i));
				break;
			case 1:
				set_option_button(option_2, node.DevuelveNumNodoOpciones(i));
				break;
			case 2:
				set_option_button(option_3, node.DevuelveNumNodoOpciones(i));
				break;
			case 3:
				set_option_button(option_4, node.DevuelveNumNodoOpciones(i));
				break;
			case 4:
				set_option_button(option_5, node.DevuelveNumNodoOpciones(i));
				break;
			case 5:
				set_option_button(option_6, node.DevuelveNumNodoOpciones(i));
				break;
			case 6:
				set_option_button(option_7, node.DevuelveNumNodoOpciones(i));
				break;
			case 7:
				set_option_button(option_8, node.DevuelveNumNodoOpciones(i));
				break;
			case 8:
				set_option_button(option_9, node.DevuelveNumNodoOpciones(i));
				break;
			case 9:
				set_option_button(option_10, node.DevuelveNumNodoOpciones(i));
				break;
			case 10:
				set_option_button(option_11, node.DevuelveNumNodoOpciones(i));
				break;
			case 11:
				set_option_button(option_12, node.DevuelveNumNodoOpciones(i));
				break;
			case 12:
				set_option_button(option_13, node.DevuelveNumNodoOpciones(i));
				break;
			case 13:
				set_option_button(option_14, node.DevuelveNumNodoOpciones(i));
				break;
			case 14:
				set_option_button(option_15, node.DevuelveNumNodoOpciones(i));
				break;
			}
		}
	}

	private void set_option_button(GameObject button, DialogueOption opt)
	{
		int num_grupo = opt.DevuelveNumeroGrupo(); //Miramos si la opción está asignada a algún grupo

		//No lo está, lo mostramos
		if(num_grupo == -1)
		{
			button.SetActive(true);
			button.GetComponentInChildren<Text>().text = opt.DevuelveTexto(); //Texto del botón
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(opt.DevuelveDestinationNodeID()); }); //Listener del botón
		}
		//La opción está asignada a un grupo
		//Si el grupo está activo y las variables son las adecuadas, mostramos la opción
		//Sino no
		else
		{
			Grupo gp = Manager.Instance.DevolverGrupoActivo(num_grupo);

			if (gp != null)
			{
				List<DialogueOptionGrupoVariables> variables = opt.DevuelveVariables();

				bool mostrar = true;

				for(int i = 0; i < variables.Count; i++)
				{
					//Si la variable de la opcion es mayor que la actual del grupo, no se muestra la opción
					if(variables[i].Valor > gp.variables[variables[i].num_variable])
					{
						mostrar = false;
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
		}
	}

	//Muestra el menu de mensajes del dialogo
	private void display_npc_mensajes()
	{
		//Mantiene el scroll arriba del todo al mostrar opciones
		dialog_options.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialog_options.SetActive(true);
		dialog_text.SetActive(false);
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		iramensajesmenu.SetActive(false);
		exit.SetActive(true);

		int i = 0; //indice

		for(i = 0; i < npc_dialogo.DevuelveNumeroTemaMensajes() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_mensaje_button(option_1, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 1:
				set_mensaje_button(option_2, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 2:
				set_mensaje_button(option_3, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 3:
				set_mensaje_button(option_4, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 4:
				set_mensaje_button(option_5, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 5:
				set_mensaje_button(option_6, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 6:
				set_mensaje_button(option_7, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 7:
				set_mensaje_button(option_8, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 8:
				set_mensaje_button(option_9, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 9:
				set_mensaje_button(option_10, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 10:
				set_mensaje_button(option_11, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 11:
				set_mensaje_button(option_12, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 12:
				set_mensaje_button(option_13, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 13:
				set_mensaje_button(option_14, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 14:
				set_mensaje_button(option_15, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			}
		}

		int j = i;

		for(i = j; i < npc_dialogo.DevuelveNumeroMensajes() + j && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_mensaje_button(option_1, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 1:
				set_mensaje_button(option_2, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 2:
				set_mensaje_button(option_3, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 3:
				set_mensaje_button(option_4, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 4:
				set_mensaje_button(option_5, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 5:
				set_mensaje_button(option_6, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 6:
				set_mensaje_button(option_7, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 7:
				set_mensaje_button(option_8, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 8:
				set_mensaje_button(option_9, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 9:
				set_mensaje_button(option_10, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 10:
				set_mensaje_button(option_11, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 11:
				set_mensaje_button(option_12, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 12:
				set_mensaje_button(option_13, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 13:
				set_mensaje_button(option_14, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 14:
				set_mensaje_button(option_15, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			}
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
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		iramensajesmenu.SetActive(true);
		exit.SetActive(true);

		for(int i = 0; i < tm.DevuelveNumeroMensajes() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_mensaje_button(option_1, tm.DevuelveTextoMensaje(i), i);
				break;
			case 1:
				set_mensaje_button(option_2, tm.DevuelveTextoMensaje(i), i);
				break;
			case 2:
				set_mensaje_button(option_3, tm.DevuelveTextoMensaje(i), i);
				break;
			case 3:
				set_mensaje_button(option_4, tm.DevuelveTextoMensaje(i), i);
				break;
			case 4:
				set_mensaje_button(option_5, tm.DevuelveTextoMensaje(i), i);
				break;
			case 5:
				set_mensaje_button(option_6, tm.DevuelveTextoMensaje(i), i);
				break;
			case 6:
				set_mensaje_button(option_7, tm.DevuelveTextoMensaje(i), i);
				break;
			case 7:
				set_mensaje_button(option_8, tm.DevuelveTextoMensaje(i), i);
				break;
			case 8:
				set_mensaje_button(option_9, tm.DevuelveTextoMensaje(i), i);
				break;
			case 9:
				set_mensaje_button(option_10, tm.DevuelveTextoMensaje(i), i);
				break;
			case 10:
				set_mensaje_button(option_11, tm.DevuelveTextoMensaje(i), i);
				break;
			case 11:
				set_mensaje_button(option_12, tm.DevuelveTextoMensaje(i), i);
				break;
			case 12:
				set_mensaje_button(option_13, tm.DevuelveTextoMensaje(i), i);
				break;
			case 13:
				set_mensaje_button(option_14, tm.DevuelveTextoMensaje(i), i);
				break;
			case 14:
				set_mensaje_button(option_15, tm.DevuelveTextoMensaje(i), i);
				break;
			}
		}
	}

	//Posiciona la cámara
	private void PosicionaCamara(PosicionCamara posC)
	{
		TP_Camera.Instance.PosicionDialogo(posC, Manager.Instance.GetInteractuable(npc_dialogo.ID_NPC));
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
			nombre = inter.DevuelveNombreDialogo();
			break;
		}

		return nombre;
	}
}