using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// It will determined what type of object is this
/// </summary>
public enum ObjectType
{
    None,
    Cube,
    Sphere    
}

/// <summary>
/// Sortable data 
/// </summary>
[Serializable]
public struct ObjectData
{
    [SerializeField] public string objectName;
    [SerializeField] public ObjectType objectType;
    [SerializeField] public Vector3 position;
    [SerializeField] public Quaternion rotation;
}
[Serializable]
public class SceneData
{
    public List<ObjectData> objects = new List<ObjectData>();
}

public class SceneManager : MonoBehaviour
{
    [SerializeField] UIHandler UI;
    [SerializeField] SelectableDragAndRotate SelectedObject;
    [SerializeField]List<GameObject> ObjectList = new List<GameObject>();
    // Start is called before the first frame update

    #region Singleton

    // Static instance to hold the singleton instance
    public static SceneManager _instance = null;


    // Optional: Ensure the singleton persists across scenes
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Make the singleton persist across scenes
        LoadScene();
    }

    
    #endregion

    #region Object Selection operations 
    // Update is called once per frame
    void Update()
    {
        //Create
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            // Create a ray from the camera's position in the direction it is facing
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the ray cast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has the "Ball" tag
                if (hit.collider.CompareTag("Floor"))
                {
                    UI.CreateObject(hit.point, Quaternion.identity);
                    Debug.Log("Hit a Ball object at: " + hit.point);
                    // You can perform any additional logic here if needed
                }
                else if (hit.collider.CompareTag("Object"))
                {
                    UI.OnObjectSelected();
                    OnObjectSelected(hit.collider.transform.gameObject);
                }
            }
        }
        //Deleted
        if (Input.GetKeyDown(KeyCode.Delete)) 
        {
            Debug.Log("Object deleted");
            if (SelectedObject != null) 
            {
                Debug.Log("Object deleted 2");
                ObjectList.Remove(SelectedObject.gameObject);
                Destroy(SelectedObject.gameObject, .01f);
                SelectedObject = null;

            }
        }
        //Deselected
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ObjectDeSelect();
        }

    }
    /// <summary>
    /// Checking if pointer on ui or not
    /// </summary>
    /// <returns></returns>
    public bool IsPointerOverUI()
    {
        // Create a pointer event
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition; // Use the mouse position or any screen point

        // Ray cast using the current event system
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Check if we hit any UI elements
        return results.Count > 0;
    }
    /// <summary>
    /// Object deselected operations here
    /// </summary>
    public void ObjectDeSelect() 
    {
        Debug.Log("Object DeSelected");
        if (SelectedObject != null)
        {
            SelectedObject.ObjectSelectToggle(false);
            SelectedObject = null;

        }
    }
    /// <summary>
    /// Object selecting operation here
    /// </summary>
    /// <param name="C_object">Selected object</param>
    public void OnObjectSelected(GameObject C_object) {
        if (SelectedObject != null)
        {
            SelectedObject.ObjectSelectToggle(false);
        }
        SelectedObject = C_object.GetComponent<SelectableDragAndRotate>();
        SelectedObject.ObjectSelectToggle(true);
    }
    /// <summary>
    /// Showing some data like which object selected 
    /// and some instructions
    /// </summary>
    void OnGUI()
    {
        // Display current status
        if (SelectedObject != null)
        {
            
            GUI.Label(new Rect(10, 10, 300, 20), $"Selected: {SelectedObject.name}");
            GUI.Label(new Rect(10, 30, 300, 20), "Press R to toggle rotate mode, ESC to deselected.");
        }
        else
        {
            GUI.Label(new Rect(10, 10, 300, 20), "No object selected. Click on an object to select.");
        }
    }



    /// <summary>
    /// Adding object to the list  
    /// </summary>
    /// <param name="NewObject">Created object</param>
    /// <returns></returns>
    public void OnObjectCreated(GameObject NewObject, ObjectType objectType) 
    {
        ObjectList.Add(NewObject);
        NewObject.name = NewObject.name+"_" + objectType.ToString()+"_"+ObjectList.Count;
    }
    #endregion

    #region DataSave operations are here
    public string saveFileName = "sceneData.json";
    /// <summary>
    /// Saveing scene data
    /// </summary>
    public void SaveScene()
    {
        SceneData sceneData = new SceneData();

        foreach (GameObject obj in ObjectList)
        {

            ObjectData data = new ObjectData
            {
                objectName = obj.name,
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                objectType = obj.GetComponent<SelectableDragAndRotate>().SelfType
            };

            sceneData.objects.Add(data);
        }

        string json = JsonUtility.ToJson(sceneData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);

        Debug.Log("Scene saved to " + Path.Combine(Application.persistentDataPath, saveFileName));
    }
    /// <summary>
    /// Call before Application quit
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveScene();
    }

    #endregion
    #region Data Load operations are here
    /// <summary>
    /// Load scene data 
    /// </summary>
    public void LoadScene()
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        SceneData sceneData = JsonUtility.FromJson<SceneData>(json);

        foreach (ObjectData objectData in sceneData.objects)
        {
            switch (objectData.objectType) 
            {
                case ObjectType.Sphere:
                    {
                        UI.CreateSphere(objectData.position, objectData.rotation);
                        break;
                    }
                case ObjectType.Cube:
                    {
                        UI.CreateCube(objectData.position, objectData.rotation);
                        break;
                    }
            }
        }

        Debug.Log("Scene loaded from " + filePath);
    }
    #endregion
}
