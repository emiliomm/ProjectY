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
		Manager.instance.SetPausa(true);
		Manager.instance.StopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón

		for(int i = 0; i < horas; i++)
		{
			//Aumentamos la hora
			Manager.instance.AvanzaHora();
		}

		Camera.main.GetComponent<TP_Camera>().SetNormalMode();
		TPController.instance.SetState(TPController.State.Normal);
		Manager.instance.SetPausa(false);
		Manager.instance.ResumeNavMeshAgents();
		
	}
}