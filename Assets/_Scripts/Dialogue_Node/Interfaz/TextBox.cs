﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

using DialogueTree;

public class TextBox : MonoBehaviour {

	public static TextBox Instance; //Instancia propia de la clase

	public GameObject DialogueWindowPrefab; //prefab que será la ventana de dialogo, NO ESTÁ EN USO

	private NPC_Dialogo npc_dialogo;
	private Interactuable inter;

	private GameObject dialogue_window;
	private GameObject dialog_name;
	private GameObject dialog_text;
	private GameObject dialog_options;
	private GameObject change_theme;
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

	private int selected_option = 0;

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

	// Use this when the object is created
	//HACER OBJETO PERDURABLE ENTRE ESCENAS, QUE SEA CREADO POR EL MANAGER
	void Awake ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		SetState(State.Ninguno);

		//Cargamos el prefab del canvas de Resources, así como la ventana de dialogo
		var canvas = (GameObject)Instantiate(Resources.Load("CanvasPrefab"));
		dialogue_window = (GameObject)Instantiate(Resources.Load("PanelDialogoPrefab"));

		dialogue_window.transform.SetParent(canvas.transform, false);

		RectTransform dia_window_transform = (RectTransform) dialogue_window.transform;

		dia_window_transform.localPosition = new Vector3(0, 0, 0);

		//Quitar gameobject find
		dialog_name = GameObject.Find("Text_NpcName");
		dialog_text = GameObject.Find("Text_DiaNodeText");

		dialog_options = GameObject.Find("ScrollView");

		option_1 = GameObject.Find("Button_Option1");
		option_2 = GameObject.Find("Button_Option2");
		option_3 = GameObject.Find("Button_Option3");
		option_4 = GameObject.Find("Button_Option4");
		option_5 = GameObject.Find("Button_Option5");
		option_6 = GameObject.Find("Button_Option6");
		option_7 = GameObject.Find("Button_Option7");
		option_8 = GameObject.Find("Button_Option8");
		option_9 = GameObject.Find("Button_Option9");
		option_10 = GameObject.Find("Button_Option10");
		option_11 = GameObject.Find("Button_Option11");
		option_12 = GameObject.Find("Button_Option12");
		option_13 = GameObject.Find("Button_Option13");
		option_14 = GameObject.Find("Button_Option14");
		option_15 = GameObject.Find("Button_Option15");

		change_theme = GameObject.Find("Button_ChangeTheme");
		exit = GameObject.Find("Button_End");

		change_theme.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-2);});
		exit.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-3);});

		DisableTextBox();
	}

	public void StartDialogue(Interactuable interActual, NPC_Dialogo npcDi)
	{
		npc_dialogo  = npcDi;
		inter = interActual;

		TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
		EnableTextBox();
	}

	public void EnableTextBox()
	{
		dialogue_window.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		RunDialogue();
	}

	public void DisableTextBox()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);

		dialogue_window.SetActive(false);
		Cursor.visible = false; //Oculta el cursor del ratón
	}

	public void RunDialogue()
	{
		StartCoroutine(run());
	}

	public void SetSelectedOption(int x)
	{
		selected_option = x;
	}

	public IEnumerator run()
	{
		//principio del dialogo
		int num_tema = -1; //solo usado con mensajes, -1 si no hay tema, x si hay tema
		int num_dialog = 0;
		int node_id = 0;
		bool conversacion_activa = true;

		Dialogue dialog = new Dialogue();

		if (npc_dialogo.DevuelveNumeroIntros() == 0)
			SetState(State.Mensajes_Menu);
		else
		{
			dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
			SetState(State.Intro_Texto);
		}
			
		while(conversacion_activa)
		{
			switch(_state)
			{
			case State.Intro_Texto:
				display_node_text (dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while (selected_option == node_id) {
					yield return new WaitForSeconds (0.25f);
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
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
					else
					{
						node_id++;
					}
					break;
				}

				break;
			case State.Intro_Opciones:
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while(selected_option == node_id)
				{
					yield return new WaitForSeconds(0.25f);
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
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
			case State.Mensajes_Menu:
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
						if (selected_option < npc_dialogo.DevuelveNumeroTemaMensajes())
						{
							SetState(State.Mensajes_Tema);
							node_id = selected_option;
							num_tema = selected_option;
						}
						//Se ha seleccionado un mensaje sin tema
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
			case State.Mensajes_Tema:
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
			case State.Mensajes_Texto:
				display_node_text(dialog.DevuelveNodo(node_id));
				selected_option = node_id;

				while(selected_option == node_id)
				{
					yield return new WaitForSeconds(0.25f);
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					SetState(State.Mensajes_Menu);
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					EliminarDialogo(ref num_dialog, num_tema);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, node_id, num_tema);
					DialogueNode dn = dialog.DevuelveNodo(node_id);
					if(dn.DevuelveNumeroOpciones() > 0)
					{
						SetState(State.Mensajes_Opciones);
					}
					else
					{
						node_id++;
					}
					break;
				}

				break;
			case State.Mensajes_Opciones:
				display_node_options(dialog.DevuelveNodo(node_id));
				selected_option = -4;

				while(selected_option == -4)
				{
					yield return new WaitForSeconds(0.25f);
				}

				switch(selected_option)
				{
				case -3: //La conversación acaba
					conversacion_activa = false;
					break;
				case -2: //Se muestran las respuestas
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

	private void FinNPCDialogo()
	{
		DisableTextBox();
		GuardarNPCDialogo();
	}

	private void GuardarNPCDialogo()
	{
		//El dialogo actual se añade solo cuando acaba el dialogo, no antes
		npc_dialogo.AddToColaObjetos();

		Manager.Instance.ActualizarDatos ();
	}

	private void EliminarDialogo(ref int num_dialog, int num_tema)
	{
		switch(_state)
		{
		case State.Intro_Texto:
		case State.Intro_Opciones:
			npc_dialogo.MirarSiDialogoSeAutodestruye(0, ref num_dialog, num_tema);
			break;
		case State.Mensajes_Texto:
		case State.Mensajes_Opciones:
			if (num_tema == -1)
				npc_dialogo.MirarSiDialogoSeAutodestruye(1, ref num_dialog, num_tema);
			else
				npc_dialogo.MirarSiDialogoSeAutodestruye(2, ref num_dialog, num_tema);
			break;
		}
	}

	private void RecorreDialogoNPC(ref int num_dialog, int node_id, int num_tema)
	{
		switch(_state)
		{
		case State.Intro_Texto:
			npc_dialogo.MarcaDialogueNodeComoLeido(0, ref num_dialog, node_id, num_tema);
			break;
		case State.Mensajes_Texto:
			if (num_tema == -1)
				npc_dialogo.MarcaDialogueNodeComoLeido(1, ref num_dialog, node_id, num_tema);
			else
				npc_dialogo.MarcaDialogueNodeComoLeido(2, ref num_dialog, node_id, num_tema);
			break;
		}
	}

	//Muestra texto del diálogo
	private void display_node_text(DialogueNode node)
	{
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
		change_theme.SetActive(true);

		dialog_name.SetActive(true);
		dialog_text.SetActive(true);

		dialog_name.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());
		dialog_text.GetComponentInChildren<Text>().text = node.DevuelveTexto();
		dialog_text.GetComponent<Button>().onClick.RemoveAllListeners();
		dialog_text.GetComponent<Button>().onClick.AddListener(delegate {SetSelectedOption(selected_option+1);}); //Listener del botón
	}

	//Muestra las opciones del dialogo
	private void display_node_options(DialogueNode node)
	{
		dialog_options.SetActive(true);

		//Mantiene el scroll arriba del todo al mostrar opciones
		dialog_options.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialog_name.SetActive(false);
		dialog_text.SetActive(false);
		change_theme.SetActive(true);
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

		for(int i = 0; i < node.DevuelveNumeroOpciones() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_option_button(option_1, node.DevuelveNodoOpciones(i));
				break;
			case 1:
				set_option_button(option_2, node.DevuelveNodoOpciones(i));
				break;
			case 2:
				set_option_button(option_3, node.DevuelveNodoOpciones(i));
				break;
			case 3:
				set_option_button(option_4, node.DevuelveNodoOpciones(i));
				break;
			case 4:
				set_option_button(option_5, node.DevuelveNodoOpciones(i));
				break;
			case 5:
				set_option_button(option_6, node.DevuelveNodoOpciones(i));
				break;
			case 6:
				set_option_button(option_7, node.DevuelveNodoOpciones(i));
				break;
			case 7:
				set_option_button(option_8, node.DevuelveNodoOpciones(i));
				break;
			case 8:
				set_option_button(option_9, node.DevuelveNodoOpciones(i));
				break;
			case 9:
				set_option_button(option_10, node.DevuelveNodoOpciones(i));
				break;
			case 10:
				set_option_button(option_11, node.DevuelveNodoOpciones(i));
				break;
			case 11:
				set_option_button(option_12, node.DevuelveNodoOpciones(i));
				break;
			case 12:
				set_option_button(option_13, node.DevuelveNodoOpciones(i));
				break;
			case 13:
				set_option_button(option_14, node.DevuelveNodoOpciones(i));
				break;
			case 14:
				set_option_button(option_15, node.DevuelveNodoOpciones(i));
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
		//Comprobamos si el grupo está activo
		//Si lo está, mostramos la opción, sino no
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
		dialog_name.SetActive(false);
		dialog_text.SetActive(false);
		change_theme.SetActive(false);
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

		int i = 0; //indice

		for(i = 0; i < npc_dialogo.DevuelveNumeroTemaMensajes() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_question_button(option_1, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 1:
				set_question_button(option_2, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 2:
				set_question_button(option_3, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 3:
				set_question_button(option_4, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 4:
				set_question_button(option_5, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 5:
				set_question_button(option_6, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 6:
				set_question_button(option_7, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 7:
				set_question_button(option_8, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 8:
				set_question_button(option_9, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 9:
				set_question_button(option_10, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 10:
				set_question_button(option_11, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 11:
				set_question_button(option_12, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 12:
				set_question_button(option_13, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 13:
				set_question_button(option_14, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			case 14:
				set_question_button(option_15, npc_dialogo.DevuelveTextoTemaMensaje(i), i);
				break;
			}
		}

		int j = i;

		for(i = j; i < npc_dialogo.DevuelveNumeroMensajes() + j && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_question_button(option_1, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 1:
				set_question_button(option_2, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 2:
				set_question_button(option_3, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 3:
				set_question_button(option_4, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 4:
				set_question_button(option_5, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 5:
				set_question_button(option_6, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 6:
				set_question_button(option_7, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 7:
				set_question_button(option_8, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 8:
				set_question_button(option_9, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 9:
				set_question_button(option_10, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 10:
				set_question_button(option_11, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 11:
				set_question_button(option_12, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 12:
				set_question_button(option_13, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 13:
				set_question_button(option_14, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			case 14:
				set_question_button(option_15, npc_dialogo.DevuelveTextoMensaje(i-j), i);
				break;
			}
		}
	}

	private void set_question_button(GameObject button, string texto, int num)
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
		dialog_name.SetActive(false);
		dialog_text.SetActive(false);
		change_theme.SetActive(false);
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

		for(int i = 0; i < tm.DevuelveNumeroMensajes() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_question_button(option_1, tm.DevuelveTextoMensaje(i), i);
				break;
			case 1:
				set_question_button(option_2, tm.DevuelveTextoMensaje(i), i);
				break;
			case 2:
				set_question_button(option_3, tm.DevuelveTextoMensaje(i), i);
				break;
			case 3:
				set_question_button(option_4, tm.DevuelveTextoMensaje(i), i);
				break;
			case 4:
				set_question_button(option_5, tm.DevuelveTextoMensaje(i), i);
				break;
			case 5:
				set_question_button(option_6, tm.DevuelveTextoMensaje(i), i);
				break;
			case 6:
				set_question_button(option_7, tm.DevuelveTextoMensaje(i), i);
				break;
			case 7:
				set_question_button(option_8, tm.DevuelveTextoMensaje(i), i);
				break;
			case 8:
				set_question_button(option_9, tm.DevuelveTextoMensaje(i), i);
				break;
			case 9:
				set_question_button(option_10, tm.DevuelveTextoMensaje(i), i);
				break;
			case 10:
				set_question_button(option_11, tm.DevuelveTextoMensaje(i), i);
				break;
			case 11:
				set_question_button(option_12, tm.DevuelveTextoMensaje(i), i);
				break;
			case 12:
				set_question_button(option_13, tm.DevuelveTextoMensaje(i), i);
				break;
			case 13:
				set_question_button(option_14, tm.DevuelveTextoMensaje(i), i);
				break;
			case 14:
				set_question_button(option_15, tm.DevuelveTextoMensaje(i), i);
				break;
			}
		}
	}


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