using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildAsset;
using Utility = BuildAsset.BuildUtility;
using Coord = BuildAsset.Coord;

[RequireComponent (typeof(Collider))]
public class Build : PooledObject, ICleanup
{
	/// <summary>
	/// Renseigne si oui ou non la taille du bâtiment doit être multipliée par la taille du BuidHandler.
	/// </summary>
	private bool applyBuildHandlerScale = false;

	/// <summary>
	/// Renseigne si oui ou non une couleur aléatoire doit être appliquée sur l'instance du bâtiment.
	/// </summary>
	private bool useRandomColor = true;

	/// <summary>
	/// La taille du collider de ce bâtiment.
	/// </summary>
	private Vector3 size;

	/// <summary>
	/// Coordonnée en X du bâtiment dans sa grille.
	/// </summary>
	private int locationX;

	/// <summary>
	/// Coordonnée en Y du bâtiment dans sa grille.
	/// </summary>
	private int locationY;

	/// <summary>
	/// Le nombre de colonnes occupées par le bâtiment.
	/// </summary>
	private int sizeX;

	/// <summary>
	/// Le nombre de lignes occupées par le bâtiment.
	/// </summary>
	private int sizeY;

	[HideInInspector]
	/// <summary>
	/// Référence au Transform du bâtiment.
	/// </summary>
	public Transform buildTransform;

	[HideInInspector]
	/// <summary>
	/// Origine du bâtiment.
	/// </summary>
	private Vector3 origin;

	[HideInInspector]
	/// <summary>
	/// Coordonnées occupées par le bâtiment dans sa grille.
	/// </summary>
	public Coord[] occupiedCoords;

	/// <summary>
	/// Renvoie le position en X du bâtiment dans sa grille.
	/// </summary>
	/// <value>La position en x.</value>
	public int LocationX 
	{
		get
		{
			return this.locationX;
		}
		set
		{
			this.locationX = value;
		}
	}

	/// <summary>
	/// Renvoie la position en Y du bâtiment dans sa grille.
	/// </summary>
	/// <value>The location y.</value>
	public int LocationY
	{
		get
		{
			return this.locationY;
		}
		set
		{
			this.locationY = value;
		}
	}

	/// <summary>
	/// Renvoie le nombre de colonnes occupées par ce bâtiment.
	/// </summary>
	/// <value>The size x.</value>
	public int SizeX
	{
		get
		{
			return this.sizeX;
		}
		set
		{
			this.sizeX = value;
		}
	}

	/// <summary>
	/// Renvoie le nombre de lignes occupées par ce bâtiment.
	/// </summary>
	/// <value>The size y.</value>
	public int SizeY
	{
		get 
		{
			return this.sizeY;
		}
		set
		{
			this.sizeY = value;
		}
	}

	/// <summary>
	/// Renvoie l'origine du bâtiment.
	/// </summary>
	/// <value>The origin.</value>
	public Vector3 Origin
	{
		get
		{
			return this.origin;
		}
		set
		{
			this.origin = value;
		}
	}

	/// <summary>
	/// Gets or sets the size.
	/// </summary>
	/// <value>The size.</value>
    public Vector3 Size
    {
        get
        {
            return this.size;
        }
		set
		{
			this.size = value;
		}
    }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Build"/> apply build handler scale.
	/// </summary>
	/// <value><c>true</c> if apply build handler scale; otherwise, <c>false</c>.</value>
	public bool ApplyBuildHandlerScale
	{
		get 
		{
			return this.applyBuildHandlerScale;
		}
		set 
		{
			applyBuildHandlerScale = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Build"/> use random color.
	/// </summary>
	/// <value><c>true</c> if use random color; otherwise, <c>false</c>.</value>
	public bool UseRandomColor
	{
		get
		{
			return this.useRandomColor;
		}
		set
		{
			useRandomColor = value;
		}
	}

	void Awake ()
	{
		// assignation du transform de ce batiment
		buildTransform = transform;
	}

	void Start ()
	{
		// assignation d'une couleur aléatoire pour reconnaitre les batiements
		GetComponent<Renderer> ().materials[0].color = Random.ColorHSV ();
	}

	// initialise cet instance
	/// <summary>
	/// Initialise cette instance de bâtiment.
	/// </summary>
	/// <param name="locationX">Position en X du bâtiment dans sa grille.</param>
	/// <param name="locationY">Position en Y du bâtiment dans sa grille.</param>
	/// <param name="sizeX">Nombre de colonnes occupées par le bâtiment.</param>
	/// <param name="sizeY">Nombre de lignes occupées par le bâtiment.</param>
	/// <param name="origin">Origine du bâtiment.</param>
	/// <param name="occupiedCoords">Coordonnées occupées par ce bâtiment.</param>
	public void BuildInitialisation (int locationX, int locationY, int sizeX, int sizeY, Vector3 origin, Coord[] occupiedCoords)
	{
		this.locationX = locationX;
		this.locationY = locationY;
		this.sizeX = sizeX;
		this.sizeY = sizeY;
		this.origin = origin;
		this.occupiedCoords = occupiedCoords;
	}

	// change les positions occupées par ce bâtiment
	/// <summary>
	/// Renseigne les coordonnées occupées par ce bâtiment dans sa grille.
	/// </summary>
	/// <param name="occupiedCoords">Coordonnées occupées par le bâtiment.</param>
	public void SetOccupiedCoords (Coord[] occupiedCoords)
	{
		this.occupiedCoords = occupiedCoords;
	}

	// change la position dans le build handler de ce bâtiment
	/// <summary>
	/// Renseigne la position de ce bâtiment dans sa grille.
	/// </summary>
	/// <param name="x">La composante X de la coordonnée.</param>
	/// <param name="y">La composante Y de la coordonnée.</param>
	public void SetLocation (int x, int y)
	{
		this.locationX = x;
		this.locationY = y;
	}

	// change la position dans le build handler de ce batiment
	/// <summary>
	/// Renseigne la position de ce bâtiment dans sa grille.
	/// </summary>
	/// <param name="location">La coordonnée de ce bâtiment.</param>
	public void SetLocation (Coord location)
	{
		this.locationX = location.x;
		this.locationY = location.y;
	}

	// change l'origin de ce batiment
	/// <summary>
	/// Renseigne l'origine du bâtiment.
	/// </summary>
	/// <param name="origin">Origine du bâtiment.</param>
	public void SetOrigin (Vector3 origin)
	{
		this.origin = origin;
	}

	// assigne les valeurs nécessaire lors ce que l'on effectue une rotation
	/// <summary>
	/// Tourne le bâtiment.
	/// </summary>
	/// <param name="occupiedCoords">Les coordonnées de ce bâtiment.</param>
	/// <param name="angle">Angle de rotation.</param>
	public void Rotation (Coord[] occupiedCoords, float angle)
	{
		SetOccupiedCoords (occupiedCoords);
		buildTransform.RotateAround (origin, Vector3.up, angle);
	}

	// assigne les valeurs nécessaire lors ce que l'on effectue un déplacement de bâtiment
	/// <summary>
	/// Déplace le bâtiment en actualisant ces données.
	/// </summary>
	/// <param name="locationX">Coordonnées en x.</param>
	/// <param name="locationY">Coordonnées en y.</param>
	/// <param name="occupiedCoords">Coordonnées occupées par le batiment.</param>
	/// <param name="origin">Origine.</param>
	/// <param name="newTransformPosition">Nouvelle position du bâtiment.</param>
	public void Move (int locationX, int locationY, Coord[] occupiedCoords, Vector3 origin, Vector3 newTransformPosition)
	{
		this.locationX = locationX;
		this.locationY = locationY;
		this.occupiedCoords = occupiedCoords;
		this.origin = origin;

		buildTransform.position = newTransformPosition;
	}

	// assigne les valeurs nécessaire lors ce que l'on effectue un déplacement de bâtiment
	/// <summary>
	/// Déplace le bâtiment en actualisant ces données.
	/// </summary>
	/// <param name="location">Coordonnées du bâtiment.</param>
	/// <param name="occupiedCoords">Coordonnées occupées par le batiment.</param>
	/// <param name="origin">Origine.</param>
	/// <param name="newTransformPosition">Nouvelle position du bâtiment.</param>
	public void Move (Coord location, Coord[] occupiedCoords, Vector3 origin, Vector3 newTransformPosition)
	{
		this.locationX = location.x;
		this.locationY = location.y;
		this.occupiedCoords = occupiedCoords;
		this.origin = origin;

		buildTransform.position = newTransformPosition;
	}

	/// <summary>
	/// Nettoie cette instance.
	/// </summary>
	public void Cleanup ()
	{
		buildTransform.rotation = Quaternion.identity;
		this.locationX = -1;
		this.locationY = -1;
		this.sizeX = -1;
		this.sizeY = -1;
		this.origin = Vector3.zero;
		this.occupiedCoords = null;
		this.buildTransform.SetParent(null);
	}

	void OnValidate ()
	{
		if (size.x <= 0)
		{
			size.x = 0.000001f;
		}

        if (size.y <= 0)
        {
            size.y = 0.000001f;
        }

		if (size.z <= 0)
		{
			size.z = 0.000001f;
		}
	}
}
