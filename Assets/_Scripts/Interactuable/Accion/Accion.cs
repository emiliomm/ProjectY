using UnityEngine;
using System.Collections;

public class Accion : MonoBehaviour{
	//Crear herencia con los diferentes tipos de acciones

	protected string nombre;

	protected virtual void Start ()
	{
		Debug.Log("Hola");
		//Valores por defecto
		nombre = "Accion1";
	}

	public string DevuelveNombre()
	{
		return nombre;
	}
}
