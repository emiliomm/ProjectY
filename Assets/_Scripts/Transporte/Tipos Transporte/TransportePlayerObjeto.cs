using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class TransportePlayerObjeto : TransportePlayer {

	//vacio
	protected override void Start()
	{

	}

	//Método de iniciación
	public void Constructor(int ID, List<int> escenas, int IDEscena)
	{
		this.ID = ID;
		this.escenas = escenas;
		this.IDEscena = IDEscena;

		base.cargarTransporte();
	}

	protected override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
	}

	protected override void OnLevelWasLoaded(int level)
	{
		base.OnLevelWasLoaded(level);
	}
}
