using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{ 
    public ItemScriptableObject item;
    public int amount;
    public bool isEmpty = true;
    public GameObject iconGO;
    public TMP_Text itemAmountText;

    private void Awake()
    {
        iconGO = transform.GetChild(0).GetChild(0).gameObject;

        itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetIcon(Sprite icon)
    {
        if (icon != null)
        {
            iconGO.GetComponent<Image>().sprite = icon;
            iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1); // Asigură-te că imaginea nu este ascunsă
        }
        else
        {
            Debug.LogWarning("Sprite-ul este null, nu s-a putut seta iconul.");
        }
    }

}
