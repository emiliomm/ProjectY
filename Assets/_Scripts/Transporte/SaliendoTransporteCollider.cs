using UnityEngine;
using System.Collections;

public class SaliendoTransporteCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del interactuable el cual debe chocar con el transporteCollider
	private GameObject transporte;

	public void SetIDInteractuable(int IDInteractuable)
	{
		this.IDInteractuable = IDInteractuable;
	}

	public void SetTransporte(GameObject transporte)
	{
		this.transporte = transporte;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Interactuable" )
		{
			InteractuableNPC interactuableNPC = other.GetComponentInParent<InteractuableNPC>();

			if(interactuableNPC != null)
			{
				if(interactuableNPC.ID == IDInteractuable)
				{
					if(transporte != null)
					{
						transporte.transform.parent.GetComponent<InteractuableObjeto>().SetNavObstacle(true);
					}
					Destroy(gameObject);
				}
			}
		}
	}
}
