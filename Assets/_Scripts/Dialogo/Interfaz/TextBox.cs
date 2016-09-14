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

	public static TextBox instance; //Instancia propia de la clase

	//Interactuable y dialogo de la conversación
	private Interactuable interactuable;
	private Dialogo dialogo;

	//Objetos de la interfaz
	private GameObject dialogWindow;
	private GameObject dialogName;
	private GameObject dialogText;
	private GameObject dialogOptions;
	private GameObject irAMensajesMenu;
	private GameObject exit;

	//Hay 15 botones de opciones
	private GameObject[] options;

	private int selectedOption = 0; //Almacena la opción escogida

	private int dialogosEnCola;

	//Estados de la clase
	public enum State {Ninguno, Intro_Texto, Intro_Opciones, Mensajes_Menu, Mensajes_Tema, Mensajes_Texto, Mensajes_Opciones};

	private State state;
	private State prevState;

	public State CurrentState {
		get { return state; } 
	}

	public State PrevState {
		get { return prevState; }
	}

	public void SetState(State newState) {
		prevState = state;
		state = newState;
	}
		
	void Awake ()
	{
		instance = this;

		dialogWindow = (GameObject)Instantiate(Resources.Load("PanelDialogoPrefab")); //Cargamos el prefab de la ventana de dialogo
		dialogWindow.transform.SetParent(Manager.Instance.canvasGlobal.transform, false); //Hacemos que la ventana sea hijo del canvas
		RectTransform diaWindowTransform = (RectTransform) dialogWindow.transform;
		diaWindowTransform.localPosition = new Vector3(0, 0, 0);

		//Inicializamos las variables de objetos de la interfaz
		dialogName = dialogWindow.transform.GetChild(1).gameObject;
		dialogText = dialogWindow.transform.GetChild(2).gameObject;
		dialogOptions =  dialogWindow.transform.GetChild(3).gameObject;

		irAMensajesMenu = dialogWindow.transform.GetChild(4).gameObject;
		exit = dialogWindow.transform.GetChild(5).gameObject;
		irAMensajesMenu.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-2);});
		exit.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-3);});

		options = new GameObject[15];
		for(int i = 0; i < 15; i++)
		{
			options[i] = dialogOptions.transform.GetChild(0).GetChild(i).gameObject;
		}

		//Establecemos el estado inicial
		SetState(State.Ninguno);

		dialogosEnCola = 0;

		OcultarInterfaz();
	}

	public void MostrarInterfaz()
	{
		dialogWindow.SetActive(true);
	}

	public void OcultarInterfaz()
	{
		dialogWindow.SetActive(false);
	}

	public void PrepararDialogo(Interactuable interactuableActual, Dialogo dialogo, int IDEvento)
	{
		StartCoroutine(PrepararDialogoCoroutine(interactuableActual, dialogo, IDEvento));
	}

	public IEnumerator PrepararDialogoCoroutine(Interactuable interactuableActual, Dialogo dialogo, int IDEvento)
	{
		if(TP_Controller.Instance.CurrentState != TP_Controller.State.Normal)
		{
			dialogosEnCola++;

			while(TP_Controller.Instance.CurrentState != TP_Controller.State.Normal)
			{
				yield return null;
			}

			yield return StartCoroutine(EmpezarDialogo(interactuableActual, dialogo));

			dialogosEnCola--;
		}
		else if(dialogosEnCola != 0)
		{
			while(dialogosEnCola != 0)
			{
				yield return null;
			}

			yield return StartCoroutine(EmpezarDialogo(interactuableActual, dialogo));
		}
		else
			yield return StartCoroutine(EmpezarDialogo(interactuableActual, dialogo));

		if(IDEvento != -1)
			ManagerRutinas.Instance.guardaEvento(IDEvento);
	}

	public IEnumerator EmpezarDialogo(Interactuable interactuableActual, Dialogo dialogo)
	{
		this.dialogo  = dialogo;
		this.interactuable = interactuableActual;

		TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
		TP_Camera.Instance.toDialogMode();
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();

		dialogWindow.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		//Iniciamos el dialogo en una couroutine para saber cuando ha acabado
		yield return StartCoroutine(Conversacion());
	}

	private IEnumerator Conversacion()
	{
		//Inicializamos variables locales
		int numDialog = 0;
		int numtema = -1; //solo usado con mensajes, -1 si no hay tema, x si hay tema
		int nodeID = 0;
		bool conversacionActiva = true;
		Dialogue dialogue = new Dialogue();

		SetState(State.Intro_Texto);//Estado inicial de la conversación

		//Si el diálogo tiene intros activas, asignamos un estado, sino otro
		if(dialogo.DevuelveNumeroIntrosActivas(ref numDialog) != 0)
		{
			dialogue = dialogo.DevuelveDialogoIntro(numDialog);
		}
		else if ((dialogo.DevuelveNumeroMensajesActivos() + dialogo.DevuelveNumeroTemaMensajesActivos()) != 0)
		{
			SetState(State.Mensajes_Menu);
		}
		//El dialogo está vacío, no contiene ningún elemento activo
		//Mostramos un mensaje por defecto
		else
		{
			dialogo  = Dialogo.LoadDialogo(Manager.rutaDialogoVacio);
			dialogue = dialogo.intros[0].DevuelveDialogo();
		}

		//Bucle que controla la conversación
		while(conversacionActiva)
		{
			switch(state)
			{
			case State.Intro_Texto: //Cuando la intro muestra el texto
				DialogueNode dialogueNode = dialogue.DevuelveNodo(nodeID);
				selectedOption = nodeID;
				DisplayNodeText (dialogueNode); //Muestra el texto del nodo

				while (selectedOption == nodeID) {
					yield return null;
				}

				switch(selectedOption)
				{
				case -3: //La conversación acaba
					RecorreDialogo(ref numDialog, numtema, dialogueNode); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref numDialog, numtema);
					conversacionActiva = false;
					break;
				case -2: //Se muestran las respuestas
					RecorreDialogo(ref numDialog, numtema, dialogueNode); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref numDialog, numtema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogo(ref numDialog, numtema, dialogueNode);
					EliminarDialogo(ref numDialog, numtema);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (dialogo.AvanzaIntro(ref numDialog))
					{
						dialogue = dialogo.DevuelveDialogoIntro(numDialog);
						nodeID = 0;
					}
					//Sino, se muestran las respuestas
					else
					{
						SetState(State.Mensajes_Menu);
					}
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogo(ref numDialog, numtema, dialogueNode);

					if(dialogueNode.DevuelveNumeroOpciones() > 0)
					{
						SetState(State.Intro_Opciones);
					}
					//Comprobamos si se puede avanzar en el dialogo
					else if(dialogue.AvanzaDialogue(nodeID))
					{
						//es lo mismo que node_id++;
						nodeID = selectedOption;
					}
					//Si hemos llegado al final del dialogo, acabamos el dialogo actual
					else
					{
						EliminarDialogo(ref numDialog, numtema);
						//Si hay más dialogos, vamos al siguiente dialogo
						if (dialogo.AvanzaIntro(ref numDialog))
						{
							dialogue = dialogo.DevuelveDialogoIntro(numDialog);
							nodeID = 0;
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
				DisplayNodeOptions(dialogue.DevuelveNodo(nodeID));
				selectedOption = nodeID;

				while(selectedOption == nodeID)
				{
					yield return null;
				}

				switch(selectedOption)
				{
				case -3: //La conversación acaba
					EliminarDialogo(ref numDialog, numtema);
					conversacionActiva = false;
					break;
				case -2: //Se muestran las respuestas
					EliminarDialogo(ref numDialog, numtema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref numDialog, numtema);
					//Si hay más dialogos, vamos al siguiente dialogo
					if (dialogo.AvanzaIntro(ref numDialog))
					{
						dialogue = dialogo.DevuelveDialogoIntro(numDialog);
						nodeID = 0;
						SetState(State.Intro_Texto);
					}
					//Sino, se muestran las respuestas
					else
					{
						SetState(State.Mensajes_Menu);
					}
					break;
				default: //Se sigue con la conversación, donde el nodo indique
					nodeID = selectedOption;
					SetState(State.Intro_Texto);
					break;
				}
				break;
			case State.Mensajes_Menu:  //Cuando se muestran el menu de mensajes
				if ((dialogo.DevuelveNumeroMensajesActivos() + dialogo.DevuelveNumeroTemaMensajesActivos()) != 0)
				{
					DisplayMensajes();
					selectedOption = -4;
					while (selectedOption == -4) {
						yield return null;
					}

					switch(selectedOption)
					{
					//Salimos del dialogo
					case -3:
					case -2:
					case -1:
						conversacionActiva = false;
						break;
					//Cargamos el dialogo escogido
					default:
						//Se ha seleccionado un mensajeTema
						//La selected_option está en el intérvalo [0-numTemaMensajes]
						if (selectedOption < dialogo.DevuelveNumeroTemaMensajes())
						{
							nodeID = selectedOption;
							numtema = selectedOption;
							SetState(State.Mensajes_Tema);
						}
						//Se ha seleccionado un mensaje sin tema
						//La selected_option está en el intérvalo [numTemaMensajes+1-numTemaMensajes+numMensajes(sueltos)]
						else
						{
							numDialog = selectedOption - dialogo.DevuelveNumeroTemaMensajes();

							numtema = -1;
							Mensaje mensaje = dialogo.DevuelveMensaje(numtema, numDialog);

							if(mensaje.GetType() == typeof(MensajeDialogo))
							{
								MensajeDialogo mensajeDialogo = mensaje as MensajeDialogo;
								dialogue = mensajeDialogo.DevuelveDialogo();
								nodeID = 0;

								SetState(State.Mensajes_Texto);
							}
							else if(mensaje.GetType() == typeof(MensajeTienda))
							{
								MensajeTienda mensajeTienda = mensaje as MensajeTienda;
								mensajeTienda.MostrarTienda();
							}
						}
						break;
					}
				}
				else
				{
					conversacionActiva = false;
				}
				break;
			case State.Mensajes_Tema:  //Cuando se muestran los mensajes de un tema
				DisplayTemaMensajes(dialogo.DevuelveTemaMensaje(nodeID));

				selectedOption = -4;
				while (selectedOption == -4) {
					yield return null;
				}

				switch(selectedOption)
				{
				//Salimos del dialogo
				case -3:
				case -1:
					conversacionActiva = false;
					break;
				//Vamos al menú mensajes
				case -2:
					SetState(State.Mensajes_Menu);
					break;
				//Cargamos el mensaje escogido
				default:
					numDialog = selectedOption;

					Mensaje mensaje = dialogo.DevuelveMensaje(numtema, numDialog);

					if(mensaje.GetType() == typeof(MensajeDialogo))
					{
						MensajeDialogo mensajeDialogo = mensaje as MensajeDialogo;
						dialogue = mensajeDialogo.DevuelveDialogo();
						nodeID = 0;

						SetState(State.Mensajes_Texto);
					}
					else if(mensaje.GetType() == typeof(MensajeTienda))
					{
						MensajeTienda mensajeTienda = mensaje as MensajeTienda;
						mensajeTienda.MostrarTienda();
					}
					break;
				}
				break;
			case State.Mensajes_Texto:  //Cuando se muestra el texto del mensaje
				DialogueNode diaNode = dialogue.DevuelveNodo(nodeID);
				selectedOption = nodeID;
				DisplayNodeText(diaNode);

				while(selectedOption == nodeID)
				{
					yield return null;
				}

				//Cambiar sistema de dialogo si sigue con botones
				//salir y cambiar_tema como ahora
				switch(selectedOption)
				{
				case -3: //La conversación acaba
					RecorreDialogo(ref numDialog, numtema, diaNode); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref numDialog, numtema);
					conversacionActiva = false;
					break;
				case -2: //Se muestran los mensajes
					RecorreDialogo(ref numDialog, numtema, diaNode); //Dejar si el dialogo se deja como ahora
					EliminarDialogo(ref numDialog, numtema);
					SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					RecorreDialogo(ref numDialog, numtema, diaNode);
					EliminarDialogo(ref numDialog, numtema);
					SetState(State.Mensajes_Menu);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					RecorreDialogo(ref numDialog, numtema, diaNode);

					if(diaNode.DevuelveNumeroOpciones() > 0)
					{
						SetState(State.Mensajes_Opciones);
					}
					//Comprobamos si se puede avanzar en el dialogo
					else if(dialogue.AvanzaDialogue(nodeID))
					{
						//es lo mismo que node_id++;
						nodeID = selectedOption;
					}
					//Si hemos llegado al final del dialogo, acabamos el dialogo actual
					else
					{
						EliminarDialogo(ref numDialog, numtema);

						if(numtema != -1)
						{
							nodeID = numtema;
							SetState(State.Mensajes_Tema);
						}
						else
							SetState(State.Mensajes_Menu);
					}
					break;
				}

				//Si la lista de objetos recientes tiene algún objeto, mostramos un popup de los objetos obtenidos
				if(Manager.Instance.devuelveNumeroObjetosRecientes() != 0)
					yield return StartCoroutine(InterfazPopUpObjetos());

				break;
			case State.Mensajes_Opciones: //Cuando se muestran las opciones del mensaje
				DisplayNodeOptions(dialogue.DevuelveNodo(nodeID));
				selectedOption = -4;

				while(selectedOption == -4)
				{
					yield return null;
				}

				switch(selectedOption)
				{
				case -3: //La conversación acaba
					EliminarDialogo(ref numDialog, numtema);
					conversacionActiva = false;
					break;
				case -2: //Se muestran las respuestas
					EliminarDialogo(ref numDialog, numtema);

					if(numtema != -1)
					{
						nodeID = numtema;
						SetState(State.Mensajes_Tema);
					}
					else
						SetState(State.Mensajes_Menu);
					break;
				case -1: //Acaba el dialogo actual
					EliminarDialogo(ref numDialog, numtema);

					if(numtema != -1)
					{
						nodeID = numtema;
						SetState(State.Mensajes_Tema);
					}
					else
						SetState(State.Mensajes_Menu);
					break;
				default: //Si el nodo tiene opciones de dialogo, se muestran, sino, se pasa al siguiente texto
					nodeID = selectedOption;

					if(numtema != -1)
					{
						nodeID = numtema;
						SetState(State.Mensajes_Tema);
					}
					else
						SetState(State.Mensajes_Menu);
					break;
				}
				break;
			}
		}

		FinDialogo();
	}

	private IEnumerator InterfazPopUpObjetos()
	{
		//Desactivamos la interfaz del diálogo y mostramos la interfaz de obtención de objetos
		dialogWindow.SetActive(false);

		yield return StartCoroutine(MostrarPopupObjetos());

		dialogWindow.SetActive(true);
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
		
	private void FinDialogo()
	{
		AcabaDialogo();
		GuardarDialogo();
	}

	//Desactiva el cuadro de diálogo
	private void AcabaDialogo()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		TP_Camera.Instance.fromDialogMode();
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();

		dialogWindow.SetActive(false);
		Cursor.visible = false; //Oculta el cursor del ratón
	}

	//Serializa determinados objetos cambiados durante el diálogo
	private void GuardarDialogo()
	{
		//El dialogo actual se añade al acabar la conversación, en ningún momento durante esta
		dialogo.AddToColaObjetos();

		Manager.Instance.ActualizarDatos();
	}

	//Comprueba si la intro o el mensaje debe eliminarse
	private void EliminarDialogo(ref int numDialogo, int numTema)
	{
		switch(state)
		{
		case State.Intro_Texto:
		case State.Intro_Opciones:
			dialogo.MirarSiDialogoSeAutodestruye(0, numTema, ref numDialogo);
			break;
		case State.Mensajes_Texto:
		case State.Mensajes_Opciones:
			if (numTema == -1)
				dialogo.MirarSiDialogoSeAutodestruye(1, numTema, ref numDialogo);
			else
				dialogo.MirarSiDialogoSeAutodestruye(2, numTema, ref numDialogo);
			break;
		}
	}

	//Ejecuta funciones del nodo actual del diálogo
	private void RecorreDialogo(ref int numDialogo, int numTema, DialogueNode nodo)
	{
		switch(state)
		{
		case State.Intro_Texto:
			dialogo.MarcaDialogueNodeComoLeido(0, numTema, ref numDialogo, nodo);
			break;
		case State.Mensajes_Texto:
			if (numTema == -1)
				dialogo.MarcaDialogueNodeComoLeido(1, numTema, ref numDialogo, nodo);
			else
				dialogo.MarcaDialogueNodeComoLeido(2, numTema, ref numDialogo, nodo);
			break;
		}
	}

	//Muestra texto del diálogo
	private void DisplayNodeText(DialogueNode node)
	{
		PosicionaCamara(node.posCamara); //Posiciona la cámara

		dialogOptions.SetActive(false);

		for(int i = 0; i < options.Length; i++)
		{
			options[i].SetActive(false);
		}

		irAMensajesMenu.SetActive(false);
		exit.SetActive(false);

		dialogText.SetActive(true);

		dialogName.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());
		dialogText.GetComponentInChildren<Text>().text = node.DevuelveTexto();

		dialogText.GetComponent<Button>().onClick.RemoveAllListeners();

		var opcion = selectedOption;

		//Dice hacia adonde continua el dialogo
		dialogText.GetComponent<Button>().onClick.AddListener(delegate
		{
				int siguienteOpcion = opcion + 1;
				switch(node.DevuelveSiguienteNodo())
				{
				//El dialogo acaba
				case -2:
					siguienteOpcion = -3;
					break;
				//Va hacia el menú de mensajes
				case -1:
					siguienteOpcion = -2;
					break;
				//opción por defecto(selected_option+1)
				case 0:
				default:
					break;
				}
				SetSelectedOption(siguienteOpcion);
		}); //Listener del botón
	}

	//Muestra las opciones del dialogo
	private void DisplayNodeOptions(DialogueNode node)
	{
		dialogOptions.SetActive(true);

		//Mantiene el scroll arriba del todo al mostrar opciones
		dialogOptions.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialogText.SetActive(false);

		for(int i = 0; i < options.Length; i++)
		{
			options[i].SetActive(false);
		}

		irAMensajesMenu.SetActive(false);
		exit.SetActive(false);

		dialogName.GetComponentInChildren<Text>().text = DevuelveNombre(node.DevuelveNombre());

		for(int i = 0; i < node.DevuelveNumeroOpciones() && i < 14; i++)
		{
			SetOptionButton(options[i], node.DevuelveNodoOpciones(i));
		}
	}

	private void SetOptionButton(GameObject button, DialogueOption dialogueOption)
	{
		bool mostrar = true;

		for(int i = 0; i < dialogueOption.DevuelveNumeroGrupos(); i++)
		{
			int IDGrupo = dialogueOption.DevuelveIDGrupo(i); //Miramos si la opción está asignada a algún grupo

			Grupo grupo = Manager.Instance.DevolverGrupoActivo(IDGrupo);

			if (grupo != null)
			{
				List<DialogueOptionGrupoVariables> variables = dialogueOption.DevuelveVariables(i);

				for(int j = 0; i < variables.Count; j++)
				{
					//Si la variable de la opcion es mayor que la actual del grupo, no se muestra la opción
					if(variables[j].valor > grupo.variables[variables[j].numVariable])
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
				ObjetoSerializable objetoSerializable = inventarioCola.GetObjeto();
				inventario = objetoSerializable as Inventario;
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
				
			for(int i = 0; i < dialogueOption.DevuelveNumeroObjetos(); i++)
			{
				int IDObjeto = dialogueOption.DevuelveIDObjeto(i);
				bool enPosesion = dialogueOption.DevuelveObjetoPosesion(i);

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
			button.GetComponentInChildren<Text>().text = dialogueOption.DevuelveTexto(); //Texto del botón
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(dialogueOption.DevuelveDestinationNodeID()); }); //Listener del botón
		}
	}

	//Muestra el menu de mensajes del dialogo
	private void DisplayMensajes()
	{
		//Mantiene el scroll arriba del todo al mostrar opciones
		dialogOptions.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialogOptions.SetActive(true);
		dialogText.SetActive(false);

		for(int k = 0; k < options.Length; k++)
		{
			options[k].SetActive(false);
		}

		irAMensajesMenu.SetActive(false);
		exit.SetActive(true);

		int i = 0; //indice

		for(i = 0; i < dialogo.DevuelveNumeroTemaMensajes() && i < 14; i++)
		{
			if(dialogo.TemaMensajeEsVisible(i))
				SetMensajeButton(options[i], dialogo.DevuelveTextoTemaMensaje(i), i);
		}

		int j = i;

		for(i = j; i < dialogo.DevuelveNumeroMensajes() + j && i < 14; i++)
		{
			if(dialogo.MensajeEsVisible(i-j))
				SetMensajeButton(options[i], dialogo.DevuelveTextoMensaje(i-j), i);
		}
	}

	//Crea el botón de un mensaje
	private void SetMensajeButton(GameObject button, string texto, int num)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = texto; //Texto del botón
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(num); }); //Listener del botón
	}

	//Muestra el menu de mensajes del dialogo
	private void DisplayTemaMensajes(TemaMensaje temaMensaje)
	{
		//Mantiene el scroll arriba del todo al mostrar opciones
		dialogOptions.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

		dialogOptions.SetActive(true);
		dialogText.SetActive(false);

		for(int k = 0; k < options.Length; k++)
		{
			options[k].SetActive(false);
		}

		irAMensajesMenu.SetActive(true);
		exit.SetActive(true);

		for(int i = 0; i < temaMensaje.DevuelveNumeroMensajes() && i < 14; i++)
		{
			SetMensajeButton(options[i], temaMensaje.DevuelveTextoMensaje(i), i);
		}
	}

	//Guarda en una variable la opción actual
	private void SetSelectedOption(int x)
	{
		selectedOption = x;
	}

	//Posiciona la cámara, si el interactuable no es nulo
	private void PosicionaCamara(PosicionCamara posicionCamara)
	{
		if(interactuable != null)
			TP_Camera.Instance.PosicionDialogo(posicionCamara, Manager.Instance.GetInteractuable(interactuable.ID));
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
			if(interactuable != null)
				nombre = interactuable.DevuelveNombreDialogo();
			else
				nombre = "";
			break;
		}

		return nombre;
	}
}