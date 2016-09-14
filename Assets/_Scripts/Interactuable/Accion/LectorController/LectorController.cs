using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LectorController : MonoBehaviour {

	//Layermask del objeto que se cargará
	//Pasar al Manager
	private int layerMask = 8; //UIObjeto

	private GameObject lector;
	private GameObject tarjeta;
	private GameObject botonSalir;

	// Use this for initialization
	void Start () {
	
	}

	public void CargarVariable(int IDObjeto, int numVariable, int valorNegativo)
	{
		lector = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Lector/Lector"), Camera.main.transform.position + Camera.main.transform.forward * 2f, Camera.main.transform.rotation);
		SetLayerRecursively(lector, layerMask);
		lector.GetComponent<Lector>().CargarValor(IDObjeto, numVariable, valorNegativo);

		tarjeta = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Lector/LLave"), Camera.main.transform.position + Camera.main.transform.forward * 2f, Camera.main.transform.rotation);
		tarjeta.transform.position += Camera.main.transform.TransformDirection(new Vector3(-0.08f, 0.5f, 0f));
		SetLayerRecursively(tarjeta, layerMask);

		botonSalir = (GameObject)MonoBehaviour.Instantiate(Resources.Load("BotonPrefab"));
		botonSalir.transform.SetParent(Manager.Instance.canvasGlobal.transform, false); //Hacemos que la ventana sea hijo del canvas
		botonSalir.GetComponentInChildren<Text>().text = "Salir";
		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir(); }); //Listener del botón
	}

	//Aplica el layer a todos los hijos del objeto y al objeto
	public static void SetLayerRecursively(GameObject gameobject, int layerNumber)
	{
		gameobject.layer = layerNumber;
		foreach (Transform trans in gameobject.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layerNumber;
		}
	}

	private void Salir()
	{
		lector.GetComponent<Lector>().GuardarValor();

		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();

		Camera.main.GetComponent<TP_Camera>().setNormalMode();

		Destroy(lector);
		Destroy(tarjeta);
		Destroy(botonSalir);
		Destroy(gameObject);
	}
}
