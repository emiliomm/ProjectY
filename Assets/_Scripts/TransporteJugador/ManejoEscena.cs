using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ManejoEscena : MonoBehaviour {

	public bool primeraEscena;
	public bool cargarEscena;

	private int numEscena1 = -1;

	// Use this for initialization
	void Start ()
	{
		if(primeraEscena)
		{
			numEscena1 = gameObject.GetComponentInParent<TransporteJugador>().IDEscena1;
		}
		else
		{
			numEscena1 = gameObject.GetComponentInParent<TransporteJugador>().IDEscena2;
		}

		if(numEscena1 == ManagerEscenas.instance.GetNumeroEscenaActual())
			GetComponent<BoxCollider>().enabled = false;

	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player" && numEscena1 != -1)
		{
			if(cargarEscena)
				CargarEscena();
			else
				EliminaEscena();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player" && numEscena1 != -1)
		{
			if(!cargarEscena)
				CargarEscena();
			else
				EliminaEscena();
		}
	}

	void CargarEscena()
	{
		//Debug.Log("Cargarndo Escena");
		ManagerEscenas.instance.CargarEscenaSegundoPlano(numEscena1);
	}

	void EliminaEscena()
	{
		//Debug.Log("Eliminando Escena");
		ManagerEscenas.instance.EliminaEscena(numEscena1);
	}
}