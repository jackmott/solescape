using UnityEngine;
using System.IO;

public class Utility
{

    static string[] skyboxes = { "Borgish Expanse", "Cool Blue", "Electric Nebula", "Fire Nebula", "Glowing Dust" };
    static string[] planetNormals = { "Cracked", "DarkDunes", "Drifting Continents", "Extreme", "Frozen Rock", "Gas Giant", "Hilly", "Hurricanes", "Lava Crust", "Lava Valleys", "Lush Green", "Mountains", "Mud Ice And Water", "Rocky", "Sand" };

    public static string[] GetSkyboxes()
    {
        return skyboxes;
    }

    public static string[] GetPlanetNormals()
    {
        return planetNormals;
    }
    public static StreamReader GetConfigFileReader(string folder, string name, string ext)
    {
        DirectoryInfo rootInfo = Directory.GetParent(Application.dataPath);
        
        string path = rootInfo.FullName + "/" + folder + "/" + name + "." + ext;

        if (! File.Exists(path))
        {
            TextAsset textAsset = Resources.Load<TextAsset>(folder+"/"+name);
            Stream planetStream = GenerateStreamFromString(textAsset.text);
            StreamReader tempReader = new StreamReader(planetStream);
            if (!Directory.Exists(rootInfo.FullName + "/" + folder)) 
            {
                Directory.CreateDirectory(rootInfo.FullName + "/" + folder);
            }                     
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(tempReader.ReadToEnd());
            writer.Close();
        }

        return new StreamReader(path);
        
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
