using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ManagerEscenas : MonoBehaviour {

	private int IDEscenaActual = -1;
	private int IDEscenaCargada = -1;

	public static ManagerEscenas instance; //Instancia propia de la clase

	private AsyncOperation async;

	void Awake ()
	{
		// First we check if there are any other instances conflicting
		if(instance != null && instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		instance = this;
	}

	public void CargarEscenaDirectamente(int indiceEscena)
	{
		bool escenaYaCargada = false;

		for (int n = 0; n < SceneManager.sceneCount; ++n)
		{
			Scene scene = SceneManager.GetSceneAt(n);
			if(scene.buildIndex == indiceEscena)
				escenaYaCargada = true;
		}

		if(!escenaYaCargada)
		{
			SceneManager.LoadScene (indiceEscena);
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(indiceEscena));

			IDEscenaCargada = IDEscenaActual;
			IDEscenaActual = indiceEscena;
		}
	}

	public void CargarEscenaSegundoPlano(int indiceEscena)
	{
		bool escenaYaCargada = false;

		for (int n = 0; n < SceneManager.sceneCount; ++n)
		{
			Scene scene = SceneManager.GetSceneAt(n);
			if(scene.buildIndex == indiceEscena)
				escenaYaCargada = true;
		}

		if(!escenaYaCargada)
		{
			StartCoroutine(load2(indiceEscena));

			/*
			SceneManager.LoadSceneAsync(indiceEscena, LoadSceneMode.Additive);
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(indiceEscena));

			escenaCargada = escenaActual;
			escenaActual = indiceEscena;
			*/
		}
	}

	IEnumerator load2(int indiceEscena) {

		yield return StartCoroutine(load(indiceEscena));

		IDEscenaCargada = IDEscenaActual;
		IDEscenaActual = indiceEscena;

		async.allowSceneActivation = true;

		//Necesario para que activar una escena activa funcione
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(indiceEscena));

		//Debug.Log(SceneManager.GetActiveScene().name);
	}

	IEnumerator load(int indiceEscena) {
		//Debug.LogWarning("ASYNC LOAD STARTED - ");

		async = SceneManager.LoadSceneAsync(indiceEscena, LoadSceneMode.Additive);
		async.allowSceneActivation = false; //async se para al 90%

		while (async.progress < 0.9f)
		{
			yield return new WaitForEndOfFrame();
			//Debug.Log(async.progress);
		}

		//yield return async;
		//yield return new WaitForSeconds(1.0f);
		yield return null;
	}

	public void EliminaEscena(int indiceEscena)
	{
		if(SceneManager.sceneCount > 0)
		{
			if(indiceEscena == IDEscenaActual)
			{
				IDEscenaActual = IDEscenaCargada;
				//IDEscenaCargada = indiceEscena;

				SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(IDEscenaActual));
			}

			IDEscenaCargada = -1;

			//SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex());
			SceneManager.UnloadSceneAsync(indiceEscena);
			Resources.UnloadUnusedAssets();
			//Debug.Log(SceneManager.GetActiveScene().name);
		}
	}

	public int GetNumeroEscenaCargada()
	{
		return IDEscenaCargada;
	}

	public int GetNumeroEscenaActual()
	{
		return IDEscenaActual;
	}

	public Scene GetEscenaActual()
	{
		return SceneManager.GetSceneByBuildIndex(IDEscenaActual);
	}
}
