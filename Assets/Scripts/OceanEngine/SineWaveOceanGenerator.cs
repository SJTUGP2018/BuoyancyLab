﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Unity Job System:
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

[CreateAssetMenu(fileName = "SineWaveOceanGenerator", 
                 menuName="Ocean Generator/Sine Wave")
]
public class SineWaveOceanGenerator : OceanGenerator {

    //Wave height and speed
    public float m_waveScale = 0.1f;
    public float m_waveSpeed = 1.0f;
    //The width between the waves
    public float m_waveDistance = 1f;
    //Noise parameters
    public float m_noiseStrength = 1f;
    [Range(0f, 1f)]
    public float m_noiseWalk = 1f;

    [Range(0f, 360f)]
    public float m_waveAngle = 0f;
    Vector2 m_waveDirection;
    public int m_waveLengthFactor = 10;
    float[] managedHeightMap;

    //public JobHandle gHeightHandle;

    // Use this for initialization
    protected override void OnEnable()
    {
		base.OnEnable();
        m_waveDirection = new Vector2(0f, 0f);
        managedHeightMap = new float[m_descriptor.pixelSize * m_descriptor.pixelSize];
    }


    public override void UpdateOcean(float _time)
    {
        Quaternion rot = Quaternion.AngleAxis(m_waveAngle, Vector3.forward);
        m_waveDirection = rot * Vector3.right;

        var gHeightMapJob = new GenerateHeightMapJob()
        {
            descriptor = m_descriptor,
            waveScale = m_waveScale,
            waveSpeed = m_waveSpeed,
            waveLengthFactor = m_waveLengthFactor,
            noiseStrength = m_noiseStrength,
            noiseWalk = m_noiseWalk,
            waveDirection = m_waveDirection,
            time = Time.time,
            heightMap = m_heightMap
        };

        heightMapHandle = gHeightMapJob.Schedule(m_descriptor.pixelSize * m_descriptor.pixelSize, 64);
    }


    public override void LateUpdateOcean(float _time)
    {
        heightMapHandle.Complete();
        m_heightMap.CopyTo(managedHeightMap);

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(managedHeightMap,
            m_descriptor.pixelSize, m_descriptor.realSize);

        oceanMesh = meshData.CreateMesh();
    }

    public override void DestroyOcean()
    {
        // m_heightMap.Dispose();
    }

    struct GenerateHeightMapJob : IJobParallelFor
    {
        [ReadOnly]
        public HeightMapDescriptor descriptor;
        [ReadOnly]
        public float waveScale;
        [ReadOnly]
        public float waveSpeed;
        [ReadOnly]
        public int waveLengthFactor;
        // [ReadOnly]
        // public float waveDistance;
        [ReadOnly]
        public float noiseStrength;
        [ReadOnly]
        public float noiseWalk;
        [ReadOnly]
        public Vector2 waveDirection;

        [ReadOnly]
        public float time;

        [WriteOnly]
        public NativeArray<float> heightMap;


        public void Execute(int index)
        {
            int row = index / descriptor.pixelSize;
            int col = index % descriptor.pixelSize;

            int row_from_bottom = (descriptor.pixelSize - 1) - row;

            float u = (float)col / (float)(descriptor.pixelSize - 1);
            float v = (float)row_from_bottom / (float)(descriptor.pixelSize - 1);

            // u -= 0.5f;
            // v -= 0.5f;
            float waveLength;
            if (Mathf.Abs(waveDirection.x) >= Mathf.Abs(waveDirection.y))
            {
                float l = 1 * Mathf.Abs(waveDirection.x);
                waveLength = l / waveLengthFactor;
            }
            else
            {
                float l = 1 * Mathf.Abs(waveDirection.y);
                waveLength = l / waveLengthFactor;
            }

            //			Debug.Log(waveLength);


            // speed = 2 * PI * v
            float omega = waveSpeed / waveLength;

            float initDist = (u * waveDirection.x + v * waveDirection.y);
            float phi = initDist / waveLength * 2 * Mathf.PI;

            float y = waveScale * Mathf.Sin((omega * time) + phi);
            // y += Mathf.PerlinNoise(u + noiseWalk, y + Mathf.Sin(time * 0.1f)) * noiseStrength;
            // y += Mathf.PerlinNoise(u + noiseWalk, v + noiseWalk) * noiseStrength;

            y += Mathf.PerlinNoise(u, v) * noiseStrength;

            heightMap[index] = y;
        }
	}
}