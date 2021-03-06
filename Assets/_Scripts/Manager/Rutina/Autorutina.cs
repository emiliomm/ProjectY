﻿using UnityEngine;
using System.Collections;
using System;

public class Autorutina{

	public int ID; //El ID de la autorutina es el mismo que el de la rutina que acompaña
	public int IDInteractuable;

	//Número de horas que deben pasar antes de que la hora cambie
	public int numHoras;

	//Hora (del juego) en la que se coloca la autorutina
	//se calcula sumando la hora de creación + numHoras
	public int posHora;

	//El numRecorridosMaximos indica cuantas veces se recorre la autorutina antes de que esta cambie
	//Se calcula con posHora + numHoras
	//numRecorridosActuales indica cuantos veces se ha recorrido la autorutina desde que fue creada
	public int numRecorridosActuales;
	public int numRecorridosMaximos;

	//ID de la rutina a la que pasa el interactuable
	public int IDSigRutina;

	//Fecha de la rutina añadida, para encontrar rutinas duplicadas (no se serializa)
	private DateTime fechaRutina;

	public Autorutina()
	{
		
	}

	public bool Recorrido()
	{
		bool SigRutina = false;

		numRecorridosActuales++;

		//Guardamos los datos
		Serialize();

		if(numRecorridosActuales == numRecorridosMaximos)
			SigRutina = true;
		
		return SigRutina;
	}

	public void SetFechaRutina(DateTime fecha)
	{
		fechaRutina = fecha;
	}

	public DateTime GetFechaRutina()
	{
		return fechaRutina;
	}

	public static Autorutina LoadAutoRutina(string path)
	{
		Autorutina autorutina = Manager.instance.DeserializeData<Autorutina>(path);

		return autorutina;
	}
		
	public void Serialize()
	{
		Manager.instance.SerializeData(this, Manager.rutaAutorutinasGuardadas, ID.ToString()  + ".xml");
	}
}
