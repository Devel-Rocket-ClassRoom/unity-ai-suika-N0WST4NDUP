using UnityEngine;

namespace Watermelon.Data
{
    [CreateAssetMenu(fileName = "FruitStageTable", menuName = "Watermelon/Fruit Stage Table")]
    public class FruitStageTable : ScriptableObject
    {
        [SerializeField] private FruitStageData[] stages;

        public int Count => stages.Length;

        public FruitStageData GetByIndex(int stageIndex)
        {
            int i = stageIndex - 1;
            if (i < 0 || i >= stages.Length) return null;
            return stages[i];
        }

        public FruitStageData[] GetSpawnables()
        {
            var result = System.Array.FindAll(stages, s => s != null && s.CanSpawn);
            return result;
        }
    }
}
