using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LectorController : MonoBehaviour {

	//Layermask del objeto que se cargará
	//Pasar al Manager
	private int layerMask = 8; //UIObjeto

	private GameObject Lector;
	private GameObject Tarjeta;
	private GameObject botonSalir;

	// Use this for initialization
	void Start () {
	
	}

	public void cargarVariable(int IDObjeto, int num_variable, int valorNegativo)
	{
		Lector = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Lector/Lector"), Camera.main.transform.position + Camera.main.transform.forward * 2f, Camera.main.transform.rotation);
		SetLayerRecursively(Lector, layerMask);
		Lector.GetComponent<Lector>().cargarValor(IDObjeto, num_variable, valorNegativo);

		Tarjeta = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Lector/LLave"), Camera.main.transform.position + Camera.main.transform.forward * 2f, Camera.main.transform.rotation);
		Tarjeta.transform.position += Camera.main.transform.TransformDirection(new Vector3(-0.08f, 0.5f, 0f));
		SetLayerRecursively(Tarjeta, layerMask);

		botonSalir = (GameObject)MonoBehaviour.Instantiate(Resources.Load("BotonPrefab"));
		botonSalir.transform.SetParent(Manager.Instance.canvasGlobal.transform, false); //Hacemos que la ventana sea hijo del canvas
		botonSalir.GetComponentInChildren<Text>().text = "Salir";
		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir(); }); //Listener del botón
	}

	//Aplica el layer a todos los hijos del objeto y al objeto
	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		go.layer = layerNumber;
		foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layerNumber;
		}
	}

	private void Salir()
	{
		Lector.GetComponent<Lector>().guardarValor();

		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();

		Camera.main.GetComponent<TP_Camera>().setNormalMode();

		Destroy(Lector);
		Destroy(Tarjeta);
		Destroy(botonSalir);
		Destroy(gameObject);
	}
}
