using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public Text hydrationCounter;
    public GameObject PlayerState;
    private float currentHydration, maxHydration;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }


    void Update()
    {
        currentHydration = PlayerState.GetComponent<PlayerState>().currentHydrationPercent;
        maxHydration = PlayerState.GetComponent<PlayerState>().maxHydrationPercent;
        float fillValue = currentHydration / maxHydration;
        slider.value = fillValue;
        hydrationCounter.text = currentHydration + "%";
    }
}
