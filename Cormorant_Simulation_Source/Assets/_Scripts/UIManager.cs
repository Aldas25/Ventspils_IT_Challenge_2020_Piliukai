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
    public string continueSimulationText = "Continue";
    public string restartSimulationText = "Restart";
    
    public TextMeshProUGUI timeText;

    public Slider birdCountSlider;
    public Slider treeCountSlider;
    public Slider childrenPerBirdSlider;
    public Slider birthTimeSlider;
    public Slider babyTimeSlider;

    public GameObject infoPanel;
    public Animator infoAnim;

    private SimulationManager simulationManager;

    private bool simulationWasOn = false;

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
       // Debug.Log ("in change to start");
        startSimTextObj.text = startSimulationText;
    }

    public void ChangeSimTextToStop () {
        startSimTextObj.text = stopSimulationText;
    }

    public void ChangeSimTextToContinue () {
        //Debug.Log ("in change to continue");
        startSimTextObj.text = continueSimulationText;
    }

    public void ChangeSimTextToRestart () {
        //Debug.Log ("in change to continue");
        startSimTextObj.text = restartSimulationText;
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
        string str = "Time: ";
        if (time < 10) str += "0";
        if (time < 1) str += "0";
        str += time.ToString("#.00");
        timeText.text = str;
    }

    public void TurnInfoPanel (bool on) {
        infoPanel.SetActive (on);

        if (on) {
            simulationWasOn = simulationManager.isSimulationStarted ();
            string tmp = (startSimTextObj.text);
            simulationManager.StopSimulation ();
            startSimTextObj.text = tmp;
        } else {
            if (simulationWasOn) {
                simulationWasOn = false;
                simulationManager.StartSimulation ();
            }
        }
    }

    public void SlideInfoPanel (bool left) {
        infoAnim.SetBool ("LeftPanelOn", left);
        infoAnim.SetBool ("PhotoOn", false);
    }

    public void SlideInfoPhoto (bool mid) {
        infoAnim.SetBool ("LeftPanelOn", false);
        infoAnim.SetBool ("PhotoOn", !mid);
    }

}
