using UnityEngine;
using System.Collections;

public class DialogoCola{

	private NPC_Dialogo dialogo;
	private int IDEvento;

	public DialogoCola(NPC_Dialogo diag, int IDEv)
	{
		dialogo = diag;
		IDEvento = IDEv;
	}

	public NPC_Dialogo devuelveDialogo()
	{
		return dialogo;
	}

	public int devuelveIDEvento()
	{
		return IDEvento;
	}
}
