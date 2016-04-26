using UnityEngine;
using System.Collections;

public class AccionDialogo : Accion {

	private int id_npc;

	protected override void Start()
	{
		Debug.Log("Personality: Start");
		base.Start();
	}

	public void ConstructorAccion(string nom, int id)
	{
		nombre = nom;
		id_npc = id;
	}

	public int DevuelveIdNpc()
	{
		return id_npc;
	}

	public void EjecutarAccion()
	{
		GameObject npc = Manager.Instance.GetNPC(id_npc);
		npc.GetComponent<NPC>().IniciaDialogo();
	}

}
