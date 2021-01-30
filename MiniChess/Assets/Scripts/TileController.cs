using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class TileController : MonoBehaviour
    {
        public Vector2Int boardPosition;
        public string tileName;
        public TileType tileType;
        public GameObject objectOnTile;
        public Material cursorMaterial;
    }
}