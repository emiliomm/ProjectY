using UnityEngine;
using System.Collections;

public class EventoInteractuable{

	public int IDEvento;
	public bool actualizado;

	public EventoInteractuable(int IDEvento)
	{
		this.IDEvento = IDEvento;
		actualizado = true;
	}
}
