using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core.Map
{
    [CreateAssetMenu]
    public class MapParameters : ScriptableObject
    {
        public Vector2Int MapSize = new(41, 41);

        public Tile StartingTile;
        public int StartingAreaSize = 3;
        public Tile BorderTile;
        public WeightedRandom<Tile> TileChances = new();


        public float TileWorldSize = 5f;
        public float TileRevealDelay = 0.3f;

        public Vector2Int SpawnPoint => MapSize / 2;
        Vector2 GetTileCenter(Vector2Int coord) => new Vector2(coord.x + 0.5f, coord.y + 0.5f) * TileWorldSize;

        public Vector3 GetTileCenter3D(Vector2Int coord)
        {
            var center = GetTileCenter(coord);
            return new Vector3(center.x, 0f, center.y);
        }
    }
}