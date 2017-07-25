using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour 
{
	// les batiments que le joueur peut créer
	public Build[] builds;
	// l'index du batiment qui va être créer
	public int selectedBuildIndex = 3;
	// masque qui représente le sol
	public LayerMask groundMask;
	// masque qui représente les bâtiments
	public LayerMask buildMask;

	// reference à la camera du jeu
	Camera myCamera;
	// le rayon à une portée globale pour ne pas chargée les allocations
	Ray ray;
	// pareil que pour le ray
	RaycastHit hit;
	// reference au build handler
	BuildHandler buildHandler;

	// stock la position à la quelle le batiment qui va être déplacé doit être déplacé
	Vector3 newBuildPosition = Vector3.zero;

	void Awake ()
	{
		myCamera = GetComponent<Camera> ();
		buildHandler = FindObjectOfType<BuildHandler> ();
	}

	void Update ()
	{
		// créer un bâtiment
		if (Input.GetMouseButtonDown(0))
		{
			ray = myCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
			{
				Debug.Log ("Ray hit point :" + hit.point.x + "," + hit.point.y + "," + hit.point.z);
				buildHandler.CreateBuild (builds[selectedBuildIndex], hit.point);
			}
		}

		// retire un bâtiment
		if (Input.GetMouseButtonDown (1))
		{
			ray = myCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, buildMask))
			{
				buildHandler.RemoveBuild (hit.transform.position);
			}
		}

		// tourne un bâtiment
		if (Input.GetKeyDown (KeyCode.R))
		{
			ray = myCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, buildMask))
			{
				buildHandler.RotateBuild (hit.transform.position, 1);
			}
		}

		// sert à assigner la position à la quelle le prochain bâtiment à déplacé sera déplacé
		if (Input.GetKeyDown (KeyCode.S))
		{
			ray = myCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundMask))
			{
				newBuildPosition = hit.point;
				Debug.Log ("New build position is set at " + hit.point.ToString ());
			}
		}

		// sert à déplacé le bâtiment visé à la position assigné
		if (Input.GetKeyDown (KeyCode.M))
		{
			ray = myCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, buildMask))
			{
				buildHandler.MoveBuild (hit.transform.position, newBuildPosition);
			}
		}
	}

    void OnValidate()
    { 
        if (selectedBuildIndex < 0)
        {
            selectedBuildIndex = 0;
        }

        if (selectedBuildIndex >= builds.Length)
        {
            selectedBuildIndex = builds.Length - 1;
        }
    }
}
