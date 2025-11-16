using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityInstancedRenderer : MonoBehaviour
{
    public Mesh[] Meshes;
    private Mesh[] _CorrectedMeshes;
    public Material material;

    public Vector2Int AreaSize = new Vector2Int(317, 317);


    public float Spacing = 10f;
    public float Variance = 3f;
    
    public Vector3 scale = Vector3.one;
    
    Matrix4x4[][] _batches;

    void OnEnable()
    {
        BuildBatches();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            BuildBatches();
    }

    public void FixZUpMesh()
    {
        if (_CorrectedMeshes == null || _CorrectedMeshes.Length != Meshes.Length)
            _CorrectedMeshes = new Mesh[Meshes.Length];

        for (int i = 0; i < Meshes.Length; i++)
        {
            var m = Instantiate(Meshes[i]);
            var v = m.vertices;
            for (int j = 0; j < v.Length; j++)
                v[j] = new Vector3(v[j].x, v[j].z, -v[j].y);
            m.vertices = v;
            m.RecalculateBounds();
            m.RecalculateNormals();
            _CorrectedMeshes[i] = m;
        }
    }
    
    void BuildBatches()
    {
        // Randome amounts of each
        int total = AreaSize.x * AreaSize.y;
        Debug.Log(total + " Objects being rendered live");

        // if any are 0, then all are 0. Skip
        if (total == 0)
        {
            _batches = null;
            return;
        }
        
        FixZUpMesh();

        List<List<Matrix4x4>> batchBuilder = new();
        for (int i = 0; i < Meshes.Length; i++) batchBuilder.Add(new List<Matrix4x4>());


        Vector3 offset = Vector3.zero - new Vector3(AreaSize.x * Spacing, 0, AreaSize.y * Spacing) * .5f;
        for (int x = 0; x < AreaSize.x; x++)
        for (int z = 0; z < AreaSize.y; z++)
        {
            int index = (int)(Random.value * (float)Meshes.Length);
            Vector2 variance = Random.insideUnitCircle * Variance;
            var pos = new Vector3(x * Spacing + variance.x, 0, z * Spacing + variance.y) + offset;
            var rot = Quaternion.Euler(x: 0, y: Random.value * 360f, z: 0);
            batchBuilder[index].Add(Matrix4x4.TRS(pos, rot, scale));
        }

        _batches = new Matrix4x4[Meshes.Length][];
        for (int i = 0; i < Meshes.Length; i++)
        {
            _batches[i] = batchBuilder[i].ToArray();
        }
    }

    void Update()
    {
        if (this._CorrectedMeshes == null
            || material == null
            || _batches == null)
            return;

        for (int i = 0; i < Meshes.Length; i++)
        {
            Graphics.DrawMeshInstanced(
                mesh: this._CorrectedMeshes[i], 
                submeshIndex: 0, 
                material: material, 
                matrices: _batches[i], 
                count: _batches[i].Length);
        }
    }
}