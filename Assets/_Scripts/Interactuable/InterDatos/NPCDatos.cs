using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ObjetoSer")]
public class NPCDatos : InterDatos{
	
	public int indiceNombre;
	public List<string> nombres;

	public NPCDatos()
	{
		nombres = new List<string>();
	}

	public override string DevuelveNombreActual()
	{
		return nombres[indiceNombre];
	}

	public void SetIndiceNombre(int indice)
	{
		indiceNombre = indice;
	}
		
	public int DevuelveIndiceNombre()
	{
		return indiceNombre;
	}

	public new static NPCDatos LoadInterDatos(string path)
	{
		NPCDatos inter_datos = Manager.Instance.DeserializeData<NPCDatos>(path);

		return inter_datos;
	}

}
