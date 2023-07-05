using UnityEngine;

//Tutorial: https://www.youtube.com/watch?v=vFvwyu_ZKfU
public class TerrainManager : MonoBehaviour
{
    // Area settings
    public int width = 256;
    public int length = 256;

    // Noise settings
    public int octaves = 5; // Number of perlin octaves. Combining perlin layers = fractional brownian motion
    public float baseAmp = 40f; // Basic amplitude = overall linear scaling factor
    public float baseFreq = 1f;
    public float scaleFreq = 3; // Frequency increase over each octave
    public float scaleAmp = 0.3f; // Amplitude degradation over each octave
    public float offsetScale = 2f; // Higher intensity perlin offset causes structured warping, leading to ridging

    // Randomisation
    public float offsetX = 100f;
    public float offsetY = 100f;

    public Terrain terrain;

    private void Start()
    {
        terrain.terrainData = GenerateTerrain();
        // Centre the position of the Terrain around 0,0
        terrain.transform.position = new Vector3(-width / 2, 0, -length / 2);
        // Set terrain collider based on generated data
        SetTerrainCollider(terrain.terrainData);
    }

    //Generally only triggered when scene view/inspector is up (non runtime)
    private void OnValidate()
    {
        terrain.terrainData = GenerateTerrain();
        // Centre the position of the Terrain around 0,0
        terrain.transform.position = new Vector3(-width / 2, 0, -length / 2);
        // Set terrain collider based on generated data
        SetTerrainCollider(terrain.terrainData);
    }

    private void SetTerrainCollider(TerrainData terrainData)
    {
        // Get or add the TerrainCollider component to the same GameObject
        TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = gameObject.AddComponent<TerrainCollider>();
        }
        // Assign the generated TerrainData to the TerrainCollider
        terrainCollider.terrainData = terrainData;
    }

    /**
     * Our custom terrain generation function - heights and texture.
     * Assumption is that mesh resolution = 1:1.
     */
    private TerrainData GenerateTerrain()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, baseAmp, length); // Sets the dimensions of the terrain
        terrainData.SetHeights(0, 0, GenerateHeights());
        // Calculate the offset to center the terrain
        float centreOffsetX = -width / 2f;
        float centreOffsetZ = -length / 2f;

        //SetTextureWeights(terrainData, generatedHeights); // TODO Texture alpha mapping based on height to implement later on

        return terrainData;
    }

    /**
     * Generate a warped brownian (stacked perlin) array with heights based on scaling parameters
     * Output is from 0-1 as that is what SetHeights() expects.
     * Scaling parameters: (width, height, octaves, scaleFreq, scaleAmp, offsetScale)
     */
    float[,] GenerateHeights()
    {
        // Combined max scale after combining all octaves
        float netAmp = 0;
        for (int i = 0; i < octaves; i++)
        {
            netAmp += Mathf.Pow(scaleAmp, i);
        }

        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                heights[x, z] = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float iterAmp = Mathf.Pow(scaleAmp, i);
                    float iterFreq = baseFreq * Mathf.Pow(scaleFreq, i);
                    heights[x, z] += (CalculateOffsetHeight(x, z, iterFreq) * iterAmp); // Return 0 to 1 value...
                }
                heights[x, z] = heights[x, z] / netAmp; // Normalise heights
            }
        }

        return heights;
    }


    /**
     * Calculate perlin height (between 0-1) for a single x,y point.
     * Adds perlin offset to the height mapping for structured noise (See https://www.youtube.com/watch?v=lctXaT9pxA0)
     * 
     * offsetX/Y - randomisation
     * freq - frequency increase
     * offsetScale - extent of offset warping
     */
    float CalculateOffsetHeight(int x, int y, float freq)
    {
        // Calculate the Perlin noise coordinates based on the input position and scale
        float scaledInputX = (((float)x / width) * freq) + offsetX;
        float scaledInputY = (((float)y / length) * freq) + offsetY;

        // Get value for "non warped" Perlin noise, from -0.5 to 0.5
        float perlinNoiseValue = Mathf.PerlinNoise(scaledInputX, scaledInputY) - 0.5f;

        // Offset the coordinates based on own position's regular perlin height
        float perlinOffsetX = scaledInputX - (perlinNoiseValue * offsetScale);
        float perlinOffsetY = scaledInputY - (perlinNoiseValue * offsetScale);

        return Mathf.PerlinNoise(perlinOffsetX, perlinOffsetY); // By adding the offset, we can warp the heights
    }

    /**
     * Set the texture details for the terrain based on generated heights.
     * TODO: WIP bounds error
     */
    void SetTextureWeights(TerrainData terrainData, float[,] heights)
    {
        int alphaMapWidth = terrainData.alphamapWidth;
        int alphaMapHeight = terrainData.alphamapHeight;
        int numTextures = terrainData.terrainLayers.Length;

        float[,,] alphaMap = new float[alphaMapWidth, alphaMapHeight, numTextures];

        for (int y = 0; y < alphaMapHeight; y++)
        {
            for (int x = 0; x < alphaMapWidth; x++)
            {
                float normX = x * 1.0f / (alphaMapWidth - 1);
                float normY = y * 1.0f / (alphaMapHeight - 1);

                float angle = terrainData.GetSteepness(normY, normX);
                int terrainHeight = (int)(heights[x, y] * terrainData.size.y);

                float[] weights = new float[numTextures];

                // Here you can set the weights based on height or angle
                // For example:
                weights[0] = terrainHeight <= 20 ? 1 : 0; // grass
                weights[1] = terrainHeight > 20 && terrainHeight <= 40 ? 1 : 0; // rock
                weights[2] = terrainHeight > 40 ? 1 : 0; // snow

                // Normalize the weights
                float totalWeight = 0;
                foreach (float weight in weights) totalWeight += weight;
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] /= totalWeight;
                    alphaMap[x, y, i] = weights[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }
}