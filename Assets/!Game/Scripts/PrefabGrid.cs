using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabGrid : MonoBehaviour
{

    public Vector2Int AreaSize = new Vector2Int(317, 317);
    public bool Plus1k = false;
    
    public float Spacing = 10f;
    public GameObject Prefab;
    bool _initialized;
    
    void Start()
    {
        _initialized = true;
        BuildBatch();
    }

    void OnValidate()
    {
        if (!_initialized)
        {
            _initialized = true;
            return; // don't run on start.
        }
        
        if (Plus1k)
        {
            Plus1k = false;
            var total = AreaSize.x * AreaSize.y;
            var target = total + 1000;
            var factor = Mathf.Sqrt(target / (float)total);
            if (AreaSize.x * AreaSize.y == 0)
            {
                AreaSize = new Vector2Int(100, 100);
            }
            else
            {
                AreaSize = new Vector2Int(
                Mathf.CeilToInt(AreaSize.x * factor),
                Mathf.CeilToInt(AreaSize.y * factor) );
            }
        }
        
        if (Application.isPlaying)
            BuildBatch();
    }
    
    void BuildBatch()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        int total = AreaSize.x * AreaSize.y;

        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        
        Vector3 offset = Vector3.zero - new Vector3(AreaSize.x * Spacing, 0, AreaSize.y * Spacing) * .5f;
        for (int x = 0; x < AreaSize.x; x++)
        for (int z = 0; z < AreaSize.y; z++)
        {
            var pos = new Vector3(x * Spacing, 0, z * Spacing) + offset;
            Instantiate(Prefab, pos, Quaternion.identity, transform);
        }
        
        Debug.Log("Created " + total + " prefabs in " + sw.ElapsedMilliseconds + " ms");
    }
}