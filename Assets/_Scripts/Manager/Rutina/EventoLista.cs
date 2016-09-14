using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventoLista{

	private List<int> interactuables;
	private Evento evento;

	public EventoLista(int IDInteractuableInicial, Evento evento)
	{
		interactuables = new List<int>();
		interactuables.Add(IDInteractuableInicial);

		this.evento = evento;
	}

	public Evento DevuelveEvento()
	{
		return evento;
	}

	public void AddInteractuable(int IDInteractuable)
	{
		if(!InterExiste(IDInteractuable))
		{
			interactuables.Add(IDInteractuable);
		}
	}

	public int PosicionInteractuable(int IDInteractuable)
	{
		return interactuables.IndexOf(IDInteractuable);
	}

	public void BorrarInteractuable(int pos)
	{
		interactuables.RemoveAt(pos);
	}

	public bool IsInterEmpty()
	{
		return interactuables.Count == 0;
	}

	private bool InterExiste(int IDInteractuable)
	{
		bool existe = PosicionInteractuable(IDInteractuable) != -1;

		return existe;
	}
}
