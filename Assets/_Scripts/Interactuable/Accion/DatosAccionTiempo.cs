using UnityEngine;
using System.Collections;

public class DatosAccionTiempo : DatosAccion
{
	public int horas;

	public DatosAccionTiempo()
	{

	}

	public override void EjecutarAccion()
	{
		ManagerTiempo.instance.SetPausa(true);
		Manager.instance.StopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		for(int i = 0; i < horas; i++)
		{
			//Aumentamos la hora
			ManagerTiempo.instance.AvanzaHora();
		}

		Debug.Log("Se ha avanzado" + horas + "horas");

		TPCamera.instance.SetNormalMode();
		TPController.instance.SetState(TPController.State.Normal);
		ManagerTiempo.instance.SetPausa(false);
		Manager.instance.ResumeNavMeshAgents();
		
	}
}