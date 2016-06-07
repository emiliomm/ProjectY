using UnityEngine;
using System.Collections;

public class CursorUIDetection : MonoBehaviour {

	Interactuable inter;

	void Start () {
		inter = transform.parent.parent.gameObject.GetComponent<Interactuable>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "AccionUI" ) {




			AccionObjeto aobj = other.GetComponent<AccionObjeto>();

			if(aobj.getID() == inter.ID)
			{
				Debug.Log("Tocado");
				inter.cursorSobreAccion = true;
				inter.AsignarAccion(aobj.getIndice());
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "AccionUI") {
			AccionObjeto aobj = other.GetComponent<AccionObjeto>();

			if(aobj.getID() == inter.ID)
			{
				inter.cursorSobreAccion = false;
				inter.setAccionNull();
			}
		}
	}
}