﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildAsset;
using Utility = BuildAsset.BuildUtility;
using Coord = BuildAsset.Coord;

[RequireComponent (typeof(Collider))]
public class BuildHandler : MonoBehaviour 
{
	/// <summary>
	/// Le type d'affichage de la grille.
	/// </summary>
	/// <value>The grid show mod.</value>
	public BuildGridShowMod GridShowMod 
	{
		get
		{
			return this.buildGridShowMod;
		}
		set
		{
			this.buildGridShowMod = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BuildHandler"/> use secure sizes.
	/// </summary>
	/// <value><c>true</c> if use secure sizes; otherwise, <c>false</c>.</value>
	public bool UseSecureSizes
	{
		get
		{
			return this.useSecureSizes;
		}
		set 
		{
			this.useSecureSizes = value;
		}
	}

	/// <summary>
	/// Gets or sets the cut count x.
	/// </summary>
	/// <value>The cut count x.</value>
	public int SpacingCountX 
	{
		get 
		{
			return this.spacingCountX;
		}
		set 
		{
			this.spacingCountX = value;
		}
	}

	/// <summary>
	/// Gets or sets the cut count y.
	/// </summary>
	/// <value>The cut count y.</value>
	public int SpacingCountY 
	{
		get
		{
			return this.spacingCountY;
		}
		set 
		{
			this.spacingCountY = value;
		}
	}

	/// <summary>
	/// Gets or sets the color of the gizmos grid.
	/// </summary>
	/// <value>The color of the gizmos grid.</value>
	public Color GizmosGridColor
	{
		get
		{
			return this.gizmosGridColor;
		}
		set 
		{
			this.gizmosGridColor = value;
		}
	}

	/// <summary>
	/// Le type d'affichage de la grille.
	/// </summary>
	public enum BuildGridShowMod {Gizmos, None};

	[SerializeField]
	/// <summary>
	/// Le type d'affichage de la grille.
	/// </summary>
	private BuildGridShowMod buildGridShowMod = BuildGridShowMod.None;

	[SerializeField]
    [Tooltip ("Renseigne si oui ou non, une vérification doit être effectuée avant la création d'un bâtiment")]
    /// <summary>
    /// Renseigne si oui ou non il faut effectuer une vérification de la taille
    /// du GameObject lors de sa création.
    /// </summary>
	private bool useSecureSizes = false;

	[SerializeField]
	[Tooltip ("Number of cut in X-axis")]
	/// <summary>
	/// Nombre de colonnes.
	/// </summary>
	private int spacingCountX = 10;

	[SerializeField]
	[Tooltip ("Number of cut in Y-axis")]
	/// <summary>
	/// Nombre de lignes.
	/// </summary>
	private int spacingCountY = 10;

	[SerializeField]
	/// <summary>
	/// La couleur de la grille.
	/// </summary>
	private Color gizmosGridColor = Color.red;

	/// <summary>
	/// Référence à la composante Collider présente sur ce GameObject.
	/// </summary>
	private Collider coll;

	/// <summary>
	/// Représente la taille du Collider dans l'espace 3D.
	/// </summary>
	private Vector3 colliderSize;

	/// <summary>
	/// Représente la taille d'une colonnes avec la composante X,
	/// représente la taille d'une ligne avec la composante Y.
	/// </summary>
	private Vector2 snapValues;

	/// <summary>
	/// Représente la moitié de la valeur de snapValues.
	/// Attention c'est la composante Z qui représente la moitié de la
	/// composante Y de snapValues.
	/// </summary>
	private Vector3 offset;

	/// <summary>
	/// Représente les bâtiments présents sur cette instance BuildHandler.
	/// </summary>
	private List<Build> builds = new List<Build> ();

	/// <summary>
	/// Référence à la composante Transform de ce GameObject.
	/// </summary>
	private Transform myTransform;

	/// <summary>
	/// Représente le bâtiment sur lequel les prochains changement vont être effectués.
	/// Attention cette référence est là pour ne pas surcharger le GC.
	/// Cette référence est mise à jour uniquement lors ce qu'un traitement est efféctué 
	/// sur un quelquonque bâtiment présent sur cette instance.
	/// </summary>
	private Build currentUsedBuild = null;

	/// <summary>
	/// Référence à la piscine de bâtiment assignée à ce gestionnaire de bâtiment.
	/// </summary>
	private Pool<Build> buildPool;

	void Awake ()
	{
		coll = GetComponent<Collider> ();
	}

	void Start ()
	{
		Initialisation ();
	}

	/// <summary>
	/// Initialise les valeurs utilisées par cette classe.
	/// </summary>
	void Initialisation ()
	{
		myTransform = transform;
		colliderSize = coll.bounds.size;
		snapValues = new Vector2 (colliderSize.x / (float)spacingCountX, colliderSize.z / (float)spacingCountY);
		offset = new Vector3 (snapValues.x / 2f, 0f, snapValues.y / 2f);

		buildPool = GameManager.instance.buildPool;

		if (buildPool == null)
		{
			throw new UnassignedReferenceException("No pool assigned!");
		}

        Debug.Log(this.ToString ());
	}

	/// <summary>
	/// Renvoie la position passée en paramètre alignée à la grille de cette instance de BuildHandler.
	/// </summary>
	/// <returns>La position alignée à la grille.</returns>
	/// <param name="nonSnapPosition">Une position à aligner à la grille.</param>
	Vector3 GetSnappedPosition (Vector3 nonSnapPosition)
	{
		return BuildUtility.RoundPosition (nonSnapPosition, snapValues);
	}

	Vector3 GetSnappedPosition (Vector3 nonSnappedPosition, Vector3 deltaPosition)
	{
		return GetSnappedPosition(nonSnappedPosition) + deltaPosition;
	}

	/// <summary>
	/// Détermine si les coordonnées passées en paramètre sont déjà occupées par un autre bâtiment.
	/// </summary>
	/// <returns><c>true</c> vrai si les coordonnées sont déjà occupées; sinon, <c>false</c>.</returns>
	/// <param name="locations">Les coordonnées à vérifier.</param>
	bool IsOccupiedCoord (Coord[] locations)
	{
		for (int i = 0; i < locations.Length; i++) 
		{
			if (IsOccupiedCoord (locations[i]))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Détermine si la coordonnée passée en paramètre est déjà occupée par un autre bâtiment.
	/// </summary>
	/// <returns><c>true</c> si la coordonnée est déjà occupée; sinon, <c>false</c>.</returns>
	/// <param name="location">La coordonnée à vérifier.</param>
	bool IsOccupiedCoord (Coord location)
	{
		return IsOccupiedCoord (location.x, location.y);
	}

	/// <summary>
	/// Détermine si la coordonnée passée en paramètre est déjà occupée par un autre bâtiment.
	/// </summary>
	/// <returns><c>true</c> if this instance is occupied coordinate the specified x y; otherwise, <c>false</c>.</returns>
	/// <param name="x">La composante X de la coordonnée.</param>
	/// <param name="y">La composante Y de la coordonnée.</param>
	bool IsOccupiedCoord (int x, int y)
	{
		foreach (Build build in builds) 
		{
			foreach (Coord item in build.occupiedCoords) 
			{
				if (item.x == x && item.y == y)
				{
					return true;
				}	
			}
		}

		return false;
	}

	/// <summary>
	/// Détermine si les coordonnées passées en paramètre est déjà occupée par un autre bâtiment, en ne prenant pas en compte certaines autres coordonnées.
	/// </summary>
	/// <returns><c>true</c> si les coordonnées sont occupées; sinon, <c>false</c>.</returns>
	/// <param name="locations">Les coordonnées à vérifier.</param>
	/// <param name="coordsToIgnore">Les coordonnées à ignorer.</param>
	bool IsOccupiedCoord (Coord[] locations, Coord[] coordsToIgnore)
	{
		for (int i = 0; i < locations.Length; i++) 
		{
			if (IsOccupiedCoord (locations[i], coordsToIgnore))
			{
				return true;
			}	
		}

		return false;
	}

	/// <summary>
	/// Détermine si la coordonnée passée en paramètre est déjà occupée par un autre bâtiment, en ne prenant pas en compte certaines autres coordonnées.
	/// </summary>
	/// <returns><c>true</c> si la coordonnée est occupée; sinon, <c>false</c>.</returns>
	/// <param name="location">La coordonnée à vérifier.</param>
	/// <param name="coordsToIgnore">Les coordonnées à ignorer.</param>
	bool IsOccupiedCoord (Coord location, Coord[] coordsToIgnore)
	{
		foreach (var build in builds) 
		{
			foreach (Coord item in build.occupiedCoords) 
			{
				bool skip = false;

				foreach (Coord coord in coordsToIgnore) 
				{
					if (coord == item)
					{
						skip = true;
					}	
				}

				if (skip) continue;

				if (item.x == location.x && item.y == location.y)
				{
					Debug.Log ("Occupied");
					return true;
				}	
			}	
		}

		return false;
	}

	/// <summary>
	/// Détermine si les coordonnées passées en paramètre sont dans le Collider de cette instance.
	/// </summary>
	/// <returns><c>true</c> si les coordonnées sont dans le Collider; sinon, <c>false</c>.</returns>
	/// <param name="locations">Les coordonnées à vérifier.</param>
	bool IsInCellule (Coord[] locations)
	{
		for (int i = 0; i < locations.Length; i++) 
		{
			if (!IsInCellule (locations[i]))
			{
				Debug.Log ("Not in cellule");
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Détermine si la coordonnée passée en paramètre est dans le Collider de cette instance.
	/// </summary>
	/// <returns><c>true</c> si la coordonnée est dans le Collider; sinon, <c>false</c>.</returns>
	/// <param name="location">La coordonnée à vérifier.</param>
	bool IsInCellule (Coord location)
	{
		return IsInCellule (location.x, location.y);
	}

	/// <summary>
	/// Détermine si la coordonnée passée en paramètre est dans le Collider de cette instance.
	/// </summary>
	/// <returns><c>true</c> si la coordonnée est dans le Collider; sinon, <c>false</c>.</returns>
	/// <param name="x">La composante X de la coordonnée.</param>
	/// <param name="y">La composante Y de la coordonnée.</param>
	bool IsInCellule (int x, int y)
	{
		return (x >= 0 && x < spacingCountX) && (y >= 0 && y < spacingCountY);
	}
		
	/// <summary>
	/// Crée un bâtiment sur cette instance du BuildHandler.
	/// </summary>
	/// <param name="buildToCreate">Le prefab du bâtiment à créer.</param>
	/// <param name="buildPosition">La position à laquelle le bâtiment sera instancié.</param>
	public void CreateBuild (Build buildToCreate, Vector3 buildPosition)
	{
		if (buildToCreate.Size.x <= 0 && buildToCreate.Size.y <= 0 && buildToCreate.Size.z <= 0)
		{
			throw new System.InvalidOperationException("Build hasn't correct size: " + buildToCreate.Size);
		}

		Vector3 snappedTransform = GetSnappedPosition(myTransform.position);
		Vector3 delta = myTransform.position - snappedTransform;

		// position aligné à la grille.
		Vector3 snappedPosition = GetSnappedPosition(buildPosition, delta);
		// position local au sol aligné à la grille
		Vector3 localSnappedPosition = GetSnappedPosition (buildPosition - myTransform.position, delta);
		Debug.Log ("Given Position: " + buildPosition);
		Debug.Log ("Snapped Position: " + snappedPosition);
		Debug.Log ("Local Snapped Position: " + localSnappedPosition);

		// calcul la position a la quel doit être instancié le batiment
		Vector3 instantiatePosition = BuildUtility.GetBuildWorldPosition (snappedPosition, buildToCreate.Size)/* + delta*/;

		Debug.Log ("Instantiate Position: " + instantiatePosition);

		// index en x du batiment
		// on divise la position local par la valeur du snap pour avoir un index allant de -cutCount / 2 jusqua cutCount / 2 - 1.
		// et le cutCount est la pour decaler l'index afin d'obtenir un index allant de 0 à cutCount.
		int locX = Mathf.RoundToInt(localSnappedPosition.x / snapValues.x) + (spacingCountX / 2);
		// index en y du batiment
		int locZ = Mathf.RoundToInt(localSnappedPosition.z / snapValues.y) + (spacingCountY / 2);

		int sizeX = Mathf.RoundToInt (buildToCreate.Size.x / snapValues.x);
		int sizeY = Mathf.RoundToInt (buildToCreate.Size.z / snapValues.y);

		Debug.Log ("LocX: " + locX + " LocZ: " + locZ);
		Debug.Log ("SizeX: " + sizeX + " SizeY" + sizeY);

        // en cas de non association parfaite possible entre la taille du prefab et la snapValues alors on arrêtera la création.
        if (useSecureSizes && (buildToCreate.Size.x % snapValues.x != 0 || buildToCreate.Size.z % snapValues.y != 0))
        {
            Debug.Log("Création impossible la taille du bâtiment ne correspond pas à la grille");
            return;
        }

		Coord[] occupiedCoords = BuildUtility.GetOccupiedBuildCoords (locX, locZ, sizeX, sizeY);

		if (IsInCellule (occupiedCoords) && !IsOccupiedCoord (occupiedCoords))
		{
			// instanciation du batiment
			//Build instantiatedBuild = Instantiate(buildToCreate, instantiatePosition, Quaternion.identity);

			if (!buildPool.IsInPool(buildToCreate))
			{
				buildPool.AddPrefabReference(buildToCreate);
			}

			Build instantiatedBuild = buildPool.GetObject (buildToCreate);
			instantiatedBuild.buildTransform.position = instantiatePosition;

			if (instantiatedBuild.ApplyBuildHandlerScale)
			{
				// assignation de la taille actuelle qu'a le bâtiment
				Vector3 currentBuildLocalScale = instantiatedBuild.buildTransform.localScale;
				// calcul de la nouvelle taille 
				Vector3 newBuildLocalScale = new Vector3 (currentBuildLocalScale.x * myTransform.localScale.x, currentBuildLocalScale.y * myTransform.localScale.y, currentBuildLocalScale.z * myTransform.localScale.z);
				// assigne la nouvelle taille au bâtiment
				instantiatedBuild.buildTransform.localScale = newBuildLocalScale;
			}

			// initialisation du batiment
			instantiatedBuild.BuildInitialisation (locX, locZ, sizeX, sizeY, snappedPosition + offset, occupiedCoords);
			// ajout du batiement a la liste des batiments présents sur ce gameobject
			builds.Add (instantiatedBuild);
            // assigne le transform parent du build
            // je fais cette manipulation après l'instantiation car sinon le build aura en plus de la scale, celle du transform de cet objet
            // ce qui risque de poser de gros problèmes.
            instantiatedBuild.buildTransform.SetParent(myTransform);

			Debug.Log(instantiatedBuild.ToString());
		}
	}

	/// <summary>
	/// Applique une rotation à un bâtiment.
	/// </summary>
	/// <param name="buildPosition">La position du Transform du bâtiment à tourner.</param>
	/// <param name="quarterCount">Le nombre de quart de cercle à appliquer à la rotation déjà appliquée sur le bâtiment.</param>
	public void RotateBuild (Vector3 buildPosition, int quarterCount)
	{
		// appliquer une matrice de rotation

		GetBuildFromTransformPosition (buildPosition, out currentUsedBuild);

		if (currentUsedBuild != null)
		{
			Coord[] newCoords = GetRotatedCoords (currentUsedBuild.occupiedCoords, currentUsedBuild.LocationX, currentUsedBuild.LocationY, quarterCount);
			// voir si les positions ne sont pas deja occupé
			if (IsInCellule (newCoords) && !IsOccupiedCoord (newCoords, currentUsedBuild.occupiedCoords))
			{
				currentUsedBuild.Rotation (newCoords, quarterCount * 90f);
			}
		}
	}

	/// <summary>
	/// Supprime un bâtiment.
	/// </summary>
	/// <param name="buildPosition">La position du Transform du bâtiment à supprimer.</param>
	public void RemoveBuild (Vector3 buildPosition)
	{
		int index = -1;
		//Build build = GetBuildFromTransformPosition (buildPosition, out index);

		GetBuildFromTransformPosition (buildPosition, out index, out currentUsedBuild);

		if (currentUsedBuild != null && index != -1)
		{
			//DestroyImmediate (currentUsedBuild.gameObject);
			buildPool.DisposeObject(currentUsedBuild);
			builds.RemoveAt (index);
		}
	}
		
	/// <summary>
	/// Déplace un bâtiment.
	/// </summary>
	/// <param name="buildPosition">La position du Transform du bâtiment à déplacer.</param>
	/// <param name="newPosition">La nouvelle position du bâtiment.</param>
	public void MoveBuild (Vector3 buildPosition, Vector3 newPosition)
	{
		//Build build = GetBuildFromTransformPosition (buildPosition);

		GetBuildFromTransformPosition (buildPosition, out currentUsedBuild);

		if (currentUsedBuild != null)
		{
			if (currentUsedBuild.buildTransform.position == buildPosition)
			{
				// donne l'origine du batiment
				Vector3 snappedPosition = GetSnappedPosition (newPosition);

				// donne l'origine du bâtiment relative au buildHandler
				Vector3 localSnappedPosition = GetSnappedPosition (newPosition - myTransform.position);

				// la difference qu'il y a entre les origins cad l'ancienne et la nouvelle
				Vector3 deltaOrigins = snappedPosition - currentUsedBuild.Origin + offset;

				// nouvelle position du transform du bâtiment
				Vector3 newTransformPosition = new Vector3 (currentUsedBuild.buildTransform.position.x + deltaOrigins.x, currentUsedBuild.buildTransform.position.y, currentUsedBuild.buildTransform.position.z + deltaOrigins.z);

				// index en x du batiment
				int locX = Mathf.RoundToInt(localSnappedPosition.x / snapValues.x) + (spacingCountX / 2);

				// index en y du batiment
				int locZ = Mathf.RoundToInt(localSnappedPosition.z / snapValues.y) + (spacingCountY / 2);

				// le decalage entre le nouveau locationX et l'ancien
				int deltaLocationX = locX - currentUsedBuild.LocationX;

				// le decalage entre le nouveau locationY et l'ancien
				int deltaLocationY = locZ - currentUsedBuild.LocationY;

				// les nouvelles positions occupées par le batiment à deplacer
				Coord[] newOccupiedCoords = new Coord[currentUsedBuild.occupiedCoords.Length];

				for (int j = 0; j < newOccupiedCoords.Length; j++) 
				{
					newOccupiedCoords[j] = new Coord (currentUsedBuild.occupiedCoords[j].x + deltaLocationX, currentUsedBuild.occupiedCoords[j].y + deltaLocationY);
				}

				if (IsInCellule (newOccupiedCoords) && !IsOccupiedCoord (newOccupiedCoords))
				{
					//Debug.Log (string.Format ("Snapped position = {0}; Last snapped position = {1}; Delta origins = {2};", snappedPosition, builds[i].origin - offset, deltaOrigins));
					//currentUsedBuild.buildTransform.position = newTransformPosition;
					currentUsedBuild.Move (locX, locZ, newOccupiedCoords, snappedPosition + offset, newTransformPosition);
				}
			}
		}
	}

	/// <summary>
	/// Renvoie le bâtiment correspondant à la position passée en paramètre.
	/// Utiliser les positions du Transform du bâtiment recherché.
	/// </summary>
	/// <returns>Le bâtiment correspondant à la position donnée.</returns>
	/// <param name="position">La position du Transform du bâtiment recherché.</param>
	Build GetBuildFromTransformPosition (Vector3 position)
	{
		for (int i = 0; i < builds.Count; i++) 
		{
			if (position == builds[i].buildTransform.position)
			{
				return builds[i];
			}
		}

		throw new UnityException ("No build found.");
	}

	/// <summary>
	/// Renvoie le bâtiment correspondant à la position passée en paramètre.
	/// Renvoie également par le biais de la variable index, l'index dans la collection de
	/// bâtiment du bâtiment recherché.
	/// Utiliser les positions du Transform du bâtiment recherché.
	/// Index sera assigné à -1 si aucun bâtiment n'a été trouvé.
	/// </summary>
	/// <returns>Le bâtiment correspondant à la position donnée.</returns>
	/// <param name="position">La position du Transform du bâtiment recherché.</param>
	/// <param name="index">Index du bâtiment recherché.</param>
	Build GetBuildFromTransformPosition (Vector3 position, out int index)
	{
		for (int i = 0; i < builds.Count; i++) 
		{
			if (position == builds[i].buildTransform.position)
			{
				index = i;
				return builds[i];
			}
		}

		index = -1;
		throw new UnityException ("No build found.");
	}

	/// <summary>
	/// Détermine le bâtiment correspondant à la position passée en paramètre.
	/// Cette implémentation renvoie le bâtiment via la référence build.
	/// </summary>
	/// <param name="position">La position du bâtiment recherché.</param>
	/// <param name="index">Index du bâtiment recherché.</param>
	/// <param name="build">Bâtiment recherché.</param>
	void GetBuildFromTransformPosition (Vector3 position, out int index, out Build build)
	{
		build = null;
		index = -1;

		for (int i = 0; i < builds.Count; i++) 
		{
			if (position == builds[i].buildTransform.position)
			{
				index = i;
				build = builds[i];
			}	
		}
	}

	/// <summary>
	/// Détermine le bâtiment correspondant à la position passée en paramètre.
	/// Cette implémentation renvoie le bâtiment via la référence build.
	/// </summary>
	/// <param name="position">La position du bâtiment recherché.</param>
	/// <param name="build">Bâtiment recherché.</param>
	void GetBuildFromTransformPosition (Vector3 position, out Build build)
	{
		build = null;

		for (int i = 0; i < builds.Count; i++) 
		{
			if (position == builds[i].buildTransform.position)
			{
				build = builds[i];
			}	
		}
	}

	/// <summary>
	/// Renvoie les coordonnées passées en paramètre tourné d'un angle quarterCount * 90.
	/// </summary>
	/// <returns>Les coordonnées tournées.</returns>
	/// <param name="coordsToRotate">Les coordonnées à tourner.</param>
	/// <param name="locationX">La coordonnée en X du bâtiment à tourner.</param>
	/// <param name="locationY">La coordonnée en Y du bâtiment à tourner.</param>
	/// <param name="quarterCount">Nombre de quart de tour à appliquer à la rotation actuelle.</param>
	Coord[] GetRotatedCoords (Coord[] coordsToRotate, int locationX, int locationY, int quarterCount)
	{
		int len = coordsToRotate.Length;
		Coord[] res = new Coord[len];

		for (int i = 0; i < len; i++) 
		{
			// la coordonné relative à la position d'origine
			int relativeX = coordsToRotate[i].x - locationX;
			int relativeY = coordsToRotate[i].y - locationY;

			// la position relative avec la rotation appliquée
			int x = Mathf.RoundToInt(relativeX * Mathf.Cos (90 * -quarterCount) - relativeY * Mathf.Sin(90 * -quarterCount));
			int y = Mathf.RoundToInt(relativeX * Mathf.Sin (90 * -quarterCount) + relativeY * Mathf.Cos (90 * -quarterCount));

			// assignation a la coordonné tournée, la somme de coordonné tourné et l'origine
			// ce qui fait que la coord est de nouveau relative à la grille.
			res[i] = new Coord (locationX + x, locationY + y);
		}
		return res;
	}

	void OnValidate ()
	{
		if (spacingCountX <= 0)
		{
			spacingCountX = 1;
		}

		if (spacingCountY <= 0)
		{
			spacingCountY = 1;
		}
	}

	void OnDrawGizmos ()
	{
		if (buildGridShowMod == BuildGridShowMod.Gizmos)
		{
			if (myTransform == null) myTransform = transform;

			Gizmos.color = this.gizmosGridColor;
			Vector3 bottomLeftCorner = myTransform.position - (colliderSize / 2f);

			for (int i = 0; i <= spacingCountX; i++) 
			{
				Vector3 currentFromPoint = bottomLeftCorner + new Vector3 ((i * snapValues.x), myTransform.position.y + 0.01f, 0);
				Vector3 currentLastPoint = bottomLeftCorner + new Vector3 ((i * snapValues.x), myTransform.position.y + 0.01f, colliderSize.z);
			

				Gizmos.DrawLine (currentFromPoint, currentLastPoint);
			}

			for (int i = 0; i <= spacingCountY; i++) 
			{
				Vector3 currentFromPoint = bottomLeftCorner + new Vector3 (0f, myTransform.position.y + 0.01f, i * snapValues.y);
				Vector3 currentLastPoint = bottomLeftCorner + new Vector3 (colliderSize.x, myTransform.position.y + 0.01f, i * snapValues.y);

				Gizmos.DrawLine (currentFromPoint, currentLastPoint);
			}
		}
	}

    public override string ToString()
    {
        return string.Format("Collider size : {0}; Snap values: {1}; Cut count x-axis: {2}; Cut count y-axis: {3};", colliderSize, snapValues, spacingCountX, spacingCountY);    
    }
}

/*
'	Fonctionnalités:
'		- Peut poser un batiment
'		- Peut retirer un batiment
'		- Peut tourner un batiment
'		- Peut deplacer un batiment
*/

/*	TO DO LIST
TODO: Tester l'asset en utilisant différents diécoupage.
TODO: Couvrir les cas impossible comme le découpage null ou négatif ou bien la taille non assigné
TODO: Implémenter des méthodes qui permettent d'obtenir des valeurs liées aux bâtiments.
TODO: Faire plusieurs options avec la possibilité de choisir soit même de changer la scale ou bien de la mettre automatiquement avec la scale du bâtiment handler
TODO: Implémenter un debug pour l'affichage d'une grille avec les gizmos. ou bien créer une texture puis l'appliquer à la texture déjà présente
TODO: Ajouter une pool de bâtiment


UNE PISTE TROUVER POUR REGLER LE PROBLEME DE DECALAGE LORS CE QUE L4ON UTILISE DES POSITION FLOTTANTE
*/