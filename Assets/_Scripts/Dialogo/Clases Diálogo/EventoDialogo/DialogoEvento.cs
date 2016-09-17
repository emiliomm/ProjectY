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

	public bool EstaActivo()
	{
		bool activo = true;

		Evento evento = ManagerRutina.instance.DevuelveEvento(IDEvento);

		if(evento != null)
		{
			for(int i = 0; i < variables.Count; i++)
			{
				if(evento.variables[variables[i].numVariable] < variables[i].valor)
					activo = false;
			}
		}
		else
		{
			activo = false;
		}

		return activo;
	}
}
