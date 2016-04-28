using UnityEngine;
using System.Collections;

public class Accion : MonoBehaviour{
	
	protected string nombre;

	protected virtual void Start ()
	{
		//nombre por defecto
		nombre = "Accion";
	}

	public string DevuelveNombre()
	{
		return nombre;
	}
}
