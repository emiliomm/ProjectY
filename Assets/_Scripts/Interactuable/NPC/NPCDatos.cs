using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDatos{

	private List<string> nombres;
	private int nombreActual;

	private NPC_Dialogo npc_diag; //NPC del cual carga el dialogo

	public NPC_Dialogo DevuelveDialogo()
	{
		return npc_diag;
	}

	public string DevuelveNombreActual()
	{
		return nombres[nombreActual];
	}
}
