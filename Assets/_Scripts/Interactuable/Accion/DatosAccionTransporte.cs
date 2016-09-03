using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DatosAccionTransporte : DatosAccion
{
	public int IDEscena;
//	public int IDTransporte;
	public float coordX, coordY, coordZ;

	public DatosAccionTransporte()
	{
		
	}

	public override void EjecutarAccion()
	{
		SceneManager.LoadScene(IDEscena, LoadSceneMode.Single);

		TP_Controller.Instance.transform.position = new Vector3(coordX, coordY, coordZ);
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);

//		GameObject transporteMono = new GameObject("Transporte Mono");
//		MonoBehaviour.DontDestroyOnLoad(transporteMono);
//		TransporteMono t = transporteMono.AddComponent<TransporteMono>();
//		t.Empezar(IDTransporte);
	}

//	public class TransporteMono : MonoBehaviour
//	{
//		int IDTransporte;
//
//		public void Empezar(int IDTransporte)
//		{
//			this.IDTransporte = IDTransporte;
//			StartCoroutine(Transporte());
//		}
//
//		private IEnumerator Transporte()
//		{
//			//Esperamos a que se inicializen los objetos de la escena
//			yield return new WaitForSeconds (0.25f);
//
//			GameObject transporte = Manager.Instance.EncontrarTransporte(this.IDTransporte);
//
//			Debug.Log(transporte.transform.position);
//
//			TP_Controller.Instance.gameObject.transform.position = transporte.transform.position + new Vector3(5f, 0f, 0f);
//		}
//	}


}
