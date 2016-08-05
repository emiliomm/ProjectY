using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml; 
using System.Xml.Serialization;

[XmlInclude(typeof(DatosAccionDialogo))]
[XmlInclude(typeof(DatosAccionObjeto))]
public class DatosAccion {

	public int ID; //necesario ¿?
	public string nombre;

	public List<ComprobarObjeto> objetos;

	public DatosAccion()
	{
		objetos = new List<ComprobarObjeto>();
	}

	public string DevolverNombre()
	{
		return nombre;
	}

	public virtual void EjecutarAccion(){  }
}
