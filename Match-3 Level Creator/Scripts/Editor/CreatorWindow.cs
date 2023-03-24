using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Match3Creator
{
    public class CreatorWindow : EditorWindow
    {
        [Range(0, 10)] int width = 5, height = 5;
        float offset = .25f, tileSize = 1f;

        List<GameObject> objects = new List<GameObject>();
        List<TileData> tileData = new List<TileData>();

        LevelData levelData;
        string fileName, filePath;

        [HideInInspector] public static bool unsaved;
        static bool justOpened;

        [MenuItem("Window/Match-3 Creator")]
        static void Init()
        {
            justOpened = true;
            CreatorWindow window = (CreatorWindow)GetWindow(typeof(CreatorWindow), false, "Match-3 Creator");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("File", EditorStyles.boldLabel);

            LevelData currentData = levelData;

            levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), true);

            if ((levelData != null && currentData == null) || levelData != currentData)
            {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "Any unsaved change will de lost!", "Go Ahead", "Cancel"))
                {
                    width = levelData.width;
                    height = levelData.height;
                    objects = levelData.objects;
                    offset = levelData.offset;
                    tileSize = levelData.tileSize;

                    tileData.Clear();

                    for (int i = 0; i < levelData.tileData.Count; i++)
                    {
                        tileData.Add(new TileData());

                        for (int j = 0; j < levelData.tileData[i].tileData.Count; j++)
                            tileData[i].tileData.Add(levelData.tileData[i].tileData[j]);
                    }

                    fileName = levelData.name;
                    var fileLoc = AssetDatabase.GetAssetPath(levelData).Split(fileName);
                    filePath = fileLoc[0];
                }
                else if (currentData == null)
                    levelData = null;
                else
                    levelData = currentData;
            }

            fileName = EditorGUILayout.TextField("File Name", fileName);

            EditorGUILayout.BeginHorizontal();

            filePath = EditorGUILayout.TextField("File Path", filePath);
            if (GUILayout.Button("Browse"))
            {
                string path = EditorUtility.OpenFolderPanel("Choose file to save", "", "");
                var pathFiles = path.Split("Assets");
                string projectPath = "Assets/";

                for (int i = 1; i < pathFiles.Length; i++)
                    projectPath += pathFiles[i];

                filePath = projectPath;
            }

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);

            GUILayout.Label("Sizes", EditorStyles.boldLabel);

            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            offset = EditorGUILayout.FloatField("Offset", offset);
            tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);

            while (tileData.Count < height)
            {
                tileData.Add(new TileData());

                unsaved = true;
            }
            /*while (tileData.Count > height)
            {
                tileData.RemoveAt(tileData.Count - 1);
            }*/

            int c = 0;
            while (c < height)
            {
                while (tileData[c].tileData.Count < width)
                {
                    tileData[c].tileData.Add(0);
                    unsaved = true;
                }

                c++;
            }

            /*while (tileData[c].Count > width)
            {
                for (int i = 0; i < width; i++)
                    tileData[c].RemoveAt(tileData[c].Count - 1);

                c++;
            }     */

            GUILayout.Space(10);

            GUILayout.Label("Template", EditorStyles.boldLabel);

            for (int i = 0; i < height; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (GUILayout.Button(tileData[i].tileData[j].ToString()))
                        {
                            ++tileData[i].tileData[j];

                            if (tileData[i].tileData[j] > objects.Count)
                                tileData[i].tileData[j] = 0;

                            unsaved = true;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.Label("Tiles", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField("0 - Empty Tile", null, typeof(Object), true);

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == null)
                {
                    unsaved = true;
                    ObjectRemoved(i);
                    continue;
                }

                objects[i] = (GameObject)EditorGUILayout.ObjectField((i + 1).ToString(), objects[i], typeof(GameObject), true);
            }

            GameObject go = null;
            go = (GameObject)EditorGUILayout.ObjectField((objects.Count + 1).ToString(), go, typeof(GameObject), true);

            if (go != null)
            {
                objects.Add(go);
                unsaved = true;
            }

            GUILayout.Space(10);

            GUILayout.Label("Control", EditorStyles.boldLabel);

            if (GUILayout.Button("Reset Window"))
            {
                if (EditorUtility.DisplayDialog("All changes will be reset!",
                    "Please make sure to save your current work!", "Reset", "Cancel"))
                {
                    fileName = null;
                    filePath = null;

                    levelData = null;

                    width = 5;
                    height = 5;
                    offset = .25f;
                    tileSize = 1f;

                    objects = new List<GameObject>();
                    tileData = new List<TileData>();

                    unsaved = false;
                }
            }

            if (GUILayout.Button("Save"))
            {
                SaveScriptable();
            }

            GUILayout.Space(10);
            GUILayout.Label("Control", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear tiles on the scene"))
            {
                ClearScene();
            }

            if (GUILayout.Button("Place objects to the scene"))
            {
                PlaceObjects();
            }

            if (justOpened)
            {
                justOpened = false;
                unsaved = false;
            }
        }

        void PlaceObjects()
        {
            if (TagCreator.TagExists("TileHolder"))
            {
                GameObject oldHolder = GameObject.FindGameObjectWithTag("TileHolder");

                if (oldHolder != null)
                {
                    DestroyImmediate(oldHolder);
                }
            }

            GameObject holder = new GameObject("Tiles");

            TagCreator.AddTag("TileHolder");
            if (TagCreator.TagExists("TileHolder"))
                holder.tag = "TileHolder";

            Vector2 topLeft = new Vector2((-width / 2f) * tileSize - (offset * (width / 2)), (height / 2f) * tileSize + (offset * (height / 2)));

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (tileData[i].tileData[j] == 0)
                        continue;

                    Vector2 pos = topLeft;
                    pos.x += (j * tileSize) + (j * offset);
                    pos.y -= (i * tileSize) + (i * offset);

                    Instantiate(objects[tileData[i].tileData[j] - 1], pos, Quaternion.identity, holder.transform);
                }
            }

            Undo.RegisterCreatedObjectUndo(holder, "Tiles Created");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        void ClearScene()
        {
            if (TagCreator.TagExists("TileHolder"))
            {
                GameObject oldHolder = GameObject.FindGameObjectWithTag("TileHolder");

                if (oldHolder != null)
                {
                    Undo.DestroyObjectImmediate(oldHolder);

                    //DestroyImmediate(oldHolder);

                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

                    Debug.Log("Scene cleared!");
                }
                else
                    Debug.Log("No object was found to clear!");
            }
        }

        void SaveScriptable()
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath))
            {
                EditorUtility.DisplayDialog("Could not save", "File Name and File Path cannot be empty", "OK");
                return;
            }

            if (width <= 0 || height <= 0 || tileSize <= 0 || offset < 0)
            {
                EditorUtility.DisplayDialog("Could not save", "You have entered invalid value(s). Please make sure all numbers are positive.", "OK");
                return;
            }

            LevelData asset = ScriptableObject.CreateInstance<LevelData>();
            asset.width = width;
            asset.height = height;
            asset.objects = objects;
            asset.offset = offset;
            asset.tileSize = tileSize;

            asset.tileData = tileData;

            AssetDatabase.CreateAsset(asset, filePath + "/" + fileName + ".asset");
            AssetDatabase.SaveAssets();

            Debug.Log("Your level data is saved succesfully");

            levelData = asset;
            unsaved = false;

            EditorUtility.SetDirty(levelData);
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        void ObjectRemoved(int i)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (tileData[j].tileData[k] == i + 1)
                        tileData[j].tileData[k] = 0;
                    else if (tileData[j].tileData[k] > i + 1)
                        tileData[j].tileData[k]--;
                }
            }

            objects.RemoveAt(i);
        }

        void OnDestroy()
        {
            if (!unsaved)
                return;

            if (EditorUtility.DisplayDialog("Save?",
                    "Do you want to save before quit?", "Save", "No"))
            {
                SaveScriptable();
            }
        }
    }

    [InitializeOnLoad]
    public class EditorWantsToQuit
    {
        static bool WantsToQuit()
        {
            if (!CreatorWindow.unsaved)
                return true;

            if (EditorUtility.DisplayDialog("Friendly reminder",
                    "Please make sure to save your Match-3 level template before quitting", "I'm done", "Cancel"))
                return true;
            else
                return false;
        }

        static EditorWantsToQuit()
        {
            EditorApplication.wantsToQuit += WantsToQuit;
        }
    }
}