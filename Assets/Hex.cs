using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    private float nodePosX, nodePosZ; // Unity Location

    public int CoordinateX, CoordinateZ; // Location in the grid (0,0 is top left)

    private bool rowIsEven;

    public Hex(int CoordinateX, int CoordinateZ) {
        this.CoordinateX = CoordinateX;
        this.CoordinateZ = CoordinateZ;
    }

    // Start is called before the first frame update
    void Start()
    {
        nodePosX = transform.position.x;
        nodePosZ = transform.position.z;
        rowIsEven = (CoordinateZ % 2 == 1); // ==1 because rows count starts from 1 instead from 0 unlike the coordinate system
    }

    public Hex Add(Hex b) {
        return new Hex(CoordinateX + b.CoordinateX, CoordinateZ + b.CoordinateZ);
    }

    // This can be used to calculate the coordinates of the hex of the given direction
    // Direction values are between 0 and 5 (inclusive)
    // 0 is top left, 1 is left, 2 is bottom left, 3 is bottom right, 4 is right, 5 is top right
    // I think that's correct :)
    public Hex Neighbor(int direction)
    {
        if (rowIsEven) {
            return Hex.directionsEven[direction];
        } else {
            return Hex.directionsOdd[direction];
        }
    }

    // Odd row = Top 6 numbers // Even row = Bottom 6 numbers
    static public List<Hex> directionsOdd = new List<Hex>{
        new Hex(-1, -1),
        new Hex(-1, 0), 
        new Hex(-1, 1), 
        new Hex(0, -1), 
        new Hex(0, 1), 
        new Hex(1, 0)};

    static public List<Hex> directionsEven = new List<Hex>{
        new Hex(-1, 0),
        new Hex(0, -1), 
        new Hex(0, 1), 
        new Hex(1, -1), 
        new Hex(1, 0), 
        new Hex(1, 1)};

    // Update is called once per frame
    void Update()
    {
        
    }
}
