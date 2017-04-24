using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransporteJugador : MonoBehaviour {

	public int IDEscena1;
	public int IDEscena2;

	void Awake()
	{
		if((ManagerEscenas.instance.GetNumeroEscenaCargada() != IDEscena1) && (ManagerEscenas.instance.GetNumeroEscenaCargada() != IDEscena2))
			DontDestroyOnLoad(gameObject);
		else
			Destroy(gameObject);
	}
}
