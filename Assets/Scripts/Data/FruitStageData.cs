using UnityEngine;

namespace Watermelon.Data
{
    [CreateAssetMenu(fileName = "FruitStageData", menuName = "Watermelon/Fruit Stage Data")]
    public class FruitStageData : ScriptableObject
    {
        [SerializeField] private int stageIndex;
        [SerializeField] private string fruitName;
        [SerializeField] private float diameter;
        [SerializeField] private int mergeScore;
        [SerializeField] private bool canSpawn;
        [SerializeField] private float spawnWeight;
        [SerializeField] private FruitStageData nextStage;

        public int StageIndex => stageIndex;
        public string FruitName => fruitName;
        public float Diameter => diameter;
        public int MergeScore => mergeScore;
        public bool CanSpawn => canSpawn;
        public float SpawnWeight => spawnWeight;
        public FruitStageData NextStage => nextStage;
    }
}
