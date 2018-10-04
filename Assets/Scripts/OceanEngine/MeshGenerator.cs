using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[] heightMap, int pixelSize, float realSize)
    {
        int width = pixelSize;
        int height = pixelSize;

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        float sizePerPixel = realSize / (pixelSize - 1);

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

//        Debug.Log(string.Format("width = {0}, height = {1}", width, height));

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float unitHeight = heightMap[y * pixelSize + x];

                // meshData.vertices[vertexIndex] = new Vector3(
                //     (topLeftX + x) * sizePerPixel,
                //     unitHeight,
                //     (topLeftZ - y) * sizePerPixel
                // );

                meshData.vertices[vertexIndex] = new Vector3(
                     (topLeftX + x) * sizePerPixel,
                     unitHeight,
                     (topLeftZ - y) * sizePerPixel
                 );

                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                }
                vertexIndex++;
            }
           
        }

        return meshData;
    }

}

public struct MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];

        triangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
}

