using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

/*
 * 	Clase de prueba que al chocar con el jugador te lleva a la escena indicada
 */
public class Transporte : MonoBehaviour
{
	public int ID; //NO SE USA

	//Lista con los indices de las escenas que son accesibles desde este transporte.
	//Se incluyen también escenas no contiguas a la actual, es decir, que están a varios transportes de la escena actual
	public List<int> escenas;

	public string haciaEscena; //nombre de la escena hacia donde nos transporta

	void Awake()
	{
		Manager.Instance.anyadirTransporte(SceneManager.GetActiveScene().buildIndex, gameObject, escenas);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" ) {
			SceneManager.LoadScene(haciaEscena, LoadSceneMode.Single);
		}
	}
}
