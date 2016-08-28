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

	// Use this for initialization
	void Start () {
		
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

		if(inventario.devolverNumeroObjetos() == 0)
		{
			Destroy(listaObjetos);
			Destroy(imagenObjeto);
			Destroy(nombreObjeto);
			descripcionObjeto.transform.GetComponent<Text>().text = "No dispones de ningún objeto";
		}
		else
		{
			cargarListaInventario();
			actualizarVistaObjeto();

			//ponemos el foco en el primer objeto
			GameObject myEventSystem = GameObject.Find("EventSystem(Clone)");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(listaObjetos.transform.GetChild(0).gameObject);
		}
	}

	private void cargarListaInventario()
	{
		for(int i = 0; i < inventario.devolverNumeroObjetos(); i++)
		{
			GameObject boton = (GameObject)Instantiate(Resources.Load("BotonInventario"));
			boton.transform.GetChild(0).GetComponent<Text>().text = inventario.devolverNombre(i);

			int j = i; //copia del entero para que funcione en el delegate anónimo
			boton.GetComponent<Button>().onClick.AddListener(delegate { setIndice(j); actualizarVistaObjeto(); }); //Listener del botón
			boton.transform.SetParent(listaObjetos.transform, false);
		}
	}

	private void setIndice(int num)
	{
		indiceObjetoActual = num;
	}
	
	//Miramos el input
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if(inventario.devolverNumeroObjetos() > 0)
			{
				if(indiceObjetoActual == inventario.devolverNumeroObjetos() -1)
					indiceObjetoActual = 0;
				else
					indiceObjetoActual++;

				actualizarVistaObjeto();
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if(inventario.devolverNumeroObjetos() > 0)
			{
				if(indiceObjetoActual == 0)
					indiceObjetoActual = inventario.devolverNumeroObjetos() -1;
				else
					indiceObjetoActual--;

				actualizarVistaObjeto();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.I))
		{
			Salir();
		}
	}

	private void Salir()
	{
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		Manager.Instance.setPausa(false);
		Manager.Instance.resumeNavMeshAgents();
		Cursor.visible = false;
		Camera.main.GetComponent<TP_Camera>().setNormalMode();

		Destroy(gameObject);
	}

	private void actualizarVistaObjeto()
	{
		//indice
		GameObject myEventSystem = GameObject.Find("EventSystem(Clone)");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(listaObjetos.transform.GetChild(indiceObjetoActual).gameObject);

		//Mantiene el scroll en la posición adecuada
		transform.GetChild(0).gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1f-indiceObjetoActual/(float)inventario.devolverNumeroObjetos());

//		imagenObjeto;
		nombreObjeto.GetComponent<Text>().text = inventario.devolverNombre(indiceObjetoActual);
		cantidadObjeto.GetComponent<Text>().text = "X " + inventario.devolverCantidad(indiceObjetoActual).ToString();
		descripcionObjeto.GetComponent<Text>().text = inventario.devolverDescripcion(indiceObjetoActual);
	}
}