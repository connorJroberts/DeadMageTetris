using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public struct LevelData : ISerializationCallbackReceiver
{

    [Header("Shapes")]
    public List<ShapeData> Shapes;
    [Header("Score")]
    public int WinningScore;
    [Header("Speed")]
    [Tooltip("Blocks/Second")] public float SpeedMin;
    [Tooltip("Blocks/Second")] public float SpeedMax;
    [Tooltip("Blocks/Second")] public float SpeedStep;

    [SerializeField, HideInInspector] private List<string> _shapeFileStrings;

    public void FindShapeFiles()
    {
        /*
        * Here I was thinking about possible designer error, such as creating a shape text
        * file outside of the Shapes folder within resources; as such I decided to use Linq
        * to query for .txt assets within the project which contain "-block", to act as an
        * identifier for shape files. From my understanding, "text files" refer to .txt
        * specifically, otherwise I would have utilized a custom file type to search.
        */
        string currentDir = Application.dataPath;
        DirectoryInfo dir = new DirectoryInfo(currentDir);
        FileInfo[] fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

        string shapeIdentifier = "-block";

        IEnumerable<string> shapeFiles = from file in fileList
                                            where file.Extension == ".txt"
                                            let fileText = File.ReadAllText(file.FullName)
                                            where fileText.Contains(shapeIdentifier)
                                            select fileText;
        string[] shapeFilesArray = shapeFiles.ToArray();

        // Setup Shape Data
        for (int i = 0; i < shapeFilesArray.Length; i++)
        {
            string identifier = new StringReader(shapeFilesArray[i]).ReadLine();
            //Ensure user can't manipulate array, only the shape classes within
            if (Shapes.Count > shapeFiles.Count()) Shapes.RemoveRange(shapeFiles.Count(), Shapes.Count - shapeFiles.Count()); 
            if (_shapeFileStrings.Count > i && new StringReader(_shapeFileStrings[i]).ReadLine() == identifier && i < Shapes.Count)
            {
                Shapes[i] = new ShapeData
                {
                    ShapeName = identifier,
                    ShapeString = shapeFilesArray[i],
                    DropWeighting = Shapes[i].DropWeighting,
                    Enabled = Shapes[i].Enabled,
                };
            }
            Shapes.Add(new ShapeData
            {
                ShapeName = identifier,
                ShapeString = shapeFilesArray[i]
            });

        }

        _shapeFileStrings = shapeFiles.ToList();

        // There is a bug with this that displays a phantom shape at the end. Don't know why yet
        // but it doesn't affect the ability to use the shape list.

    }

    public void OnAfterDeserialize() => FindShapeFiles();

    public void OnBeforeSerialize()
    {

    }

}
