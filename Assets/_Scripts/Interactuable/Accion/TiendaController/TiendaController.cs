using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TiendaController : MonoBehaviour {

	Inventario inventario;

	//Número de objetos por fila y columna en cada pantalla
//	public int numX;
	public int numY;

	public Font fuente;

	public Image fondo;

	private GameObject contenedorMenus;

	//Empieza en 1
	private int numMenu;

//	public void InicializarTienda(int numObjetosFila, int numObjetosColumna, Font fuente, Inventario inventario, Image fondo)
	public void InicializarTienda(int numObjetosColumna, Inventario inventario)
	{
//		numX = numObjetosFila;
		numY = numObjetosColumna;
		//this.fuente = fuente;
		this.inventario = inventario;
		//this.fondo = fondo;

		gameObject.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);
		contenedorMenus = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

		numMenu = 1;
		crearInterfaz();

	}

	private void crearInterfaz()
	{
		int num_menu = 0;
		GameObject menu = contenedorMenus.transform.GetChild(0).gameObject;

		for(int i = 0; i < inventario.devolverNumeroObjetos(); i++)
		{
			if(i/numY != num_menu)
			{
				num_menu++;
				menu = (GameObject)Instantiate(Resources.Load("Tienda/Menu"));
				menu.transform.SetParent(contenedorMenus.transform, false);
			}

			var button = (GameObject)Instantiate(Resources.Load("Tienda/BotonObjetoTienda"));
			button.transform.SetParent(menu.transform, false);
			button.transform.GetChild(0).GetComponent<Text>().text = inventario.devolverNombre(i);
		}
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}
