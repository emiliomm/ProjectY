using UnityEngine;

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
}
