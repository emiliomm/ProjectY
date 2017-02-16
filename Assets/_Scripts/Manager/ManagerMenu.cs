using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMenu : MonoBehaviour {

	private GameObject panelTiempo;
	private GameObject panelInventario;

	private bool estadoPanelInventario;

	private bool estadoPanelTiempo;
	private bool activarPanelTiempo;

	void Start ()
	{
		estadoPanelInventario = false;
		estadoPanelTiempo = false;
		activarPanelTiempo = false;

		panelTiempo = Manager.instance.canvasGlobal.transform.GetChild(1).gameObject;
	}
		
	void Update ()
	{
		MenuInput ();
	}

	//Controla el input de los menus
	private void MenuInput()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			estadoPanelInventario = !estadoPanelInventario;

			if(estadoPanelInventario)
			{
				panelInventario = (GameObject)Instantiate (Resources.Load ("PanelInventarioPrefab"));
				panelInventario.transform.SetParent (Manager.instance.canvasGlobal.transform, false);

				panelTiempo.SetActive(false);
				estadoPanelTiempo = false;
			}
			else
			{
				panelInventario.GetComponent<InventarioController> ().Salir();
			}
		}

		if(TPController.instance.CurrentState == TPController.State.Normal)
			PanelTiempoInput ();
	}

	private void PanelTiempoInput()
	{
		if(estadoPanelTiempo)
		{
			if (!Input.GetKey(KeyCode.H))
			{
				activarPanelTiempo = false;
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.H))
			{
				activarPanelTiempo = true;
			}
		}

		if(activarPanelTiempo != estadoPanelTiempo)
		{
			if(activarPanelTiempo)
			{
				panelTiempo.SetActive(true);
			}
			else
			{
				panelTiempo.SetActive(false);
			}

			estadoPanelTiempo = !estadoPanelTiempo;
		}
	}
}
