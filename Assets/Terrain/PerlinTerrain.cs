using UnityEngine;


//Tutorial: https://www.youtube.com/watch?v=vFvwyu_ZKfU
public class PerlinTerrain : MonoBehaviour
{
    // Area settings
    public int width = 256;
    public int length = 256;

    // Noise settings
    public int octaves = 5; // Number of perlin octaves. Combining perlin layers is actually fractional brownian motion but whatever
    public float baseAmp = 40; // Amplitude of base hill
    public float scaleFreq = 3; // Size change over each octave
    public float scaleAmp = 0.3f; // Amplitude degradation over each octave

    public float offsetScale = 2f; // Noisy offset creates ridging


    private float netAmp = 0; // Sum of all scaleAmp over iterations for 0-1 normalisation

    // Randomisation
    public float offsetX = 100f;
    public float offsetY = 100f;

    private Terrain terrain;

    // Limit the settings
    private void OnValidate()
    {
    }

    private void Start()
    {
        // Preempetively calculate the total terrain height for alignment purposes
        netAmp = 0;
        for (int i = 0; i < octaves; i++)
        {
            netAmp += baseAmp * Mathf.Pow(scaleAmp, i);
        }

        // Feed the noise into the Terrain component
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    void Update()
    {
        // Add movement for testing
        //offsetX += Time.deltaTime * 0.5f;
        //terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, baseAmp * netAmp, length); // Sets the dimensions of the terrain
        terrainData.SetHeights(0, 0, GenerateHeights()); //0,0 is the starting point

        return terrainData;
    }

    /**
     * Generate a brownian [width, height] array that contains series of heights between 0 and 1
     */
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                heights[x, y] = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float iterAmp = Mathf.Pow(scaleAmp, i);
                    float iterScale = Mathf.Pow(scaleFreq, i);
                    heights[x, y] += (CalculateOffsetHeight(x, y, iterScale) * iterAmp);
                }
                heights[x, y] = heights[x, y] / netAmp; // Normalise to 0-1
            }
        }
        return heights;
    }

    /**
     * Calculate perlin offset height for a single x,y point.
     */
    float CalculateHeight(int x, int y, float scale)
    {
        float xCoord = (((float)x / width) * scale) + offsetX;
        float yCoord = (((float)y / length) * scale) + offsetY;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    /**
     * Calculate perlin offset height for a single x,y point.
     * Adds perlin offset to for structured noise (See https://www.youtube.com/watch?v=lctXaT9pxA0)
     * Returns a value between 0 and "scale
     */
    float CalculateOffsetHeight(int x, int y, float scale)
    {
        // Factor in x/y configuration
        float xCoord = (((float)x / width) * scale) + offsetX;
        float yCoord = (((float)y / length) * scale) + offsetY;

        float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) - 0.5f; // Returns value between -0.5 to 0.5

        // Consider playing with structured offset noise at some point
        float noisyOffsetX = xCoord - (noiseValue * offsetScale);
        float noisyOffsetY = yCoord - (noiseValue * offsetScale);

        return Mathf.PerlinNoise(noisyOffsetX, noisyOffsetY);
    }
}

// Consider mixed 2D texture procedural mix
// Ep4: Color https://www.youtube.com/watch?v=RDQK1_SWFuc&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=4
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}