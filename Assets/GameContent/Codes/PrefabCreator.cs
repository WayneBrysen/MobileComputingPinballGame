using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PrefabCreator : MonoBehaviour
{
    [SerializeField] private GameObject tablePrefab;
    [SerializeField] private Vector3 prefabOffset;

    private GameObject table;
    private ARTrackedImageManager ARTrackedImageManager;

    private void OnEnable()
    {
        ARTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();

        ARTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach(ARTrackedImage image in obj.added)
        {
            table = Instantiate(tablePrefab, image.transform);
            table.transform.position += prefabOffset;
        }
        
    }
}
