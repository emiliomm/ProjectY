using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Contenedor{

	private List<Autorutina> autorutinas;
	private List<LugarSiguiente> lugarSiguientes;

	public Contenedor()
	{
		
	}

	public void AddAutorutina(Autorutina autorutina)
	{
		if(autorutinas == null)
			autorutinas = new List<Autorutina>();
		
		autorutinas.Add(autorutina);
	}

	public void AddLugarSig(LugarSiguiente lugarSig)
	{
		if(lugarSiguientes == null)
			lugarSiguientes = new List<LugarSiguiente>();
		
		lugarSiguientes.Add(lugarSig);
	}

	public List<LugarSiguiente> DevuelveLugarSiguientes()
	{
		return lugarSiguientes;
	}

	public List<Autorutina> DevuelveAutorutina()
	{
		return autorutinas;
	}
}
