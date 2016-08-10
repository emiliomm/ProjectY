using UnityEngine;
using UnityEngine.UI;

/*
 * 	Clase que controla la interfaz de al manipular un objeto
 */
public class ObjetoController : MonoBehaviour {

	private GameObject objeto; //Objeto que se muestra en pantalla

	//Objetos de la interfaz
	private GameObject panelObjetoPrefab;
	private GameObject botonSalir;

	//Guardar en el Manager
	//Parámetros que determinan la sensibilidad de movimiento del ratón al mover un objeto
	private float X_MouseSensitivity = 3f;
	private float Y_MouseSensitivity = 3f;

	//Inicializa algunas variables
	void Start ()
	{
		panelObjetoPrefab = (GameObject)Instantiate(Resources.Load("PanelObjetoPrefab"));
		panelObjetoPrefab.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

		botonSalir = panelObjetoPrefab.transform.GetChild(0).gameObject;
		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir();});
	}

	//Asigna el objeto que se mostrará en pantalla
	public void AsignarObjeto(GameObject gObj)
	{
		objeto = gObj;
	}

	void Update()
	{
		//Comprueba si se está pulsando click izquierdo del ratón
		if (Input.GetMouseButton(0))
		{
			float rotX = Input.GetAxis("Mouse X")*X_MouseSensitivity*Mathf.Deg2Rad;
			float rotY = Input.GetAxis("Mouse Y")*Y_MouseSensitivity*Mathf.Deg2Rad;

			objeto.transform.RotateAround(Camera.main.transform.up, -rotX);
			objeto.transform.RotateAround(Camera.main.transform.right, rotY);
		}
	}

	//Sale de la pantalla del objeto
	private void Salir()
	{
		Destroy(objeto);
		Destroy(panelObjetoPrefab);
		Destroy(this);

		Camera.main.GetComponent<TP_Camera>().setNormalMode();
		TP_Controller.Instance.SetState(TP_Controller.State.Normal);
		Manager.Instance.setPausa(false);
	}
}
