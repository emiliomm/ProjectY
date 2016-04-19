using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	Objeto obj; //Objeto del que forma parte el cursor

	void Start () {
		obj = transform.parent.parent.gameObject.GetComponent<Objeto>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AccionUI" ) {
			Debug.Log ("Entrando");
			obj.SetPulsarSobreAccion(true);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "AccionUI") {
			Debug.Log ("Saliendo");
			obj.SetPulsarSobreAccion(false);
		}
	}
}