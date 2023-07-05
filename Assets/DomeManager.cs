using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeManager : MonoBehaviour
{
    [SerializeField]
    private float groundRadius; //of the circle where the dome meets the ground

    private Terrain terrain; //parent object

    // Start is called before the first frame update
    void Start()
    {
        positionDome();
    }

    void OnValidate()
    {
        positionDome();
    }

    // Position the dome based on the terrain
    void positionDome()
    {
        this.transform.position = new Vector3(0, 0, 0); //World space, may need to do math in future if we use localPosition 
        // Zero height to terrain base to reduce concaving effect near ground
    }

    // Update is called once per frame
    void Update()
    {

    }
}
