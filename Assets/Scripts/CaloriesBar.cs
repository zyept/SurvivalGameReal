using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public Text caloriesCounter;
    public GameObject PlayerState;
    private float currentCalories, maxCalories;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }


    void Update()
    {
        currentCalories = PlayerState.GetComponent<PlayerState>().currentCalories;
        maxCalories = PlayerState.GetComponent<PlayerState>().maxCalories;
        float fillValue = currentCalories / maxCalories;
        slider.value = fillValue;
        caloriesCounter.text = currentCalories + "/" + maxCalories;
    }
}
