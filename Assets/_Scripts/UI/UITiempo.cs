using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITiempo : MonoBehaviour {

	private Text horaText;
	private Text minutoText;

	private void Awake ()
	{
		horaText = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
		minutoText = gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
	}

	void Update()
	{
		MostrarHora ();
	}

	public void MostrarHora()
	{
		if(ManagerTiempo.instance.GetHoraActual() < 10)
			this.horaText.text = "0" + ManagerTiempo.instance.GetHoraActual().ToString();
		else
			this.horaText.text = ManagerTiempo.instance.GetHoraActual().ToString();

		if(ManagerTiempo.instance.GetMinutoActual() < 10)
			this.minutoText.text = "0" + ManagerTiempo.instance.GetMinutoActual().ToString();
		else
			this.minutoText.text = ManagerTiempo.instance.GetMinutoActual().ToString();
	}
}
