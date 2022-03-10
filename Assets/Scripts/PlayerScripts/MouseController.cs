using UnityEngine;
 
public class MouseController : MonoBehaviour
{
    public float mouseSpeed;

    private float xRotation = 0.0f;
    [SerializeField] GameObject cameraHolder;

    private float lookXLimit = 45.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LockAndUnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void Update()
    {
        LockAndUnlockCursor();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            xRotation += -Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);
            cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime, 0);
#endif

#if UNITY_EDITOR
            xRotation += -Input.GetAxis("Mouse Y");
            xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);
            cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
#endif
        }
    }
}