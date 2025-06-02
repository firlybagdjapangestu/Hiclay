using UnityEngine;

public class PrefabIdentifier : MonoBehaviour
{
    public GameObject prefab { get; private set; }

    public void SetPrefab(GameObject source)
    {
        prefab = source;
    }
}
