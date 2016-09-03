using UnityEngine;
using System.Collections;

public class EventoDialogo : Evento {

	public int IDInter;
	public int IDDialog;

	//HACER EVENTOS DE UN SOLO USO

	public EventoDialogo()
	{
		
	}

	//Ejecuta el diálogo con los parámetros indicados
	public override void EjecutarEvento()
	{
		NPC_Dialogo dialog = NPC_Dialogo.BuscarDialogo(IDInter, IDDialog);
		TextBox.Instance.EmpezarDialogo(dialog);
	}
}
