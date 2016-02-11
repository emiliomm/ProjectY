using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

using DialogueTree;

public class TextBox : MonoBehaviour {

	public static TextBox Instance; //Instancia propia de la clase

	public GameObject DialogueWindowPrefab; //prefab que será la ventana de dialogo
	public string DialogueDataFilePath; //ruta del fichero xml
	public int SizeOfLine; //Tamaño de la linea de texto antes de un salto de línea
	public bool isActive; //indica si la caja de texto está activa o no

	private Dialogue dia;
	private NPC npc;

	private GameObject dialogue_window;
	private GameObject dialog_text;
	private GameObject dialog_options;
	private GameObject exit;

	private GameObject option_1;
	private GameObject option_2;
	private GameObject option_3;
	private GameObject option_4;
	private GameObject option_5;
	private GameObject option_6;
	private GameObject option_7;
	private GameObject option_8;
	private GameObject option_9;
	private GameObject option_10;
	private GameObject option_11;
	private GameObject option_12;
	private GameObject option_13;
	private GameObject option_14;
	private GameObject option_15;

	private int selected_option = -2;

	// Use this when the object is created
	void Awake ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		var canvas = GameObject.Find("Canvas");

		dialogue_window = Instantiate<GameObject>(DialogueWindowPrefab);

		dialogue_window.transform.parent = canvas.transform;

		RectTransform dia_window_transform = (RectTransform) dialogue_window.transform;

		dia_window_transform.localPosition = new Vector3(0, 0, 0);

		dialog_text = GameObject.Find("Text_DiaNodeText");

		dialog_options = GameObject.Find("ScrollView");

		option_1 = GameObject.Find("Button_Option1");
		option_2 = GameObject.Find("Button_Option2");
		option_3 = GameObject.Find("Button_Option3");
		option_4 = GameObject.Find("Button_Option4");
		option_5 = GameObject.Find("Button_Option5");
		option_6 = GameObject.Find("Button_Option6");
		option_7 = GameObject.Find("Button_Option7");
		option_8 = GameObject.Find("Button_Option8");
		option_9 = GameObject.Find("Button_Option9");
		option_10 = GameObject.Find("Button_Option10");
		option_11 = GameObject.Find("Button_Option11");
		option_12 = GameObject.Find("Button_Option12");
		option_13 = GameObject.Find("Button_Option13");
		option_14 = GameObject.Find("Button_Option14");
		option_15 = GameObject.Find("Button_Option15");

		exit = GameObject.Find("Button_End");

		exit.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(-1);});

		DisableTextBox();
	}

	public void reloadDialogue(NPC npc_dialogo, string path)
	{
		npc = npc_dialogo;
		DialogueDataFilePath = path;
		dia = Dialogue.LoadDialogue("Assets/" + DialogueDataFilePath);
		EnableTextBox();
	}

	public void EnableTextBox()
	{
		TP_Controller.Instance.canMove = false;
		isActive = true;

		RunDialogue();
	}

	public void DisableTextBox()
	{
		dialogue_window.SetActive(false);
		TP_Controller.Instance.canMove = true;
		isActive = false;
	}

	public void RunDialogue()
	{
		StartCoroutine(run());
	}

	public void SetSelectedOption(int x)
	{
		selected_option = x;
	}

	public IEnumerator run()
	{
		dialogue_window.SetActive(true);

		// create a indexer, set it to 0 - the dialogues Start node.
		int node_id = 0; //principio del dialogo

		//while the next node is not an exit node, traverse the dialogue tree based on the user
		//input
		while(node_id != -1)
		{
			display_node_text(dia.Nodes[node_id]);
			selected_option = node_id;
			while(selected_option == node_id)
			{
				yield return new WaitForSeconds(0.25f);
			}

			if (dia.Nodes[node_id].Options.Count > 0)
			{
				display_node_answers(dia.Nodes[node_id]);
				selected_option = -2;
				while(selected_option == -2)
				{
					yield return new WaitForSeconds(0.25f);
				}
			}

			node_id = selected_option;
		}
		DisableTextBox();
		CheckIfDialogueEnds();
	}

	private void CheckIfDialogueEnds()
	{
		if (npc.indice + 1 < npc.dialogos.Count)
		{
			npc.indice++;
			reloadDialogue (npc, npc.dialogos[npc.indice]);
		}
	}

	//Muestra el nodo de texto del diálogo
	private void display_node_text(DialogueNode node)
	{
		dialog_options.SetActive(false);
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		exit.SetActive(false);

		dialog_text.SetActive(true);
		dialog_text.GetComponentInChildren<Text>().text = node.Text;
		dialog_text.GetComponent<Button>().onClick.RemoveAllListeners();
		dialog_text.GetComponent<Button>().onClick.AddListener(delegate {SetSelectedOption(selected_option+1);}); //Listener del botón
	}

	//Muestra el nodo de respuestas del dialogo
	private void display_node_answers(DialogueNode node)
	{
		dialog_options.SetActive(true);
		dialog_text.SetActive(false);
		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);
		option_4.SetActive(false);
		option_5.SetActive(false);
		option_6.SetActive(false);
		option_7.SetActive(false);
		option_8.SetActive(false);
		option_9.SetActive(false);
		option_10.SetActive(false);
		option_11.SetActive(false);
		option_12.SetActive(false);
		option_13.SetActive(false);
		option_14.SetActive(false);
		option_15.SetActive(false);
		exit.SetActive(false);

		for(int i = 0; i < node.Options.Count && i < 14; i++)
		{
			switch(i)
			{
			case 0:
				set_option_button(option_1, node.Options[i]);
				break;
			case 1:
				set_option_button(option_2, node.Options[i]);
				break;
			case 2:
				set_option_button(option_3, node.Options[i]);
				break;
			case 3:
				set_option_button(option_4, node.Options[i]);
				break;
			case 4:
				set_option_button(option_5, node.Options[i]);
				break;
			case 5:
				set_option_button(option_6, node.Options[i]);
				break;
			case 6:
				set_option_button(option_7, node.Options[i]);
				break;
			case 7:
				set_option_button(option_8, node.Options[i]);
				break;
			case 8:
				set_option_button(option_9, node.Options[i]);
				break;
			case 9:
				set_option_button(option_10, node.Options[i]);
				break;
			case 10:
				set_option_button(option_11, node.Options[i]);
				break;
			case 11:
				set_option_button(option_12, node.Options[i]);
				break;
			case 12:
				set_option_button(option_13, node.Options[i]);
				break;
			case 13:
				set_option_button(option_14, node.Options[i]);
				break;
			case 14:
				set_option_button(option_15, node.Options[i]);
				break;
			}
		}
	}

	private void set_option_button(GameObject button, DialogueOption opt)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = ResolveTextSize(opt.Text, SizeOfLine); //Texto del botón dividido en lineas
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(opt.DestinationNodeID); }); //Listener del botón
	}

	// Divide el texto por tamaño de línea
	private string ResolveTextSize(string input, int lineLength){

		// Split string by char " "         
		string[] words = input.Split(" "[0]);

		// Prepare result
		string result = "";

		// Temp line string
		string line = "";

		// for each all words        
		foreach(string s in words){
			// Append current word into line
			string temp = line + " " + s;

			// If line length is bigger than lineLength
			if(temp.Length > lineLength){

				// Append current line into result
				result += line + "\n";
				// Remain word append into new line
				line = s;
			}
			// Append current word into current line
			else {
				line = temp;
			}
		}

		// Append last line into result        
		result += line;

		// Remove first " " char
		return result.Substring(1,result.Length-1);
	}
}
