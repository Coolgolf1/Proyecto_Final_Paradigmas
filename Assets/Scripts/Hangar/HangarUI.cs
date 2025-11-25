using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HangarUI : MonoBehaviour
{
    //[SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button buttonBack;
    [SerializeField] private TMP_Text airplaneID;
    private InfoSingleton _info;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _info = InfoSingleton.GetInstance();
        airplaneID.text = _info.airplaneToHangar.TailNumber;
        buttonBack.onClick.AddListener(_info.GoToSpace);
        GameObject airplaneGO = Instantiate(_info.airplaneToHangar.gameObject, new Vector3(0,0,0), new Quaternion(0, 0.306609124f, 0, 0.951835513f));
        airplaneGO.GetComponentInChildren<TrailRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
