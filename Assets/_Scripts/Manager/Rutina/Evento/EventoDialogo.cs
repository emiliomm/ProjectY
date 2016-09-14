using UnityEngine;
using System.Collections;

public class EventoDialogo : Evento {

	public int IDInteractuable;
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
