using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Contenedor{

	private List<Autorutina> autorutinas;
	private List<Lugar_Siguiente> lugarSiguientes;

	public Contenedor()
	{
		
	}

	public void addAutorutina(Autorutina auto)
	{
		if(autorutinas == null)
			autorutinas = new List<Autorutina>();
		
		autorutinas.Add(auto);
	}

	public void addLugarSig(Lugar_Siguiente lSig)
	{
		if(lugarSiguientes == null)
			lugarSiguientes = new List<Lugar_Siguiente>();
		
		lugarSiguientes.Add(lSig);
	}

	public List<Lugar_Siguiente> devuelveLugarSiguientes()
	{
		return lugarSiguientes;
	}

	public List<Autorutina> devuelveAutorutina()
	{
		return autorutinas;
	}
}
