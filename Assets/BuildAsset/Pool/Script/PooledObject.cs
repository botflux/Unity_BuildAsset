using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour 
{
	public int PrefabID
	{
		get
		{
			return this.prefabID;
		}
	}

	private int prefabID;

	public void Initialize (int prefabID)
	{
		this.prefabID = prefabID;
	}
}
