using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

public class TestNoiseFunctions : MonoBehaviour
{
    private TerrainData myTerrainData;
    public Vector3 worldSize;
    public int resolution=129;
    float[,] heightArray; 

    void Start()
    {
        myTerrainData = gameObject.GetComponent<TerrainCollider>().terrainData; 
        worldSize = new Vector3(200, 50, 200); 
        myTerrainData.size = worldSize; 
        myTerrainData.heightmapResolution = resolution; 
        heightArray = new float[resolution, resolution];
        //Fill the height array with values!
        //Uncomment the Rampand Perlin methods to test them out!
        //Flat(0.0f);
        FBM();
        //Perlin();
        //AssignvaluesfromheightArrayintotheterrainobject'sheightmap
        myTerrainData.SetHeights(0,0,heightArray);
    }
    void Update()
    {
        // Nothing necessary here
    }
    void Flat(float value)
    {
        //FillheightArraywith a static value
        for(int i=0;i<resolution;i++)
        {
            for(int j=0;j<resolution;j++)
            {
                heightArray[i,j]=value;
            }
        }
    }

    void Perlin()
    {
        float xCoord = 0;
        for (int i = 0; i < resolution; i++)
        {
            float yCoord = 0;

            for (int j = 0; j < resolution; j++)
            {
                heightArray[i, j] = Mathf.PerlinNoise(i / (float)resolution, j / (float)resolution);
            }
        }


    }

    void FBM()
    {

        string seed = UnityEngine.Random.Range(0, 100).ToString();
        

        System.Random rand = new System.Random(seed.GetHashCode());

        float next = (float)rand.Next(10, 20);

        float xCoord = 0;
        for (int i = 0; i < resolution; i++)
        {
            float yCoord = 0;
            xCoord += next;

            for (int j = 0; j < resolution; j++)
            {
                yCoord += next;
                //heightArray[i, j] = Mathf.PerlinNoise(xCoord / (float)resolution, yCoord / (float)resolution);

                heightArray[i, j] = GetNoiseAt(xCoord / (float)resolution, yCoord / (float)resolution, 0.5f, 4, 0.9f, 0.7f);

            }
        }

    }

    public static float GetNoiseAt(float x, float z, float heightMultiplier, int octaves, float persistance, float lacunarity)
    {
        float PerlinValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            // Get the perlin value at that octave and add it to the sum
            PerlinValue += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value
        return PerlinValue * heightMultiplier;
    }
}
