using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildAsset
{
	/// <summary>
	/// Classe représentant les champs principaux de la classe Build.
	/// </summary>
	public class Log_Build
	{
		/// <summary>
		/// Renseigne si oui ou non la taille du bâtiment doit être multipliée par la taille du BuidHandler.
		/// </summary>
		public bool applyBuildHandlerScale;
		/// <summary>
		/// La taille du collider de ce bâtiment.
		/// </summary>
		public Vector3 size;
		/// <summary>
		/// Coordonnée en X du bâtiment dans sa grille.
		/// </summary>
		public int locationX;
		/// <summary>
		/// Coordonnée en Y du bâtiment dans sa grille.
		/// </summary>
		public int locationY;
		/// <summary>
		/// Le nombre de colonnes occupées par le bâtiment.
		/// </summary>
		public int sizeX;
		/// <summary>
		/// Le nombre de lignes occupées par le bâtiment.
		/// </summary>
		public int sizeY;
		/// <summary>
		/// Origine du bâtiment.
		/// </summary>
		public Vector3 origin;
		/// <summary>
		/// Coordonnées occupées par le bâtiment dans sa grille.
		/// </summary>
		public Coord[] occupiedCoords;

		/// <summary>
		/// Initialise une nouvelle instance de la classe <see cref="BuildAsset.Log_Build"/>.
		/// </summary>
		/// <param name="applyBuildHandlerScale">Si attribuer à <c>true</c> appliquer la taille du buildHandler à celle du bâtiment.</param>
		/// <param name="size">Taille du collider du bâtiment.</param>
		/// <param name="locationX">Position en X dans la grille de bâtiment.</param>
		/// <param name="locationY">Position en Y dans la grille de bâtiment.</param>
		/// <param name="sizeX">Nombre de colonnes occupées en X par ce bâtiment.</param>
		/// <param name="sizeY">Nombre de lignes occupées en Y par ce bâtiment.</param>
		/// <param name="origin">Origine de ce bâtiment dans l'espace 3D.</param>
		/// <param name="occupiedCoords">Coordonnées occupées par ce bâtiment dans la grille.</param>
		public Log_Build (bool applyBuildHandlerScale, Vector3 size, int locationX, int locationY, int sizeX, int sizeY, Vector3 origin, Coord[] occupiedCoords)
		{
			this.applyBuildHandlerScale = applyBuildHandlerScale;
			this.size = size;
			this.locationX = locationX;
			this.locationY = locationY;
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			this.origin = origin;
			this.occupiedCoords = occupiedCoords;
		}
	}
}
