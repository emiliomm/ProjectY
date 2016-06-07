using UnityEngine;
using System.Collections;

public class AccionObjeto : MonoBehaviour {

	private int ID; //ID del interactuable al cual pertenece la accion
	private int indice; //indice de la accion en el vector de acciones

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setID(int num)
	{
		ID = num;
	}

	public int getID()
	{
		return ID;
	}

	public void setIndice(int num)
	{
		indice = num;
	}

	public int getIndice()
	{
		return indice;
	}
}
