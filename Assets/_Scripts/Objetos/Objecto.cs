using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class Objecto : MonoBehaviour {

	public int ID;
	public List<Accion> acciones;

	//Sensibilidad del ratón
	public float X_MouseSensitivity = 0.02f;
	public float Y_MouseSensitivity = 0.02f;

	public float distanciaMin = 4.0f; //Distancia máxima con la que se puede interactuar con el objeto

	private GameObject canvas;
	private Transform cursorUI; //Objeto que representa al cursor

	private float distance; //distancia entre el jugador y el objeto
	private Vector3 initialPosition; //posición inicial del cursorUI
	private Vector3 moveVector; //vector de movimiento del ratón

	public bool cursorSobreAccion;

	void Start () {
		cargarNombres();

		cursorSobreAccion = false;

		//Buscamos el world canvas del objeto
		canvas = gameObject.transform.GetChild(1).gameObject;

		//Buscamos la cámara activa y se la asignamos al canvas
		Camera cameraToLookAt = GameObject.FindWithTag("MainCamera").GetComponent<Camera> ();
		canvas.GetComponent<Canvas>().worldCamera = cameraToLookAt;

		//Le añadimos el script MirandoCamara
		canvas.AddComponent<MirandoCamara>();

		//Buscamos el objeto cursorUI y lo asignamos
		cursorUI = gameObject.transform.GetChild(1).GetChild(0).gameObject.transform;

		CreateAcciones();

		//Asignamos la posicion inicial y el vector de movimientos
		initialPosition = cursorUI.transform.position;
		moveVector = new Vector3(0f, 0f, 0f);
	}

	private void cargarNombres()
	{
		acciones = new List<Accion>();
		acciones.Add(new Accion());
		acciones.Add(new Accion());
		acciones.Add(new Accion());
		acciones.Add(new Accion());
		acciones.Add(new Accion());
		acciones.Add(new Accion());
	}


	void Update () {
		distance = Vector3.Distance(TP_Controller.CharacterController.transform.position, transform.position);

		ShowCanvas();

		//Si la distancia es mayor a la fijada, no podemos mover el objeto
		if (distance > distanciaMin)
		{
			
		}
		else
		{
			MoverCursorUI();
		}
	}

	private void MoverCursorUI()
	{
		//Si pulsamos click izquierdo
		if (Input.GetMouseButton(0))
		{
			TP_Controller.Instance.canMove = false; //Hacemos que el jugador no se pueda mover

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
			cursorUI.position = CursorLimit;
		}
		else if(cursorSobreAccion)
		{
			TP_Controller.Instance.canMove = true; //Hacemos que el jugador se pueda mover
			moveVector = new Vector3(0f, 0f, 0f); //Reseteamos el vector de movimiento
			cursorUI.position = initialPosition; //Asignamos la posición inicial al objeto
		}
	}

	private void ShowCanvas() {
		//Regula la transparencia del canvas según la distancia
		float alpha = 3 - distance / 2.0f;
		canvas.GetComponent<CanvasGroup>().alpha = alpha;
	}

	private void CreateAcciones()
	{
		float ang = 0;
		float radio = 600;
		for(int i = 0; i < acciones.Count; i++)
		{
			Vector3 vec = new Vector3();

			vec.x = radio*Mathf.Cos(ang);
			vec.y = radio*Mathf.Sin(ang);
			vec.z = 0f;

			GameObject TextGO = new GameObject("myTextGO");
			TextGO.transform.SetParent(canvas.transform, false);
			TextGO.tag = "AccionUI";
			BoxCollider collider = TextGO.AddComponent<BoxCollider>();
			collider.size =  new Vector2(430f, 140f);

			Text myText = TextGO.AddComponent<Text>();
			myText.text = acciones[i].DevuelveNombre();
			myText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			myText.fontSize = 80;
			myText.rectTransform.sizeDelta = new Vector2(430f, 140f);
			myText.material = Resources.Load("UI") as Material;

			TextGO.transform.localPosition += vec;

			ang += (360/acciones.Count)*Mathf.Deg2Rad;
		}

		cursorUI.SetAsLastSibling(); //Mueve el cursor al final de la jerarquía, mostrándolo encima de los demás GameObjects
	}
}
