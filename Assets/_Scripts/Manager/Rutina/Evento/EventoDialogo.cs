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

			NPC_Dialogo dialog = NPC_Dialogo.BuscarDialogo(IDInter, IDDialog);
			TextBox.Instance.PrepararDialogo(inter, dialog, ID);
			ejecutado = true;
		}
	}
}
