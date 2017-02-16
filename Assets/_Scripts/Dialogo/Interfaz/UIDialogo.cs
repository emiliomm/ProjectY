using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using DialogueTree;

public class UIDialogo : MonoBehaviour
{
	public static UIDialogo instance; //Instancia propia de la clase

	//Objetos de la interfaz
	private GameObject dialogName;
	private GameObject dialogText;
	private GameObject dialogOptions;
	private GameObject irAMensajesMenu;
	private GameObject exit;

	//Hay 15 botones de opciones
	private GameObject[] options;

	void Awake ()
	{
		instance = this;

		transform.SetParent(Manager.instance.canvasGlobal.transform, false); //Hacemos que la ventana sea hijo del canvas
		RectTransform diaWindowTransform = (RectTransform) transform;
		diaWindowTransform.localPosition = new Vector3(0, 0, 0);

		//Inicializamos las variables de objetos de la interfaz
		dialogName = transform.GetChild(1).gameObject;
		dialogText = transform.GetChild(2).gameObject;
		dialogOptions =  transform.GetChild(3).gameObject;

		irAMensajesMenu = transform.GetChild(4).gameObject;
		exit = transform.GetChild(5).gameObject;
		irAMensajesMenu.GetComponent<Button>().onClick.AddListener(delegate { ManagerDialogo.instance.SetSelectedOption(-2);});
		exit.GetComponent<Button>().onClick.AddListener(delegate { ManagerDialogo.instance.SetSelectedOption(-3);});

		options = new GameObject[15];
		for(int i = 0; i < 15; i++)
		{
			options[i] = dialogOptions.transform.GetChild(0).GetChild(i).gameObject;
		}

		OcultarInterfaz();
	}

	public void MostrarInterfaz()
	{
		gameObject.SetActive(true);
		Cursor.visible = true; //Muestra el cursor del ratón
	}

	public void OcultarInterfaz()
	{
		gameObject.SetActive(false);
		Cursor.visible = false;
	}

	public IEnumerator InterfazPopUpObjetos()
	{
		//Desactivamos la interfaz del diálogo y mostramos la interfaz de obtención de objetos
		gameObject.SetActive(false);

		yield return StartCoroutine(MostrarPopupObjetos());

		gameObject.SetActive(true);
	}

	//Muestra un popup de los objetos obtenidos
	public IEnumerator MostrarPopupObjetos()
	{
		GameObject panelObjeto = (GameObject)Instantiate(Resources.Load("PanelPopupObjeto"));
		panelObjeto.transform.SetParent(Manager.instance.canvasGlobal.transform, false);

		//Recorremos los objetos obtenidos recientemente
		for(int i = 0; i < Manager.instance.DevuelveNumeroObjetosRecientes(); i++)
		{
			panelObjeto.transform.GetChild(0).GetChild(0).transform.GetComponent<Text>().text = "Has obtenido " + Manager.instance.DevuelveNombreObjetoReciente(i);

			var opcion = -4;

			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
				{ opcion = -3; }); //Listener del botón

			while (opcion == -4) {
				yield return null;
			}
		}

		Manager.instance.VaciarObjetosRecientes();
		Destroy(panelObjeto);
	}

	//Muestra texto del diálogo
	public void DisplayNodeText(DialogueNode node)
	{
		dialogOptions.SetActive(false);

		for(int i = 0; i < options.Length; i++)
		{
			options[i].SetActive(false);
		}

		irAMensajesMenu.SetActive(false);
		exit.SetActive(false);

		dialogText.SetActive(true);

		dialogName.GetComponentInChildren<Text>().text = ManagerDialogo.instance.DevuelveNombre(node.DevuelveNombre());
		dialogText.GetComponentInChildren<Text>().text = node.DevuelveTexto();

		dialogText.GetComponent<Button>().onClick.RemoveAllListeners();

		var opcion = ManagerDialogo.instance.GetSelectedOption ();

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
				ManagerDialogo.instance.SetSelectedOption(siguienteOpcion);
			}); //Listener del botón
	}

	//Muestra las opciones del dialogo
	public void DisplayNodeOptions(DialogueNode node)
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

		dialogName.GetComponentInChildren<Text>().text = ManagerDialogo.instance.DevuelveNombre(node.DevuelveNombre());

		for(int i = 0; i < node.DevuelveNumeroOpciones() && i < 14; i++)
		{
			SetOptionButton(options[i], node.DevuelveNodoOpciones(i));
		}
	}

	public void SetOptionButton(GameObject button, DialogueOption dialogueOption)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = dialogueOption.DevuelveTexto(); //Texto del botón
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().onClick.AddListener(delegate { ManagerDialogo.instance.SetSelectedOption(dialogueOption.DevuelveDestinationNodeID()); }); //Listener del botón
	}

	//Muestra el menu de mensajes del dialogo
	public void DisplayMensajes()
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
	}

	//Crea el botón de un mensaje
	public void SetMensajeButton(int indiceBoton, string texto, int num)
	{
		options[indiceBoton].SetActive(true);
		options[indiceBoton].GetComponentInChildren<Text>().text = texto; //Texto del botón
		options[indiceBoton].GetComponent<Button>().onClick.RemoveAllListeners();
		options[indiceBoton].GetComponent<Button>().onClick.AddListener(delegate { ManagerDialogo.instance.SetSelectedOption(num); }); //Listener del botón
	}

	//Muestra el menu de mensajes del dialogo
	public void DisplayTemaMensajes(TemaMensaje temaMensaje)
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
			SetMensajeButton(i, temaMensaje.DevuelveTextoMensaje(i), i);
		}
	}
}
