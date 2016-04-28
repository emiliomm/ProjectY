using UnityEngine;
using System.Collections;

public class AccionDialogo : Accion {

	private int id_npc;

	protected override void Start()
	{
		//LLamamos al método de la clase base
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

	//Ejecuta la acción de AccionDialogo
	//Carga el dialogo del npc indicado
	public void EjecutarAccion()
	{
		GameObject npc = Manager.Instance.GetNPC(id_npc);
		npc.GetComponent<NPC>().IniciaDialogo();
	}

}
