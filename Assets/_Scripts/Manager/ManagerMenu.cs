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

		panelTiempo = Manager.instance.canvasGlobal.transform.GetChild(0).gameObject;
	}
		
	void Update ()
	{
		MenuInput ();
	}

	//Controla el input de los menus
	private void MenuInput()
	{
		PanelInventarioInput();

		PanelTiempoInput();
	}

	private void PanelInventarioInput()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			if(!estadoPanelInventario && TPController.instance.CurrentState == TPController.State.Normal)
			{
				panelInventario = (GameObject)Instantiate (Resources.Load ("Inventario/InventarioPrefab"));
				panelInventario.transform.SetParent (Manager.instance.canvasGlobal.transform, false);

				estadoPanelInventario = !estadoPanelInventario;
			}
			else if(estadoPanelInventario)
			{
				panelInventario.GetComponent<InventarioController> ().Salir();

				estadoPanelInventario = !estadoPanelInventario;
			}
		}
	}

	private void PanelTiempoInput()
	{
		if(TPController.instance.CurrentState == TPController.State.Normal && !estadoPanelInventario)
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
		}
		else
		{
			activarPanelTiempo = false;
		}

		//Debug.Log(activarPanelTiempo);

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
