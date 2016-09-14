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
	public void Constructor(int IDTransporte, List<int> escenas, int IDEscena)
	{
		this.ID = IDTransporte;
		this.escenas = escenas;
		this.IDEscena = IDEscena;

		base.CargarTransporte();
	}

	protected override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
	}

	protected override void OnLevelWasLoaded(int level)
	{
		base.OnLevelWasLoaded(level);
	}
}
