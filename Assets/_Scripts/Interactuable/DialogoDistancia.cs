using UnityEngine;
using System.Collections;

/*
 * 	Clase que indica si un dialogo es a distancia. Activa el dialogo al colisionar con el jugador
 */
public class DialogoDistancia : MonoBehaviour {

	Interactuable inter; //Interactuable al cual pertecene el dialogo
	DatosAccionDialogo datosAccionDialogo; //DatosAccion del dialogo

	//Establece las variables
	public void cargarDialogo(Interactuable inter, DatosAccionDialogo dAcc)
	{
		datosAccionDialogo = dAcc;
		this.inter = inter;
	}

	//Comprueba si se ha colisionado con el jugador
	//Si el estado del jugador es el normal, inicia el dialogo a distancia
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			if (TP_Controller.Instance.CurrentState == TP_Controller.State.Normal) 
			{
				TP_Controller.Instance.SetState(TP_Controller.State.Dialogo);
				IniciaDialogo();
			}
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
		yield return StartCoroutine(TextBox.Instance.DialogoCoroutine(inter, datosAccionDialogo.diag));

		//Quitamos la propiedad a distancia del diálogo y actualizamos las acciones del interactuable
		datosAccionDialogo.setADistancia(false);
		inter.GuardarAcciones();

		Destroy(gameObject);
	}
}
