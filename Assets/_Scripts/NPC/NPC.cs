using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour {

	//Los dialogos deben estar ordenados por prioridad

	public List<string> dialogos;
	public int indice;

	// Use this for initialization
	void Awake () {
		List<string> dialogos = new List<string>();
	}
}
