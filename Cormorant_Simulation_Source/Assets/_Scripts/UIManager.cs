using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI startSimTextObj;
    public string startSimulationText = "Start";
    public string stopSimulationText = "Stop";
    
    public TextMeshProUGUI timeText;

    public Slider birdCountSlider;
    public Slider treeCountSlider;
    public Slider childrenPerBirdSlider;
    public Slider birthTimeSlider;
    public Slider babyTimeSlider;

    private SimulationManager simulationManager;

    void Awake () {
        simulationManager = gameObject.GetComponent<SimulationManager> ();
    }

    void Start () {
        OnBirdCountSliderValueChanged ();
        OnTreeCountSliderValueChanged ();
        OnChildrenPerBirdSliderValueChanged ();
        OnBirthTimeSliderValueChanged ();
        OnBabyTimeSliderValueChanged ();
    }

    public void ToggleSimulationState () {
        simulationManager.ToggleSimulationState ();
    }

    public void ChangeSimTextToStart () {
        startSimTextObj.text = startSimulationText;
    }

    public void ChangeSimTextToStop () {
        startSimTextObj.text = stopSimulationText;
    }

    public void OnBirdCountSliderValueChanged () {
        simulationManager.ChangeBirdCount ((int)birdCountSlider.value);
    }

    public void OnTreeCountSliderValueChanged () {
        simulationManager.ChangeTreeCount ((int)treeCountSlider.value);
    }

    public void OnChildrenPerBirdSliderValueChanged () {
        simulationManager.ChangeChildrenPerBirdNum ((int)childrenPerBirdSlider.value);
    }

    public void OnBirthTimeSliderValueChanged () {
        simulationManager.ChangeBirthTime (birthTimeSlider.value);
    }

    public void OnBabyTimeSliderValueChanged () {
        simulationManager.ChangeBabyTime (babyTimeSlider.value);
    }

    public void UpdateTimeText (float time) {
        string str = "";
        if (time < 10) str += "0";
        if (time < 1) str += "0";
        str += time.ToString("#.00");
        timeText.text = str;
    }

}
