using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ObjetoSer")]
public class NPCDatos : ObjetoSer{
	
	public int ID;
	public int indiceNombre;
	public List<string> nombres;

	public NPCDatos()
	{
		nombres = new List<string>();
	}

	public void SetIndiceNombre(int indice)
	{
		indiceNombre = indice;
	}

	public string DevuelveNombreActual()
	{
		return nombres[indiceNombre];
	}

	public int DevuelveIndiceNombre()
	{
		return indiceNombre;
	}

	public static NPCDatos LoadNPCDatos(string path)
	{
		NPCDatos npc_datos = Manager.Instance.DeserializeDataWithReturn<NPCDatos>(path);

		return npc_datos;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaNPCDatosGuardados, Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
	}

	public void AddToColaObjetos()
	{
		Manager.Instance.AddToColaObjetos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml", this);
	}
}
