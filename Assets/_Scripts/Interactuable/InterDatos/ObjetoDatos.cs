using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

[XmlRoot("ObjetoSer")]
public class ObjetoDatos : InterDatos{

	public string nombre;

	public ObjetoDatos()
	{
		
	}

	public override string DevuelveNombreActual()
	{
		return nombre;
	}

	public new static ObjetoDatos LoadInterDatos(string path)
	{
		ObjetoDatos inter_datos = Manager.Instance.DeserializeDataWithReturn<ObjetoDatos>(path);

		return inter_datos;
	}
}
