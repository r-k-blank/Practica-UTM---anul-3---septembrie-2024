using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIBG;
    public Transform inventoryPanel;
    public Transform quickslotPanel;
    public GameObject Crosshair;
    public List<InventorySlot> slots = new List<InventorySlot> ();
    public bool IsOpened;
    private Camera mainCamera;
    public float reachDistance =3f;
    public CinemachineVirtualCamera virtualCamera;
    // Start is called before the first frame update
    private void Awake()
    {
        UIBG.SetActive(true);
    }
    void Start()
    {
        mainCamera = Camera.main;
        
        for (int iterator = 0; iterator< inventoryPanel.childCount; iterator++)
        {
            if(inventoryPanel.GetChild(iterator).GetComponent<InventorySlot>()!=null)
            {
                slots.Add(inventoryPanel.GetChild(iterator).GetComponent<InventorySlot>());
            }
        }
        for (int iterator = 0; iterator < quickslotPanel.childCount; iterator++)
        {
            if (quickslotPanel.GetChild(iterator).GetComponent<InventorySlot>() != null)
            {
                slots.Add(quickslotPanel.GetChild(iterator).GetComponent<InventorySlot>());
            }
        }
        UIBG.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsOpened = !IsOpened;
            if(IsOpened)
            {
                UIBG.SetActive(true);
                inventoryPanel.gameObject.SetActive(true);
                Crosshair.SetActive(false);
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = 0;
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 0;
                // Прекрепляем курсор к середине экрана
                Cursor.lockState = CursorLockMode.None;
                // и делаем его невидимым
                Cursor.visible = true;
            }
            else
            {
                UIBG.SetActive(false);
                inventoryPanel.gameObject.SetActive(false);
                Crosshair.SetActive(true);
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";
                virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "Mouse Y";
                // Прекрепляем курсор к середине экрана
                Cursor.lockState = CursorLockMode.Locked;
                // и делаем его невидимым
                Cursor.visible = false;
            }
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, reachDistance))
                {
                if (hit.collider.gameObject.GetComponent<Item>() != null)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject);
                }
            }

        }

    }
    public void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.item== _item)
            {
                if(slot.amount+_amount<=_item.maximumAmount)
                {
                    slot.amount += _amount;
                    slot.itemAmountText.text = slot.amount.ToString();
                    slot.SetIcon(_item.icon);
                    return;
                }
                continue;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if(slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                if(slot.item.maximumAmount != 1)
                {
                    slot.itemAmountText.text = _amount.ToString();
                }
                
                break;
            }
        }
    }
}
