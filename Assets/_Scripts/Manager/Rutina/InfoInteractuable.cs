using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class InfoInteractuable{

	private int tipoInteractuable;
	private int IDEscena;

	//-1: El interactuable no tiene rutina
	//x: ID de la rutina
	private int IDRutina;
	private List<EventoInteractuable> eventos; //usado para saber cuales son los eventos del lugar actual
	private DateTime ultimaFechaCambioLugar;
	private DateTime ultimaFechaCambioRutina;

	public InfoInteractuable()
	{
		eventos = new List<EventoInteractuable>();
		IDEscena = -1; //valor inicial
	}

	public void SetTipoInter(int tipo)
	{
		tipoInteractuable = tipo;
	}

	public int DevolverTipo()
	{
		return tipoInteractuable;
	}

	public void SetIDEscena(int IDEscena)
	{
		this.IDEscena = IDEscena;
	}

	public int DevolverIDEscena()
	{
		return IDEscena;
	}

	public void SetIDRutina(int IDRutina)
	{
		this.IDRutina = IDRutina;
	}

	public int DevolverIDRutina()
	{
		return IDRutina;
	}

	public int DevolverNumeroEventos()
	{
		return eventos.Count;
	}

	public void DesactualizarEvento(int num)
	{
		eventos[num].SetActualizado(false);
	}

	public bool DevuelveEventoActualizado(int num)
	{
		return eventos[num].DevuelveActualizado();
	}

	public int DevuelveIDEvento(int num)
	{
		return eventos[num].DevuelveIDEvento();
	}

	public void EliminarEvento(int num)
	{
		eventos.RemoveAt(num);
	}

	public DateTime GetFechaCambioLugar()
	{
		return ultimaFechaCambioLugar;
	}

	public void SetFechaCambioLugar(DateTime fecha)
	{
		ultimaFechaCambioLugar = fecha;
	}

	public DateTime GetFechaCambioRutina()
	{
		return ultimaFechaCambioRutina;
	}

	public void SetFechaCambioRutina(DateTime fecha)
	{
		ultimaFechaCambioRutina = fecha;
	}

	public void AddEvento(int IDEvento)
	{
		int indice = eventos.FindIndex(x => x.DevuelveIDEvento() == IDEvento);

		//evento no encontrado
		if(indice == -1)
		{
			EventoInteractuable eventoInteractuable = new EventoInteractuable(IDEvento);
			eventos.Add(eventoInteractuable);
		}
		//Si se ha encontrado, se actualiza el estado ha actualizado
		else
		{
			eventos[indice].SetActualizado(true);
		}
	}
}
