using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// handling the UI operations and object creation
/// </summary>
public class UIHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Add button here")] Button cube, sphere;
    [SerializeField, Tooltip("Add cube here")] GameObject _cubePrefab;
    [SerializeField, Tooltip("Add sphere here")] GameObject _spherePrefab;
    [SerializeField, Tooltip("Current object type")] ObjectType _type = ObjectType.None;

    /// <summary>
    /// Adding listener to the button
    /// </summary>
    private void Awake()
    {
        cube.onClick.AddListener(OnCubeButtonPress);
        sphere.onClick.AddListener(OnSphereButtonPress);
    }
    /// <summary>
    /// Cube button operation
    /// </summary>
    private void OnCubeButtonPress()
    {

        SceneManager._instance.ObjectDeSelect();
        if (_type == ObjectType.None || _type == ObjectType.Sphere)
        {
            _type = ObjectType.Cube;
            cube.image.color = Color.yellow;
            sphere.image.color = Color.white;
        }
        else if (_type == ObjectType.Cube)
        {
            _type = ObjectType.None;
            cube.image.color = Color.white;

        }
    }
    /// <summary>
    /// Sphere button operation
    /// </summary>
    private void OnSphereButtonPress()
    {
        SceneManager._instance.ObjectDeSelect();
        if (_type == ObjectType.None || _type == ObjectType.Cube)
        {
            _type = ObjectType.Sphere;
            sphere.image.color = Color.yellow;
            cube.image.color = Color.white;
        }
        else if (_type == ObjectType.Sphere)
        {
            _type = ObjectType.None;
            sphere.image.color = Color.white;
        }

    }
    /// <summary>
    /// all objects deselected
    /// </summary>
    public void OnObjectSelected()
    {
        _type = ObjectType.None;
        sphere.image.color = Color.white;
        cube.image.color = Color.white;
    }

    /// <summary>
    /// Ray object create 
    /// </summary>
    public void CreateObject(Vector3 positon, Quaternion rotation) 
    {
        switch (_type)
        {
            case ObjectType.Sphere:CreateSphere(positon, rotation); break;
            case ObjectType.Cube: CreateCube(positon, rotation); break;
            case ObjectType.None: Debug.Log("Game point"); break;
        }
    }


    /// <summary>
    /// creating cube here
    /// </summary>
    public void CreateCube(Vector3 position, Quaternion rotation) 
    {
        GameObject cube = Instantiate(_cubePrefab, position , rotation);
        OnObjectCreate(cube, ObjectType.Cube);

    }
    /// <summary>
    /// creating Sphere here
    /// </summary>
    public void CreateSphere(Vector3 positon, Quaternion rotation)
    {
        GameObject sphere = Instantiate(_spherePrefab, positon, rotation);
        OnObjectCreate(sphere, ObjectType.Sphere);
    }

    /// <summary>
    /// Creating object and assigning to the list
    /// </summary>
    /// <param name="item"></param>
    private void OnObjectCreate(GameObject item, ObjectType objectType)
    {
        Debug.Log(item.name);
        SceneManager._instance.OnObjectCreated(item, objectType);
        item.GetComponent<SelectableDragAndRotate>().SelfType = objectType;
    }
}
