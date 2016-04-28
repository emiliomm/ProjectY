using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	Interactuable inter;

	void Start () {
		inter = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AccionUI" ) {
			inter.cursorSobreAccion = true;
			Accion ac = other.GetComponent<Accion>();
			inter.AsignarAccion(ac);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "AccionUI") {
			inter.cursorSobreAccion = false;
			inter.setAccionNull();
		}
	}
}