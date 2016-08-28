using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class TransportePlayer : TransporteInter
{
	public string haciaEscena; //nombre de la escena hacia donde nos transporta

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();
	}

	protected virtual void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" ) {
			SceneManager.LoadScene(haciaEscena, LoadSceneMode.Single);
		}
	}
}
