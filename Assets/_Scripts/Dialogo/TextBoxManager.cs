using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

	public GameObject textBox;

	public Text theText; //texto mostrado en pantalla

	public TextAsset textFile; //archivo de texto
	public string[] textLines; //string donde se guarda el texto; en [] guarda el numero de línea

	public int currentLine; //Línea actual
	public int endAtLine; //Línea donde acaba el texto

	public TP_Controller player; //Controlador del jugador

	public bool isActive; //la caja de texto está activa o no
	public bool stopPlayerMovement; //indica si el jugador se puede mover o no mientras que está hablando

	private bool isTyping = false; //Se está escribiendo texto en pantalla
	private bool cancelTyping = false;

	public float textSpeed; //velocidad de scroll del texto (cuanto más pequeño el valor, más rápido avanza el texto)

	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<TP_Controller>();

		if (textFile != null)
		{
			//Separamos el texto en varias líneas
			textLines = (textFile.text.Split('\n'));
		}

		//Si el valor de endLine es 0 (el predeterminado) cogemmos el número de líneas del archivo
		if(endAtLine == 0)
		{
			endAtLine = textLines.Length - 1;
		}

		if(isActive)
		{
			EnableTextBox();
		}
		else
		{
			DisableTextBox();
		}
	}

	void Update()
	{
		//Si no está activo,no hacemos nada
		if (!isActive)
		{
			return;
		}

		//el texto que se muestra es 
		//theText.text = textLines[currentLine];

		//al apretar el botón, interactuamos con el texto
		if (Input.GetKeyDown(KeyCode.Return))
		{
			//Si se está escribiendo texto en pantalla (todo el texto se ha mostrado)
			//Pasamos a la siguiente línea
			if(!isTyping)
			{
				currentLine ++;

				//Si estamos al final, no mostramos la caja de texto, la desactivamos
				if (currentLine > endAtLine)
				{
					DisableTextBox();
				}
				//Sino, hacemos scroll al texto
				else
				{
					StartCoroutine(TextScroll(textLines[currentLine]));
				}
			}
			//Mostramos todo el texto
			else if(isTyping && !cancelTyping)
			{
				cancelTyping = true;
			}

		}
	}

	//Corouitine (se ejecutan en su propio timepo, más lento que la función Update)
	private IEnumerator TextScroll (string lineOfText)
	{
		int letter = 0; //En que letra vamos en el string
		theText.text = ""; //Lo que muestra la caja de texto
		isTyping = true; //Estamos escribiendo el texto
		cancelTyping = false;

		//mientras se esté mostrando el texto, no hayamos cancelado el scroll
		//y queden teclas por mostrar
		while(isTyping && !cancelTyping && (letter < lineOfText.Length - 1))
		{
			//avanzamos una letra el texto mostrado
			theText.text += lineOfText[letter];
			letter += 1;
			yield return new WaitForSeconds(textSpeed); //esperamos el tiempo que hemos indicado en la velocidad de texto
		}
		//Mostramos todo el texto (por si cancelamos el scroll o este se acaba)
		theText.text = lineOfText;
		isTyping = false;
		cancelTyping = false;
	}


	public void EnableTextBox()
	{
		textBox.SetActive(true);

		//Si hemos puesto que el jugador no se puede mover al hablar, para
		//el movimiento del jugador
		if(stopPlayerMovement)
		{
			player.canMove = false;
		}

		isActive = true;

		StartCoroutine(TextScroll(textLines[currentLine]));
	}

	public void DisableTextBox()
	{
		textBox.SetActive(false);

		player.canMove = true;

		isActive = false;
	}

	//Le pasamos un archivo de texto
	public void ReloadScript(TextAsset theText)
	{
		//Comprobamos que el texto no está vacío
		if (theText != null)
		{
			textLines = new string[1]; //sustituye el texto
			textLines = (theText.text.Split('\n'));
		}
	}
}
