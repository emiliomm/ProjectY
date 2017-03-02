using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPopUpObjeto : MonoBehaviour {

	//Muestra un popup de los objetos obtenidos
	public static IEnumerator MostrarPopupObjetos()
	{
		GameObject panelObjeto = (GameObject)Instantiate(Resources.Load("UI/UIPopupObjeto"));
		panelObjeto.transform.SetParent(Manager.instance.canvasGlobal.transform, false);

		//Recorremos los objetos obtenidos recientemente
		for(int i = 0; i < Manager.instance.DevuelveNumeroObjetosRecientes(); i++)
		{
			panelObjeto.transform.GetChild(0).GetChild(0).transform.GetComponent<Text>().text = "Has obtenido " + Manager.instance.DevuelveNombreObjetoReciente(i);

			var opcion = -4;

			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
			panelObjeto.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
				{ opcion = -3; }); //Listener del botón

			while (opcion == -4) {
				yield return null;
			}
		}

		Manager.instance.VaciarObjetosRecientes();
		Destroy(panelObjeto);
	}
}
