using UnityEngine;
using UnityEditor;
using Watermelon.Data;

namespace Watermelon.Editor
{
    public static class FruitStageAssetGenerator
    {
        private static readonly (string name, int score, bool canSpawn, float weight, float diameter)[] StageDefinitions =
        {
            ("체리",     1,  true,  5f, 0.5f),
            ("딸기",     3,  true,  4f, 0.7f),
            ("포도",     6,  true,  3f, 0.9f),
            ("데코폰",  10,  true,  2f, 1.1f),
            ("감",      15,  true,  1f, 1.3f),
            ("사과",    21,  false, 0f, 1.6f),
            ("배",      28,  false, 0f, 1.9f),
            ("복숭아",  36,  false, 0f, 2.2f),
            ("파인애플",45,  false, 0f, 2.6f),
            ("멜론",    55,  false, 0f, 3.0f),
            ("수박",    66,  false, 0f, 3.5f),
        };

        private const string OutputPath = "Assets/ScriptableObjects/Fruits";

        [MenuItem("Watermelon/Generate Fruit Stage Assets")]
        public static void Generate()
        {
            var assets = new FruitStageData[StageDefinitions.Length];

            for (int i = 0; i < StageDefinitions.Length; i++)
            {
                var (fruitName, mergeScore, canSpawn, spawnWeight, diameter) = StageDefinitions[i];
                string assetPath = $"{OutputPath}/Stage_{i + 1:D2}_{fruitName}.asset";

                var existing = AssetDatabase.LoadAssetAtPath<FruitStageData>(assetPath);
                if (existing == null)
                {
                    existing = ScriptableObject.CreateInstance<FruitStageData>();
                    AssetDatabase.CreateAsset(existing, assetPath);
                }

                var so = new SerializedObject(existing);
                so.FindProperty("stageIndex").intValue = i + 1;
                so.FindProperty("fruitName").stringValue = fruitName;
                so.FindProperty("diameter").floatValue = diameter;
                so.FindProperty("mergeScore").intValue = mergeScore;
                so.FindProperty("canSpawn").boolValue = canSpawn;
                so.FindProperty("spawnWeight").floatValue = spawnWeight;
                so.ApplyModifiedPropertiesWithoutUndo();

                assets[i] = existing;
            }

            for (int i = 0; i < assets.Length - 1; i++)
            {
                var so = new SerializedObject(assets[i]);
                so.FindProperty("nextStage").objectReferenceValue = assets[i + 1];
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            var tablePath = $"{OutputPath}/FruitStageTable.asset";
            var table = AssetDatabase.LoadAssetAtPath<FruitStageTable>(tablePath);
            if (table == null)
            {
                table = ScriptableObject.CreateInstance<FruitStageTable>();
                AssetDatabase.CreateAsset(table, tablePath);
            }

            var tableSo = new SerializedObject(table);
            var stagesProp = tableSo.FindProperty("stages");
            stagesProp.arraySize = assets.Length;
            for (int i = 0; i < assets.Length; i++)
                stagesProp.GetArrayElementAtIndex(i).objectReferenceValue = assets[i];
            tableSo.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[FruitStageAssetGenerator] {assets.Length}개 FruitStageData + FruitStageTable 생성 완료");
        }
    }
}
