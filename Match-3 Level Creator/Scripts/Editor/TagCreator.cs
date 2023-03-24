using UnityEditor;
using UnityEngine;


namespace Match3Creator
{
    public class TagCreator
    {
        static int maxTagCount = 10000;

        public static bool AddTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            if (tagsProp.arraySize >= maxTagCount)
            {
                Debug.LogWarning("Max count of tags has been reached! Please remove some, otherwise, some features might not work as intended.");
                return false;
            }

            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;

                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);

                sp.stringValue = tagName;
                tagManager.ApplyModifiedProperties();

                return true;
            }

            return false;
        }

        public static bool TagExists(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            return PropertyExists(tagsProp, 0, maxTagCount, tagName);
        }
       
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = start; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}