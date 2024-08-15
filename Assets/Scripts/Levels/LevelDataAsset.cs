using UnityEngine;

[CreateAssetMenu]
public class LevelDataAsset : ScriptableObject
{
    [SerializeField, Tooltip("Change these values to tune each level")] public LevelData LevelData;
}
