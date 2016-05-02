using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDatos{
	
	public int ID;

	[SerializeField]
	private int nombreActual;
	[SerializeField]
	private List<string> nombres;

	public NPCDatos()
	{
		nombres = new List<string>();
	}

	public string DevuelveNombreActual()
	{
		return nombres[nombreActual];
	}

	public static NPCDatos LoadNPCDatos(string path)
	{
		NPCDatos npc_datos = Manager.Instance.DeserializeDataWithReturn<NPCDatos>(path);

		return npc_datos;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaNPCDatos, Manager.rutaNPCDatos + ID.ToString()  + ".xml");
	}
}
