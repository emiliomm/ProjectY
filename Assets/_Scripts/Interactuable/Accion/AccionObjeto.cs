using UnityEngine;
using UnityEngine.UI;

/*
 * 	Clase objeto que almacena información sobre una Acción de un interactuable
 */
public class AccionObjeto : MonoBehaviour {

	private int ID; //ID del interactuable al cual pertenece la accion
	private int indice; //posición de la acción en la lista de acciones del interactuable

	//Establecemos los valores de la instancia
	public void Inicializar(int ID, int indice)
	{
		this.ID = ID;
		this.indice = indice;
	}

	public int getID()
	{
		return ID;
	}

	public int getIndice()
	{
		return indice;
	}

	//UNITY NO DETECTA CORRECTAMENTE ONTRIGGEREXIT CON EL CHARACTERCONTROLLER DEL JUGADOR. BUG ¿?
//	void OnTriggerEnter(Collider other)
//	{
//		//Cada vez que colisionamos con el jugador, cambiamos el material para que el cursor se muestre correctamente sin atravesar la geometría del jugador
//		if (other.tag == "Player")
//		{
//			Debug.Log("entra");
//			gameObject.GetComponent<Text>().material = Resources.Load("UI2") as Material; //Mover la carga del material a otro sitio (Manager)
//		}
//	}
//		
//	void OnTriggerExit(Collider other)
//	{
//		Debug.Log("sale");
//		//Volvemos al material original si hemos dejado de colisionar con el jugador
//		if (other.tag == "Player")
//		{
//			
//			gameObject.GetComponent<Text>().material = Resources.Load("UI") as Material; //Mover la carga del material a otro sitio (Manager)
//		}
	}
}
