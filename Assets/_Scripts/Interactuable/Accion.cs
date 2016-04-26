using UnityEngine;
using System.Collections;

public class Accion : MonoBehaviour{
	//Crear herencia con los diferentes tipos de acciones

	private int num_accion;
	private string nombre;

	void Start ()
	{
		//Valores por defecto
		nombre = "Accion1";
		num_accion = -1;
	}

	public void ConstructorAccion(int num, string nom)
	{
		num_accion = num;
		nombre = nom;
	}

	public string DevuelveNombre()
	{
		return nombre;
	}
}
