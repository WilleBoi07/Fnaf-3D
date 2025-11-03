using UnityEngine;

public class OfficeViewController : MonoBehaviour
{
    [Header("View Transforms")]
    public Transform forwardView;
    public Transform rightView;
    public Transform leftView;
    public Transform monitorView;

    [Header("Settings")]
    public float turnSpeed = 3f;
    public float edgeThreshold = 0.05f;     // Edge percentage for switching (left/right)
    public float centerThreshold = 0.4f;    // Must return to center before next switch
    public float bottomThreshold = 0.1f;    // Hover near bottom to enter monitor

    private Transform targetView;
    private bool canSwitch = true;
    private bool inMonitorView = false;

    void Start()
    {
        targetView = forwardView;
    }

    void Update()
    {
        float mouseX = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
        float mouseY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);

        // Reset ability to switch once back near center zone
        bool inHorizontalCenter = mouseX > centerThreshold && mouseX < 1f - centerThreshold;
        if (!canSwitch && inHorizontalCenter)
        {
            canSwitch = true;
        }

        // Enter monitor view if hovering near bottom
        if (!inMonitorView && mouseY <= bottomThreshold)
        {
            EnterMonitorView();
        }

        // Exit monitor view if Esc is pressed
        if (inMonitorView && Input.GetKeyDown(KeyCode.Escape))
        {
            targetView = forwardView;
            inMonitorView = false;
            canSwitch = false; // Prevent immediate left/right switch
        }

        // Only handle horizontal switching if not in monitor view
        if (canSwitch && !inMonitorView)
        {
            if (mouseX >= 1f - edgeThreshold)
            {
                if (targetView == forwardView)
                    targetView = rightView;
                else if (targetView == leftView)
                    targetView = forwardView;

                canSwitch = false;
            }
            else if (mouseX <= edgeThreshold)
            {
                if (targetView == forwardView)
                    targetView = leftView;
                else if (targetView == rightView)
                    targetView = forwardView;

                canSwitch = false;
            }
        }

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetView.position, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetView.rotation, Time.deltaTime * turnSpeed);
    }

    // Call this method to enter monitor view
    public void EnterMonitorView()
    {
        targetView = monitorView;
        inMonitorView = true;
        canSwitch = false;
    }
}
