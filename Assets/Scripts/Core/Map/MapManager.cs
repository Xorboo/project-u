using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UrUtils.Extensions;
using UrUtils.Misc;
using Random = UnityEngine.Random;

namespace Core.Map
{
    public class MapManager : Singleton<MapManager>, IPointerDownHandler
    {
        public event Action<Vector2Int, Tile> OnTileClicked = delegate { };


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

            for (int i = 0; i < Parameters.MysticTilesCount; i++)
                CreateMysticTile();
            return start;
        }

        void CreateMysticTile()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var coord = RandomCoords;
                if (Map[coord.y, coord.x] == null)
                {
                    CreateTile(coord, Parameters.MysticTile);
                    break;
                }
            }
        }

        Vector2Int RandomCoords => new Vector2Int(Random.Range(0, Parameters.MapSize.x), Random.Range(0, Parameters.MapSize.y));

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

            OnTileClicked(pressCoordinates, Map[pressCoordinates.y, pressCoordinates.x]);
        }

        public Tile RevealTile(Vector2Int coord, int dieResult)
        {
            return SpawnRandomTile(coord, dieResult);
        }

        Tile SpawnRandomTile(Vector2Int coord, int dieResult)
        {
            var randomTile = Parameters.NormalTiles[dieResult - 1];
            CreateTile(coord, randomTile);
            return randomTile;
        }

        bool IsInBound(Vector2Int coord)
        {
            return coord.x >= 0 && coord.x < Parameters.MapSize.x &&
                   coord.y >= 0 && coord.y < Parameters.MapSize.y;
        }
    }
}