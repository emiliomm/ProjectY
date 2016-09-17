using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITiempo : MonoBehaviour {

	private Text hora;
	private Text minuto;

	private void Awake ()
	{
		hora = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
		minuto = gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
	}

	public void EstablecerHora(int hora, int minuto)
	{
		if(hora < 10)
			this.hora.text = "0" + hora.ToString();
		else
			this.hora.text = hora.ToString();

		if(minuto < 10)
			this.minuto.text = "0" + minuto.ToString();
		else
			this.minuto.text = minuto.ToString();
	}
}
