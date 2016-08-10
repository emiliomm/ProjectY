using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 	Clase de prueba que al chocar con el jugador te lleva a la escena indicada
 */

public class Transporte : MonoBehaviour {

	public string nombreEscena;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" ) {
			SceneManager.LoadScene(nombreEscena, LoadSceneMode.Single);
		}
	}
}
