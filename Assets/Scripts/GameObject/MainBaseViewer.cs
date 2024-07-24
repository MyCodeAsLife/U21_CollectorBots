using TMPro;
using UnityEngine;

[RequireComponent(typeof(MainBase))]
public class MainBaseViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayNumberOfFood;
    [SerializeField] private TMP_Text _displayNumberOfTimber;
    [SerializeField] private TMP_Text _displayNumberOfMarble;

    private MainBase _model;

    private void OnEnable()
    {
        _model = GetComponent<MainBase>();
        _model.FoodQuantityChanged += OnChangeNumberOfFood;
        _model.TimberQuantityChanged += OnChangeNumberOfTimber;
        _model.MarbleQuantityChanged += OnChangeNumberOfMarble;
    }

    private void OnDisable()
    {
        _model.FoodQuantityChanged -= OnChangeNumberOfFood;
        _model.TimberQuantityChanged -= OnChangeNumberOfTimber;
        _model.MarbleQuantityChanged -= OnChangeNumberOfMarble;
    }

    private void Start()
    {
        OnChangeNumberOfFood(0);
        OnChangeNumberOfTimber(0);
        OnChangeNumberOfMarble(0);
    }

    private void OnChangeNumberOfFood(int number) => _displayNumberOfFood.text = "Food: " + number.ToString();
    private void OnChangeNumberOfTimber(int number) => _displayNumberOfTimber.text = "Timber: " + number.ToString();
    private void OnChangeNumberOfMarble(int number) => _displayNumberOfMarble.text = "Marble: " + number.ToString();
}
