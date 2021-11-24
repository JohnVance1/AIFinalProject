using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

public class TestNoiseFunctions : MonoBehaviour
{
    private TerrainData myTerrainData;
    public Vector3 worldSize;
    public int resolution = 129;
    float[,] heightArray;
    public GameObject prefab;

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
        //RandomFill();
        Gaussian();
        //FBM();
        //Perlin();
        //Assign values from heightArray into the terrainobject's heightmap
        myTerrainData.SetHeights(0, 0, heightArray);
    }

    void Flat(float value)
    {
        //FillheightArraywith a static value
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                heightArray[i, j] = value;
            }
        }
    }

    public void RandomFill()
    {
        //System.Random rand = new System.Random();

        // Actually populates the map
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                heightArray[x, y] = UnityEngine.Random.Range(0f, 1f);


            }
        }
    }

    public void Gaussian()
    {
        float mean = 0f;
        float stdDev = 1f;


        int val1;
        int val2;
        int val3;
        float s;
        

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float size = resolution % 2 == 0 ? resolution : resolution - 1;


                float dist = Vector2.Distance(new Vector2(x, y), new Vector2((size / 2), size / 2));

                heightArray[x, y] = (resolution - dist) / resolution;


                //do
                //{
                //    val1 = UnityEngine.Random.Range(0, resolution);
                //    val2 = UnityEngine.Random.Range(0, resolution);
                //    val3 = UnityEngine.Random.Range(0, resolution);
                //    s = val1 * val1 + val2 * val2;
                //    //s = val1 * val2;
                //} while (val1 == 0f);

                //s = Mathf.Sqrt((-2.0f * Mathf.Log(s / resolution)) / s);


                //float gaussValueX = Mathf.Sqrt(-2.0f * Mathf.Log((x) / (float)(resolution * 2))) * Mathf.Cos(2.0f * Mathf.PI * (float)(x) / (resolution));
                ////gaussValueX = s * val1;
                //float gaussValueY = Mathf.Sqrt(-2.0f * Mathf.Log((y) / (float)(resolution * 2))) * Mathf.Sin(2.0f * Mathf.PI * (float)(y) / (resolution));
                ////gaussValueY = s * val2;

                //if(gaussValueX >= 100000000 || gaussValueY >= 1000000000 || gaussValueX <= -100000000 || gaussValueY <= -1000000000)
                //{
                //    Debug.Log("Infinity");
                //}
                //if (gaussValueX == float.NaN || gaussValueY == float.NaN)
                //{
                //    Debug.Log("NaN");
                //}

                //gaussValueX = mean + stdDev * gaussValueX;
                //gaussValueY = mean + stdDev * gaussValueY;
                ////float gaussianY = mean + stdDev * gaussValueY;

                //Instantiate(prefab, new Vector3(gaussValueX, gaussValueY, 0), Quaternion.identity);

                //heightArray[x, y] = -gaussValueX;
            }
        }

        



    
    }       
    

    
    void Perlin()
    {
        for (int i = 0; i < resolution; i++)
        {           
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
