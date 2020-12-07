using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Deformation : MonoBehaviour
{
    public bool playing = true;
    public ComputeShader cs;
    public Material darkMat;
    public Material lightMat;

    private ComputeBuffer buffer;
    private VertexData[] output;
    private VertexData[] data;
    private float ts;

    private Mesh mesh;
    // runtime vertices
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    struct VertexData
    {
        public Vector3 pos;
        public float time;
    };

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        triangles = mesh.triangles;
        vertices = mesh.vertices;
        uvs = mesh.uv;

        buffer = new ComputeBuffer(vertices.Length, sizeof(float) * 4);
        data = new VertexData[vertices.Length];
        output = new VertexData[vertices.Length];

        for (int i = 0; i < data.Length; ++i)
        {
            data[i].pos = vertices[i];
            data[i].time = ts;
        }
    }

    private void Update()
    {
        if (cs == null) return;

        if (playing)
        {
            GetComponent<MeshRenderer>().material = lightMat;

            // update anim time
            ts += Time.deltaTime;
            for (int i = 0; i < data.Length; ++i)
            {
                data[i].time = ts;
            }
            // send to shader
            buffer.SetData(data);
            int kernel = cs.FindKernel("deform");
            cs.SetBuffer(kernel, "dataBuffer", buffer);
            cs.Dispatch(kernel, data.Length / 8, 8, 1);

            buffer.GetData(output);
            for (int i = 0; i < output.Length; ++i)
            {
                vertices[i] = output[i].pos;
            }
            UpdateMesh();
        }
        else
        {
            GetComponent<MeshRenderer>().material = darkMat;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
