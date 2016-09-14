using UnityEngine;
using System.Collections;

public class LLave : MonoBehaviour {

	private Vector3 moveVector;
	public float YMouseSensitivity = 0.02f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		MoviendoCursorUI();
	}

	//Controla cuanto se mueve el cursorUI cuando se interactúa con él moviendo el ratón
	private void MoviendoCursorUI()
	{
		if (Input.GetMouseButton(0))
		{
			//Cogemos el eje Y del Input del raton multiplicada por la sensibilidad
			moveVector.y = Input.GetAxis ("Mouse Y") * YMouseSensitivity;

			//Asignamos la posición al objeto que hace de cursor
			transform.position = transform.position + moveVector;
		}
	}
}
