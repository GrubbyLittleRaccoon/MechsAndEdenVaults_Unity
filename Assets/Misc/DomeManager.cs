using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeManager : MonoBehaviour
{
    [SerializeField]
    private float groundRadius; //of the circle where the dome meets the ground
    public float overallRadius; //overall radius of the dome

    // Start is called before the first frame update
    void Start()
    {
        setupDome();
    }

    public float getGroundRadius()
    {
        return groundRadius;
    }

    void OnValidate()
    {
        setupDome();
    }

    // Position the dome based on the terrain
    void setupDome()
    {
        //World space, may need to do math in future if we use localPosition 
        // Zero height to terrain base to reduce concaving effect near ground
        this.transform.position = new Vector3(0, 0, 0);
        this.transform.localScale = new Vector3(overallRadius, overallRadius, overallRadius);

        // More accurate would be using trig and the terrain's netAmp * baseAmp or something, but this should be okay for now.
        groundRadius = overallRadius - 1; //Lower bound of the ground radius
    }

    // Update is called once per frame
    void Update()
    {

    }
}
