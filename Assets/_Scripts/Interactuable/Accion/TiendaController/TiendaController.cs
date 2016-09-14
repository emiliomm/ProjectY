using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TiendaController : MonoBehaviour {

	Inventario inventario;

	public bool escaparate; //Si es verdadero, no se pueden comprar objetos, solo mirarlos

	//Número de objetos por fila y columna en cada pantalla
//	public int numX;
	public int numY;

//	public Font fuente;

//	public Image fondo;

	private GameObject contenedorMenus;
	private GameObject descripcion;

	private GameObject botonSalir;
	private GameObject nombreObjetoSeleccionado;
	private GameObject precioObjetoSeleccionado;
	private GameObject descripcionObjetoSeleccionado;
	private GameObject botonCompra;

	//Número de menú posicionado actualmente
	//Empieza en 1
	private int numMenu;

//	public void InicializarTienda(int numObjetosFila, int numObjetosColumna, Font fuente, Inventario inventario, Image fondo)
	public void InicializarTienda(bool escaparate, int numObjetosColumna, Inventario inventario, bool dialogo)
	{
		this.inventario = inventario;

		this.escaparate = escaparate;

//		numX = numObjetosFila;
		numY = numObjetosColumna;
		//this.fuente = fuente;
		//this.fondo = fondo;

		gameObject.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

		contenedorMenus = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		descripcion = gameObject.transform.GetChild(1).gameObject;
		botonSalir = descripcion.transform.GetChild(0).gameObject;
		nombreObjetoSeleccionado = descripcion.transform.GetChild(1).GetChild(0).gameObject;
		descripcionObjetoSeleccionado = descripcion.transform.GetChild(1).GetChild(2).gameObject;

		botonCompra = descripcion.transform.GetChild(2).gameObject;
		if(escaparate)
			Destroy(botonCompra);

		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir(dialogo); }); //Listener del botón

		numMenu = 1;
		CrearInterfaz();
	}

	private void CrearInterfaz()
	{
		int num_menu = 0;
		GameObject menuGO = contenedorMenus.transform.GetChild(0).gameObject;

		for(int i = 0; i < inventario.DevolverNumeroObjetos(); i++)
		{
			if(i/numY != num_menu)
			{
				num_menu++;
				menuGO = (GameObject)Instantiate(Resources.Load("Tienda/Menu"));
				menuGO.transform.SetParent(contenedorMenus.transform, false);
			}

			var button = (GameObject)Instantiate(Resources.Load("Tienda/BotonObjetoTienda"));
			button.transform.SetParent(menuGO.transform, false);
			button.transform.GetChild(0).GetComponent<Text>().text = inventario.DevolverNombre(i);

			int j = i; //copia del entero para que funcione en el delegate anónimo
			button.GetComponent<Button>().onClick.AddListener(delegate { ActualizarDescripcion(j); }); //Listener del botón
		}
	}

	private void ActualizarDescripcion(int num)
	{
		nombreObjetoSeleccionado.transform.GetComponent<Text>().text = inventario.DevolverNombre(num);
//		precioObjetoSeleccionado;
		descripcionObjetoSeleccionado.transform.GetComponent<Text>().text = inventario.DevolverDescripcion(num);

		if(!escaparate)
		{
			botonCompra.GetComponent<Button>().onClick.RemoveAllListeners();
			botonCompra.GetComponent<Button>().onClick.AddListener(delegate { CompraObjeto(num); }); //Listener del botón
		}
	}

	private void CompraObjeto(int num)
	{
		var compraPopup = (GameObject)Instantiate(Resources.Load("Tienda/CompraPopup"));
		compraPopup.transform.SetParent(gameObject.transform, false);

		var siGO = compraPopup.transform.GetChild(1).gameObject;
		var noGO = compraPopup.transform.GetChild(2).gameObject;

		siGO.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(AddObjeto(num, compraPopup)); }); //Listener del botón
		noGO.GetComponent<Button>().onClick.AddListener(delegate { Destroy(compraPopup); }); //Listener del botón
	}

	private IEnumerator AddObjeto(int num, GameObject popupGO)
	{
		Inventario inventarioPropio;

		//Buscamos el inventario en la colaobjetos
		ColaObjeto inventarioCola = Manager.Instance.GetColaObjetos(Manager.rutaInventario + "Inventario.xml");

		//Se ha encontrado en la cola de objetos
		if(inventarioCola != null)
		{
			ObjetoSerializable objetoSerializable = inventarioCola.GetObjeto();
			inventarioPropio = objetoSerializable as Inventario;
		}
		//No se ha encontrado en la cola de objetos
		else
		{
			//Cargamos el inventario si existe, sino lo creamos
			if(System.IO.File.Exists(Manager.rutaInventario + "Inventario.xml"))
			{
				inventarioPropio = Inventario.LoadInventario(Manager.rutaInventario + "Inventario.xml");
			}
			else
			{
				inventarioPropio = new Inventario();
			}
		}

		inventarioPropio.AddObjeto(inventario.DevolverID(num), 1);
		inventarioPropio.AddToColaObjetos();

		yield return StartCoroutine(TextBox.instance.MostrarPopupObjetos());

		Destroy(popupGO);
	}
		
	private void Salir(bool dialogo)
	{
		if(!dialogo)
		{
			//Actualizamos el inventario si hemos comprado algo
			Manager.Instance.SerializarCola();


			TP_Controller.Instance.SetState(TP_Controller.State.Normal);
			Manager.Instance.setPausa(false);
			Manager.Instance.resumeNavMeshAgents();
		}
		else
		{
			TextBox.instance.MostrarInterfaz();
		}

		Camera.main.GetComponent<TP_Camera>().setNormalMode();

		Destroy(gameObject);
	}
}
