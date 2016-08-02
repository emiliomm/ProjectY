using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjetoController : MonoBehaviour {

	private GameObject objeto;
	private GameObject panelObjetoPrefab;
	private GameObject botonSalir;

	public float X_MouseSensitivity = 3f;
	public float Y_MouseSensitivity = 3f;

	public void AsignarObjeto(GameObject gObj)
	{
		objeto = gObj;
	}

	void Start ()
	{
		panelObjetoPrefab = (GameObject)Instantiate(Resources.Load("PanelObjetoPrefab"));

		panelObjetoPrefab.transform.SetParent(Manager.Instance.canvasGlobal.transform, false);

		botonSalir = panelObjetoPrefab.transform.GetChild(0).gameObject;

		botonSalir.GetComponent<Button>().onClick.AddListener(delegate { Salir();});
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			float rotX = Input.GetAxis("Mouse X")*X_MouseSensitivity*Mathf.Deg2Rad;
			float rotY = Input.GetAxis("Mouse Y")*Y_MouseSensitivity*Mathf.Deg2Rad;

			objeto.transform.RotateAround(Camera.main.transform.up, -rotX);
			objeto.transform.RotateAround(Camera.main.transform.right, rotY);
		}
	}

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
