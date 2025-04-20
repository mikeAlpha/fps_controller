using UnityEngine;
using UnityEngine.Rendering;

public class FoliageSpawner : MonoBehaviour
{
    public GameObject foliagePrefab;
    public ComputeShader computeShader;
    public int instanceCount = 10000;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private MaterialPropertyBlock propertyBlock;
    private BoxCollider spawnArea;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        if (spawnArea == null || foliagePrefab == null || computeShader == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }

        InitializeBuffers();
        GenerateInstances();
    }

    void InitializeBuffers()
    {
        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        Mesh mesh = foliagePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;

        args[0] = (uint)mesh.GetIndexCount(0);
        args[1] = (uint)instanceCount;
        args[2] = (uint)mesh.GetIndexStart(0);
        args[3] = (uint)mesh.GetBaseVertex(0);

        argsBuffer.SetData(args);
    }

    void GenerateInstances()
    {
        computeShader.SetInt("_InstanceCount", instanceCount);
        computeShader.SetVector("_BoxMin", spawnArea.bounds.min);
        computeShader.SetVector("_BoxMax", spawnArea.bounds.max);
        computeShader.SetBuffer(0, "_InstancePositions", positionBuffer);

        computeShader.Dispatch(0, Mathf.CeilToInt(instanceCount / 1024f), 1, 1);

        Material mat = foliagePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        mat.SetBuffer("_PositionBuffer", positionBuffer);
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetBuffer("_PositionBuffer", positionBuffer);
    }

    void Update()
    {
        if (foliagePrefab != null)
        {
            Mesh mesh = foliagePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            Material material = foliagePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;

            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, spawnArea.bounds, argsBuffer, 0, propertyBlock);
        }
    }

    void OnDestroy()
    {
        positionBuffer?.Release();
        argsBuffer?.Release();
    }
}
