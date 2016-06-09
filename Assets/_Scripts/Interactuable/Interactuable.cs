﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using System.Xml.Serialization;

using UnityEngine.UI;

public class Interactuable : MonoBehaviour {

	public int ID;

	//Sensibilidad del ratón
	public float X_MouseSensitivity = 0.02f;
	public float Y_MouseSensitivity = 0.02f;
	public float distanciaMin = 4.0f; //Distancia máxima con la que se puede interactuar NO ASIGNADA

	public bool cursorSobreAccion;

	//implementar esto aqui o en acciones dialogo, donde el dialogo se marque como automatico
	//public bool requiredButtonPress; //indica si se requiere que se pulse una tecla para iniciar la conversación

	//	void OnTriggerEnter(Collider other)
	//	{
	//		//Sirve para los dialogos automaticos
	//		//Como implementar el dialogo automatico ¿?
	//		if (other.tag == "Player")
	//		{
	//			if (!requiredButtonPress && TP_Controller.Instance.CurrentState == TP_Controller.State.Normal) 
	//			{
	//				IniciaDialogo();
	//			}
	//		}
	//	}

	//	//Inicia el dialogo
	//	public void IniciaDialogo()
	//	{
	//		TextBox.Instance.StartDialogue(this, npc_diag);
	//	}

	private List<GameObject> AccionesGO;
	private List<DatosAccion> Acciones;
	private int accionActiva; //indice de la accion activa, -1 = ninguna

	private GameObject canvas;
	private GameObject nombre; //Nombre del interactuable
	private GameObject Objeto;
	private GameObject cursorUI; //Objeto que representa al cursor
	private Camera camara; //La cámara del juego

	private bool moverCanvas; //Indica si el canvas debe moverse con respecto a la cámara
	private float distance; //distancia entre el jugador y el objeto
	private Vector3 initialPosition; //posición inicial del cursorUI
	private Vector3 moveVector; //vector de movimiento del ratón

	public enum State { Alejado, Accionable, Accionando, Accionado }

	State _state = State.Alejado;
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

	protected virtual void Start ()
	{
		//Añadimos el interactuable al diccionario para tenerlo disponible
		Manager.Instance.AddToInteractuables(ID, gameObject);

		//Asignamos los objetos correspondientes
		Objeto = gameObject.transform.GetChild(0).gameObject;
		canvas = gameObject.transform.GetChild(1).gameObject;
		nombre = canvas.transform.GetChild(0).gameObject;
		cursorUI = canvas.transform.GetChild(1).gameObject;

		accionActiva = -1;

		//Buscamos la cámara activa y se la asignamos al canvas
		camara = GameObject.FindWithTag("MainCamera").GetComponent<Camera> ();
		canvas.GetComponent<Canvas>().worldCamera = camara;

		//Asignamos la posicion inicial y el vector de movimientos
		initialPosition = cursorUI.transform.position;
		moveVector = new Vector3(0f, 0f, 0f);

		//Asignamos el estado inicial
		SetState(State.Alejado);

		cursorSobreAccion = false;

		CargarAcciones();
	}

	void OnDestroy()
	{
		//Borramos el valor del diccionario cuando el npc no existe
		Manager.Instance.RemoveFromInteractuables(ID);
	}

	private void CargarAcciones()
	{
		AccionesGO = new List<GameObject>();

		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaDatosAccionGuardados + ID.ToString()  + ".xml"))
		{
			Acciones = LoadDatosAccion(Manager.rutaDatosAccionGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			Acciones = LoadDatosAccion(Manager.rutaDatosAccion + ID.ToString()  + ".xml");
		}

		for(int i = 0; i < Acciones.Count; i++)
		{
			GameObject AccionGO = new GameObject("Accion");
			AccionObjeto aobj = AccionGO.AddComponent<AccionObjeto>();
			aobj.setIndice(i);
			aobj.setID(ID);

			if(Acciones[i].GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo d = Acciones[i] as DatosAccionDialogo;
				d.CargaDialogo();
			}

			CargaAccion(AccionGO);
		}

		//Cargamos la UI de las acciones actuales
		CargarAccionesUI();
	}

	public static List<DatosAccion> LoadDatosAccion(string path)
	{
		List<DatosAccion> datosAccion = Manager.Instance.DeserializeDataWithReturn<List<DatosAccion>>(path);

		return datosAccion;
	}

	private void CargaAccion(GameObject AccionGO)
	{
		AccionGO.transform.SetParent(canvas.transform, false);
		AccionGO.transform.localPosition = new Vector3(0f, 0f, 0f);
		AccionGO.tag = "AccionUI";

		BoxCollider collider = AccionGO.AddComponent<BoxCollider>();
		collider.size =  new Vector2(430f, 140f);

		Text myText = AccionGO.AddComponent<Text>();
		myText.text = Acciones[AccionesGO.Count].DevolverNombre();
		myText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		myText.fontSize = 80;
		myText.rectTransform.sizeDelta = new Vector2(430f, 140f);
		myText.material = Resources.Load("UI") as Material;

		AccionesGO.Add(AccionGO);
	}

//	public void AddAccion(DatosAccion acc)
//	{
//		CargaAccion(AccionGO);
//
//		//Actualiza la UI de las acciones, ahora que se ha añadido una más
//		CargarAccionesUI();
//	}

	private void CargarAccionesUI()
	{
		float ang = 0;
		float radio = 600;

		for(int i = 0; i < AccionesGO.Count; i++)
		{
			Vector3 vec = new Vector3();

			vec.x = radio*Mathf.Cos(ang);
			vec.y = radio*Mathf.Sin(ang);
			vec.z = 0f;

			GameObject AccionGO = AccionesGO[i];

			AccionGO.transform.localPosition = new Vector3(0f, 0f, 0f);
			AccionGO.transform.localPosition += vec;

			ang += (360/AccionesGO.Count)*Mathf.Deg2Rad;
		}

		cursorUI.transform.SetAsLastSibling(); //Mueve el cursor al final de la jerarquía, mostrándolo encima de los demás GameObjects
	}

	//Devuelve un objeto NPC_Dialogo null si no lo ha encontrado
	public NPC_Dialogo DevolverDialogo(int ID_Dialogo)
	{
		NPC_Dialogo diag = null;

		for(int i = 0; i < Acciones.Count; i++)
		{
			DatosAccion dac = Acciones[i];

			if(dac.GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo dacdial = dac as DatosAccionDialogo;
				if(ID_Dialogo == dacdial.DevuelveIDDiag())
				{
					diag = dacdial.DevuelveDialogo();
				}
			}
		}

		return diag;
	}

	public List<NPC_Dialogo> DevolverDialogos()
	{
		List<NPC_Dialogo> dialogos = new List<NPC_Dialogo>();

		for(int i = 0; i < Acciones.Count; i++)
		{
			DatosAccion dac = Acciones[i];

			if(dac.GetType() == typeof(DatosAccionDialogo))
			{
				DatosAccionDialogo dacdial = dac as DatosAccionDialogo;
				dialogos.Add(dacdial.DevuelveDialogo());
			}
		}

		return dialogos;
	}

//	public GameObject DevolverObjeto()
//	{
//		return Objeto;
//	}

	//Devuelve el nombre del interactuable que aparece en el dialogo (no es el mismo que el mostrado en los interactuableobjeto)
	public virtual string DevuelveNombreDialogo()
	{
		return "";
	}

	public void SetNombre(string n)
	{
		nombre.GetComponent<Text>().text = n;
	}

	public void AsignarAccion(int num)
	{
		accionActiva = num;
	}

	public void setAccionNull()
	{
		accionActiva = -1;
	}
		
	void Update() {
		switch(_state)
		{
		case State.Alejado:
			CalcularDistancia();
			ShowCanvas();
			MoverCanvas();
			break;
		case State.Accionable:
			CalcularDistancia();
			ShowCanvas();
			MoverCanvas();

			//Si pulsamos click izquierdo
			if (Input.GetMouseButton(0) && TP_Controller.Instance.CurrentState == TP_Controller.State.Normal)
			{
				TP_Controller.Instance.SetState(TP_Controller.State.Interactuables);
				SetState(State.Accionando);
				ChangeCursorUI(Resources.Load<Sprite>("cursor"));
			}
			break;
		case State.Accionando:
			ShowCanvas();

			if (Input.GetMouseButton(0))
			{
				MoviendoCursorUI();
			}
			else if (!cursorSobreAccion)
			{
				DefaultCursorUI();
				TP_Controller.Instance.SetState(TP_Controller.State.Normal);
				SetState(State.Accionable);
			}
			else
			{
				SetState(State.Accionado);
			}
			break;
		case State.Accionado:
			EjecutarAccion();
			break;
		}
	}

	private void CalcularDistancia()
	{
		distance = Vector3.Distance(TP_Controller.CharacterController.transform.position, transform.position);
	}

	private void ShowCanvas() {
		//Regula la transparencia del canvas según la distancia
		//		float alpha = 3 - distance / 2.0f;
		//		canvas.GetComponent<CanvasGroup>().alpha = alpha;
	}

	private void MoverCanvas()
	{
		canvas.transform.LookAt(canvas.transform.position + camara.transform.rotation * Vector3.forward, camara.transform.rotation * Vector3.up);
	}

	public void ChangeCursorUI(Sprite im2)
	{
		Image im = cursorUI.GetComponent<Image>();
		im.sprite = im2;
	}

	private void MoviendoCursorUI()
	{
		//Movemos las coordenadas del raton segun el movimiento
		//Cogemos el eje X del Input del raton multiplicada por la sensibilidad
		moveVector.x += Input.GetAxis ("Mouse X") * X_MouseSensitivity;

		//Cogemos el eje Y del Input del raton multiplicada por la sensibilidad
		moveVector.y += Input.GetAxis ("Mouse Y") * Y_MouseSensitivity;

		//Limitamos el módulo del vector convirtiéndolo en unitario
		//y haciendo que el rango de movimiento esté limitado a un círculo
		moveVector = Vector3.ClampMagnitude(moveVector, 1.0f);

		//Transforma los movimientos del ratón según el punto de vista de la cámara
		Vector3 delta = new Vector3(moveVector.x,moveVector.y,0);
		delta = Camera.main.transform.TransformDirection(delta);

		//Asignamos la posición al objeto que hace de cursor
		Vector3 CursorLimit = new Vector3(0f, 0f, 0f);
		CursorLimit = initialPosition + delta;
		cursorUI.transform.position = CursorLimit;
	}

	private void DefaultCursorUI()
	{
		moveVector = new Vector3(0f, 0f, 0f); //Reseteamos el vector de movimiento
		cursorUI.transform.position = initialPosition; //Asignamos la posición inicial al objeto
		ChangeCursorUI(Resources.Load<Sprite>("mouse"));
	}

	public void EjecutarAccion()
	{
		DefaultCursorUI();
		SetState(State.Alejado);
		Acciones[accionActiva].EjecutarAccion();
	}

	public bool isRendered()
	{
		return Objeto.GetComponent<Renderer>().isVisible;
	}

	public void ActivarTextoAcciones()
	{
		nombre.SetActive(false);
		cursorUI.SetActive(true);
		for(int i = 0; i < AccionesGO.Count; i++)
		{
			AccionesGO[i].SetActive(true);
		}
	}

	public void DesactivarTextoAcciones()
	{
		nombre.SetActive(true);
		cursorUI.SetActive(false);
		for(int i = 0; i < AccionesGO.Count; i++)
		{
			AccionesGO[i].SetActive(false);
		}
	}
}
