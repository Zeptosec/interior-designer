using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class InOut
{
    public static void Save(string filePath, Transform objectWithData)
    {
        List<SaveData> data = new List<SaveData>();
        for (int i = 0; i < objectWithData.childCount; i++)
        {
            var o = objectWithData.GetChild(i);
            SaveData sd = new SaveData()
            {
                ObjectName = o.name.Replace("(Clone)", ""),
                PosX = o.position.x,
                PosY = o.position.y,
                PosZ = o.position.z,
                RotX = o.eulerAngles.x,
                RotY = o.eulerAngles.y,
                RotZ = o.eulerAngles.z,
                R = o.GetChild(0).GetComponent<MeshRenderer>().material.color.r,
                G = o.GetChild(0).GetComponent<MeshRenderer>().material.color.g,
                B = o.GetChild(0).GetComponent<MeshRenderer>().material.color.b,
                A = o.GetChild(0).GetComponent<MeshRenderer>().material.color.a,
            };
            data.Add(sd);
        }
        if (data.Count == 0)
            throw new InvalidDataException("There was no data to save.");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("saved");
    }

    public static List<string> GetAllFileNames(string dirPath)
    {
        List<string> names = new List<string>();
        foreach(var item in Directory.EnumerateFiles(dirPath))
        {
            int lastIndex = item.LastIndexOf('/');
            names.Add(item.Substring(lastIndex + 1, item.Length - lastIndex - 7));
        }
        return names;
    }

    public static List<SaveData> Load(string file)
    {
        if (!File.Exists(file))
            throw new FileNotFoundException($"File {file} was not found");
        BinaryFormatter bf = new BinaryFormatter();
        using(FileStream fs = File.OpenRead(file))
        {
            List<SaveData> data = (List<SaveData>)bf.Deserialize(fs);
            return data;
        }
    }
}

[Serializable]
public class SaveData
{
    public string ObjectName;
    public float PosX;
    public float PosY;
    public float PosZ;
    public float RotX;
    public float RotY;
    public float RotZ;
    public float R;
    public float G;
    public float B;
    public float A;
}
