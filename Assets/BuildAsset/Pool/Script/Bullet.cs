using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject, ICleanup 
{
	public float speed;

	void Start ()
	{
		speed = Random.Range(2.0f,20.0f);
		transform.rotation = Random.rotation;
	}

	void FixedUpdate ()
	{
		transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
	}

	public void Cleanup ()
	{
		speed = 0.0f;
		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
	}
}
