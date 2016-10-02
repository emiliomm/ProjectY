using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogoEvento{

	public int IDEvento;
	public bool activo;
	public List<VariableEventoDialogo> variables;

	public DialogoEvento()
	{
		variables = new List<VariableEventoDialogo>();
	}

	public bool SeCumplenCondiciones()
	{
		bool seCumplenCondiciones = true;

		Evento evento = ManagerRutina.instance.DevuelveEvento(IDEvento);

		if(evento != null)
		{
			if(activo)
			{
				for(int i = 0; i < variables.Count; i++)
				{
					if(evento.variables[variables[i].numVariable] < variables[i].valor)
						seCumplenCondiciones = false;
				}
			}
			else
				seCumplenCondiciones = false;
		}
		else
		{
			seCumplenCondiciones = !activo;
		}

		return seCumplenCondiciones;
	}
}
