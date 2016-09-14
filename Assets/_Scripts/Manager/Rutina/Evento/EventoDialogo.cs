using UnityEngine;
using System.Collections;

public class EventoDialogo : Evento {

	public int IDInter;
	public int IDDialog;

	public bool ejecutado; //Valor por defecto es false

	public EventoDialogo()
	{
		
	}

	//Ejecuta el diálogo con los parámetros indicados
	public override void EjecutarEvento()
	{
		if(!ejecutado)
		{
			Interactuable inter = null;

			Dialogo dialog = Dialogo.BuscarDialogo(IDInter, IDDialog);
			TextBox.instance.PrepararDialogo(inter, dialog, ID);
			ejecutado = true;
		}
	}
}
