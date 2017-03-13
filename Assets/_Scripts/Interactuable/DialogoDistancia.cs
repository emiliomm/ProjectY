using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 	Clase que indica si un dialogo es a distancia. Activa el dialogo al colisionar con el jugador
 */
public class DialogoDistancia : MonoBehaviour {

	private Interactuable interactuable; //Interactuable al cual pertecene el dialogo
	private Dialogo dialogo; 
	private Intro intro;

	//Establece las variables
	public void cargarDialogo(Interactuable interactuable, Dialogo dialogo, Intro intro)
	{
		this.dialogo = dialogo;
		this.interactuable = interactuable;
		this.intro = intro;
	}

	//Comprueba si se ha colisionado con el jugador
	//Si el estado del jugador es el normal, inicia el dialogo a distancia
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if(intro.SeMuestra())
				IniciaDialogo();
		}
	}

	//Inicia el dialogo a distancia
	private void IniciaDialogo()
	{
		StartCoroutine(DialogoEnCurso());
	}

	//Inicia el dialogo en una coroutine para saber cuando ha acabado
	//Cuando el dialogo acaba, elimina el objeto
	private IEnumerator DialogoEnCurso()
	{
		yield return StartCoroutine(ManagerDialogo.instance.PrepararDialogoCoroutine(interactuable, dialogo, -1));

		//Quitamos la propiedad a distancia del diálogo y actualizamos las acciones del interactuable
		//datosAccionDialogo.SetADistancia(false);
		//interactuable.GuardarAcciones();

		if(intro.DevuelveAutodestruye())
			Destroy(gameObject);
	}
}
