using System.Collections.Generic;
using UnityEngine;

public class ObjectColliderChecker : MonoBehaviour
{
    [SerializeField] private GameObject objectToCheck; // The object to check for in the collider
    [SerializeField] private List<GameObject> objectsToEnable; // List of GameObjects to enable/disable

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToCheck)
        {
            // Enable all GameObjects in the list
            foreach (GameObject obj in objectsToEnable)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToCheck)
        {
            // Disable all GameObjects in the list
            foreach (GameObject obj in objectsToEnable)
            {
                obj.SetActive(false);
            }
        }
    }
}
