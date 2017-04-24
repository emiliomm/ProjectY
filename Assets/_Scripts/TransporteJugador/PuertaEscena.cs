using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaEscena : MonoBehaviour {

	public ManejoEscena man;
	public ManejoEscena man2;

	public bool tr;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			if(tr)
			{
				man.gameObject.GetComponent<BoxCollider>().enabled = false;
				man2.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				man.gameObject.GetComponent<BoxCollider>().enabled = true;
				man2.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}
}
