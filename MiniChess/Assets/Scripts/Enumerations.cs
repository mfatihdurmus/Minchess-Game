using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Helper
    {
        public static readonly List<TileType> enemyTypes = new List<TileType>() {
            TileType.Pawn, 
            TileType.Bishop, 
            TileType.Queen, 
            TileType.Knight, 
            TileType.Rook
        };

        public static List<TileType> GetEnemyTileTypes()
        {
            return enemyTypes;
        }
    }

    public enum TileType
    {
        Invalid,

        // neutral
        Empty,
        Player,
        Coin,

        // enemy
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen
    };

}
