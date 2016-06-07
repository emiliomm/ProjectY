using UnityEngine;
using System.Collections;

using System.Xml; 
using System.Xml.Serialization;

[XmlInclude(typeof(DatosAccionDialogo))]
public class DatosAccion {

	public int ID; //necesario ¿?
	public string nombre;

	public DatosAccion() {   }

	public string DevolverNombre()
	{
		return nombre;
	}

	public virtual void EjecutarAccion(){  }
}
