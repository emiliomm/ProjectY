using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class TransportePlayer : TransporteInter
{
	public int IDEscena = -1;
	public int IDTransporte;

	public bool transportando;

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();
		transportando = false;
	}

	protected override void CargarTransporte()
	{
		if(!escenas.Contains(IDEscena))
			escenas.Add(IDEscena);

		Manager.instance.AnyadirTransporte(SceneManager.GetActiveScene().buildIndex, gameObject, escenas);
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !transportando && !TP_Controller.Instance.getTransportando())
		{
			DontDestroyOnLoad(gameObject);
			SceneManager.LoadScene(IDEscena, LoadSceneMode.Single);
			TP_Controller.Instance.setTransportando(true);
			transportando = true;
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && transportando) 
		{
			transportando = false;
		}
	}

	protected virtual void OnLevelWasLoaded(int level)
	{
		if(transportando && level == IDEscena)
		{
			StartCoroutine(Transporte());
		}
	}

	protected IEnumerator Transporte()
	{
		yield return null;

		GameObject transporteGO = Manager.instance.EncontrarTransporte(IDTransporte);

		if(transporteGO != null)
		{
			//El número -1.13676f en el eje Y es la resta entre el centro del objeto del transporte y el centro del objeto del jugador, cambiar si se cambia el objeto del jugador
			TP_Controller.Instance.transform.position = new Vector3(transporteGO.transform.position.x, transporteGO.transform.position.y - 1.13676f, transporteGO.transform.position.z);
			//MOVER CÁMARA AQUÍ EN EL FUTURO
			transporteGO.GetComponent<TransportePlayer>().transportando = true;
			TP_Controller.Instance.setTransportando(false);
		}
		Destroy(gameObject);
	}
}
