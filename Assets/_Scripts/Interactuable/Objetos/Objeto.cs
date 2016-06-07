using UnityEngine;
using System.Collections;

public class Objeto : MonoBehaviour {

	private int ID;
	public string nombre;

	// Use this for initialization
	void Start () {
		StartCoroutine(Cargar());
	}

	//Carga las acciones y el nombre
	public IEnumerator Cargar()
	{
		yield return new WaitForSeconds (0.25f);

		Interactuable inter = transform.parent.gameObject.GetComponent<Interactuable>();

		ID = inter.ID;

		inter.SetNombre(nombre);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
