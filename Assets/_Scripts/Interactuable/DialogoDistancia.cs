using UnityEngine;
using System.Collections;

public class DialogoDistancia : MonoBehaviour {

	Interactuable inter;
	DatosAccionDialogo datosAccionDialogo;

	// Use this for initialization
	void Start () {
	
	}

	public void cargarDialogo(Interactuable inter, DatosAccionDialogo dAcc)
	{
		datosAccionDialogo = dAcc;
		this.inter = inter;
	}

	void OnTriggerEnter(Collider other)
	{
		//Sirve para los dialogos automaticos
		//Como implementar el dialogo automatico ¿?
		if (other.tag == "Player")
		{
			if (TP_Controller.Instance.CurrentState == TP_Controller.State.Normal) 
			{
				IniciaDialogo();
			}
		}
	}

	//Inicia el dialogo
	public void IniciaDialogo()
	{
		TextBox.Instance.StartDialogue(inter, datosAccionDialogo.diag);
	}
}
