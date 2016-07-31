using UnityEngine;
using System.Collections;

public class InteractuableCollider : MonoBehaviour {

	public static InteractuableCollider Instance;

	//Guarda el interactuable más cercano
	Transform nearestInteractuable = null;

	Ray ray;

	void Awake () {
		//Inicializamos la variable instancia
		Instance = this;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Interactuable" ) {
			Manager.Instance.addInteractuableCercano(other.transform.parent.gameObject);
			Interactuable inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Accionable);
			inter.desactivado = false;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Interactuable") {
			Manager.Instance.deleteInteractuableCercano(other.transform.parent.gameObject);
			Interactuable inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Desactivado);
			inter.OcultaCanvas();
			inter.desactivado = true;
		}
	}

	public void GetNearestTaggedObject()
	{
		var nearestDistanceSqr = Mathf.Infinity;

		//Creamos un rayo que va desde la cámara hacia adelante
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));

		//Dubujamos el rayo
		Debug.DrawRay(ray.origin, ray.direction*100, Color.blue);

		//Recorremos todos los objetos, guardando el más cercano
		//Poniéndolos en estado alejado
		foreach (var obj in Manager.Instance.interactuablesCercanos)
		{
			Interactuable inter = obj.GetComponent<Interactuable> ();

			Vector3 objectPos = obj.transform.position;
			float distanceSqr, distancePlayerNPC;

			distanceSqr = DistanceToLine (ray, objectPos);
			distancePlayerNPC = (objectPos - transform.position).sqrMagnitude;

			if (distanceSqr < nearestDistanceSqr && inter.isRendered())
			{
				if(nearestInteractuable != null)
				{
					inter = nearestInteractuable.gameObject.GetComponent<Interactuable>();
					inter.SetState (Interactuable.State.Accionable);
					inter.DesactivarTextoAcciones();
				}

				nearestInteractuable = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}

		//Si existe el más cercano, le cambiamos el estado a accionable
		//Dándole foco
		if(nearestInteractuable != null && Manager.Instance.interactuablesCercanos.Count != 0)
		{
			Interactuable inter = nearestInteractuable.gameObject.GetComponent<Interactuable>();
			inter.SetState(Interactuable.State.Seleccionado);
			inter.ActivarTextoAcciones();
		}
	}

	private float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}
}
