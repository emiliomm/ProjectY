using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

/*
 * 	Clase de prueba que al chocar con el jugador te lleva a la escena indicada
 */
public class TransporteInter : MonoBehaviour
{
	public int ID;

	//Lista con los indices de las escenas que son accesibles desde este transporte.
	//Se incluyen también escenas no contiguas a la actual, es decir, que están a varios transportes de la escena actual
	public List<int> escenas;

	protected virtual void Start()
	{
		cargarTransporte();
	}

	protected virtual void cargarTransporte()
	{
		Manager.Instance.anyadirTransporte(SceneManager.GetActiveScene().buildIndex, gameObject, escenas);
	}
}
