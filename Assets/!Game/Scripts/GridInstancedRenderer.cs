using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GridInstancedRenderer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    public bool Plus10k = false;
    public Vector2Int BatchSize = Vector2Int.one * 50;
    public Vector2Int GridSize = Vector2Int.one * 40;
    
    
    public float spacing = 1f;
    public Vector3 origin = Vector3.zero;
    public Vector3 scale = Vector3.one;
    public Vector3 eulerRotation = Vector3.zero;
    [Range(0f,2f)]
    public float Frequency = 1f;
    [Range(5f,7f)]
    public float WaveLength = 1f;

    [Range(0f, 3f)] 
    public float Amplitude = 1f;
    
    
    Matrix4x4[][] _batches;
    bool _initialized;

    void Start()
    {
        _initialized = true;
        BuildBatches();
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
            var total = BatchSize.x * BatchSize.y * GridSize.x * BatchSize.y;
            var target = total + 10000;
            
            if (total == 0)
            {
                BatchSize = new Vector2Int(10, 10);
                GridSize = new Vector2Int(10, 10);  
            }
            else
            {
                var factor = Mathf.Sqrt(Mathf.Sqrt(target / (float)total));
                BatchSize = new Vector2Int(
                    Mathf.CeilToInt(BatchSize.x * factor),
                    Mathf.CeilToInt(BatchSize.y * factor)); 
                GridSize = new Vector2Int(
                    Mathf.CeilToInt(GridSize.x * factor),
                    Mathf.CeilToInt(GridSize.y * factor)); 
            }
        }
        
        if (Application.isPlaying)
            BuildBatches();
    }
    
    void BuildBatches()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        // if any are 0, then all are 0. Skip
        if (GridSize.x * GridSize.y * BatchSize.x * BatchSize.y == 0)
        {
            _batches = null;
            return;
        }
        
        Debug.Log(GridSize.x * GridSize.y * BatchSize.x * BatchSize.y + " Cubes");
        
        var rotation = Quaternion.Euler(eulerRotation);
        Vector3 offset = Vector3.zero - new Vector3(GridSize.x * BatchSize.x * spacing, 0, GridSize.y * BatchSize.y * spacing) * .5f;
        _batches = new Matrix4x4[GridSize.x * GridSize.y][];
        for (int gridX = 0; gridX < GridSize.x; gridX++)
        {
            for (int gridY = 0; gridY < GridSize.y; gridY++)
            {
                var batch = new Matrix4x4[BatchSize.x * BatchSize.y];
                
                for (int batchX = 0; batchX < BatchSize.x; batchX++)
                {
                    for (int batchY = 0; batchY < BatchSize.y; batchY++)
                    {
                        int batchIndex =  batchX * BatchSize.y + batchY;
                        Vector3 pos = origin 
                                      + offset 
                                      + new Vector3(
                                          (batchX + (gridX * BatchSize.x)) * spacing, 
                                          0f, 
                                          (batchY + (gridY * BatchSize.y) ) * spacing);
                        batch[batchIndex] = Matrix4x4.TRS(pos, rotation, scale);
                    }
                }
                
                int gridIndex =  gridX * GridSize.y + gridY;
                _batches[gridIndex] = batch;
            }
        }
        
        Debug.Log("Created " + (GridSize.x * GridSize.y * BatchSize.x * BatchSize.y) + " prefabs in " + sw.ElapsedMilliseconds + " ms");
    }

    void Update()
    {
        if (mesh == null || material == null || _batches == null)
            return;

        float time = Time.time * Frequency;
        for (int gridIndex = 0; gridIndex < GridSize.x * GridSize.y; gridIndex++)
        {
            var batch = _batches[gridIndex];

            for (int batchIndex = 0; batchIndex < batch.Length; batchIndex++)
            {
                // Position in R3, in C0=X, C1=Y, C2=Z
                var matrix = batch[batchIndex];
                matrix.m13 = Amplitude * Mathf.Sin(WaveLength * (time + matrix.m03)) * .5f + Mathf.Sin(WaveLength * (time + matrix.m23)) * .5f;
                batch[batchIndex] = matrix;
            }
            
            Graphics.DrawMeshInstanced(
                mesh: mesh, 
                submeshIndex: 0, 
                material: material, 
                matrices: batch, 
                count: batch.Length);
        }
    }
}