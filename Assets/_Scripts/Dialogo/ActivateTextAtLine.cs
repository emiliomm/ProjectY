using UnityEngine;
using System.Collections;

public class ActivateTextAtLine : MonoBehaviour {

	public TextAsset theText; //archivo de texto

	public int startLine;
	public int endLine;

	public TextBoxManager theTextBox;

	public bool requiredButtonPress; //Indica si es necesario pulsar un boton para activar el texto
	private bool waitForPress;

	public bool destroyWhenActivated; //Indica si queremos que el objeto qua activa el script se destruya al ser activado

	// Use this for initialization
	void Start () {
		theTextBox = FindObjectOfType<TextBoxManager>();
	}
	
	// Update is called once per frame
	void Update () {
		//Si está esperando a pulsar la tecla y pulsamos J,
		if(waitForPress && Input.GetKeyDown(KeyCode.J))
		{
			theTextBox.ReloadScript(theText);
			theTextBox.currentLine = startLine;

			if(endLine == 0)
			{
				endLine = theTextBox.textLines.Length - 1;
			}

			theTextBox.endAtLine = endLine;
			theTextBox.EnableTextBox();

			if(destroyWhenActivated)
			{
				Destroy(gameObject);
			}
		}
	}

	//Si colisionamos con el jugador, cargamos el nuevo texto
	void OnTriggerEnter(Collider other)
	{

		if(other.tag == "Player")
		{
			//Si se necesita pulsar el boton,activamos la variable waitfropress
			if(requiredButtonPress)
			{
				waitForPress = true;
				return;
			}

			theTextBox.ReloadScript(theText);
			theTextBox.currentLine = startLine;

			if(endLine == 0)
			{
				endLine = theTextBox.textLines.Length - 1;
			}

			theTextBox.endAtLine = endLine;
			theTextBox.EnableTextBox();

			if(destroyWhenActivated)
			{
				Destroy(gameObject);
			}
		}
	}

	//Al salir de la colision, desactivactivamos la variable waitforpress
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			waitForPress = false;
		}
	}

}
