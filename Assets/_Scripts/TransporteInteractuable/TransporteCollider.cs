﻿using UnityEngine;
using System.Collections.Generic;

public class TransporteCollider : MonoBehaviour {

	private int IDInteractuable; //Indica el ID del inetractuable el cual debe chocar con el transporteCollider
	private GameObject transporte;

	public void SetIDInteractuable(int IDInteractuable)
	{
		this.IDInteractuable = IDInteractuable;
	}

	public void SetTransporte(GameObject transporteGO)
	{
		this.transporte = transporteGO;
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
