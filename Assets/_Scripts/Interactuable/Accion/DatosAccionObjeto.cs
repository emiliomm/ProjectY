using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class DatosAccionObjeto : DatosAccion{

	public string nombreObjecto;

	int layerMask = 5; //UI

	public DatosAccionObjeto()
	{
		
	}

	public override void EjecutarAccion()
	{
		Manager.Instance.setPausa(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		var obj = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Objetos/" + nombreObjecto));
		obj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
		obj.layer = layerMask;

		ObjetoController objCon = obj.AddComponent<ObjetoController>();
		objCon.AsignarObjeto(obj);

		Camera.main.GetComponent<TP_Camera>().setObjectMode();
	}
}
