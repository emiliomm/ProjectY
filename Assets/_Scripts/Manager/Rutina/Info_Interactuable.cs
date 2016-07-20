using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Info_Interactuable{

	public int tipoInter;
	public int IDEscena;
	public int IDRutina;
	public List<EventoInteractuable> eventos;
	public DateTime fechaAnyadido;

	public Info_Interactuable()
	{
		eventos = new List<EventoInteractuable>();
	}

	public void addEvento(int IDEvento)
	{
		int indice = eventos.FindIndex(x => x.IDEvento == IDEvento);

		//evento no encontrado
		if(indice == -1)
		{
			EventoInteractuable ev = new EventoInteractuable(IDEvento);
			eventos.Add(ev);
		}
		//Si se ha encontrado, se actualiza el estado ha actualizado
		else
		{
			eventos[indice].actualizado = true;
		}
	}
}
