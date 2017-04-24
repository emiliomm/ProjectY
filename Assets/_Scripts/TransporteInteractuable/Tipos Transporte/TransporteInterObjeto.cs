using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class TransporteInterObjeto : TransporteInter {

	//vacio
	protected override void Start()
	{
		
	}

	//Método de iniciación
	public void Constructor(int IDTransporte, List<int> escenas)
	{
		this.ID = IDTransporte;
		this.escenas = escenas;

		base.CargarTransporte();
	}
}
