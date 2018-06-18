using UnityEngine;
using System.Collections;

//Controlls the water
public class WaterController : MonoBehaviour
{
    public static WaterController current;

    public bool isMoving;

    //Wave height and speed
    public float scale = 0.1f;
    public float speed = 1.0f;
    //The width between the waves
    public float waveDistance = 1f;
    //Noise parameters
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    Mesh waterMesh;

    void Start()
    {
        current = this;
        waterMesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        ModifyMesh();
    }

    public static float SinXWave(
        Vector3 position,
        float speed,
        float scale,
        float waveDistance,
        float noiseStrength,
        float noiseWalk,
        float timeSinceStart)
    {
        float x = position.x;
        float y = 0f;
        float z = position.z;

        //Using only x or z will produce straight waves
        //Using only y will produce an up/down movement
        //x + y + z rolling waves
        //x * z produces a moving sea without rolling waves

        float waveType = z;

        y += Mathf.Sin((timeSinceStart * speed + waveType) / waveDistance) * scale;

        //Add noise to make it more realistic
        y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
    }

    //Get the y coordinate from whatever wavetype we are using
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        if (isMoving)
        {
        return SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
        }
        else
        {
        return 0f;
        }

    }

    //Find the distance from a vertice to water
    //Make sure the position is in global coordinates
    //Positive if above water
    //Negative if below water
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }

    void ModifyMesh()
    {
        Vector3[] vertices = waterMesh.vertices;
        for(int i = 0; i < vertices.Length; ++i){
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            float newY = GetWaveYPos(worldPos, Time.time);
            worldPos.y = newY;
            vertices[i] = transform.InverseTransformPoint(worldPos);
        }

        waterMesh.vertices = vertices;
        waterMesh.RecalculateBounds();
    }
}