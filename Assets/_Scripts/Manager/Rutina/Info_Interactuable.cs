using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Info_Interactuable{

	private int tipoInter;
	private int IDEscena;
	private int IDRutina;
	private List<EventoInteractuable> eventos;
	private DateTime fechaAnyadido;

	public Info_Interactuable()
	{
		eventos = new List<EventoInteractuable>();
	}

	public void setTipoInter(int tipo)
	{
		tipoInter = tipo;
	}

	public int devolverTipo()
	{
		return tipoInter;
	}

	public void setIDEscena(int IDE)
	{
		IDEscena = IDE;
	}

	public int devolverIDEscena()
	{
		return IDEscena;
	}

	public void setIDRutina(int IDR)
	{
		IDRutina = IDR;
	}

	public int devolverIDRutina()
	{
		return IDRutina;
	}

	public int devolverNumeroEventos()
	{
		return eventos.Count;
	}

	public void actualizarEvento(int num)
	{
		eventos[num].setActualizado(true);
	}

	public bool devuelveEventoActualizado(int num)
	{
		return eventos[num].devuelveActualizado();
	}

	public int devuelveIDEvento(int num)
	{
		return eventos[num].devuelveIDEvento();
	}

	public void eliminarEvento(int num)
	{
		eventos.RemoveAt(num);
	}

	public DateTime getFecha()
	{
		return fechaAnyadido;
	}

	public void setFecha(DateTime fecha)
	{
		fechaAnyadido = fecha;
	}

	public void addEvento(int IDEvento)
	{
		int indice = eventos.FindIndex(x => x.devuelveIDEvento() == IDEvento);

		//evento no encontrado
		if(indice == -1)
		{
			EventoInteractuable ev = new EventoInteractuable(IDEvento);
			eventos.Add(ev);
		}
		//Si se ha encontrado, se actualiza el estado ha actualizado
		else
		{
			eventos[indice].setActualizado(true);
		}
	}
}
