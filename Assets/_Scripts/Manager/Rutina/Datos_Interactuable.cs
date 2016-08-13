using UnityEngine;
using System.Collections;

public class Datos_Interactuable{

	public int IDInter;
	public int tipoInter;
	public int IDRutinaActual;

	public Datos_Interactuable()
	{
		
	}

	public Datos_Interactuable(int IDInter, int tipoInter, int IDRutinaActual)
	{
		this.IDInter = IDInter;
		this.tipoInter = tipoInter;
		this.IDRutinaActual = IDRutinaActual;
	}

	public void Serialize()
	{
		Manager.Instance.SerializeData(this, Manager.rutaDatosInteractuableGuardados, IDInter.ToString()  + ".xml");
	}
}
