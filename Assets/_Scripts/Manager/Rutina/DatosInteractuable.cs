using UnityEngine;
using System.Collections;

public class DatosInteractuable{

	public int IDInteractuable;
	public int tipoInter;
	public int IDRutinaActual;

	public DatosInteractuable()
	{
		
	}

	public DatosInteractuable(int IDInteractuable, int tipoInter, int IDRutinaActual)
	{
		this.IDInteractuable = IDInteractuable;
		this.tipoInter = tipoInter;
		this.IDRutinaActual = IDRutinaActual;
	}

	public void Serialize()
	{
		Manager.instance.SerializeData(this, Manager.rutaDatosInteractuableGuardados, IDInteractuable.ToString()  + ".xml");
	}
}
