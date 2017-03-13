using UnityEngine;

using System.Xml.Serialization;

/*
 * 	Clase derivada de DatosAccion que permite a una acción ejecutar un diálogo
 */
public class DatosAccionDialogo : DatosAccion{

	//No se serializa al guardar la accion ya que la serialización de los diálogos se maneja independientemente de esta clase
	[XmlIgnore]
	public Dialogo dialogo;

	public int IDInteractuable; //ID del Interactuable que inicia el diálogo
	public int IDDialogo; //ID del dialogo

	public DatosAccionDialogo()
	{
		dialogo = new Dialogo();
	}

	//Busca el fichero del diálogo en el directorio correspondiente
	public void CargaDialogo()
	{
		//Si existe un fichero guardado, cargamos ese fichero, sino cargamos el fichero por defecto
		if (System.IO.File.Exists(Manager.rutaInterDialogosGuardados + IDInteractuable.ToString() + "-" + IDDialogo.ToString() + ".xml"))
		{
			dialogo = Dialogo.LoadDialogo(Manager.rutaInterDialogosGuardados + IDInteractuable.ToString() + "-" + IDDialogo.ToString() + ".xml");
		}
		else
		{
			dialogo = Dialogo.LoadDialogo(Manager.rutaInterDialogos + IDInteractuable.ToString() + "-" + IDDialogo.ToString() + ".xml");
		}
	}

	public int DevuelveIDDialogo()
	{
		return IDDialogo;
	}

	public Dialogo DevuelveDialogo()
	{
		return dialogo;
	}

	//Ejecuta el diálogo
	public override void EjecutarAccion()
	{
		GameObject gameobject = Manager.instance.GetInteractuable(IDInteractuable);

		if(gameobject != null)
		{
			Interactuable interactuable = gameobject.GetComponent<Interactuable>() as Interactuable;

			if(interactuable != null)
			{
				TPController.instance.SetState(TPController.State.Normal);
				ManagerDialogo.instance.PrepararDialogo(interactuable, dialogo, -1);
			}
		}
	}
}
