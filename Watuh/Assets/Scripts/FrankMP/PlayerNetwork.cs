using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    float cameraVerticalRotation = 0f;
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);
    private Camera cam;
    [SerializeField]
    GameObject certainbone;
    [SerializeField, Range(0, 15)]
    private float speed;
    private Vector2 _movement = new Vector2();
    [SerializeField]
    private GameObject _camera;



    private void Start()
    {
        if(IsOwner)
        {
            _camera.tag = "Untagged";
            HandleCamera_ServerRpc(false);
            _camera.GetComponent<Camera>().enabled = true;
        }
        List<GameObject> cameras = GameObject.FindGameObjectsWithTag("Camera").ToList();
        foreach (GameObject camTemp in cameras)
        {

                camTemp.SetActive(false);
        }
        if (!IsOwner)
        {
            Destroy(_camera);
            enabled = false;
            return;

        }
        if (Camera.main.enabled == true)
        {
            GameObject tempMainCam = Camera.main.gameObject;
            tempMainCam.tag = "Untagged";
            tempMainCam.GetComponent<Camera>().enabled = false;
            Destroy(tempMainCam);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = _camera.GetComponent<Camera>();
       // InputEventManager.KeyboardInput.Move += UpdateMovement;
        //InputEventManager.KeyboardInput.MoveCancel += CancelMovement;
    }

    void Update()
    {
        Debug.Log(_movement);
        if(_movement != new Vector2(0,0))
        {
            MoveNotServer();
           // HandleMovement_ServerRpc();
        }

        RotNotServer();
       // HandleRot_ServerRpc();

    }

    private void UpdateMovement(Vector2 movement)
    {
        _movement = movement;

    }
    private void CancelMovement()
    {
        _movement = new Vector2(0, 0);
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void MoveNotServer()
    {
        Debug.Log("Got here;");
        if (!IsOwner) return;
        if (_movement.y != 0)
        {

            transform.position += (transform.forward * _movement.y) * speed * Time.deltaTime;
        }
        if (_movement.x != 0)
        {

            transform.position += (transform.right * _movement.x) * speed * Time.deltaTime;
        }
    }

    private void RotNotServer()
    {
        if (!IsOwner) return;
        transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * 2f));

        cameraVerticalRotation -= Input.GetAxis("Mouse Y") * 2f;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        certainbone.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        _camera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        _camera.transform.position = transform.position + (Vector3.up * 2);
    }

    [ServerRpc]
    private void HandleRot_ServerRpc()
    {
        if (!IsOwner) return;
        transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * 2f));

        cameraVerticalRotation -= Input.GetAxis("Mouse Y") * 2f;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        certainbone.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        _camera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        _camera.transform.position = transform.position + (Vector3.up * 2);
    }

    [ServerRpc]
    private void HandleCamera_ServerRpc(bool test)
    {
        _camera.GetComponent<Camera>().enabled = test;
    }


    [ServerRpc] 
    private void HandleMovement_ServerRpc()
    {
        Debug.Log("Got here;");
        if (!IsOwner) return;
        if (_movement.y != 0)
        {

            transform.position += (transform.forward * _movement.y) * speed * Time.deltaTime;
        }
        if (_movement.x != 0)
        {

            transform.position += (transform.right * _movement.x) * speed * Time.deltaTime;
        }
    }

    [ClientRpc]
    private void Test_ClientRpc()
    {

    }
}
