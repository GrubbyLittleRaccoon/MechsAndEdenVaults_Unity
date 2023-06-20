using UnityEngine;


//Tutorial: https://www.youtube.com/watch?v=vFvwyu_ZKfU
public class PerlinTerrain : MonoBehaviour
{
    // Area settings
    public int width = 256;
    public int height = 256;

    // Noise settings
    public int iterations = 5; // Number of noise layers
    public float baseFreq = 2; // Lowest frequency
    public float baseAmp = 40; // Amplitude of base hill
    public float scaleFreq = 3;
    public float scaleAmp = 0.3f; // Amplitude degradation

    public float offsetScale = 2f; // Noisy offset creates ridging

    private float netAmp = 0; // Sigma scaleAmp over iterations

    // Randomisation
    public float offsetX = 100f;
    public float offsetY = 100f;

    private Terrain terrain;

    // Limit the settings
    private void OnValidate() {
        
    }

    /*
    private void Start() {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }*/

    private void Start() {
        netAmp = 0;
        for (int i = 0; i < iterations; i++) {
            netAmp += baseAmp * Mathf.Pow(scaleAmp, i);
        }

        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData); // Feed into the terrain component
    }

    void Update(){

        // offsetX += Time.deltaTime * 5f;
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width+1;
        terrainData.size = new Vector3(width, baseAmp*netAmp, height); // Sets the dimensions of the terrain
        terrainData.SetHeights(0, 0, GenerateHeights()); //0,0 is the starting point?

        return terrainData;
    }

    // 2D representing heights for an x,y?
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // height values must be between 0 and 1
                heights[x, y] = 0;
                for (int i=0; i< iterations; i++) {
                    float iterAmp = Mathf.Pow(scaleAmp, i);
                    float iterScale = Mathf.Pow(scaleFreq, i);
                    // heights[x, y] += (CalculateHeight(x, y, iterScale) * iterAmp);
                    heights[x, y] += (CalculateOffsetHeight(x, y, iterScale) * iterAmp);
                }
                heights[x, y] = heights[x, y] / netAmp;
            }
        }

        return heights;
    }

    
    float CalculateHeight(int x, int y, float scale) {
        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    float CalculateOffsetHeight(int x, int y, float scale) {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float noiseValue = Mathf.PerlinNoise(xCoord, yCoord)-0.5f; // Returns value between -0.5 to 0.5

        float noisyOffsetX = xCoord - (noiseValue * offsetScale);
        float noisyOffsetY = yCoord - (noiseValue * offsetScale);

        return Mathf.PerlinNoise(noisyOffsetX, noisyOffsetY);
    }
}

// Ep4: Color https://www.youtube.com/watch?v=RDQK1_SWFuc&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=4
[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}