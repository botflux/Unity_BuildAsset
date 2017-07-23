using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour 
{
	public Bullet bullet;
	public int id;

	private Pool<Bullet> bulletPool = new Pool<Bullet> ();
	private Stack<Bullet> bulletStack = new Stack<Bullet> ();

	private float currentTime = 0f;
	private float timeBtw = 0.1f;

	void Start ()
	{
		bulletPool.AddPrefabReference(bullet, out id);
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			bulletStack.Push(bulletPool.GetObject(id));
			Debug.Log ("Create");
		}

		if (currentTime + timeBtw <= Time.time)
		{
			if (bulletStack.Count > 0)
			{
				bulletPool.DisposeObject(bulletStack.Pop());
				Debug.Log ("Dispose");
			}	
			currentTime = Time.time;
			Debug.Log (bulletPool.GreaterCount);
		}
	}
}
