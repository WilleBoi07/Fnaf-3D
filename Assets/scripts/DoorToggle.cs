using UnityEngine;
using System;

public class DoorToggle : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = -90f;
    public float rotationSpeed = 5f;

    public event Action OnDoorOpened;   // <---  EVENT

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ToggleDoor();
        }

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        Debug.Log($"[DOOR] Door toggled: {(isOpen ? "OPEN" : "CLOSED")}");

        if (isOpen)
        {
            OnDoorOpened?.Invoke(); //  tell listeners (e.g., monster)
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
