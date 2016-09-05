﻿using UnityEngine;

/*
 * 	Clase que almacena los interactuables que colisionan con su collider en una lista situada en la clase Manager
 *  Se usa para saber que interactuables están cerca del jugador
 */
public class InteractuableCollider : MonoBehaviour {

	public static InteractuableCollider Instance;

	private Transform nearestInteractuable = null; //Guarda el interactuable más cercano
	private float nearestDistanceSqr; //distancia más cercana en raiz cuadrada (ahorra rendimiento)

	private Ray ray;

	private Interactuable inter; //Variable que se usa principalmente para guardar referencias temporales, teniendo una variable se ahorra en rendimiento

	void Awake () {
		//Inicializamos la variable instancia
		Instance = this;
	}

	//Los interactuables que chocan con el collider pasan a estar en estado accionable
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Interactuable" ) {
			Debug.Log("Entra: " + other.transform.parent.gameObject.GetComponent<Interactuable>().ID);
			Manager.Instance.addInteractuableCercano(other.transform.parent.gameObject);
			inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Accionable);
			inter.DesactivarTextoAcciones();
		}
	}

	//Los interactuables que salen del collider pasan a estar desactivados
	void OnTriggerExit(Collider other) {
		if (other.tag == "Interactuable") {
			Debug.Log("Sale: " + other.transform.parent.gameObject.GetComponent<Interactuable>().ID);
			Manager.Instance.deleteInteractuableCercano(other.transform.parent.gameObject);
			inter = other.transform.parent.gameObject.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Desactivado);
			inter.OcultaCanvas();
			inter.reiniciarDistancia();
		}
	}

	//Establecemos el interactuable más cercano a un ray de la lista de interactuablescercanos a Accionable
	public void EncontrarInteractuablesCercanos()
	{
		nearestDistanceSqr = Mathf.Infinity;

		//Creamos un rayo que va desde la cámara hacia adelante
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));

		//Dubujamos el rayo
		Debug.DrawRay(ray.origin, ray.direction*100, Color.blue);

		//Si tenemos guardado una referencia al interactuable más cercano, le quitamos el estado seleccionado
		if(nearestInteractuable != null)
		{
			inter = nearestInteractuable.GetComponent<Interactuable>();

			//Si el interactuable es visible por la cámara y tiene más de una acción
			if(inter.CurrentState != Interactuable.State.Desactivado)
			{
				if(inter.isVisible() && inter.DevolverAccionesCreadas() > 0)
				{
					inter.SetState (Interactuable.State.Accionable);
					inter.DesactivarTextoAcciones();
				}
				else
				{
					inter.SetState (Interactuable.State.Desactivado);
					inter.OcultaCanvas();
					inter.reiniciarDistancia();
				}
			}

			nearestInteractuable = null;
		}

		for(int i = Manager.Instance.devuelveNumeroInteractuablesCercanos() - 1; i >= 0; i--)
		{
			GameObject interCercano = Manager.Instance.devuelveInteractuableCercano(i);

			//el interactuable existe
			if(interCercano != null)
			{
				inter = interCercano.GetComponent<Interactuable>();

				//Si el interactuable es visible por la cámara y tiene más de una acción
				if(inter.isVisible() && inter.DevolverAccionesCreadas() > 0)
				{
					Vector3 objectPos = interCercano.transform.position;
					float distanceSqr = DistanceToLine(ray, objectPos);

					if (distanceSqr < nearestDistanceSqr)
					{
						nearestInteractuable = interCercano.transform;
						nearestDistanceSqr = distanceSqr;
					}
				}
				else if(inter.CurrentState != Interactuable.State.Desactivado)
				{
					inter.SetState (Interactuable.State.Desactivado);
					inter.OcultaCanvas();
					inter.reiniciarDistancia();
				}
			}
			//El interactuable ya no existe, ha sido eliminado de la escena, lo eliminamos de la lista
			else
			{
				Manager.Instance.deleteInteractuableCercano(interCercano);
			}
		}

		//Si existe el más cercano, le cambiamos el estado a accionable
		if(nearestInteractuable != null && Manager.Instance.devuelveNumeroInteractuablesCercanos() != 0)
		{
			inter = nearestInteractuable.gameObject.GetComponent<Interactuable>();
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
