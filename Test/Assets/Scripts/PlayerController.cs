using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float interactionDistance = 3.0f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform UpPos;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] Transform Camera;
    [SerializeField] Transform marker;
    [SerializeField] GameObject _Item;

    private float cameraPitch = 0f;
    [SerializeField] float minPitch = -45f;
    [SerializeField] float maxPitch = 45f;

    private Rigidbody rb;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        InteractWithItem();
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = Camera.TransformDirection(movement);
        movement.y = 0;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
        Camera.localEulerAngles = new Vector3(cameraPitch, 0, 0);
    }

    void InteractWithItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.position, Camera.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("PlayerRaycast"))
            {
                Item item = null;
                if (hit.collider.GetComponent<Item>() !=null)
                item = hit.collider.GetComponent<Item>();

                Mesto mesto = null;
                    if(hit.collider.GetComponent<Mesto>()!=null)
                    mesto = hit.collider.GetComponent<Mesto>();

                // Проверяем, есть ли объект, с которым мы взаимодействуем
                if (item != null && _Item == null)
                {
                    marker.gameObject.SetActive(true);
                    UpdateMarkerPosition(hit.collider);
                }
                else if(mesto != null && !mesto.zanato && _Item != null)
                {
                    marker.gameObject.SetActive(true);
                    UpdateMarkerPosition(hit.collider);
                }
                else
                {
                    marker.gameObject.SetActive(false);
                }

                HandleItemPickup(item, mesto, hit);
            }
            else
            {
                marker.gameObject.SetActive(false);
            }
        }
        else
        {
            marker.gameObject.SetActive(false);
        }

        HandleItemDrop();
    }

    void UpdateMarkerPosition(Collider objectCollider)
    {
        Collider col = objectCollider;
        Vector3 markerPosition = col.bounds.center + Vector3.up * (col.bounds.extents.y + 0.2f);
        marker.position = markerPosition;
        marker.GetComponent<Marker>().view = true;
    }

    void HandleItemPickup(Item item, Mesto mesto, RaycastHit hit)
    {
        if (Input.GetKeyDown(KeyCode.E) && _Item == null && item != null)
        {
                if (item.mesto != null)
            {
                item.mesto.GetComponent<Mesto>().zanato = false;
            }

            _Item = item.gameObject;
            _Item.transform.parent = UpPos;
            _Item.layer = 2;
            _Item.transform.localScale /= 2f;
            _Item.transform.localPosition = Vector3.zero;
            _Item.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && _Item != null && mesto != null && !mesto.zanato)
        {
            _Item.SetActive(true);
            _Item.transform.localScale *= 2f;
            _Item.transform.parent = hit.collider.transform;
            _Item.GetComponent<Rigidbody>().isKinematic = true;
            _Item.GetComponent<Item>().mesto = mesto.gameObject;
            _Item.transform.position = hit.collider.transform.position;
            mesto.zanato = true; 
            _Item.layer = 0;
            _Item = null;
        }
    }

    void HandleItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _Item != null)
        {
            _Item.transform.localScale *= 2f;
            _Item.SetActive(true);
            _Item.GetComponent<Rigidbody>().isKinematic = false;
            _Item.transform.parent = null;
            _Item.layer = 0;
            _Item = null;
        }
    }
}
