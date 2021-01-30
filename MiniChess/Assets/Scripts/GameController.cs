using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public GameObject tilePrefab;
        private Renderer cursorRenderer;
        public TileController[,] map;
        public GameObject cursorPrefab;
        public GameObject playerPrefab;
        public GameObject pawnPrefab;
        public GameObject bishopPrefab;
        public GameObject rookPrefab;
        public GameObject coinPrefab;
        public GameObject knightPrefab;
        public GameObject queenPrefab;

        // for animation
        private TileController selectedTile;
        private TileController targetTile;

        // debug
        public bool playersTurn = true;
        public bool onAnimation = false;
        public bool inMenu = true;
        public bool inYouWinAnim = false;
        public bool inYouLoseAnim = false;

        private AudioSource beep;
        public Animator cameraAnimator;
        public Animator youWinAnimator;
        public Animator youLoseAnimator;

        public Levels levels { get; set; }
        public int levelIndex = 0;

        List<Vector2Int> knightMoves = new List<Vector2Int>()
        {
            new Vector2Int(-1, 2),
            new Vector2Int(-1, -2),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
        };

        void Start()
        {
            beep = GetComponent<AudioSource>();
            levels = new Levels();

            map = new TileController[5, 6];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    CreateTile(x, y, TileType.Empty);
                }
            }

            LoadLevel(levelIndex);
        }

        private void LoadLevel(int levelIndex)
        {
            var level = levels.levels[levelIndex];

            foreach (var item in level)
            {
                ChangeTile(item.x, item.y, item.type);
            }
        }

        void Update()
        {
            if (inYouWinAnim)
            {
                if (!youWinAnimator.GetCurrentAnimatorStateInfo(0).IsName("Exit"))
                {
                    youWinAnimator.SetBool("PlayAnimation", false);
                    inYouWinAnim = false;

                    if(levelIndex + 1 < levels.levels.Count)
                    {
                        levelIndex++;
                        LoadLevel(levelIndex);
                    }
                    else
                    {
                        levelIndex = 0;
                        LoadLevel(levelIndex);
                        inMenu = true;
                        cameraAnimator.SetBool("SceneToMenu", true);
                        cameraAnimator.SetBool("MenuToScene", false);
                    }
                    return;
                }
            }


            if (inYouLoseAnim)
            {
                if (!youLoseAnimator.GetCurrentAnimatorStateInfo(0).IsName("Exit"))
                {
                    youLoseAnimator.SetBool("PlayAnimation", false);
                    inYouLoseAnim = false;

                    LoadLevel(levelIndex);
                    return;
                }
            }

            if (inMenu)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    inMenu = false;
                    cameraAnimator.SetBool("MenuToScene", true);
                    cameraAnimator.SetBool("SceneToMenu", false);
                }
                return;
            }

            if (onAnimation)
            {
                HandlePlayerAnimation();
                return;
            }

            if (playersTurn)
            {
                ShowCursors();
                HandlePlayersTurn();
            }
            else
            {
                HideCursors();
                HandleComputersTurn();
            }
        }

        private List<TileController> FindEnemies()
        {
            List<TileController> enemyTiles = new List<TileController>();

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    if (Helper.GetEnemyTileTypes().Contains(map[x, y].tileType))
                        enemyTiles.Add(map[x, y]);
                }
            }
            return enemyTiles;
        }


        private void HandlePlayerAnimation()
        {
            var playerObject = selectedTile.objectOnTile;
            playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, targetTile.transform.position, Time.deltaTime * 5);

            if (Vector3.Distance(playerObject.transform.position, targetTile.transform.position) < 0.01)
            {
                beep.Play();
                if (selectedTile.tileType == TileType.Player && targetTile.tileType == TileType.Coin)
                {
                    youWinAnimator.SetBool("PlayAnimation", true);
                    inMenu = true;
                    onAnimation = false;
                    inYouWinAnim = true;
                    ClearLevel();
                    return;
                }
                if(Helper.GetEnemyTileTypes().Contains(selectedTile.tileType) && targetTile.tileType == TileType.Player)
                {
                    youLoseAnimator.SetBool("PlayAnimation", true);
                    inMenu = true;
                    onAnimation = false;
                    inYouLoseAnim = true;
                    ClearLevel();
                    return;
                }

                targetTile.objectOnTile = selectedTile.objectOnTile;
                targetTile.tileType = selectedTile.tileType;

                selectedTile.tileType = TileType.Empty;
                selectedTile = null;
                onAnimation = false;


                playersTurn = !playersTurn;
            }
        }

        private bool CheckIfPlayerCanReach(int x, int y)
        {
            if (x >= 0 && x < 5 && y >= 0 && y < 6)
            {
                if (map[x, y].tileType == TileType.Empty || map[x, y].tileType == TileType.Coin)
                {

                    TileController playerTile = FindTileByType(TileType.Player);
                    if (playerTile == null)
                        return false;

                    if (Math.Abs(playerTile.boardPosition.x - x) <= 1 && Math.Abs(playerTile.boardPosition.y - y) <= 1)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        private TileType GetTileTypeByPos(int x, int y)
        {
            if (x >= 0 && x < 5 && y >= 0 && y < 6)
            {
                return map[x, y].tileType;
            }
            else
                return TileType.Invalid;
        }

        private TileController FindTileByType(TileType tileType)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    if (map[x, y].tileType == tileType)
                        return map[x, y];
                }
            }
            return null;
        }

        private void ClearLevel()
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    ChangeTile(x, y, TileType.Empty);
                }
            }
        }

        private void ChangeTile(int x, int y, TileType tileType)
        {
            var tile = map[x, y];
            if(tile.objectOnTile != null)
            {
                Destroy(tile.objectOnTile);
                tile.tileType = TileType.Empty;
            }

            GameObject objectOnTile = GetPrefabByTileType(tileType);
            if (objectOnTile != null)
            {
                objectOnTile = Instantiate(objectOnTile, tile.transform.position, Quaternion.identity);
                map[x, y].objectOnTile = objectOnTile;
                map[x, y].tileType = tileType;
            }
        }

        private void CreateTile(int x, int y, TileType tileType)
        {
            float objPosX = x * 1.2f;
            float objPosY = y * 1.2f;

            GameObject tile = Instantiate(tilePrefab, new Vector3(objPosX, 0, objPosY), Quaternion.identity);
            tile.transform.parent = gameObject.transform;
            map[x, y] = tile.GetComponent<TileController>();
            map[x, y].boardPosition.x = x;
            map[x, y].boardPosition.y = y;
            map[x, y].tileName = FindTileName(x, y);
            tile.name = FindTileName(x, y);
            map[x, y].tileType = tileType;
            var cursorObject = Instantiate(cursorPrefab, new Vector3(objPosX, 0, objPosY), Quaternion.identity);
            cursorObject.transform.parent = tile.transform;
            map[x, y].cursorMaterial = cursorObject.GetComponentInChildren<Renderer>().material;

            GameObject objectOnTile = GetPrefabByTileType(tileType);
            if (objectOnTile != null)
            {
                objectOnTile = Instantiate(objectOnTile, new Vector3(objPosX, 0, objPosY), Quaternion.identity);
                map[x, y].objectOnTile = objectOnTile;
            }
        }

        private GameObject GetPrefabByTileType(TileType tileType)
        {
            if (tileType == TileType.Player) return playerPrefab;
            else if (tileType == TileType.Pawn) return pawnPrefab;
            else if (tileType == TileType.Rook) return rookPrefab;
            else if (tileType == TileType.Bishop) return bishopPrefab;
            else if (tileType == TileType.Coin) return coinPrefab;
            else if (tileType == TileType.Knight) return knightPrefab;
            else if (tileType == TileType.Queen) return queenPrefab;
            return null;
        }

        private string FindTileName(int x, int y)
        {
            char first = (char)((int)'A' + x);
            char second = (char)((int)'1' + y);
            char[] chars = { first, second };
            return new string(chars);
        }


        private void HandlePlayersTurn()
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.transform.tag == "Selectable")
            {
                var hoveredTile = hitInfo.transform.GetComponent<TileController>();

                if (CheckIfPlayerCanReach(hoveredTile.boardPosition.x, hoveredTile.boardPosition.y))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var player = FindTileByType(TileType.Player);
                        StartAnimation(player, hoveredTile.boardPosition.x, hoveredTile.boardPosition.y);
                    }
                }
            }
        }

        private void ShowCursors()
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {

                    if (CheckIfPlayerCanReach(x, y))
                    {
                        map[x, y].gameObject.SetActive(true);
                        map[x, y].cursorMaterial.SetColor("_Color", Color.green);
                    }
                    else
                    {
                        map[x, y].gameObject.SetActive(false);
                        map[x, y].cursorMaterial.SetColor("_Color", Color.red);
                    }
                }
            }

        }

        private void HideCursors()
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    map[x, y].gameObject.SetActive(false);
                }
            }

        }

        private void HandleComputersTurn()
        {
            var enemies = FindEnemies();
            enemies = enemies.OrderByDescending(e => (int)e.tileType).ToList();

            foreach (var enemy in enemies)
            {
                // When enemy can strike player
                if (enemy.tileType == TileType.Pawn)
                {
                    HandlePawnStrike(enemy);
                }

                if (enemy.tileType == TileType.Rook)
                {
                    HandleRookStrike(enemy);
                }

                if (enemy.tileType == TileType.Bishop)
                {
                    HandleBishopStrike(enemy);
                }

                if (enemy.tileType == TileType.Queen)
                {
                    HandleQueenStrike(enemy);
                }

                if (enemy.tileType == TileType.Knight)
                {
                    HandleKnightStrike(enemy);
                }

                if (onAnimation == true)
                {
                    playersTurn = true;
                    return;
                }
            }

            foreach (var enemy in enemies)
            {
                // When enemy cannot strike but move
                if (enemy.tileType == TileType.Pawn)
                {
                    HandlePawnMove(enemy);
                }

                if (enemy.tileType == TileType.Rook)
                {
                    HandleRookMove(enemy);
                }

                if (enemy.tileType == TileType.Bishop)
                {
                    HandleBishopMove(enemy);
                }

                if (enemy.tileType == TileType.Knight)
                {
                    HandleKnightMove(enemy);
                }

                if (enemy.tileType == TileType.Queen)
                {
                    HandleQueenMove(enemy);
                }

                if (onAnimation == true)
                {
                    break;
                }
            }

            // ai didnt find any moves
            if (onAnimation != true)
            {
                playersTurn = true;
            }
        }

        private void HandleKnightMove(TileController enemy)
        {
            var player = FindTileByType(TileType.Player);

            List<Vector2Int> emptyTiles = new List<Vector2Int>();

            foreach (var move in knightMoves)
            {
                if (GetTileTypeByPos(enemy.boardPosition.x + move.x, enemy.boardPosition.y + move.y) == TileType.Empty)
                {
                    emptyTiles.Add(new Vector2Int(enemy.boardPosition.x + move.x, enemy.boardPosition.y + move.y));
                }
            }

            foreach (var firstMove in emptyTiles)
            {
                foreach (var secondMove in knightMoves)
                {
                    if((firstMove.x + secondMove.x == player.boardPosition.x) && (firstMove.y + secondMove.y == player.boardPosition.y))
                    {
                        StartAnimation(enemy, firstMove.x, firstMove.y);
                        return;
                    }
                }
            }
        }

        private void HandleKnightStrike(TileController enemy)
        {
            var player = FindTileByType(TileType.Player);

            foreach (var move in knightMoves)
            {
                if((enemy.boardPosition.x + move.x == player.boardPosition.x) && (enemy.boardPosition.y + move.y == player.boardPosition.y))
                {
                    StartAnimation(enemy, player.boardPosition.x, player.boardPosition.y);
                    return;
                }
            }
        }

        private void HandleQueenMove(TileController enemy)
        {
            HandleRookMove(enemy);
            HandleBishopMove(enemy);
        }

        private void HandleQueenStrike(TileController enemy)
        {
            HandleRookStrike(enemy);
            HandleBishopStrike(enemy);
        }

        private void HandleBishopMove(TileController enemy)
        {
            var player = FindTileByType(TileType.Player);
            var playersDiagons = GetTilesEmptyDiagons(player);
            var bishopsDiagons = GetTilesEmptyDiagons(enemy);

            var crossingDiagons = playersDiagons.Where(v => bishopsDiagons.Contains(v));

            if (crossingDiagons.Any())
            {
                var target = crossingDiagons.First();
                if (CheckIfDiagonalEmpty(target.x, target.y, enemy.boardPosition.x, enemy.boardPosition.y))
                {
                    StartAnimation(enemy, target.x, target.y);
                    return;
                }
            }
        }


        private List<Vector2Int> GetTilesEmptyDiagons(TileController tile)
        {
            List<Vector2Int> diagonalTiles = new List<Vector2Int>();

            int playerX = tile.boardPosition.x;
            int playerY = tile.boardPosition.y;

            int Up, Down, Left, Right;

            for (int i = 1; i < 5; i++)
            {
                Left = playerX - i;
                Right = playerX + i;
                Up = playerY + i;
                Down = playerY - i;

                if(GetTileTypeByPos(Left, Up) == TileType.Empty)
                {
                    diagonalTiles.Add(new Vector2Int(Left, Up));
                }

                if (GetTileTypeByPos(Left, Down) == TileType.Empty)
                {
                    diagonalTiles.Add(new Vector2Int(Left, Down));
                }

                if (GetTileTypeByPos(Right, Up) == TileType.Empty)
                {
                    diagonalTiles.Add(new Vector2Int(Right, Up));
                }

                if (GetTileTypeByPos(Right, Down) == TileType.Empty)
                {
                    diagonalTiles.Add(new Vector2Int(Right, Down));
                }
            }
            return diagonalTiles;
        }

        private void HandleBishopStrike(TileController enemy)
        {
            var player = FindTileByType(TileType.Player);
            
            // Player and bishop positioned in diagonal tiles
            if(Math.Abs(player.boardPosition.x - enemy.boardPosition.x) == Math.Abs(player.boardPosition.y - enemy.boardPosition.y))
            {
                if(CheckIfDiagonalEmpty(player.boardPosition.x, player.boardPosition.y, enemy.boardPosition.x, enemy.boardPosition.y))
                {
                    StartAnimation(enemy, player.boardPosition.x, player.boardPosition.y);
                    return;
                }
            }
        }

        private bool CheckIfDiagonalEmpty(int x1, int y1, int x2, int y2)
        {
            // 3x3  0x0 --> 1x1 2x2
            // 3x3  5x5 --> 4x4
            // 3x3  5x1 --> 4x2
            // 0x4  3x1 --> 1x3 2x2

            /*
              x <-3
               x
                x
                 x <-0
             */
            /*
                 x <-3
                x
               x
              x <-0
             */

            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int yStart = 0;
            int yEnd = 0;
            int step = 0;
            if (minX == x1)
            {
                yStart = y1;
                yEnd = y2;
            }
            else
            {
                yStart = y2;
                yEnd = y1;
            }
            if (yStart > yEnd)
                step = -1;
            else
                step = 1;

            int y = yStart;
            for (int i = 1; i < (maxX-minX); i++)
            {
                int x = minX + i;
                y += step;
                if (GetTileTypeByPos(x, y) != TileType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private void HandlePawnMove(TileController enemy)
        {
            int enemyX = enemy.boardPosition.x;
            int enemyY = enemy.boardPosition.y;
            if (GetTileTypeByPos(enemyX, enemyY - 1) == TileType.Empty)
            {
                StartAnimation(enemy, enemyX, enemyY - 1);
                return;
            }
        }

        private void HandleRookMove(TileController enemy)
        {
            int enemyX = enemy.boardPosition.x;
            int enemyY = enemy.boardPosition.y;
            var player = FindTileByType(TileType.Player);
            int playerX = player.boardPosition.x;
            int playerY = player.boardPosition.y;

            // Check if we can go to players column
            if (GetTileTypeByPos(playerX, enemyY) == TileType.Empty)
            {
                //Check if empty between
                if(CheckRowEmpty(enemyY, playerX, enemyX))
                {
                    StartAnimation(enemy, playerX, enemyY);
                    return;
                }
            }

            // Check if we can go to players row
            if (GetTileTypeByPos(enemyX, playerY) == TileType.Empty)
            {
                //Check if empty between
                if (CheckColumnEmpty(enemyX, playerY, enemyY))
                {
                    StartAnimation(enemy, enemyX, playerY);
                    return;
                }
            }
        }

        private void HandleRookStrike(TileController enemy)
        {
            int enemyX = enemy.boardPosition.x;
            int enemyY = enemy.boardPosition.y;
            var player = FindTileByType(TileType.Player);
            int playerX = player.boardPosition.x;
            int playerY = player.boardPosition.y;

            // Same row
            if (enemyY == playerY)
            {
                if (CheckRowEmpty(enemyY, playerX, enemyX))
                {
                    StartAnimation(enemy, playerX, playerY);
                    return;
                }
            }

            // Same column
            if (enemyX == playerX)
            {
                if (CheckColumnEmpty(enemyX, playerY, enemyY))
                {
                    StartAnimation(enemy, playerX, playerY);
                    return;
                }
            }
        }

        private void HandlePawnStrike(TileController enemy)
        {
            int enemyX = enemy.boardPosition.x;
            int enemyY = enemy.boardPosition.y;

            if (enemyY - 1 >= 0)
            {
                if ((GetTileTypeByPos(enemyX - 1, enemyY - 1) == TileType.Player))
                {
                    StartAnimation(enemy, enemyX - 1, enemyY - 1);
                    return;
                }

                if ((GetTileTypeByPos(enemyX + 1, enemyY - 1) == TileType.Player))
                {
                    StartAnimation(enemy, enemyX + 1, enemyY - 1);
                    return;
                }
            }
        }

        private bool CheckRowEmpty(int row, int firstPos, int secondPos)
        {
            int start = Math.Min(firstPos, secondPos);
            int end = Math.Max(firstPos, secondPos);

            bool betweenEmpty = true;
            for (int x = start + 1; x < end; x++)
            {
                if (GetTileTypeByPos(x, row) != TileType.Empty)
                {
                    betweenEmpty = false;
                }
            }

            return betweenEmpty;
        }

        private bool CheckColumnEmpty(int column, int firstPos, int secondPos)
        {
            int start = Math.Min(firstPos, secondPos);
            int end = Math.Max(firstPos, secondPos);

            bool betweenEmpty = true;
            for (int y = start + 1; y < end; y++)
            {
                if (GetTileTypeByPos(column, y) != TileType.Empty)
                {
                    betweenEmpty = false;
                }
            }

            return betweenEmpty;
        }

        private void StartAnimation(TileController tile, int targetX, int targetY)
        {
            selectedTile = tile;
            targetTile = map[targetX, targetY];
            onAnimation = true;
        }
    }
}