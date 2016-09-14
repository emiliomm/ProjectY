using UnityEngine;
using System.Collections.Generic;

public class TransporteCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del inetractuable el cual debe chocar con el transporteCollider
	private GameObject transporte;

	public void setIDInteractuable(int ID)
	{
		IDInteractuable = ID;
	}

	public void setTransporte(GameObject trans)
	{
		transporte = trans;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Interactuable" )
		{
			InteractuableNPC inter = other.GetComponentInParent<InteractuableNPC>();

			if(inter != null)
			{
				if(inter.ID == IDInteractuable)
				{
					Manager.Instance.deleteNavhMeshAgent(inter.DevuelveNavhMeshAgent());
					Destroy(other.transform.parent.gameObject);
					Destroy(gameObject);

					if(transporte != null)
					{
						transporte.transform.parent.GetComponent<InteractuableObjeto>().SetNavObstacle(true);
					}
				}
			}
		}
	}
}
