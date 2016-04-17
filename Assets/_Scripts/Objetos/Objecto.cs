using UnityEngine;
using System.Collections;

public class Objecto : MonoBehaviour {

	public int ID;

	//Sensibilidad del ratón
	public float X_MouseSensitivity = 0.02f;
	public float Y_MouseSensitivity = 0.02f;

	public float distanciaMin = 4.0f; //Distancia máxima con la que se puede interactuar con el objeto

	private GameObject canvas;
	private Transform cursorUI; //Objeto que representa al cursor

	private float distance; //distancia entre el jugador y el objeto
	private Vector3 initialPosition; //posición inicial del cursorUI
	private Vector3 moveVector; //vector de movimiento del ratón

	void Start () {
		//Buscamos el world canvas del objeto
		canvas = gameObject.transform.GetChild(1).gameObject;

		//Buscamos la cámara activa y se la asignamos al canvas
		Camera cameraToLookAt = GameObject.FindWithTag("MainCamera").GetComponent<Camera> ();
		canvas.GetComponent<Canvas>().worldCamera = cameraToLookAt;

		//Le añadimos el script MirandoCamara
		canvas.AddComponent<MirandoCamara>();

		//Buscamos el objeto cursorUI y lo asignamos
		cursorUI = gameObject.transform.GetChild(1).GetChild(0).gameObject.transform;

		//Asignamos la posicion inicial y el vector de movimientos
		initialPosition = cursorUI.transform.position;
		moveVector = new Vector3(0f, 0f, 0f);
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
			Vector3 CursorLimit = cursorUI.position;
			CursorLimit = initialPosition + delta;
			cursorUI.position = CursorLimit;
		}
		else
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
}
