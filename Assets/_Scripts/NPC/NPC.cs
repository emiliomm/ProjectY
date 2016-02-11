using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour {

	public List<string> dialogos;
	public int indice;

	// Use this for initialization
	void Awake () {
		List<string> dialogos = new List<string>();
	}
}
