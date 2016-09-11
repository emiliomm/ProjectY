using UnityEngine;

using System.Xml.Serialization;

/*
 * 	Clase derivada de DatosAccion que permite a una acción ejecutar un diálogo
 */
public class DatosAccionDialogo : DatosAccion{

	//No se serializa al guardar la accion ya que la serialización de los diálogos se maneja independientemente de esta clase
	[XmlIgnore]
	public NPC_Dialogo diag;

	public int ID_NPC; //ID del Interactuable que inicia el diálogo
	public int ID_Diag; //ID del dialogo

	//Indica si el diálogo se puede ejecutar a distancia. Si es true, los parámetros tam indican el tamaño
	//del prisma rectangular con el cual, el jugador al colisionar inicia el diálogo
	public bool aDistancia;
	public int tamX;
	public int tamY;
	public int tamZ;

	public DatosAccionDialogo()
	{
		diag = new NPC_Dialogo();
	}

	//Busca el fichero del diálogo en el directorio correspondiente
	public void CargaDialogo()
	{
		//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
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

	//Modifica si el diálogo se puede ejecutar a distancia o no
	public void setADistancia(bool valor)
	{
		aDistancia = valor;
	}

	//Ejecuta el diálogo
	public override void EjecutarAccion()
	{
		GameObject gobj = Manager.Instance.GetInteractuable(ID_NPC);

		if(gobj != null)
		{
			Interactuable inter = gobj.GetComponent<Interactuable>() as Interactuable;

			if(inter != null)
			{
				TP_Controller.Instance.SetState(TP_Controller.State.Normal);
				TextBox.Instance.PrepararDialogo(inter, diag, -1);
			}
		}
	}
}
