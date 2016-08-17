using UnityEngine;
using System.Collections.Generic;

public class TransporteCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del inetractuable el cual debe chocar con el transporteCollider

	public void setIDInteractuable(int ID)
	{
		IDInteractuable = ID;
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
					Manager.Instance.deleteNavhMeshAgent(inter.devuelveNavhMeshAgent());
					Destroy(other.transform.parent.gameObject);
					Destroy(gameObject);
				}
			}
		}
	}
}
