using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour{

	//Los dialogos deben estar ordenados por prioridad

	public List<string> dialogos;
	public List<Pregunta> preguntas = new List<Pregunta>();

	public int indice;

	// Use this for initialization
	void Start() {
		List<string> dialogos = new List<string>();
		//List<Pregunta> preguntas = new List<Pregunta>();

		for(int i = 0; i < 5; i++)
		{
			preguntas.Add(new Pregunta("Opcion " + i.ToString(),"_Texts/text_dia3.xml"));
		}
	}
}
