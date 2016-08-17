using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Info_Interactuable{

	private int tipoInter;
	private int IDEscena;

	//-1: El interactuable no tiene rutina
	//x: ID de la rutina
	private int IDRutina;
	private List<EventoInteractuable> eventos;
	private DateTime ultimaFechaCambioLugar;
	private DateTime ultimaFechaCambioRutina;

	public Info_Interactuable()
	{
		eventos = new List<EventoInteractuable>();
		IDEscena = -1; //valor inicial
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

	public DateTime getFechaCambioLugar()
	{
		return ultimaFechaCambioLugar;
	}

	public void setFechaCambioLugar(DateTime fecha)
	{
		ultimaFechaCambioLugar = fecha;
	}

	public DateTime getFechaCambioRutina()
	{
		return ultimaFechaCambioRutina;
	}

	public void setFechaCambioRutina(DateTime fecha)
	{
		ultimaFechaCambioRutina = fecha;
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
