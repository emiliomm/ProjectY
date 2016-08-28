using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Xml.Serialization;

[XmlInclude(typeof(ObjetoTransportePlayer))]
public class ObjetoTransporteInter
{
	public int ID;
	public List<int> escenas;

	public ObjetoTransporteInter()
	{
		escenas = new List<int>();
	}

	//Devuelve el mensaje de un xml indicado en la ruta
	public static ObjetoTransporteInter LoadObjetoTransporteInter(string path)
	{
		ObjetoTransporteInter objTransInter = Manager.Instance.DeserializeData<ObjetoTransporteInter>(path);

		return objTransInter;
	}
}
