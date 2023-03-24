using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Match3Creator
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Match-3 Creator/Create New Level Data", order = 1)]
    [System.Serializable]
    public class LevelData : ScriptableObject
    {
        [HideInInspector][Range(0, 20)] public int width = 5, height = 5;
        [HideInInspector] public float offset = .25f, tileSize = 1f;

        [HideInInspector] public List<GameObject> objects = new List<GameObject>();
        [HideInInspector] public List<TileData> tileData = new List<TileData>();
        [HideInInspector] public List<List<int>> tiles = new List<List<int>>();

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject as LevelData != null)
            {
                EditorApplication.ExecuteMenuItem("Window/Match-3 Creator");
                return true;
            }
            else
                return false;
        }
    }

    [System.Serializable]
    public class TileData
    {
        public List<int> tileData = new List<int>();
    }
}