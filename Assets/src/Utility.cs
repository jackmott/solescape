using UnityEngine;
using System.IO;

public class Utility
{

    public static string[] GetAllFilesInFolder(string folder,string ext)
    {
        Debug.Log(Application.dataPath);
   
        string[]  files = Directory.GetFiles(Application.dataPath + "/Resources/" + folder, "*." + ext);
        
                
        for (int i = 0; i < files.Length;i++)
        {
            int dotIndex = files[i].IndexOf('.');            
            int slashIndex = files[i].IndexOf('\\');
            files[i] = files[i].Substring(slashIndex+1, dotIndex-slashIndex-1);            
        }

        return files;
    }
}
