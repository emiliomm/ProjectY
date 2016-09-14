using UnityEngine;
using UnityEngine.UI;

/*
 * 	Clase que controla que acción es activada por el cursor de la interfaz de un interactuable
 */
public class CursorUIDetection : MonoBehaviour {

	Interactuable interactuable; //Referencia al interactuable al cual pertenece el cursor

	void Start () {
		interactuable = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	//Al detectar una colisión con un trigger, comprueba que se trata de un gameObject Accion
	void OnTriggerEnter(Collider other)
	{
		//Si se ha colisionado con un gameObject Accion, se activa la acción en la clase interactuable
		//para que este sepa que acción está en contacto con el cursor
		if (other.tag == "AccionUI")
		{
			AccionObjeto accionObjeto = other.GetComponent<AccionObjeto>();

			if(accionObjeto.GetID() == interactuable.ID)
			{
				interactuable.AsignarAccionActiva(accionObjeto.GetIndice());
			}
		}

		//Cada vez que colisionamos con el jugador, cambiamos el material para que el cursor se muestre correctamente sin atravesar la geometría del jugador
		if (other.tag == "Player")
		{
			gameObject.GetComponent<Image>().material = Resources.Load("UI2") as Material; //Mover la carga del material a otro sitio (Manager)
		}
	}

	//Al detectar la salida de una colisión con un trigger, comprueba que se trata de un gameObject Accion
	void OnTriggerExit(Collider other)
	{
		//Si se ha colisionado con un gameObject Accion, se desactiva la acción en la clase interactuable
		if (other.tag == "AccionUI")
		{
			AccionObjeto accionObjeto = other.GetComponent<AccionObjeto>();

			if(accionObjeto.GetID() == interactuable.ID)
			{
				interactuable.SetAccionActivaNull();
			}
		}

		//Volvemos al material original si hemos dejado de colisionar con el jugador
		if (other.tag == "Player")
		{
			gameObject.GetComponent<Image>().material = Resources.Load("UI") as Material; //Mover la carga del material a otro sitio (Manager)
		}
	}
}