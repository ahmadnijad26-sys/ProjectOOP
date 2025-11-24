// Enums/GameEnums.cs
namespace ProjectOOPGame_Fresh
{
    public enum TileType { Grass, Tree, Water, Stone, Chest, Exit }
    public enum EntityState { Idle, Walking, Attacking, Dead }
    public enum EnemyType { Snake, Spider }
    public enum GameState { Playing, GameOver, Cutscene }
    public enum EnemyAIState { Idle, Wander, Chase, Attack, Flee }
}