using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour {
    //Power of noise
    public float power = 3.0f;
    //Noise scale
    public float scale = 1.0f;
    //Start position (center of map)
    private Vector2 v2SampleStart = new Vector2(0f, 0f);

    void Start()
    {
        //Make terrain on startup
        //MakeSomeNoise();
    }

    void Update()
    {
        //Randomly reset terrain on space press
        if (Input.GetKeyDown(KeyCode.Space))
        {            
            MakeSomeNoise();
        }
    }

    //Making terrain with perlin noise
    void MakeSomeNoise()
    {
        //Get map center
        v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));

        //Get map object components for mesh
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = transform.GetChild(0).GetComponent<MeshCollider>();
        //Vertices of the plain
        Vector3[] verts = mf.mesh.vertices;

        //Generate noise for each of the vertices
        for (int i = 0; i < verts.Length; i++)
        {
            float xCoord = v2SampleStart.x + verts[i].x * scale;
            float yCoord = v2SampleStart.y + verts[i].z * scale;
            //Noise generation
            verts[i].y = (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * power;
        }
        //Set the plain based on the noise generation
        mf.mesh.vertices = verts;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();
        //Reset the collider to suit the new mesh shape
        mc.sharedMesh = mf.mesh;
        //Reset the ball position to avoid falling through map
        ResetPlayerPos();
    }

    //Reseting the player position
    private void ResetPlayerPos()
    {
        GameObject.Find("Player").transform.position = new Vector3(0, GetComponent<MeshRenderer>().bounds.max.y, 0);
    }
} 

