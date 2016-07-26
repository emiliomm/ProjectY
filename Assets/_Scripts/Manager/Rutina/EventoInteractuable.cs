using UnityEngine;
using System.Collections;

public class EventoInteractuable{

	private int IDEvento;
	private bool actualizado;

	public EventoInteractuable(int IDEvento)
	{
		this.IDEvento = IDEvento;
		actualizado = true;
	}

	public int devuelveIDEvento()
	{
		return IDEvento;
	}

	public bool devuelveActualizado()
	{
		return actualizado;
	}

	public void setActualizado(bool ac)
	{
		actualizado = ac;
	}
}
