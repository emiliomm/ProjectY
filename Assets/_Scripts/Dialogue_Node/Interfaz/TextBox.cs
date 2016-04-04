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

	public GameObject DialogueWindowPrefab; //prefab que será la ventana de dialogo

	private bool isActive; //indica si la caja de texto está activa o no

	private NPC_Dialogo npc_dialogo;
	private NPC npc;

	private GameObject dialogue_window;
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

	enum GameState {Intro_Texto, Intro_Opciones, Mensajes_Menu, Mensajes_Texto, Mensajes_Opciones};
	GameState current;

	// Use this when the object is created
	void Awake ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		//CAMBIAR GAMEOBJECT FIND Y AÑADIRLOS DIRECTAMENTE DESDE UNITY CON SERIALIZE

		var canvas = GameObject.Find("Canvas");

		dialogue_window = Instantiate<GameObject>(DialogueWindowPrefab);

		dialogue_window.transform.SetParent(canvas.transform, false);

		RectTransform dia_window_transform = (RectTransform) dialogue_window.transform;

		dia_window_transform.localPosition = new Vector3(0, 0, 0);

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

	public bool EstaActivo()
	{
		return isActive;
	}

	public void StartDialogue(NPC npc_inst, NPC_Dialogo npcDi)
	{
		npc_dialogo  = npcDi;
		npc = npc_inst;
		
		EnableTextBox();
	}

	public void EnableTextBox()
	{
		TP_Controller.Instance.canMove = false;
		isActive = true;

		RunDialogue();
	}

	public void DisableTextBox()
	{
		dialogue_window.SetActive(false);
		TP_Controller.Instance.canMove = true;
		isActive = false;
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
		dialogue_window.SetActive(true);

		int num_dialog = 0;
		int node_id = 0; //principio del dialogo
		bool conversacion_activa = true;

		Dialogue dialog = new Dialogue();

		if (npc_dialogo.DevuelveNumeroIntros() == 0)
			current = GameState.Mensajes_Menu;
		else
		{
			dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
			current = GameState.Intro_Texto;
		}


		while(conversacion_activa)
		{
			switch(current)
			{
			case GameState.Intro_Texto:
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
					current = GameState.Mensajes_Menu;
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogoNPC(ref num_dialog, node_id);
					EliminarDialogo(ref num_dialog);
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
						current = GameState.Mensajes_Menu;
					}

					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, node_id);
					DialogueNode dn = dialog.DevuelveNodo(node_id);
					if(dn.DevuelveNumeroOpciones() > 0)
					{
						current = GameState.Intro_Opciones;
					}
					else
					{
						node_id++;
					}
					break;
				}

				break;
			case GameState.Intro_Opciones:
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
					current = GameState.Mensajes_Menu;
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref num_dialog);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (npc_dialogo.AvanzaIntro(num_dialog))
					{
						num_dialog++;
						dialog = npc_dialogo.DevuelveDialogoIntro(num_dialog);
						node_id = 0;
						current = GameState.Intro_Texto;
					}
					//Sino, se muestran las respuestas
					else
					{
						current = GameState.Mensajes_Menu;
					}
					break;
				default: //Se sigue con la conversación, donde el nodo indique
					node_id = selected_option;
					current = GameState.Intro_Texto;
					break;
				}

				break;
			case GameState.Mensajes_Menu:
				if (npc_dialogo.DevuelveNumeroMensajes() != 0)
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
						current = GameState.Mensajes_Texto;
						num_dialog = selected_option;
						dialog = npc_dialogo.DevuelveDialogoMensajes(num_dialog);
						node_id = 0;
						break;
					}
				}
				else
				{
					conversacion_activa = false;
				}

				break;
			case GameState.Mensajes_Texto:
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
					current = GameState.Mensajes_Menu;
					break;
				case -1: //Acaba el dialogo actual
					current = GameState.Mensajes_Menu;
					EliminarDialogo(ref num_dialog);
					RecorreDialogoNPC(ref num_dialog, node_id);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogoNPC(ref num_dialog, node_id);
					DialogueNode dn = dialog.DevuelveNodo(node_id);
					if(dn.DevuelveNumeroOpciones() > 0)
					{
						current = GameState.Mensajes_Opciones;
					}
					else //CAMBIAR LISTENER
					{
						node_id++;
					}
					break;
				}

				break;
			case GameState.Mensajes_Opciones:
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
					current = GameState.Mensajes_Menu;
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref num_dialog);
					current = GameState.Mensajes_Menu;
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					node_id = selected_option;
					current = GameState.Mensajes_Texto;
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
		npc.ActualizarDialogo(npc_dialogo);
	}

	private void EliminarDialogo(ref int num_dialog)
	{
		switch(current)
		{
		case GameState.Intro_Texto:
		case GameState.Intro_Opciones:
			npc_dialogo.MirarSiDialogoSeAutodestruye(0, ref num_dialog);
			break;
		case GameState.Mensajes_Texto:
		case GameState.Mensajes_Opciones:
			npc_dialogo.MirarSiDialogoSeAutodestruye(1, ref num_dialog);
			break;
		}
	}

	private void RecorreDialogoNPC(ref int num_dialog, int node_id)
	{
		switch(current)
		{
		case GameState.Intro_Texto:
			npc_dialogo.MarcaDialogueNodeComoLeido(0, ref num_dialog, node_id);
			break;
		case GameState.Mensajes_Texto:
			npc_dialogo.MarcaDialogueNodeComoLeido(1, ref num_dialog, node_id);
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

		dialog_text.SetActive(true);
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

	//Muestra el menu de mensajes del dialogo
	private void display_npc_mensajes()
	{
		dialog_options.SetActive(true);
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

		for(int i = 0; i < npc_dialogo.DevuelveNumeroMensajes() && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_question_button(option_1, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 1:
				set_question_button(option_2, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 2:
				set_question_button(option_3, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 3:
				set_question_button(option_4, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 4:
				set_question_button(option_5, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 5:
				set_question_button(option_6, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 6:
				set_question_button(option_7, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 7:
				set_question_button(option_8, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 8:
				set_question_button(option_9, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 9:
				set_question_button(option_10, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 10:
				set_question_button(option_11, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 11:
				set_question_button(option_12, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 12:
				set_question_button(option_13, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 13:
				set_question_button(option_14, npc_dialogo.DevuelveTextoMensaje(i), i);
				break;
			case 14:
				set_question_button(option_15, npc_dialogo.DevuelveTextoMensaje(i), i);
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
			Grupo gp = Manager.Instance.ComprobarGrupo(num_grupo);

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

	private void set_question_button(GameObject button, string texto, int num)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = texto; //Texto del botón
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(num); }); //Listener del botón
	}
}