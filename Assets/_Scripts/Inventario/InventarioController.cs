using UnityEngine;
using System.Collections;

public class InventarioController : MonoBehaviour {

	Inventario inventario;

	// Use this for initialization
	void Start () {
		
		inventario = new Inventario();

		if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
		{
			inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
		}
	}
	
	//Miramos el input
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			TP_Controller.Instance.SetState(TP_Controller.State.Normal);
			Destroy(gameObject);
		}
	}
}
