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
		if (other.tag == "Player" && !transportando && !TPController.instance.GetTransportando())
		{
			DontDestroyOnLoad(gameObject);
			SceneManager.LoadScene(IDEscena, LoadSceneMode.Single);
			TPController.instance.SetTransportando(true);
			ManagerTiempo.instance.GuardarTiempo();
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

	protected override void OnEnable() {
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected override void OnDisable() {
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(transportando && scene.buildIndex == IDEscena)
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
			TPController.instance.transform.position = new Vector3(transporteGO.transform.position.x, transporteGO.transform.position.y - 1.13676f, transporteGO.transform.position.z);
			//MOVER CÁMARA AQUÍ EN EL FUTURO
			transporteGO.GetComponent<TransportePlayer>().transportando = true;
			TPController.instance.SetTransportando(false);
		}
		Destroy(gameObject);
	}
}
