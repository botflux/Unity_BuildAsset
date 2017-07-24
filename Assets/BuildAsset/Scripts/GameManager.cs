using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	public static GameManager instance;

	public Pool<Build> buildPool;

	void Awake ()
	{
		buildPool = new Pool<Build> ();
		instance = this;
	}
}
