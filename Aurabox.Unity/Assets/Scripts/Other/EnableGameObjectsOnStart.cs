using System;
using UnityEngine;

namespace Aurabox
{
    public class EnableGameObjectsOnStart : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _gameObjects = Array.Empty<GameObject>();

        private void Start()
        {
            foreach (var component in _gameObjects)
                component.gameObject.SetActive(true);
        }
    }
}