using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour{

	//Los dialogos deben estar ordenados por prioridad

	public List<string> dialogos = new List<string>();
	public List<Pregunta> preguntas = new List<Pregunta>();

	public int indice = 0;

	// Use this for initialization
	void Start() {

		for(int i = 0; i < 5; i++)
		{
			preguntas.Add(new Pregunta("Opcion " + i.ToString(),"_Texts/text_dia3.xml"));
		}
	}
}
