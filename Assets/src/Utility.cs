using UnityEngine;
using System.IO;

public class Utility
{

    static string[] skyboxes = { "Borgish Expanse", "Electric Nebula", "Glowing Dust" };
    static string[] planetNormals = { "Cracked", "Dark Dunes", "Drifting Continents", "Extreme", "Frozen Rock", "Gas Giant", "Hilly", "Hurricanes", "Lava Crust", "Lava Valleys", "Lush Green", "Mountains", "Mud Ice And Water", "Rocky", "Sand" };

    public static string[] GetSkyboxes()
    {
        return skyboxes;
    }

    public static bool IsWeb()
    {
        return Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer;
    }

    public static string[] GetPlanetNormals()
    {
        return planetNormals;
    }

    public static bool FileExists(string folder, string name, string ext)
    {
        return File.Exists(ComputePath(folder, name, ext));
    }

    private static string ComputePath(string folder, string name, string ext)
    {
        DirectoryInfo rootInfo = Directory.GetParent(Application.dataPath);
        return rootInfo.FullName + "/" + folder + "/" + name + "." + ext;
    }

    private static string ComputePath(string folder)
    {
        DirectoryInfo rootInfo = Directory.GetParent(Application.dataPath);
        return rootInfo.FullName + "/" + folder;
    }

    private static void CheckAndMakeDirectory(string folder)
    {
         DirectoryInfo rootInfo = Directory.GetParent(Application.dataPath);
        if (!Directory.Exists(rootInfo.FullName + "/" + folder))
        {
            Directory.CreateDirectory(rootInfo.FullName + "/" + folder);
        }
    }


    public static StreamWriter GetConfigFileWriter(string folder, string name, string ext)
    {
        CheckAndMakeDirectory(folder);
        return new StreamWriter(ComputePath(folder,name,ext), false);
    }

    public static StreamReader GetConfigFileReader(string folder, string name, string ext)
    {
        if (Application.platform == RuntimePlatform.OSXWebPlayer
        || Application.platform == RuntimePlatform.WindowsWebPlayer)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(folder + "/" + name);
            Stream planetStream = GenerateStreamFromString(textAsset.text);
            return new StreamReader(planetStream);
        }
        else
        {
            
            CheckAndMakeDirectory(folder);
            string path = ComputePath(folder, name, ext);
            if (!File.Exists(path))
            {
                TextAsset textAsset = Resources.Load<TextAsset>(folder + "/" + name);
                Stream planetStream = GenerateStreamFromString(textAsset.text);
                StreamReader tempReader = new StreamReader(planetStream);               
                StreamWriter writer = new StreamWriter(path, false);
                writer.Write(tempReader.ReadToEnd());
                writer.Close();
            }

            return new StreamReader(path);
        }
        
    }

    public static string[] GetAllFilesInFolder(string folder, string ext)
    {
        string path = ComputePath(folder);
        Debug.Log(path);
        string[] files = Directory.GetFiles(path,"*."+ext);

        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i]);
            int index = files[i].IndexOf('.');
            files[i] = files[i].Substring(0, index);
            index = files[i].LastIndexOf('\\') + 1;
            files[i] = files[i].Substring(index);
        }
        return files;
    }

    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
