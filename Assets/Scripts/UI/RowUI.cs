using UnityEngine;
using TMPro;

public class RowUI : MonoBehaviour
{

    [SerializeField] private TMP_Text cell1;
    [SerializeField] private TMP_Text cell2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValues(string v1 = "", string v2 = "")
    {
        cell1.text = v1;
        cell2.text = v2;

    }
}
