using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UrUtils.Misc;
using Random = UnityEngine.Random;

namespace Core.Map
{
    public class MapManager : Singleton<MapManager>, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler
    {
        public event Action<Vector2Int, Tile> OnTileClicked = delegate { };


        [SerializeField]
        MapParameters Parameters;

        [SerializeField]
        Transform MapRoot;

        Tile[,] Map;

        [SerializeField]
        BoxCollider MapCollider;

        [SerializeField]
        GameObject TileHighlightObject;

        [SerializeField]
        Vector3 TileHighlightShift = new Vector3(0, 0.1f, 0);


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
            FillStartingArea(start);
            FillBorders();
            FillRandomTiles();

            return start;
        }

        void FillStartingArea(Vector2Int coord)
        {
            int d = Parameters.StartingAreaSize / 2;
            for (int y = coord.y - d; y <= coord.y + d; y++)
            {
                for (int x = coord.x - d; x <= coord.x + d; x++)
                {
                    var tile = CreateTile(new Vector2Int(x, y), Parameters.StartingTile);
                    if (tile != null)
                        tile.RevealImmediately();
                }
            }
        }

        void FillBorders()
        {
            for (int y = 0; y < Parameters.MapSize.y; y++)
            {
                CreateTile(new Vector2Int(0, y), Parameters.BorderTile);
                CreateTile(new Vector2Int(Parameters.MapSize.x - 1, y), Parameters.BorderTile);
            }

            for (int x = 0; x < Parameters.MapSize.x; x++)
            {
                CreateTile(new Vector2Int(x, 0), Parameters.BorderTile);
                CreateTile(new Vector2Int(x, Parameters.MapSize.y - 1), Parameters.BorderTile);
            }
        }

        void FillRandomTiles()
        {
            for (int y = 0; y < Parameters.MapSize.y; y++)
            {
                for (int x = 0; x < Parameters.MapSize.x; x++)
                {
                    var randomTile = Parameters.TileChances.GetRandom();
                    CreateTile(new Vector2Int(x, y), randomTile);
                }
            }
        }

        Vector2Int RandomCoords => new Vector2Int(Random.Range(0, Parameters.MapSize.x), Random.Range(0, Parameters.MapSize.y));

        Tile CreateTile(Vector2Int pos, Tile tileData)
        {
            if (Map[pos.y, pos.x] != null)
                return null;

            var tile = Instantiate(tileData, MapRoot);
            tile.transform.position = Parameters.GetTileCenter3D(pos);

            Map[pos.y, pos.x] = tile;
            return tile;
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

        public void RevealTilesAround(Vector2Int pos, Action onFinished)
        {
            float revealDelay = 0f;
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; x++)
                {
                    if (!IsInBound(x, y))
                        continue;

                    var tile = Map[y, x];
                    if (tile == null || tile.IsRevealed)
                        continue;

                    tile.Reveal(revealDelay);
                    revealDelay += Parameters.TileRevealDelay;
                }
            }

            DOTween.Sequence().AppendInterval(revealDelay).OnComplete(() => onFinished());
        }

        Vector2Int GetPointerCoordinate(Vector3 pressPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pressPos.x / Parameters.TileWorldSize),
                Mathf.FloorToInt(pressPos.z / Parameters.TileWorldSize));
        }

        bool IsInBound(Vector2Int coord) => IsInBound(coord.x, coord.y);
        bool IsInBound(int x, int y) => x >= 0 && x < Parameters.MapSize.x && y >= 0 && y < Parameters.MapSize.y;


        #region Pointer Events

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Vector2Int pressCoordinates = GetPointerCoordinate(eventData.pointerPressRaycast.worldPosition);
            if (!IsInBound(pressCoordinates))
                return;

            OnTileClicked(pressCoordinates, Map[pressCoordinates.y, pressCoordinates.x]);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            Vector2Int pressCoordinates = GetPointerCoordinate(eventData.pointerCurrentRaycast.worldPosition);
            var tile = Map[pressCoordinates.y, pressCoordinates.x];

            if (!IsInBound(pressCoordinates) || !GameManager.Instance.CanClickOnTile(pressCoordinates, tile))
            {
                if (TileHighlightObject.activeSelf)
                    TileHighlightObject.SetActive(false);
                return;
            }

            Vector3 desiredPosition = tile.transform.localPosition + TileHighlightShift;
            if (TileHighlightObject.transform.localPosition != desiredPosition)
                TileHighlightObject.transform.localPosition = desiredPosition;
            if (!TileHighlightObject.activeSelf)
                TileHighlightObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            TileHighlightObject.SetActive(false);
        }

        #endregion
    }
}