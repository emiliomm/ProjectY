using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class TransportePlayerObjeto : TransportePlayer {

	//vacio
	protected override void Start()
	{

	}

	//Método de iniciación
	public void Constructor(int ID, List<int> escenas, string haciaEscena)
	{
		this.ID = ID;
		this.escenas = escenas;
		this.haciaEscena = haciaEscena;

		Manager.Instance.anyadirTransporte(SceneManager.GetActiveScene().buildIndex, gameObject, escenas);
	}

	protected override void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" ) {
			SceneManager.LoadScene(haciaEscena, LoadSceneMode.Single);
		}
	}
}
