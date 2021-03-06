﻿using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

//Usada en DatosAccionTransporte
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
		ManagerEscenas.instance.CargarEscenaDirectamente(IDEscena);
		//SceneManager.LoadScene(IDEscena, LoadSceneMode.Single);
	}

	protected virtual void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected virtual void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(enTransporte && scene.buildIndex == IDEscena)
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
			transporte = Manager.instance.EncontrarTransporte(IDTransporte);
		}

		if(transporte != null)
		{
			//El número -1.13676f en el eje Y es la resta entre el centro del objeto del transporte y el centro del objeto del jugador, cambiar si se cambia el objeto del jugador
			TPController.instance.transform.position = new Vector3(transporte.transform.position.x, transporte.transform.position.y - 1.13676f, transporte.transform.position.z);
			//MOVER CÁMARA AQUÍ EN EL FUTURO
		}
		else
		{
			TPController.instance.transform.position = coord;
		}

		TPController.instance.SetState(TPController.State.Normal);

		Destroy(gameObject);
	}
}
