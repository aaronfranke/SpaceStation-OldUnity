
using System;
using UnityEngine;

[Serializable]
public struct ObjMetadata
{
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;

    public string name;
    public int dataSize;

    public int circumference;
    public float targetGravity;

    // Ships
    public ObjMetadata(Vector3 Position, Quaternion Rotation, string Name, Vector3 Velocity, int DataSize)
    {
        position = Position;
        rotation = Rotation;
        name = Name;
        velocity = Velocity;

        dataSize = DataSize;

        circumference = 0;
        targetGravity = 0;
    }

    // Station
    public ObjMetadata(Vector3 Position, Quaternion Rotation, string Name, int DataSize, int Circumference, float TargetGravity)
    {
        position = Position;
        rotation = Rotation;
        name = Name;
        velocity = Vector3.zero;

        dataSize = DataSize;
        circumference = Circumference;
        targetGravity = TargetGravity;
    }

    override
    public string ToString()
    {
        string s = "Object " + name + ", located at " + position + ".";
        return s;
    }

}










