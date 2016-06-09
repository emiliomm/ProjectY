using UnityEngine;
using System.Collections;

public class InteractuableNPC : Interactuable {

	NPCDatos datos;

	protected override void Start()
	{
		//Ejecuta el metodo del padre
		base.Start();

		if (System.IO.File.Exists(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml"))
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaNPCDatosGuardados + ID.ToString()  + ".xml");
		}
		else
		{
			datos = NPCDatos.LoadInterDatos(Manager.rutaNPCDatos + ID.ToString()  + ".xml");
		}

		SetNombre(datos.DevuelveNombreActual());
	}

	public int DevuelveIndiceNombre()
	{
		return datos.DevuelveIndiceNombre();
	}

	public void SetIndiceNombre(int num)
	{
		datos.SetIndiceNombre(num);
	}

	public string DevuelveNombreActual()
	{
		return datos.DevuelveNombreActual();
	}

	public void AddDatosToColaObjetos()
	{
		datos.AddToColaObjetos();
	}

	public override string DevuelveNombreDialogo()
	{
		return DevuelveNombreActual();
	}
}
