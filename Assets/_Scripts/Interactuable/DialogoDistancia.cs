using UnityEngine;
using System.Collections;

/*
 * 	Clase que indica si un dialogo es a distancia. Activa el dialogo al colisionar con el jugador
 */
public class DialogoDistancia : MonoBehaviour {

	private Interactuable interactuable; //Interactuable al cual pertecene el dialogo
	private DatosAccionDialogo datosAccionDialogo; //DatosAccion del dialogo

	//Establece las variables
	public void cargarDialogo(Interactuable interactuable, DatosAccionDialogo datosAccionDialogo)
	{
		this.datosAccionDialogo = datosAccionDialogo;
		this.interactuable = interactuable;
	}

	//Comprueba si se ha colisionado con el jugador
	//Si el estado del jugador es el normal, inicia el dialogo a distancia
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
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
		yield return StartCoroutine(ManagerDialogo.instance.PrepararDialogoCoroutine(interactuable, datosAccionDialogo.dialogo, -1));

		//Quitamos la propiedad a distancia del diálogo y actualizamos las acciones del interactuable
		datosAccionDialogo.SetADistancia(false);
		interactuable.GuardarAcciones();

		Destroy(gameObject);
	}
}
