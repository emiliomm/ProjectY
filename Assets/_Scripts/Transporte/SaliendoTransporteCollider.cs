using UnityEngine;
using System.Collections;

public class SaliendoTransporteCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del interactuable el cual debe chocar con el transporteCollider
	private GameObject transporte;

	public void setIDInteractuable(int ID)
	{
		IDInteractuable = ID;
	}

	public void setTransporte(GameObject trans)
	{
		transporte = trans;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Interactuable" )
		{
			InteractuableNPC inter = other.GetComponentInParent<InteractuableNPC>();

			if(inter != null)
			{
				if(inter.ID == IDInteractuable)
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
