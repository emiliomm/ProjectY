using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class InventarioController : MonoBehaviour {

	private Inventario inventario;

	private GameObject listaObjetos;
	private GameObject imagenObjeto;
	private GameObject nombreObjeto;
	private GameObject cantidadObjeto;
	private GameObject descripcionObjeto;
	private GameObject salir;

	//Número del objeto en la lista que tiene el foco (teclado/ratón)
	private int indiceObjetoActual = 0;

	void Awake()
	{
		TPController.instance.SetState(TPController.State.Dialogo);
		ManagerTiempo.instance.SetPausa(true);
		Manager.instance.StopNavMeshAgents();
		Cursor.visible = true; //Muestra el cursor del ratón
		TPCamera.instance.SetObjectMode();
	}

	void Start ()
	{
		
		inventario = new Inventario();

		if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
		{
			inventario = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
		}

		listaObjetos = transform.GetChild(0).GetChild(0).gameObject;
		imagenObjeto = transform.GetChild(1).GetChild(0).gameObject;
		nombreObjeto = transform.GetChild(1).GetChild(1).gameObject;
		cantidadObjeto = transform.GetChild(1).GetChild(2).gameObject;
		descripcionObjeto = transform.GetChild(1).GetChild(3).gameObject;

		salir = transform.GetChild(2).gameObject;
		salir.GetComponent<Button>().onClick.AddListener(delegate { Salir(); }); //Listener del botón

		if(inventario.DevolverNumeroObjetos() == 0)
		{
			Destroy(listaObjetos);
			Destroy(imagenObjeto);
			Destroy(nombreObjeto);
			descripcionObjeto.transform.GetComponent<Text>().text = "No dispones de ningún objeto";
		}
		else
		{
			CargarListaInventario();
			ActualizarVistaObjeto();

			//ponemos el foco en el primer objeto
			GameObject myEventSystem = Manager.instance.eventSystem;
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(listaObjetos.transform.GetChild(0).gameObject);
		}
	}

	private void CargarListaInventario()
	{
		for(int i = 0; i < inventario.DevolverNumeroObjetos(); i++)
		{
			GameObject botonGO = (GameObject)Instantiate(Resources.Load("BotonInventario"));
			botonGO.transform.GetChild(0).GetComponent<Text>().text = inventario.DevolverNombre(i);

			int j = i; //copia del entero para que funcione en el delegate anónimo
			botonGO.GetComponent<Button>().onClick.AddListener(delegate { SetIndice(j); ActualizarVistaObjeto(); }); //Listener del botón
			botonGO.transform.SetParent(listaObjetos.transform, false);
		}
	}

	private void SetIndice(int num)
	{
		indiceObjetoActual = num;
	}
	
	//Miramos el input
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if(inventario.DevolverNumeroObjetos() > 0)
			{
				if(indiceObjetoActual == inventario.DevolverNumeroObjetos() -1)
					indiceObjetoActual = 0;
				else
					indiceObjetoActual++;

				ActualizarVistaObjeto();
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if(inventario.DevolverNumeroObjetos() > 0)
			{
				if(indiceObjetoActual == 0)
					indiceObjetoActual = inventario.DevolverNumeroObjetos() -1;
				else
					indiceObjetoActual--;

				ActualizarVistaObjeto();
			}
		}
	}

	public void Salir()
	{
		TPController.instance.SetState(TPController.State.Normal);
		ManagerTiempo.instance.SetPausa(false);
		Manager.instance.ResumeNavMeshAgents();
		Cursor.visible = false;
		TPCamera.instance.SetNormalMode();

		Destroy(gameObject);
	}

	private void ActualizarVistaObjeto()
	{
		//indice
		GameObject myEventSystem = Manager.instance.eventSystem;
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(listaObjetos.transform.GetChild(indiceObjetoActual).gameObject);

		//Mantiene el scroll en la posición adecuada
		transform.GetChild(0).gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1f-indiceObjetoActual/(float)inventario.DevolverNumeroObjetos());

//		imagenObjeto;
		nombreObjeto.GetComponent<Text>().text = inventario.DevolverNombre(indiceObjetoActual);
		cantidadObjeto.GetComponent<Text>().text = "X " + inventario.DevolverCantidad(indiceObjetoActual).ToString();
		descripcionObjeto.GetComponent<Text>().text = inventario.DevolverDescripcion(indiceObjetoActual);
	}
}