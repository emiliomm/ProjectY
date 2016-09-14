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
	private float XMouseSensitivity = 3f;
	private float YMouseSensitivity = 3f;

	//Inicializa algunas variables
	void Start ()
	{
		panelObjetoPrefab = (GameObject)Instantiate(Resources.Load("PanelObjetoPrefab"));
		panelObjetoPrefab.transform.SetParent(Manager.instance.canvasGlobal.transform, false);

		botonSalir = panelObjetoPrefab.transform.GetChild(0).gameObject;
		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir();});
	}

	//Asigna el objeto que se mostrará en pantalla
	public void AsignarObjeto(GameObject gameobject)
	{
		objeto = gameobject;
	}

	void Update()
	{
		//Comprueba si se está pulsando click izquierdo del ratón
		if (Input.GetMouseButton(0))
		{
			float rotacionX = Input.GetAxis("Mouse X")*XMouseSensitivity*Mathf.Deg2Rad;
			float rotacionY = Input.GetAxis("Mouse Y")*YMouseSensitivity*Mathf.Deg2Rad;

			objeto.transform.RotateAround(Camera.main.transform.up, -rotacionX);
			objeto.transform.RotateAround(Camera.main.transform.right, rotacionY);
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
		Manager.instance.SetPausa(false);
		Manager.instance.ResumeNavMeshAgents();
	}
}
