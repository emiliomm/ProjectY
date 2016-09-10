using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class TransporteController : MonoBehaviour
{
	private bool enTransporte;

	private int IDEscena;
	private int IDTransporte;
	private Vector3 coord;

	void Awake()
	{
		enTransporte = false;
	}

	public void Constructor(int IDEscena, int IDTransporte, Vector3 coord)
	{
		this.IDEscena = IDEscena;
		this.IDTransporte = IDTransporte;
		this.coord = coord;

		DontDestroyOnLoad(gameObject);
		enTransporte = true;
		SceneManager.LoadScene(IDEscena, LoadSceneMode.Single);
	}

	void OnLevelWasLoaded(int level)
	{
		if(enTransporte && level == IDEscena)
		{
			enTransporte = false;
			StartCoroutine(Transporte());
		}
	}

	public IEnumerator Transporte()
	{
		yield return null;
		GameObject transporte = null;

		if(IDTransporte != -1)
		{
			transporte = Manager.Instance.EncontrarTransporte(IDTransporte);
		}

		if(transporte != null)
		{
			//El número -1.13676f en el eje Y es la resta entre el centro del objeto del transporte y el centro del objeto del jugador, cambiar si se cambia el objeto del jugador
			TP_Controller.Instance.transform.position = new Vector3(transporte.transform.position.x, transporte.transform.position.y - 1.13676f, transporte.transform.position.z);
			//MOVER CÁMARA AQUÍ EN EL FUTURO
		}
		else
		{
			TP_Controller.Instance.transform.position = coord;
		}

		TP_Controller.Instance.SetState(TP_Controller.State.Normal);

		Destroy(gameObject);
	}
}
