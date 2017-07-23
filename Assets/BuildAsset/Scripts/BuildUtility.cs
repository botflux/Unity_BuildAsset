using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildAsset
{
	/// <summary>
	/// Classe contenant une collection de méthodes servant dans l'ensemble du projet BuildAsset.
	/// </summary>
	public static class BuildUtility
	{
		// arrondie une position à la snapValues la plus proche
		/// <summary>
		/// Arrondie la position passée en paramètre.
		/// </summary>
		/// <returns>La position arrondie.</returns>
		/// <param name="nonSnappedPosition">La position à arrondir.</param>
		/// <param name="snapValues">L'échelle.</param>
		public static Vector3 RoundPosition (Vector3 nonSnappedPosition, Vector2 snapValues)
		{
			return new Vector3 (
				snapValues.x * Mathf.Round(nonSnappedPosition.x / snapValues.x),
				0f,
				snapValues.y * Mathf.Round(nonSnappedPosition.z / snapValues.y)
			);
		}

		// renvoie la liste des coords utilisés par le paramètre utilisées
		/// <summary>
		/// Renvoie les coordonnées utilisées par un bâtiment.
		/// </summary>
		/// <returns>Les coordonnées utilisées par le bâtiment.</returns>
		/// <param name="location">L'origine du bâtiment.</param>
		/// <param name="size">La taille du bâtiment.</param>
		public static Coord[] GetOccupiedBuildCoords (Coord location, Coord size)
		{
			return GetOccupiedBuildCoords (location.x, location.y, size.x, size.y);
		}

		// renvoie la liste des coords utilisés par le paramètre utilisées
		/// <summary>
		/// Renvoie les coordonnées utilisées par un bâtiment.
		/// </summary>
		/// <returns>Les coordonnées utilisées par le bâtiment.</returns>
		/// <param name="locationX">L'origine en X du bâtiment.</param>
		/// <param name="locationY">L'origine en Y du bâtiment.</param>
		/// <param name="sizeX">La taille en X du bâtiment.</param>
		/// <param name="sizeY">La taille en Y du bâtiment.</param>
		public static Coord[] GetOccupiedBuildCoords (int locationX, int locationY, int sizeX, int sizeY)
		{
			Coord[] res = new Coord[sizeX * sizeY];
			int currentIndex = 0;

			for (int i = 0; i < sizeX; i++) 
			{
				for (int j = 0; j < sizeY; j++) 
				{
					res[currentIndex] = new Coord (locationX + i, locationY + j);
					currentIndex++;
				}
			}

			return res;
		}

		// renvoie le millieu d'un bâtiment
		/// <summary>
		/// Renvoie la position dans l'espace 3D du milieu du bâtiment.
		/// </summary>
		/// <returns>Le milieu du bâtiment dans l'espace 3D.</returns>
		/// <param name="snappedPosition">L'origine du bâtiment dans l'espace 3D.</param>
		/// <param name="size">La taille du bâtiment.</param>
		public static Vector3 GetBuildWorldPosition (Vector3 snappedPosition, Vector3 size)
		{
			//return (snappedPosition + (snappedPosition + size)) / 2f;

            return new Vector3((snappedPosition.x + (snappedPosition.x + size.x)) / 2f, size.y / 2f, (snappedPosition.z + (snappedPosition.z + size.z)) / 2f);
		}
	}

	[System.Serializable]
	/// <summary>
	/// Représente une position entière dans un repère en deux dimensions.
	/// </summary>
	public struct Coord
	{
		/// <summary>
		/// La composante X de la coordonnée.
		/// </summary>
		public int x;
		/// <summary>
		/// La composante Y de la coordonnée.
		/// </summary>
		public int y;

		/// <summary>
		/// Initialise une nouvelle instance de <see cref="BuildAsset.Coord"/> struct.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Coord (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Coord operator + (Coord c1, Coord c2)
		{
			return new Coord (c1.x + c2.x, c1.y + c2.y);
		}

		public static Coord operator - (Coord c1, Coord c2)
		{
			return new Coord (c1.x - c2.x, c1.y - c2.y);
		}

		public static Coord operator * (Coord c1, int f)
		{
			return new Coord (c1.x * f, c1.y * f);
		}

		public static Coord operator / (Coord c1, int f)
		{
			return new Coord (c1.x / f, c1.y / f);
		}

		public static bool operator == (Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator != (Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}

		public override bool Equals (object obj)
		{
			return base.Equals (obj);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Retourne un <see cref="System.String"/> qui représente l'instance actuelle de <see cref="BuildAsset.Coord"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BuildAsset.Coord"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("({0},{1})", x, y);
		}
	}
}