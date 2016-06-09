using UnityEngine;
using System.Collections;

public class InteractuableObjeto : Interactuable {

	ObjetoDatos datos;

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();

		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = ObjetoDatos.LoadInterDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		SetNombre(datos.DevuelveNombreActual());
	}

	public override string DevuelveNombreDialogo()
	{
		//Los objetos no tienen nombre en el dialogo
		return "";
	}
}

