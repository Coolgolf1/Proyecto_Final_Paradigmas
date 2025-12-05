using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HangarUI : MonoBehaviour
{
    //[SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button buttonBack;
    [SerializeField] private TMP_Text airplaneID;
    [SerializeField] private TMP_Text airplaneStats;
    [SerializeField] private Button buttonUpgrade;
    [SerializeField] private TMP_Text upgradeText;
    private InfoSingleton _info;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _info = InfoSingleton.GetInstance();
        airplaneID.text = _info.airplaneToHangar.TailNumber;
        buttonBack.onClick.AddListener(_info.GoToSpace);
        GameObject airplaneGO = Instantiate(_info.airplaneToHangar.gameObject, new Vector3(0,0,0), new Quaternion(0, 0.306609124f, 0, 0.951835513f));
        airplaneGO.GetComponentInChildren<TrailRenderer>().enabled = false;
        SetLayerRecursively(airplaneGO, "HangarObjects");

        buttonUpgrade.onClick.AddListener(UpgradePlane);

        airplaneStats.text = $"- Speed: {_info.airplaneToHangar.Speed} km/h\n- Capacity: {_info.airplaneToHangar.Capacity} pass.\n- Range: {_info.airplaneToHangar.Range} km";
        

        var escena = SceneManager.GetSceneByName("Hangar");
        SceneManager.MoveGameObjectToScene(airplaneGO, escena);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetLayerRecursively(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerName);
        }
    }

    void UpgradePlane()
    {

    }
}
