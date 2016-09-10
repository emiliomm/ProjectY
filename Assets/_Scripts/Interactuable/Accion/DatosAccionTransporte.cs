using UnityEngine;
using System.Collections;

public class DatosAccionTransporte : DatosAccion
{
	public int IDEscena;
	public int IDTransporte;
	public float coordX, coordY, coordZ;

	public DatosAccionTransporte()
	{
		
	}

	public override void EjecutarAccion()
	{
		GameObject transporteController = new GameObject("TransporteController");
		TransporteController tCon = transporteController.AddComponent<TransporteController>();
		tCon.Constructor(IDEscena, IDTransporte, new Vector3(coordX, coordY, coordZ));
	}
}
