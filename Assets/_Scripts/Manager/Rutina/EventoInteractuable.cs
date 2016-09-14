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

	public int DevuelveIDEvento()
	{
		return IDEvento;
	}

	public bool DevuelveActualizado()
	{
		return actualizado;
	}

	public void SetActualizado(bool actualizado)
	{
		this.actualizado = actualizado;
	}
}
