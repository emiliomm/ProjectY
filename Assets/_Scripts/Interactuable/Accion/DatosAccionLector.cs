using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DatosAccionLector : DatosAccion {

	public int IDObjeto;
	public int numVariable;
	public int valorNegativo;

	public DatosAccionLector()
	{
		
	}
		
	public override void EjecutarAccion()
	{
		ManagerTiempo.instance.SetPausa(true);
		Manager.instance.StopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		GameObject lectorControllerGO = new GameObject("LectorController");

		LectorController lectorController = lectorControllerGO.AddComponent<LectorController>();
		lectorController.CargarVariable(IDObjeto, numVariable, valorNegativo);

		//Se establece el modo de la cámara en el Modo Objeto
		TPCamera.instance.SetObjectMode();
	}
}
