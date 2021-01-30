using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Levels
    {
        public List<List<LevelItem>> levels;
        public Levels()
        {
            List<LevelItem> level1 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 3, 5),
                new LevelItem(TileType.Pawn, 2, 4),
                new LevelItem(TileType.Pawn, 0, 4),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level2 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 2, 5),
                new LevelItem(TileType.Pawn, 2, 4),
                new LevelItem(TileType.Knight, 2, 3),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level3 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 0, 5),
                new LevelItem(TileType.Pawn, 2, 4),
                new LevelItem(TileType.Rook, 1, 4),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level4 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 1, 5),
                new LevelItem(TileType.Pawn, 2, 4),
                new LevelItem(TileType.Bishop, 1, 4),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level5 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 2, 5),
                new LevelItem(TileType.Bishop, 0, 4),
                new LevelItem(TileType.Rook, 1, 4),
                new LevelItem(TileType.Coin, 4, 5),
            };


            List<LevelItem> level6 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 3, 3),
                new LevelItem(TileType.Bishop, 0, 4),
                new LevelItem(TileType.Bishop, 2, 5),
                new LevelItem(TileType.Coin, 4, 5),
            };


            List<LevelItem> level7 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 3, 3),
                new LevelItem(TileType.Knight, 0, 5),
                new LevelItem(TileType.Knight, 2, 3),
                new LevelItem(TileType.Pawn, 2, 2),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level8 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Pawn, 3, 3),
                new LevelItem(TileType.Knight, 0, 5),
                new LevelItem(TileType.Knight, 2, 3),
                new LevelItem(TileType.Bishop, 0, 4),
                new LevelItem(TileType.Pawn, 2, 2),
                new LevelItem(TileType.Coin, 4, 5),
            };

            List<LevelItem> level9 = new List<LevelItem>()
            {
                new LevelItem(TileType.Player, 0, 0),
                new LevelItem(TileType.Queen, 0, 4),
                new LevelItem(TileType.Pawn, 0, 5),
                new LevelItem(TileType.Pawn, 3, 4),
                new LevelItem(TileType.Pawn, 1, 4),
                new LevelItem(TileType.Pawn, 1, 3),
                new LevelItem(TileType.Pawn, 1, 2),
                new LevelItem(TileType.Pawn, 0, 2),
                new LevelItem(TileType.Coin, 4, 5),
            };

            levels = new List<List<LevelItem>>();
            levels.Add(level1);
            levels.Add(level2);
            levels.Add(level3);
            levels.Add(level4);
            levels.Add(level5);
            levels.Add(level6);
            levels.Add(level7);
            levels.Add(level8);
            levels.Add(level9);
        }
    }

    public class LevelItem
    {
        public TileType type;
        public int x;
        public int y;

        public LevelItem( TileType type, int x, int y )
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }
    }
}
