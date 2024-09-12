using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIPanel;
    public Transform inventoryPanel;
    public List<InventorySlot> slots = new List<InventorySlot> ();
    public bool IsOpened;
    private Camera mainCamera;
    public float reachDistance =2;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        UIPanel.SetActive(false);
        for (int iterator = 0; iterator< inventoryPanel.childCount; iterator++)
        {
            if(inventoryPanel.GetChild(iterator).GetComponent<InventorySlot>()!=null)
            {
                slots.Add(inventoryPanel.GetChild(iterator).GetComponent<InventorySlot>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsOpened = !IsOpened;
            if(IsOpened)
            {
                UIPanel.SetActive(true);
            }
            else
            {
                UIPanel.SetActive(false);
            }
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, reachDistance ))
        {
            Debug.DrawRay(ray.origin, ray.direction*reachDistance, Color.green);
            if(hit.collider.gameObject.GetComponent<Item>() != null)
            {
                AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction*reachDistance, Color.red);
        }
    }
    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.item== _item)
            {
                slot.amount += _amount;
                return;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if(!slot.isEmpty)
            {
                slot.item = _item;
                slot.amount -= _amount;
                slot.isEmpty = false;   
            }
        }
    }
}
