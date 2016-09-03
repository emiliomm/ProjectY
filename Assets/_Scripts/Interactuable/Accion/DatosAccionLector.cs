using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DatosAccionLector : DatosAccion {

	public int IDObjeto;
	public int num_variable;
	public int valorNegativo;

	public DatosAccionLector()
	{
		
	}
		
	public override void EjecutarAccion()
	{
		Manager.Instance.setPausa(true);
		Manager.Instance.stopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		GameObject LectorController = new GameObject("LectorController");

		LectorController lectorController = LectorController.AddComponent<LectorController>();
		lectorController.cargarVariable(IDObjeto, num_variable, valorNegativo);

		//Se establece el modo de la cámara en el Modo Objeto
		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}
}
