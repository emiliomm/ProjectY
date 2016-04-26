using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	Interactuable obj;

	void Start () {
		obj = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AccionUI" ) {
			obj.cursorSobreAccion = true;
			Accion ac = other.GetComponent<Accion>();
			obj.AsignarAccion(ac);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "AccionUI") {
			obj.cursorSobreAccion = false;
			obj.setAccionNull();
		}
	}
}