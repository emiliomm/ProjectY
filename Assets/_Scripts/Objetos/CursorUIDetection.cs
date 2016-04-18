using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	private Objecto Obj;

	// Use this for initialization
	void Start () {
		//Cogemos el objeto del cursor
		GameObject ObjetoCogido = transform.parent.parent.gameObject;
//		ObjetoCogido.GetComponent<Objeto>();
	}

	void OnTriggerEnter(Collider other) {
		
		if (other.tag == "AccionUI") {
			Obj.cursorSobreAccion = true;
			Debug.Log ("Colisiooooooon");
		}
	}

	void OnTriggerExit(Collider other) {
		
		if (other.tag == "AccionUI") {
			Obj.cursorSobreAccion = false;
			Debug.Log ("Colisiooooooon");
		}
	}
}
