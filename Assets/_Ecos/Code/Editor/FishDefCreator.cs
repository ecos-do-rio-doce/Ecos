using Ecos;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FishDefCreator
{
    private const string TargetFolder = "Assets/_Ecos/Game/Data/Fishes/";

    [MenuItem("Tools/Fish/Create Missing FishDefs From Selection")]
    private static void CreateMissingFishDefsFromSelection()
    {
        if (!AssetDatabase.IsValidFolder(TargetFolder))
        {
            CreateFolderIfNeeded(TargetFolder);
        }

        foreach (Object selected in Selection.objects)
        {
            if (selected == null)
                continue;

            string assetName = SanitizeFileName(selected.name);
            string assetPath = $"{TargetFolder}/{assetName}.asset";

            // Skip if already exists
            if (AssetDatabase.LoadAssetAtPath<FishDef>(assetPath) != null)
            {
                Debug.Log($"FishDef already exists: {assetPath}");
                continue;
            }

            FishDef fishDef = ScriptableObject.CreateInstance<FishDef>();
            fishDef.name = assetName;

            if (selected is Texture2D texture)
            {
                Sprite sprite = GetSpriteFromTexture(texture);
                if (sprite != null)
                {
                    fishDef.FishSprite = sprite;
                }
                else
                {
                    Debug.LogWarning(
                        $"Texture '{texture.name}' does not have an associated Sprite sub-asset. " +
                        $"Make sure its Import Settings are set to Sprite (2D and UI).");
                }
            }


            AssetDatabase.CreateAsset(fishDef, assetPath);
            Debug.Log($"Created FishDef: {assetPath}", fishDef);
            EditorGUIUtility.PingObject(fishDef);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Sprite GetSpriteFromTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(path))
            return null;

        Object[] assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (Object obj in assetsAtPath)
        {
            if (obj is Sprite sprite)
            {
                // Prefer the sprite whose name matches the texture name.
                if (sprite.name == texture.name)
                    return sprite;
            }
        }

        // Fallback: return the first sprite found at that path.
        foreach (Object obj in assetsAtPath)
        {
            if (obj is Sprite sprite)
                return sprite;
        }

        return null;
    }


    private static void CreateFolderIfNeeded(string path)
    {
        string[] parts = path.Split('/');
        if (parts.Length < 2 || parts[0] != "Assets")
        {
            Debug.LogError("TargetFolder must be inside Assets/.");
            return;
        }

        string currentPath = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = $"{currentPath}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }
            currentPath = nextPath;
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName.Trim();
    }
}