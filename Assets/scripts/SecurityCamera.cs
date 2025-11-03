using UnityEngine;

public enum FloorType
{
    Basement,
    GroundFloor,
    SecondFloor
}

public class SecurityCamera : MonoBehaviour
{
    public string cameraName;
    public FloorType floor;
}
