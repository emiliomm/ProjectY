using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class DatosAccionDialogo : DatosAccion{

	public NPC_Dialogo diag;

	public int ID_NPC;
	public int ID_Diag;

	public DatosAccionDialogo()
	{
		diag = new NPC_Dialogo();
	}

	public void CargaDialogo()
	{
		//Cargamos el dialogo
		//Si existe un fichero guardado, cargamos ese fichero, sino
		//cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaNPCDialogosGuardados + ID_NPC.ToString() + "-" + ID_Diag.ToString() + ".xml"))
		{
			diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogosGuardados + ID_NPC.ToString() + "-" + ID_Diag.ToString() + ".xml");
		}
		else
		{
			diag = NPC_Dialogo.LoadNPCDialogue(Manager.rutaNPCDialogos + ID_NPC.ToString() + "-" + ID_Diag.ToString() + ".xml");
		}
	}

	public int DevuelveIDDiag()
	{
		return ID_Diag;
	}

	public NPC_Dialogo DevuelveDialogo()
	{
		return diag;
	}

	public override void EjecutarAccion()
	{
		GameObject gobj = Manager.Instance.GetInteractuables(ID_NPC);

		if(gobj != null)
		{
			Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;

			//Si el objeto es un NPC
			if(inter != null)
			{
				TextBox.Instance.StartDialogue(inter, diag);
			}
		}
	}
}
