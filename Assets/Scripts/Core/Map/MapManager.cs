using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UrUtils.Extensions;
using UrUtils.Misc;

namespace Core.Map
{
    public class MapManager : Singleton<MapManager>, IPointerDownHandler
    {
        [SerializeField]
        MapParameters Parameters;

        [SerializeField]
        Transform MapRoot;

        Tile[,] Map;

        [SerializeField]
        BoxCollider MapCollider;


        protected override bool IsPersistent => false;


        #region Unity

        void Update()
        {
            if (Map == null)
                return;
        }

        #endregion


        public Vector2Int InitializeMap()
        {
            ClearMap();

            Map = new Tile[Parameters.MapSize.y, Parameters.MapSize.x];
            MapCollider.size = new Vector3(Parameters.MapSize.x, 0.01f, Parameters.MapSize.y) * Parameters.TileWorldSize;
            MapCollider.center = MapCollider.size / 2f;

            var start = Parameters.SpawnPoint;
            CreateTile(start, Parameters.StartingTile);
            return start;
        }

        void CreateTile(Vector2Int pos, Tile tileData)
        {
            var tile = Instantiate(tileData, MapRoot);
            tile.transform.position = Parameters.GetTileCenter3D(pos);

            Map[pos.y, pos.x] = tile;
        }

        void ClearMap()
        {
            if (Map == null)
                return;

            foreach (var tile in Map)
            {
                if (tile == null)
                    continue;
                Destroy(tile.gameObject);
            }

            Map = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 pressPos = eventData.pointerPressRaycast.worldPosition;
            Vector2Int pressCoordinates = new Vector2Int(
                Mathf.FloorToInt(pressPos.x / Parameters.TileWorldSize),
                Mathf.FloorToInt(pressPos.z / Parameters.TileWorldSize));
            bool isInBound = IsInBound(pressCoordinates);
            Debug.Log($"Clicked {pressCoordinates} -> {isInBound}");
            if (!isInBound)
                return;

            ProcessTileClick(pressCoordinates);
        }

        void ProcessTileClick(Vector2Int coord)
        {
            var tile = Map[coord.y, coord.x];
            if (tile == null)
                SpawnRandomTile(coord);
        }

        void SpawnRandomTile(Vector2Int coord)
        {
            var randomTile = Parameters.NormalTiles.Random();
            CreateTile(coord, randomTile);
        }

        bool IsInBound(Vector2Int coord)
        {
            return coord.x >= 0 && coord.x < Parameters.MapSize.x &&
                   coord.y >= 0 && coord.y < Parameters.MapSize.y;
        }
    }
}