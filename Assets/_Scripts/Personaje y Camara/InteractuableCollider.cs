using UnityEngine;

/*
 * 	Clase que almacena los interactuables que colisionan con su collider en una lista situada en la clase Manager
 *  Se usa para saber que interactuables están cerca del jugador
 */
public class InteractuableCollider : MonoBehaviour {

	public static InteractuableCollider Instance;

	//Guarda el interactuable más cercano
	Transform nearestInteractuable = null;

	Ray ray;

	void Awake () {
		//Inicializamos la variable instancia
		Instance = this;
	}

	//Los interactuables que chocan con el collider pasan a estar en estado accionable
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Interactuable" ) {
			Manager.Instance.addInteractuableCercano(other.transform.parent.gameObject);
			Interactuable inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Accionable);
			inter.setDesactivado(false);
		}
	}

	//Los interactuables que salen del collider pasan a estar desactivados
	void OnTriggerExit(Collider other) {
		if (other.tag == "Interactuable") {
			Manager.Instance.deleteInteractuableCercano(other.transform.parent.gameObject);
			Interactuable inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Desactivado);
			inter.OcultaCanvas();
			inter.setDesactivado(true);
		}
	}

	//Establecemos el interactuable más cercano a un ray de la lista de interactuablescercanos a Accionable
	public void EncontrarInteractuablesCercanos()
	{
		var nearestDistanceSqr = Mathf.Infinity;

		//Creamos un rayo que va desde la cámara hacia adelante
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));

		//Dubujamos el rayo
		Debug.DrawRay(ray.origin, ray.direction*100, Color.blue);

		foreach (var obj in Manager.Instance.interactuablesCercanos)
		{
			Interactuable inter = obj.GetComponent<Interactuable> ();

			Vector3 objectPos = obj.transform.position;
			float distanceSqr;

			distanceSqr = DistanceToLine (ray, objectPos);

			if (distanceSqr < nearestDistanceSqr && inter.isVisible())
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
		if(nearestInteractuable != null && Manager.Instance.interactuablesCercanos.Count != 0)
		{
			Interactuable inter = nearestInteractuable.gameObject.GetComponent<Interactuable>();
			inter.SetState(Interactuable.State.Seleccionado);
			inter.ActivarTextoAcciones();
		}
	}

	//Devuelve la distancia entre una recta con un punto
	private float DistanceToLine(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}
}
