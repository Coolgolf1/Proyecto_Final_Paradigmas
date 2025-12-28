using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HangarUI : MonoBehaviour
{
    //[SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Button buttonBack;
    [SerializeField] private TMP_Text airplaneID;
    [SerializeField] private Button buttonUpgrade;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private TMP_Text airplaneSpeed;
    [SerializeField] private TMP_Text speedUpgrade;
    [SerializeField] private TMP_Text airplaneRange;
    [SerializeField] private TMP_Text airplaneCapacity;
    private InfoSingleton _info;

    private EconomyManager _economy = EconomyManager.GetInstance();

    private int _upgradeQuantity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _info = InfoSingleton.GetInstance();
        airplaneID.text = _info.airplaneInHangar.TailNumber;
        buttonBack.onClick.AddListener(_info.GoToSpace);
        GameObject airplaneGO = Instantiate(_info.airplaneInHangar.gameObject, new Vector3(0, 0, 0), new Quaternion(0, 0.306609124f, 0, 0.951835513f));
        airplaneGO.GetComponentInChildren<TrailRenderer>().enabled = false;
        SetLayerRecursively(airplaneGO, "HangarObjects");

        buttonUpgrade.onClick.AddListener(UpgradePlane);
        UpdateInfo();

        var escena = SceneManager.GetSceneByName("Hangar");
        SceneManager.MoveGameObjectToScene(airplaneGO, escena);
    }

    private void UpdateInfo()
    {

        airplaneSpeed.text = $"{(int)(_info.airplaneInHangar.Speed * 5)} km/h";
        airplaneCapacity.text = $"{_info.airplaneInHangar.Capacity}";
        airplaneRange.text = $"{_info.airplaneInHangar.Range} km";

        _upgradeQuantity = (int)(_info.airplaneInHangar.Speed * 5 * 0.15);

        Image buttonImage = buttonUpgrade.GetComponent<Image>();
        if (_economy.GetBalance() > _upgradeQuantity * 100)
        {
            buttonImage.color = new Color(57, 205, 77);
            buttonUpgrade.interactable = true;
        }
        else
        {
            buttonImage.color = new Color(166, 166, 166);
            buttonUpgrade.interactable = false;
        }


        if (_info.airplaneInHangar.Level != Levels.Elite)
        {
            speedUpgrade.text = $"+{_upgradeQuantity}";
            upgradeText.text = $"{Auxiliary.FormatValue(_upgradeQuantity * 100)}";
        }
        else
        {
            speedUpgrade.text = "";
            upgradeText.text = "Max Level";
            buttonImage.color = new Color(166, 166, 166);
            buttonUpgrade.interactable = false;
        }

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
        if (_info.airplaneInHangar.Level != Levels.Elite)
        {
            if (_economy.SubtractCoins(_upgradeQuantity * 100))
            {
                _info.airplaneInHangar.Upgrade();
                UpdateInfo();

            }
        }
    }
}
