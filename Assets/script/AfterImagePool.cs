using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;
    private Queue<GameObject> availableObject = new Queue<GameObject>();
    public static AfterImagePool Instance { get; private set; }

    private void Awake(){
        Instance = this;
        GrowPool();
    }

    private void GrowPool(){
        for(int i = 0; i < 10; i++){
            var InstanceToAdd = Instantiate(afterImagePrefab);
            InstanceToAdd.transform.SetParent(transform);
            AddToPool(InstanceToAdd);

        }
    }
    public void AddToPool(GameObject instance){
        instance.SetActive(false);
        availableObject.Enqueue(instance);
    }
    public GameObject GetFromPool(){
        if(availableObject.Count == 0){
            GrowPool();
        }
        var instance = availableObject.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
