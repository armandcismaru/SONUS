using UnityEngine;
using Photon.Pun;
 
public class MouseController : MonoBehaviour
{
    public float mouseSpeed;

    private float xRotation = 0.0f;
    [SerializeField] GameObject cameraHolder;

    private float lookXLimit = 45.0f;
    private PhotonView view;

    void Awake() {
        mouseSpeed = RoomManager.Instance.mouseSpeed;
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
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
        if (Cursor.lockState == CursorLockMode.Locked && view.IsMine)
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
  public void setMouseSpeed(float volume)
    {
        RoomManager.Instance.mouseSpeed = volume;
        mouseSpeed = RoomManager.Instance.mouseSpeed;
    }

}