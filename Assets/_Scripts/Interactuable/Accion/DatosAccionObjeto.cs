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
//		GameObject canvas = new GameObject("Canvas");
//		Canvas c = canvas.AddComponent<Canvas>();
//		c.renderMode = RenderMode.ScreenSpaceCamera;
//		c.worldCamera = Camera.main;
//		c.planeDistance = 3;
//
//		canvas.layer = layerMask;
//
//		RawImage rimg = canvas.AddComponent<RawImage>();
//		rimg.color = new Color32(73, 67, 67, 230);

//		desactivarCanvases();

		Manager.Instance.setPausa(true);
		Cursor.visible = true; //Muestra el cursor del ratón

		var obj = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Objetos/" + nombreObjecto));
		obj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
		obj.layer = layerMask;

		ObjetoController objCon = obj.AddComponent<ObjetoController>();
		objCon.AsignarObjeto(obj);

		Camera.main.GetComponent<TP_Camera>().setObjectMode();

//		obj.transform.SetParent(c.transform, false);
		//		obj.transform.localScale = Vector3.one;
	}

	private void desactivarCanvases()
	{
		foreach (var obj in Manager.Instance.interactuablesCercanos)
		{
			Interactuable inter = obj.GetComponent<Interactuable> ();
			inter.SetState (Interactuable.State.Desactivado);
			inter.OcultaCanvas();
			inter.desactivado = true;
		}
	}
}
