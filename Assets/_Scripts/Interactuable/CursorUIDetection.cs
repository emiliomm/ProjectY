using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	Interactuable obj;

	void Start () {
		obj = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AccionUI" ) {
			Debug.Log ("Entrando");
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "AccionUI") {
			Debug.Log ("Saliendo");
		}
	}
}