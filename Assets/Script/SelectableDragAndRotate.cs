using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableDragAndRotate : MonoBehaviour
{

    private Camera mainCamera;
    private bool isDragging = false;
    private bool isRotating = false;
    private bool isSelected = false;
    private float rotationSpeed = 1f;
    Vector3 offset = Vector3.zero;
    private MeshRenderer meshRenderer;
    [SerializeField] private Material HilitedMaterial;
    private Material startMatrial;
    private Vector3 lastMousePosition;
    private ObjectType self_type;
    public ObjectType SelfType {get{ return self_type; } set { self_type = value; } }

    void Start()
    {
        mainCamera = Camera.main; // Reference to the main camera
        meshRenderer = GetComponent<MeshRenderer>();
        startMatrial = meshRenderer.material;
    }

    /// <summary>
    /// Updating object Position and rotation
    /// </summary>
    void Update()
    {
        if (!isSelected)
            return;

        // Handle drag
        if (Input.GetMouseButtonDown(0)&& !isRotating) // Left mouse button pressed
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                isDragging = true;
                offset = new Vector3( transform.position.x - hit.point.x,0f, transform.position.z-hit.point.z);
            }
        }
        else if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                transform.position = new Vector3(hit.point.x + offset.x, transform.position.y, hit.point.z+offset.z);
            }
        }

        // Handle rotation
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            isRotating = !isRotating;
        }
        
        if (isRotating)
        {
            if (Input.GetMouseButtonDown(0) && isRotating)
            {
                lastMousePosition = Input.mousePosition; // Store initial mouse position
            }
            if (Input.GetMouseButton(0)) 
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition; // Calculate mouse movement
                lastMousePosition = Input.mousePosition; // Update last position

                // Rotate object based on mouse movement
                transform.Rotate(Vector3.up, -mouseDelta.x * rotationSpeed, Space.World); // Horizontal rotation
            }
            
        }
    }
    /// <summary>
    /// Call when object selected
    /// </summary>
    public void ObjectSelectToggle(bool objectSelected) 
    {
        if (objectSelected)
        {
            meshRenderer.material = HilitedMaterial;
            isSelected = true;
        }
        else 
        {
            meshRenderer.material = startMatrial;
            isSelected = false;
            isRotating = false;
        }

    }
    
}

