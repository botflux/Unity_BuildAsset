using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool <T> where T : PooledObject, ICleanup
{
	// les objects en attentes
	private List<T> pool = new List<T>();
	// contient les reference au prefab avec leur id de prefab 
	private List<KeyValuePair<int, T>> knowsObjects = new List<KeyValuePair<int, T>> ();

	private int greaterCount = 0;

	public int PoolCount
	{
		get
		{
			return pool.Count;
		}
	}

	public int KnowsPrefabCount
	{
		get
		{
			return knowsObjects.Count;
		}
	}

	public int GreaterCount
	{
		get
		{
			return this.greaterCount;
		}
	}

	public bool IsInPool (T prefab)
	{
		if (prefab.PrefabID == 0)
		{
			return false;
		}

		if (knowsObjects.Find(x => x.Key == prefab.PrefabID).Value == null)
		{
			return false;
		}

		return true;
	}

	public void AddPrefabReference (T prefab)
	{
		prefab.Initialize (prefab.gameObject.GetInstanceID());
		knowsObjects.Add(new KeyValuePair<int, T>(prefab.PrefabID, prefab));
	}

	public void AddPrefabReference (T prefab, out int prefabID)
	{
		prefab.Initialize (prefab.gameObject.GetInstanceID());
		knowsObjects.Add(new KeyValuePair<int, T>(prefab.PrefabID, prefab));

		prefabID = prefab.PrefabID;
	}

	public int GetIDFromPrefab (T prefab)
	{
		KeyValuePair<int, T> kv = knowsObjects.Find(x => x.Value == prefab);

		if (kv.Value == null)
		{
			throw new UnassignedReferenceException("ID of an unknow prefab has been requested!");
		}

		return kv.Key;
	}

	public T GetPrefabFromID (int prefabID)
	{
		KeyValuePair<int, T> kv = knowsObjects.Find(x => x.Key == prefabID);

		if (kv.Value == null)
		{
			throw new UnassignedReferenceException("Prefab of an unknow ID has been requested!");
		}

		return kv.Value;
	}

	public T GetObject (int prefabID)
	{
		if (knowsObjects.Find(x => x.Key == prefabID).Value == null)
		{
			throw new UnassignedReferenceException("Instance of an unknow Prefab ID has been requested!");
		}

		T instance = pool.Find(x => x.PrefabID == prefabID);

		if (instance != null)
		{
			pool.Remove(instance);
			instance.gameObject.SetActive(true);
			return instance;
		}
		else
		{
			T ins = MonoBehaviour.Instantiate(knowsObjects.Find(x => x.Key == prefabID).Value);
			ins.Initialize(prefabID);
			pool.Add(ins);
			return GetObject(prefabID);
		}
	}

	public T GetObject (T obj)
	{
		if (knowsObjects.Find(x => x.Key == obj.PrefabID).Value == null)
		{
			throw new UnassignedReferenceException("Instance of an unknow Prefab ID has been requested!");
		}

		T instance = pool.Find(x => x.PrefabID == obj.PrefabID);

		if (instance != null)
		{
			pool.Remove(instance);
			instance.gameObject.SetActive(true);
			return instance;
		}
		else
		{
			T ins = MonoBehaviour.Instantiate(knowsObjects.Find(x => x.Key == obj.PrefabID).Value);
			ins.Initialize(obj.PrefabID);
			pool.Add(ins);
			return GetObject(obj.PrefabID);
		}

	}

	public void DisposeObject (T objToDispose)
	{
		if (knowsObjects.Find(x => x.Key == objToDispose.PrefabID).Value == null)
		{
			throw new UnassignedReferenceException("Instance of an unknow Prefab ID has been requested!");
		}

		if (pool.Count > greaterCount)
		{
			greaterCount = pool.Count;
		}

		objToDispose.Cleanup();
		objToDispose.gameObject.SetActive(false);
		pool.Add(objToDispose);
		objToDispose = null;
	}
}

// voir si ca marche avec plusieurs prefab différent
// implémenter SmartPool