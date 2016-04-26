using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class Interactuable : MonoBehaviour {
	
	//Sensibilidad del ratón
	public float X_MouseSensitivity = 0.02f;
	public float Y_MouseSensitivity = 0.02f;

	public bool cursorSobreAccion;
	private Accion accionActiva;

	private List<GameObject> Acciones;

	private GameObject canvas;
	private GameObject nombre;
	private GameObject objetoRender;
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

	public float distanciaMin = 4.0f; //Distancia máxima con la que se puede interactuar

	void Start () {
		
		//Asignamos los objetos correspondientes
		objetoRender = gameObject.transform.GetChild(0).gameObject;
		canvas = gameObject.transform.GetChild(1).gameObject;
		nombre = canvas.transform.GetChild(0).gameObject;
		cursorUI = canvas.transform.GetChild(1).gameObject;

		//Buscamos la cámara activa y se la asignamos al canvas
		camara = GameObject.FindWithTag("MainCamera").GetComponent<Camera> ();
		canvas.GetComponent<Canvas>().worldCamera = camara;

		//Asignamos la posicion inicial y el vector de movimientos
		initialPosition = cursorUI.transform.position;
		moveVector = new Vector3(0f, 0f, 0f);

		//Asignamos el estado inicial
		SetState(State.Alejado);

		cursorSobreAccion = false;
		accionActiva = null;

		CargarAcciones();
	}

	public void AsignarAccion(Accion ac)
	{
		accionActiva = ac;
	}

	public void setAccionNull()
	{
		accionActiva = null;
	}

	private void CargarAcciones()
	{
		Acciones = new List<GameObject>();

//		GameObject AccionGO = new GameObject("Accion");
//		AccionGO.AddComponent<Accion>();
//		Acciones.Add(AccionGO);

//		acciones.Add(new Accion());
//		acciones.Add(new Accion());
//		acciones.Add(new Accion());
//		acciones.Add(new Accion());
//		acciones.Add(new Accion());

		//Cargamos la UI de las acciones actuales
		CargarAccionesUI();
	}
		
	public void AddAccion(GameObject AccionGO)
	{
		Accion ac = AccionGO.GetComponent<Accion>();

		Acciones.Add(AccionGO);

		AccionGO.transform.SetParent(canvas.transform, false);
		AccionGO.tag = "AccionUI";

		BoxCollider collider = AccionGO.AddComponent<BoxCollider>();
		collider.size =  new Vector2(430f, 140f);

		Text myText = AccionGO.AddComponent<Text>();
		myText.text = ac.DevuelveNombre();
		myText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		myText.fontSize = 80;
		myText.rectTransform.sizeDelta = new Vector2(430f, 140f);
		myText.material = Resources.Load("UI") as Material;

		CargarAccionesUI();
	}

	private void CargarAccionesUI()
	{
		float ang = 0;
		float radio = 600;

		for(int i = 0; i < Acciones.Count; i++)
		{
			Vector3 vec = new Vector3();

			vec.x = radio*Mathf.Cos(ang);
			vec.y = radio*Mathf.Sin(ang);
			vec.z = 0f;

			GameObject AccionGO = Acciones[i];

			AccionGO.transform.localPosition = new Vector3(0f, 0f, 0f);
			AccionGO.transform.localPosition += vec;

			ang += (360/Acciones.Count)*Mathf.Deg2Rad;
		}

		cursorUI.transform.SetAsLastSibling(); //Mueve el cursor al final de la jerarquía, mostrándolo encima de los demás GameObjects
	}
		
	void Update () {

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
				TP_Controller.Instance.SetState(TP_Controller.State.Objetos);
				SetState(State.Accionando);
				Sprite s = Resources.Load<Sprite>("cursor");
				ChangeCursorUI(s);
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

	public void EjecutarAccion()
	{
		accionActiva.GetComponent<AccionDialogo>().EjecutarAccion();
		SetState(State.Alejado);
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
		Sprite s = Resources.Load<Sprite>("mouse");
		ChangeCursorUI(s);
	}

	public void ChangeCursorUI(Sprite im2)
	{
		Image im = cursorUI.GetComponent<Image>();
		im.sprite = im2;
	}

	public bool isRendered()
	{
		return objetoRender.GetComponent<Renderer>().isVisible;
	}

	public void ActivarTextoAcciones()
	{
		nombre.SetActive(false);
		cursorUI.SetActive(true);
		for(int i = 0; i < Acciones.Count; i++)
		{
			Acciones[i].SetActive(true);
		}
	}

	public void DesactivarTextoAcciones()
	{
		nombre.SetActive(true);
		cursorUI.SetActive(false);
		for(int i = 0; i < Acciones.Count; i++)
		{
			Acciones[i].SetActive(false);
		}
	}
}
