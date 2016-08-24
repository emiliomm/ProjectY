using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rutina{

	public int ID;
	public bool Autorutina; //Indica si la rutina tiene autorutina

	public List<PosicionLugarSiguiente> posLugarSiguientes;

	public Rutina()
	{
		posLugarSiguientes = new List<PosicionLugarSiguiente>();
	}
		
	public static Rutina LoadRutina(string path)
	{
		Rutina rutina = Manager.Instance.DeserializeData<Rutina>(path);

		return rutina;
	}
}
