using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraMonitorSystem : MonoBehaviour
{
    [Header("Monitor Settings")]
    public RenderTexture monitorTexture;
    public Camera playerViewCamera;       // The player's office camera (to switch back)
    public Transform cameraDisplayPoint;  // Position to place active cam, or reference point

    [Header("UI References")]
    public GameObject cameraButtonPrefab; // Prefab for camera buttons
    public Transform cameraButtonPanel;   // Where buttons will be created
    public Button basementButton;
    public Button groundFloorButton;
    public Button secondFloorButton;
    public Button settingsButton;

    private List<SecurityCamera> allCameras = new List<SecurityCamera>();
    private Camera activeCamera;
    private FloorType currentFloor;

    void Start()
    {
        // Find all cameras in the scene
        SecurityCamera[] found = FindObjectsOfType<SecurityCamera>();
        allCameras.AddRange(found);

        // Hook up floor buttons
        basementButton.onClick.AddListener(() => SwitchFloor(FloorType.Basement));
        groundFloorButton.onClick.AddListener(() => SwitchFloor(FloorType.GroundFloor));
        secondFloorButton.onClick.AddListener(() => SwitchFloor(FloorType.SecondFloor));

        // Default to Ground Floor
        SwitchFloor(FloorType.GroundFloor);
    }

    void SwitchFloor(FloorType floor)
    {
        currentFloor = floor;
        ClearCameraButtons();
        PopulateCameraButtons(floor);
    }

    void PopulateCameraButtons(FloorType floor)
    {
        foreach (SecurityCamera cam in allCameras)
        {
            if (cam.floor == floor)
            {
                GameObject buttonObj = Instantiate(cameraButtonPrefab, cameraButtonPanel);
                TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();
                label.text = cam.cameraName;

                Button btn = buttonObj.GetComponent<Button>();
                btn.onClick.AddListener(() => SwitchToCamera(cam.GetComponent<Camera>()));
            }
        }
    }

    void ClearCameraButtons()
    {
        foreach (Transform child in cameraButtonPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void SwitchToCamera(Camera cam)
    {
        // Disable old camera’s rendering
        if (activeCamera != null)
        {
            activeCamera.targetTexture = null;
            activeCamera.enabled = false;
        }

        // Enable and route new camera to monitor
        activeCamera = cam;
        activeCamera.targetTexture = monitorTexture;
        activeCamera.enabled = true;
    }

}
