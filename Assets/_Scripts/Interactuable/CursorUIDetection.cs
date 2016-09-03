using UnityEngine;
using UnityEngine.UI;

/*
 * 	Clase que controla que acción es activada por el cursor de la interfaz de un interactuable
 */
public class CursorUIDetection : MonoBehaviour {

	Interactuable inter; //Referencia al interactuable al cual pertenece el cursor

	void Start () {
		inter = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	//Al detectar una colisión con un trigger, comprueba que se trata de un gameObject Accion
	void OnTriggerEnter(Collider other)
	{
		//Si se ha colisionado con un gameObject Accion, se activa la acción en la clase interactuable
		//para que este sepa que acción está en contacto con el cursor
		if (other.tag == "AccionUI")
		{
			AccionObjeto aobj = other.GetComponent<AccionObjeto>();

			if(aobj.getID() == inter.ID)
			{
				inter.AsignarAccionActiva(aobj.getIndice());
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
			AccionObjeto aobj = other.GetComponent<AccionObjeto>();

			if(aobj.getID() == inter.ID)
			{
				inter.setAccionActivaNull();
			}
		}

		//Volvemos al material original si hemos dejado de colisionar con el jugador
		if (other.tag == "Player")
		{
			gameObject.GetComponent<Image>().material = Resources.Load("UI") as Material; //Mover la carga del material a otro sitio (Manager)
		}
	}
}