using UnityEngine;
using System.Collections.Generic;

public class RutaCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del inetractuable el cual debe chocar con el transporteCollider

	public void SetIDInteractuable(int IDInteractuable)
	{
		this.IDInteractuable = IDInteractuable;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Interactuable" )
		{
			InteractuableNPC interactuableNPC = other.GetComponentInParent<InteractuableNPC>();

			if(interactuableNPC != null)
			{
				if(interactuableNPC.ID == IDInteractuable)
				{
					Manager.instance.DeleteNavMeshAgent(interactuableNPC.DevuelveNavhMeshAgent());
					Destroy(gameObject);
				}
			}
		}
	}
}
