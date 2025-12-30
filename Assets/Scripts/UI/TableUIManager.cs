using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class TableUIManager : MonoBehaviour
{
    [SerializeField] private GameObject rowPrefab;

    private InfoSingleton _info = InfoSingleton.GetInstance();

    List<RowUI> rows = new List<RowUI>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIEvents.OnEndGameEnter.AddListener(Restart);
        UIEvents.OnRestartGame.AddListener(Restart);
        CheckRows();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Restart()
    {
        List<RowUI> rowsCopy = rows.ToList();

        foreach (RowUI row in rowsCopy)
        {
            rows.Remove(row);
            Destroy(row.gameObject);
        }
        
    }
    
    void CheckRows()
    {
        while (rows.Count < Mathf.Ceil(((float)Player.UnlockedAirports.Count - 1) / 2))
        {
            CreateRow();
        }
    }

    public void UpdateTable(Airport airport)
    {
        CheckRows();
        var orderedTravellers = airport.TravellersToAirport.Where(kv1 => Player.UnlockedAirports.Contains(kv1.Key)).OrderByDescending(kv2 => kv2.Value).ToList();

        

        for (int i = 0; i < orderedTravellers.Count; i += 2)
        {
            var kv1 = orderedTravellers[i];
            
            string v1 = $"{kv1.Key.Id.ToUpper()}: {kv1.Value}";
            string v2 = null;
            if (i + 1 < orderedTravellers.Count)
            {
                var kv2 = orderedTravellers[i + 1];
                v2 = $"{kv2.Key.Id.ToUpper()}: {kv2.Value}";
            }

            rows[(i / 2)].UpdateValues(v1, v2);
        }

        //if (orderedTravellers.Count % 2 != 0)
        //{
        //    var kv1 = orderedTravellers[orderedTravellers.Count - 1];
        //    rows[rows.Count - 1].UpdateValues(v1: $"{kv1.Key.Id.ToUpper()}: {kv1.Value}");
        //}
    }

    private void CreateRow()
    {
        GameObject newRow = Instantiate(rowPrefab, transform, false);

        RectTransform rt = newRow.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0f);
        rt.localScale = Vector3.one;
        rt.anchoredPosition3D = new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0f);

        rows.Add(newRow.GetComponent<RowUI>());

    }
}
