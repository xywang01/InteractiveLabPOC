using System;
using System.Collections;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PDSystemState : MonoBehaviour
{
    private int state = -1;
    private int oldState = -1;
    public Text statusUI;
    public Text timer; // time to be displayed in game
    public Text score; // score to be displayed in game
    
    public GameObject partOneText;
    public GameObject partTwoText;
    public GameObject partThreeText;
    public GameObject endScreen;

    public GameObject partOneTextVR;
    public GameObject partTwoTextVR;
    public GameObject partThreeTextVR;
    public GameObject endScreenVR;

    private GameObject _partOneTextActive;
    private GameObject _partTwoTextActive;
    private GameObject _partThreeTextActive;
    private GameObject _endScreenActive;
    
    private bool _modeIsSet = false;

    public Text finalScore; // final score to be displayed in end screen
    public Text finalScoreVR; // final score to be displayed in end screen
    public Text timePlayed; // final time to be displayed in end screen
    public Text timePlayedVR; // final time to be displayed in end screen

    private Text _finalScoreActive;
    private Text _timePlayedActive;
    
    private float timeStart; // internal time variable
    private float currentScore; // internal score variable
    private Transform textContent;
    private TextMeshProUGUI content;
    private int section = 1;
    private TwoWayValve[] twoWayValves;
    private ThreeWayValve[] threeWayValves;
    private CircleValve[] circleValves;
    private PRVValve[] PRVs;
    private InfoGauge[] infoGauges;
    private Boolean partOneComplete = false;
    private Boolean partTwoComplete = false;
    private Boolean partThreeComplete = false;


    void SetTestMode(TestMode testMode)
    {
        Debug.Log($"Test mode is set {testMode} in PDSystemState");
        
        switch (testMode)
        {
            case TestMode.Screen:
                _partOneTextActive = partOneText;
                _partTwoTextActive = partTwoText;
                _partThreeTextActive = partThreeText;
                _endScreenActive = endScreen;
                _finalScoreActive = finalScore;
                _timePlayedActive = timePlayed;
                break;
            case TestMode.VR:
                _partOneTextActive = partOneTextVR;
                _partTwoTextActive = partTwoTextVR;
                _partThreeTextActive = partThreeTextVR;
                _endScreenActive = endScreenVR;
                _finalScoreActive = finalScoreVR;
                _timePlayedActive = timePlayedVR;
                break;
        }
        _modeIsSet = true;
    }

    private void OnEnable()
    {
        TestMode testMode = ModeManagerEvents.GetCurrentMode();
        SetTestMode(testMode);
    }
    
    private void OnDisable()
    {
        // restart();
    }

    void Start()
    {
        timer.text = "Time: 00:00.00";
        score.text = "Score: 0";
        twoWayValves = UnityEngine.Object.FindObjectsOfType<TwoWayValve>();
        threeWayValves = UnityEngine.Object.FindObjectsOfType<ThreeWayValve>();
        circleValves = UnityEngine.Object.FindObjectsOfType<CircleValve>();
        PRVs = UnityEngine.Object.FindObjectsOfType<PRVValve>();
        // textContent = partOneText.transform;
        infoGauges = UnityEngine.Object.FindObjectsOfType<InfoGauge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_modeIsSet)
        {
            Debug.Log("test mode is not set");
            return;
        }
        
        if (partOneComplete && partTwoComplete && partThreeComplete) {
            TimeSpan timePlaying = TimeSpan.FromSeconds(timeStart);
            _finalScoreActive.text = "Score: " + currentScore;
            _timePlayedActive.text = "Time Played: " + timePlaying.ToString("mm':'ss'.'ff");
            _endScreenActive.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        } else {
            timeStart += Time.deltaTime;
            TimeSpan timePlaying = TimeSpan.FromSeconds(timeStart);
            timer.text = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
        }

        updateScore();

        if (section == 1 && !partOneComplete)
        {
            _partOneTextActive.SetActive(true);
            textContent = _partOneTextActive.transform;
        } else if (section == 1 && partOneComplete) {
            section = 2;
            _partOneTextActive.SetActive(false);
            _partTwoTextActive.SetActive(true);
            textContent = _partTwoTextActive.transform;
            state = 0;
            OnChange();
            // _endScreenActive.SetActive(true);
            // textContent = _endScreenActive.transform;
            // state = 0;
            // UpdateEndScreen();
        }
        else if (section == 2 && partTwoComplete) {
            section = 3;
            _partTwoTextActive.SetActive(false);
            _partThreeTextActive.SetActive(true);
            textContent = _partThreeTextActive.transform;
            state = 0;
            OnChange();
        } else if (section == 3 && partThreeComplete) {
            section = 4;
            _partThreeTextActive.SetActive(false);
            _endScreenActive.SetActive(true);
            textContent = _endScreenActive.transform;
            state = 0;
            OnChange();
        }
    }
    
    public void updateScore() {
        float newScore = state * 100;
        if (section == 2) {
            newScore += 900;
        } else if (section == 3) {
            newScore += 1700;
        }

        if (newScore > currentScore) {
            currentScore = newScore;
            score.text = "Score: " + currentScore;
        }
    }

    public void restart() {
        partOneComplete = false;
        partTwoComplete = false;
        partThreeComplete = false;
        timeStart = 0;
        currentScore = 0;
        _endScreenActive.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        updateScore();
    }

    public void OnChange() {
        oldState = state;

        if (section == 1) {
            PartOne();
        } else if (section == 2) {
            PartTwo();
        } else if (section == 3) {
            PartThree();
        }

        if (oldState > state) {
            FindObjectOfType<SoundManager>().Play("Error");
        } else if (oldState < state) {
            FindObjectOfType<SoundManager>().Play("Correct");
        }
    }
    
    // Helper method to set state and update UI text
    private void SetState(int newState, string gauge1Value = null, string gauge2Value = null)
    {
        state = newState;
        statusUI.text = $"Step {newState}";
        if (gauge1Value != null) UpdateGaugeValue("Temp", int.Parse(gauge1Value));
        if (gauge2Value != null) UpdateGaugeValue("PI802", int.Parse(gauge2Value));
    }
    
    private void ExecuteSteps(List<(int currState, Func<bool> condition, Action action)> steps)
    {
        foreach (var (currState, condition, action) in steps)
        {
            Debug.Log($"state {currState} condition {condition} which is {condition()} ");
            if (!condition()) break;
            action();
        }
        UpdateStatus();
    }

    private void PartOne() {
        Debug.Log("In Part One");

        var steps = new List<(int currState, Func<bool> condition, Action action)>
        {
            // todo make them into a two-way valve (from three-way) for now because i can't close the valve in VR
            // (1, () => CheckPosition("HV700", Position.bottom), () => { SetState(1); UpdateGaugeValue("Temp", 10); }),
            // (2, () => CheckPosition("HV701", Position.bottom), () => SetState(2)),
            (1, () => CheckOpen("HV700"), () => { SetState(1); UpdateGaugeValue("Temp", 10); }),
            (2, () => CheckOpen("HV701"), () => SetState(2)),
            // todo same as above make FIC703 a two-way valve for now
            // (3, () => CheckTurn("FIC703", 1), () => SetState(3)),
            (3, () => CheckOpen("FIC703"), () => SetState(3)),
            (4, () => CheckTurn("HV806", 1), () => SetState(4)),
            (5, () => CheckTurn("PRV807", 1), () => { SetState(5); UpdateGaugeValue("PI802", 10); }),
            (6, () => CheckCircle("HV704") && CheckCircle("HV705"), () => SetState(6)),
            (7, () => true, CompletePartOne)
            // (7, () => checkOpen("HV901"), CompletePartOne)
        };

        ExecuteSteps(steps);
    }

    private void PartTwo() {
        Debug.Log("In Part Two");

        var steps = new List<(int currState, Func<bool> condition, Action action)>
        {
            (1, () => CheckOpen("HV203"), () => SetState(1)),
            // todo FIC204 should be of type PRV (count turns), but for now use it as a two-way valve so open/close is a binary operation
            // (2, () => CheckTurn("FIC204", 1), () => SetState(2)),
            (2, () => CheckOpen("FIC204"), () => SetState(2)),
            // (3, () => CheckOpen("HV402") && !CheckOpen("HV403") && CheckOpen("HV404"), () => SetState(3)),
            // (4, () => CheckTurn("FIC401", 1), () => SetState(4)),  // this should be the correct one, but because we later need to shut it down, will change the implementation to a two-way valve so the actions are binary
            (3, () => !CheckOpen("HV403"), () => SetState(3)),
            (4, () => CheckOpen("HV402"), () => SetState(4)),
            (5, () => CheckOpen("HV404"), () => SetState(5)),
            (6, () => CheckOpen("FIC401"), () => SetState(6)),
            (7, () => CheckOpen("HV802"), () => SetState(7)),
            (8, () => CheckTurn("PRV803", 1), () => { SetState(8); UpdateGaugeValue("PI801", 3); CompletePartTwo(); }),
        };

        ExecuteSteps(steps);
    }

    private void PartThree() {
        Debug.Log("In Shutdown Procedure");
        
        statusUI.text = "Shutdown Procedure";
        
        var steps = new List<(int currState, Func<bool> condition, Action action)>
        {
            (1, () => !CheckOpen("FIC401"), () => SetState(1)),
            // (1, () => CheckTurn("FIC401", 0), () => SetState(1)),  // the original was incorrect since this is a PRV need to check for turns
            // todo FIC204 should be of type PRV (count turns), but for now use it as a two-way valve so open/close is a binary operation
            // (2, () => CheckTurn("FIC204", 0), () => SetState(2)),
            (2, () => !CheckOpen("FIC204"), () => SetState(2)),
            (3, () => !CheckOpen("HV203"), () => SetState(3)),
            // (4, () => !CheckOpen("HS201"), () => SetState(4)),  // todo there is no HS201!!!
            (4, () => true, () => SetState(4)),
            (5, () => CheckOpen("HV303"), () => SetState(5)),
            // todo: HS301 is just a switch and not implemented
            (6, () => !CheckOpen("HV402") && CheckOpen("HV403"), () => SetState(6)),
            (7, () => !CheckOpen("FIC703") && !CheckOpen("HV701"), CompletePartThree) // todo update instructions
        };

        ExecuteSteps(steps);
    }
    
    private void CompletePartOne()
    {
        SetState(7);
        partOneComplete = true;
        statusUI.text = "Part 1 Complete! Starting Part 2";
    }

    private void CompletePartTwo()
    {
        SetState(6);
        partTwoComplete = true;
        statusUI.text = "Part 2 Complete! Starting Shutdown Procedure";
    }

    private void CompletePartThree()
    {
        SetState(7);
        partThreeComplete = true;
        statusUI.text = "Shutdown Procedure Complete!";
    }

    // check if a two way valve is open given valve id
    private bool CheckOpen(string id)
    {
        bool isOpen = Array.Find(twoWayValves, v => v.id == id).open;
        OutputManagerEvents.RecordToOutput(id, isOpen ? "Open" : "Close");
        return isOpen;
    }

    // check if a three way valve is in the right position given valve id and target position
    private bool CheckPosition(string id, Position p) {
        bool isOpen = Array.Find(threeWayValves, v => v.id == id).position == p;
        OutputManagerEvents.RecordToOutput(id, isOpen ? "Open" : "Close");
        return isOpen;
    }

    // check if a circle valve is open given valve id
    private bool CheckCircle(string id) {
        bool isOpen = Array.Find(circleValves, v => v.id == id).open;
        OutputManagerEvents.RecordToOutput(id, isOpen ? "Open" : "Close");
        return isOpen;
    }

    // check if a PRV has at least x number of turns
    private bool CheckTurn(string id, int turn)
    {
        int nTurns = Array.Find(PRVs, v => v.id == id).turn;
        Debug.Log($"PRV valve {id} has {Array.Find(PRVs, v => v.id == id).turn} turns");
        OutputManagerEvents.RecordToOutput(id, nTurns.ToString());
        return  nTurns >= turn;
    }

    // update the value of an info gauge
    private void UpdateGaugeValue(string id, int value) {
        InfoGauge target = Array.Find(infoGauges, g => g.id == id);
        target.updateValue(value);
    }

    private void UpdateEndScreen()
    {
        // update the child components FinalScore and TimePlayed
        _finalScoreActive.text = "Score: " + currentScore;
        TimeSpan timePlaying = TimeSpan.FromSeconds(timeStart);
        _timePlayedActive.text = "Time Played: " + timePlaying.ToString("mm':'ss'.'ff");
    }

    private void UpdateStatus() {
        Debug.Log($"Update status, current state {state}");
        int index = 0;
        
        for (int i = 1; i < textContent.transform.childCount; ++i) {
            index++;
            Transform instruction = textContent.transform.GetChild(i);
            content = instruction.GetComponent<TextMeshProUGUI>();

            if (index <= state) {
                content.fontStyle = FontStyles.Strikethrough;
            } else {
                content.fontStyle = FontStyles.Normal;
            }
        }
    }
}
