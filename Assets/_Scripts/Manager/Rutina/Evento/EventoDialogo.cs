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
			NPC_Dialogo dialog = NPC_Dialogo.BuscarDialogo(IDInter, IDDialog);
			TextBox.Instance.EmpezarDialogo(dialog, ID);
			ejecutado = true;
		}
	}
}
