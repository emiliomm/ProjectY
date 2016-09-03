using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogoEvento{

	public int IDEvento;
	public List<VariableEventoDialogo> variables;

	public DialogoEvento()
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
				if(ev.variables[variables[i].num_variable] < variables[i].valor)
					activo = false;
			}
		}
		else
			activo = false;

		return activo;
	}
}
