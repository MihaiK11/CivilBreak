using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class StatManager : MonoBehaviour
{

    public Text moneyText;      
    public Slider popularitySlider; 
    public Slider moraleSlider;   
    public Slider criminalitySlider;


    public float money;
    public float popularity;
    public float morale;
    public float criminality;
    // Start is called before the first frame update
    void Start()
    {
        money = 1000f;
        popularity = 50f;
        morale = 70f;
        criminality = 30f;

        UpdateStatsUI();
    }

    // Update is called once per frame
    void Update()
    {
        money += Time.deltaTime * 10;  
        popularity += Mathf.Sin(Time.time) * 2;  
        UpdateStatsUI();
    }
    public void UpdateStatsUI()
    {
        moneyText.text = "$" + Mathf.FloorToInt(money).ToString();
        popularitySlider.value = Mathf.Clamp01(popularity);  
        moraleSlider.value = Mathf.Clamp01(morale);         
        criminalitySlider.value = Mathf.Clamp01(criminality);
    }
}
