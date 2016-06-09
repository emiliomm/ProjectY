using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ObjetoSer")]
[XmlInclude(typeof(NPCDatos))]
[XmlInclude(typeof(ObjetoDatos))]
public class InterDatos : ObjetoSer{
	
	public int ID;

	public InterDatos()
	{
		
	}

	public virtual string DevuelveNombreActual()
	{
		return "";
	}

	public static InterDatos LoadInterDatos(string path)
	{
		InterDatos inter_datos = Manager.Instance.DeserializeDataWithReturn<InterDatos>(path);

		return inter_datos;
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
