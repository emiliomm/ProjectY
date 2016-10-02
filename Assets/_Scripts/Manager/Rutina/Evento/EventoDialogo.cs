using UnityEngine;
using System.Collections;

public class EventoDialogo : Evento {

	public int IDInteractuable; //-1 si es un diálogo que no pertenece a ningún interactuable (Dialogo que sería -1-x, utilizado en casos especiales, como este).
	public int IDDialogo;

	public bool ejecutado; //Valor por defecto es false

	public EventoDialogo()
	{
		
	}

	//Ejecuta el diálogo con los parámetros indicados
	public override void EjecutarEvento()
	{
		if(!ejecutado)
		{
			Interactuable interactuable = null;

			Dialogo dialogo = Dialogo.BuscarDialogo(IDInteractuable, IDDialogo);
			TextBox.instance.PrepararDialogo(interactuable, dialogo, ID);
			ejecutado = true;
		}
	}
}
