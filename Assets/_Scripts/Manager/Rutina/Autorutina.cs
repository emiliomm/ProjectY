﻿using UnityEngine;
using System.Collections;

public class Autorutina{

	public int ID;
	public int IDInter;

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

	public static Autorutina LoadAutoRutina(string path)
	{
		Autorutina autorut = Manager.Instance.DeserializeData<Autorutina>(path);

		return autorut;
	}
		
	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaAutoRutinasGuardadas, ID.ToString()  + ".xml");
	}
}
