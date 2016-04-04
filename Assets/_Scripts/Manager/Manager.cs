﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Manager : MonoBehaviour {

	public static Manager Instance { get; private set; } //singleton

	public Dictionary<int,GameObject> npcs; //grupos de npcs cargados en la escena, QUIZÁS MEJOR EN OTRO OBJECTO
	public List<Grupo> GruposActivos; //grupos de misiones, QUIZÁS MEJOR EN OTRO OBJECTO

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		GruposActivos = new List<Grupo>();
		npcs = new Dictionary<int,GameObject>();

		//Creamos el directorio donde guardaremos los dialogos de los NPCs si no existe ya
		string copyTo = Application.persistentDataPath + "/NPC_Dialogo_Saves/";
		if (!System.IO.Directory.Exists(copyTo))
		{
			System.IO.Directory.CreateDirectory(copyTo);
		}

		//Cargamos el escenario
		SceneManager.LoadScene("Demo");
	}

	public void AddToNpcs(int id, GameObject gobj)
	{
		npcs.Add(id, gobj);
	}

	public void RemoveFromNpcs(int id)
	{
		npcs.Remove(id);
	}

	public GameObject GetNPC(int id)
	{
		GameObject npc;

		//Coge el GameObject mediante referencia, sino existe, el gameobject es null
		npcs.TryGetValue(id,out npc);

		return npc;
	}

	//Devuelve una lista de los valores del diccionario
	public List<GameObject> GetAllNPCs()
	{
		return npcs.Select(d=> d.Value).ToList();
	}

	public void AddToGrupos(Grupo g)
	{
		GruposActivos.Add(g);
	}

	public Grupo ComprobarGrupo(int id)
	{
		return GruposActivos.Find(x => x.DevolverGrupoID() == id);
	}

}

