using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventoDialogo{

	public int IDEvento;
	public List<VariableEventoDialogo> variables;

	public EventoDialogo()
	{
		variables = new List<VariableEventoDialogo>();
	}

	public bool estaActivo()
	{
		bool activo = true;

		Evento ev = ManagerRutinas.Instance.devuelveEvento(IDEvento);

		if(ev != null)
		{
			for(int i = 0; i < variables.Count; i++)
			{
				Debug.Log(variables[i].num_variable);
				Debug.Log(ev.variables[0]);
				if(ev.variables[variables[i].num_variable] < variables[i].valor)
					activo = false;
			}
		}
		else
			activo = false;

		return activo;
	}
}
