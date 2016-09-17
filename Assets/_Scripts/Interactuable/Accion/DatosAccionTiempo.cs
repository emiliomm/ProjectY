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

		Camera.main.GetComponent<TPCamera>().SetNormalMode();
		TPController.instance.SetState(TPController.State.Normal);
		ManagerTiempo.instance.SetPausa(false);
		Manager.instance.ResumeNavMeshAgents();
		
	}
}