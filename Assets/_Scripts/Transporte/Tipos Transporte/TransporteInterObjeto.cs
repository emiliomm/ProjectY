using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class TransporteInterObjeto : TransporteInter {

	//vacio
	protected override void Start()
	{
		
	}

	//Método de iniciación
	public void Constructor(int ID, List<int> escenas)
	{
		this.ID = ID;
		this.escenas = escenas;

		Manager.Instance.anyadirTransporte(SceneManager.GetActiveScene().buildIndex, gameObject, escenas);
	}
}
