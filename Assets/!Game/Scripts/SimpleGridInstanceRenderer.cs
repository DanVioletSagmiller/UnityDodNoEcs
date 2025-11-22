using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleInstancedRenderer : MonoBehaviour
{
    public Mesh Mesh;
    public Material material;
    
    public Vector2Int AreaSize = new Vector2Int(317, 317);
    public bool Plus10k = false;

    public float Spacing = 10f;
    
    public Vector3 scale = Vector3.one;
    
    Matrix4x4[] _batch;
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
        
        if (Plus10k)
        {
            Plus10k = false;
            var total = AreaSize.x * AreaSize.y;
            var target = total + 10000;
            var factor = Mathf.Sqrt(target / (float)total);
            AreaSize = new Vector2Int(
                Mathf.CeilToInt(AreaSize.x * factor),
                Mathf.CeilToInt(AreaSize.y * factor)
            );
        }
        
        if (Application.isPlaying)
            BuildBatch();
    }
    
    void BuildBatch()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        int total = AreaSize.x * AreaSize.y;
        
        _batch = new Matrix4x4[total];
        int batchIndex = 0;
        Vector3 offset = Vector3.zero - new Vector3(AreaSize.x * Spacing, 0, AreaSize.y * Spacing) * .5f;
        for (int x = 0; x < AreaSize.x; x++)
        for (int z = 0; z < AreaSize.y; z++)
        {
            var pos = new Vector3(x * Spacing, 0, z * Spacing) + offset;
            var rot = Quaternion.Euler(x: 0, y: Random.value * 360f, z: 0);
            _batch[batchIndex++] = Matrix4x4.TRS(pos, rot, scale);
        }
        
        Debug.Log("Created " + total + " prefabs in " + sw.ElapsedMilliseconds + " ms");
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(
            mesh: this.Mesh, 
            submeshIndex: 0, 
            material: material, 
            matrices: _batch, 
            count: _batch.Length);
    }
}