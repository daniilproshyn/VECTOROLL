using UnityEditor;
using UnityEngine;
using System.IO;

public static class ProjectFolderSetup
{
    // Adds a button to Unity's top menu: Tools > Create Project Folders
    [MenuItem("Tools/Create Project Folders")]
    public static void CreateFolders()
    {
        // All the folders we want to create
        string[] folders = new string[]
        {
            "_Project",
            "_Project/Animations",
            "_Project/Audio/Music",
            "_Project/Audio/SFX",
            "_Project/Materials",
            "_Project/Models",
            "_Project/Prefabs/Environment",
            "_Project/Prefabs/Player",
            "_Project/Prefabs/UI",
            "_Project/Scenes",
            "_Project/ScriptableObjects",
            "_Project/Scripts/Core",
            "_Project/Scripts/Environment",
            "_Project/Scripts/Player",
            "_Project/Scripts/UI",
            "_Project/Scripts/Variables"
        };

        foreach (string folder in folders)
        {
            string path = Path.Combine(Application.dataPath, folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        // Refresh the Project window so the new folders show up right away
        AssetDatabase.Refresh();
        Debug.Log("All project folders created!");
    }
}