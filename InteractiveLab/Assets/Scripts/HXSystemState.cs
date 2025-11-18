using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HXSystemState : MonoBehaviour
{
    private int state = -1;
    private int oldState = -1;

    public GameObject CondensationTrapOne;
    public GameObject CondensationTrapTwo;
    public Text statusUI;
    public Text timer; // time to be displayed in game
    public Text timerVR;
    public Text score; // score to be displayed in game
    
    public GameObject partOneText;
    public GameObject partTwoText;
    public GameObject partThreeText;
    public GameObject shutdownProcedureText;
    public GameObject endScreen;

    public GameObject partOneTextVR;
    public GameObject partTwoTextVR;
    public GameObject partThreeTextVR;
    public GameObject shutdownProcedureTextVR;
    public GameObject endScreenVR;
    
    private GameObject _partOneTextActive;
    private GameObject _partTwoTextActive;
    private GameObject _partThreeTextActive;
    private GameObject _shutdownProcedureTextActive;
    private GameObject _endScreenActive;

    public MeterValues meters;
    public Text finalScore; // final score to be displayed in end screen
    public Text timePlayed; // final time to be displayed in end screen
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
    private Boolean shutDownComplete = false;

    private void SetTestMode(TestMode testMode)
    {
        Debug.Log("Test mode is set in HXSystemState");
        switch (testMode)
        {
            case TestMode.Screen:
                _partOneTextActive = partOneText;
                _partTwoTextActive = partTwoText;
                _partThreeTextActive = partThreeText;
                _shutdownProcedureTextActive = shutdownProcedureText;
                _endScreenActive = endScreen;
                break;
            case TestMode.VR:
                _partOneTextActive = partOneTextVR;
                _partTwoTextActive = partTwoTextVR;
                _partThreeTextActive = partThreeTextVR;
                _shutdownProcedureTextActive = shutdownProcedureTextVR;
                _endScreenActive = endScreenVR;
                break;
        }
    }

    private void OnEnable()
    {
        TestMode testMode = ModeManagerEvents.GetCurrentMode();
        SetTestMode(testMode);
    }
    
    void OnDisable() {
        Restart();
    }

    private void Start() {
        timer.text = "Time: 00:00.00";
        timerVR.text = timer.text;
        score.text = "Score: 0";
        twoWayValves = FindObjectsOfType<TwoWayValve>();
        threeWayValves = FindObjectsOfType<ThreeWayValve>();
        circleValves = FindObjectsOfType<CircleValve>();
        PRVs = FindObjectsOfType<PRVValve>();
        infoGauges = FindObjectsOfType<InfoGauge>();
        textContent = _partOneTextActive.transform;

        StartupCheck();
    }

    public void ChangeTube() {
        _partOneTextActive.SetActive(true);
        _partTwoTextActive.SetActive(false);
        _partThreeTextActive.SetActive(false);
        _shutdownProcedureTextActive.SetActive(false);
        ResetValves();
        section = 1;
        state = 1;
        textContent = _partOneTextActive.transform;
    }

    public void ChangePlate() {
        partOneComplete = true;
        _partOneTextActive.SetActive(false);
        _partTwoTextActive.SetActive(true);
        _partThreeTextActive.SetActive(false);
        _shutdownProcedureTextActive.SetActive(false);
        ResetValves();
        section = 2;
        state = 1;
        textContent = _partTwoTextActive.transform;
    }

    public void ChangeColumn() {
        partOneComplete = true;
        partTwoComplete = true;
        _partOneTextActive.SetActive(false);
        _partTwoTextActive.SetActive(false);
        _partThreeTextActive.SetActive(true);
        _shutdownProcedureTextActive.SetActive(false);
        ResetValves();
        section = 3;
        state = 1;
        textContent = _partThreeTextActive.transform;
    }

    private void Update() {
        if (partOneComplete && partTwoComplete && partThreeComplete && shutDownComplete) {
            TimeSpan timePlaying = TimeSpan.FromSeconds(timeStart);
            finalScore.text = "Score: " + currentScore;
            timePlayed.text = "Time Played: " + timePlaying.ToString("mm':'ss'.'ff");
            _endScreenActive.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        } else {
            timeStart += Time.deltaTime;
            TimeSpan timePlaying = TimeSpan.FromSeconds(timeStart);
            timer.text = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timerVR.text = timer.text;
        }

        UpdateScore();

        // for testing only (completes all procedures)
        if (Input.GetKeyDown("v")) {
            partOneComplete = true;
            partTwoComplete = true;
            partThreeComplete = true;
            shutDownComplete = true;
        }

        // completes individual procedures
        if (Input.GetKeyDown("j")) {
            _partOneTextActive.SetActive(true);
            _partTwoTextActive.SetActive(false);
            _partThreeTextActive.SetActive(false);
            _shutdownProcedureTextActive.SetActive(false);
            ResetValves();
            section = 1;
            state = 1;
            textContent = _partOneTextActive.transform;
        } else if (Input.GetKeyDown("k")) {
            partOneComplete = true;
            _partOneTextActive.SetActive(false);
            _partTwoTextActive.SetActive(true);
            _partThreeTextActive.SetActive(false);
            _shutdownProcedureTextActive.SetActive(false);
            ResetValves();
            section = 2;
            state = 1;
            textContent = _partTwoTextActive.transform;
        } else if (Input.GetKeyDown("l")) {
            partOneComplete = true;
            partTwoComplete = true;
            _partOneTextActive.SetActive(false);
            _partTwoTextActive.SetActive(false);
            _partThreeTextActive.SetActive(true);
            _shutdownProcedureTextActive.SetActive(false);
            ResetValves();
            section = 3;
            state = 1;
            textContent = _partThreeTextActive.transform;
        }

        if (Input.GetKeyDown("c")) {
            if (
                (section == 1 && state == 9) || 
                (section == 2 && state == 8) ||
                (section == 3 && state == 11)
            ){
                section = 4;
                textContent = _shutdownProcedureTextActive.transform;
                _partOneTextActive.SetActive(false);
                _partTwoTextActive.SetActive(false);
                _partThreeTextActive.SetActive(false);
                _shutdownProcedureTextActive.SetActive(true);
                Shutdown();
            } else if (section == 4 && state == 6){
                if (partOneComplete && partTwoComplete && partThreeComplete) {
                    shutDownComplete = true;
                }
                if (partOneComplete && partTwoComplete) {
                    section = 3;
                    textContent = _partThreeTextActive.transform;
                    _partOneTextActive.SetActive(false);
                    _partTwoTextActive.SetActive(false);
                    _partThreeTextActive.SetActive(true);
                    _shutdownProcedureTextActive.SetActive(false);
                    PartThree();
                } else if (partOneComplete) {
                    section = 2;
                    textContent = _partTwoTextActive.transform;
                    _partOneTextActive.SetActive(false);
                    _partTwoTextActive.SetActive(true);
                    _partThreeTextActive.SetActive(false);
                    _shutdownProcedureTextActive.SetActive(false);
                    PartTwo();
                }
            }
        }
    }

    void UpdateScore() {
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

    public void Restart() {
        partOneComplete = false;
        partTwoComplete = false;
        partThreeComplete = false;
        timeStart = 0;
        currentScore = 0;
        _endScreenActive.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        UpdateScore();
    }

    public void OnChange() {
        ClearCondensationCheck();
        oldState = state;

        if (state == -1) {
            StartupCheck();
        }

        if (state > -1) {
            if (section == 1) {
                PartOne();
            } else if (section == 2) {
                PartTwo();
            } else if (section == 3) {
                PartThree();
            } else {
                Shutdown();
            }
        }

        if (oldState > state) {
            FindObjectOfType<SoundManager>().Play("Error");
        } else if (oldState < state) {
            FindObjectOfType<SoundManager>().Play("Correct");
        }
    }

    private bool StartupCheck() {
        // check all specified valves are closed
        string[] closedValves = {"V111", "V121", "V128", "V114", "V130", "V132", "V134"};
        foreach(TwoWayValve v in twoWayValves) {
            if (Array.Exists(closedValves, x => x == v.id) && v.open) {
                state = -1;
                statusUI.text = "Start up checklist unmet";
                return false;
            }
        }

        foreach(CircleValve v in circleValves) {
            if (Array.Exists(closedValves, x => x == v.id) && v.open) {
                state = -1;
                statusUI.text = "Start up checklist unmet";
                return false;
            }
        }
        
        // check liquid level in condensation traps
        if (CondensationTrapOne.GetComponent<CondensationTrap>().liquidLevel + CondensationTrapTwo.GetComponent<CondensationTrap>().liquidLevel > 0) {
            state = -1;
            statusUI.text = "Start up checklist unmet";
            return false;
        }

        if (state == -1) {
            state = 0;
        }

        statusUI.text = "Start up checklist met";
        return true;
    }

    private void ClearCondensationCheck() {

        if (CheckOpen("V125")) {
            CondensationTrapOne.GetComponent<CondensationTrap>().ClearLiquidLevel();
        }

        if (CheckOpen("V126")) {
            CondensationTrapTwo.GetComponent<CondensationTrap>().ClearLiquidLevel();
        }
    }
    
    private void SetState(int newState, string meterValue0 = null, string meterValue1 = null) {
        Debug.Log($"Part {section} step {newState}");
        
        state = newState;
        statusUI.text = $"Part {section}: step {state}";
        if (meterValue0 != null) meters.changeValue(0, meterValue0);
        if (meterValue1 != null) meters.changeValue(1, meterValue1);
    }

    private void ResetToStartup() {
        Debug.Log("Reset to start up!!!");
        Debug.Log("=================");
        state = 0;
        StartupCheck();
    }

    private void PartOne() {
        Debug.Log("In Part One");
        var steps = new List<(int currState, Func<bool> condition, Action action)> {
            (1, () => CheckPosition("V122", Position.left), () => SetState(1, "70.0", "6.0")),
            (2, () => CheckCircle("V121"), () => SetState(2, "78.0", "8.0")),
            (3, () => CheckPosition("V131", Position.left), () => SetState(3)),
            (4, () => CheckPosition("V119", Position.left) && CheckPosition("V123", Position.left), () => SetState(4)),
            (5, () => CheckPosition("V124", Position.left) && CheckOpen("V125"), () => SetState(5)),
            (6, () => CheckCircle("V132"), () => SetState(6)),
            (7, () => CheckTurn("PRV10", 1), () => SetState(7)),
            (8, () => CheckCircle("V111"), () => SetState(8)),
            (9, () => CheckTurn("PRV10", 3), CompletePartOne)
        };

        foreach (var (currState, condition, action) in steps) {
            if (!condition()) break;
            action();
        }
        UpdateStatus();
        Debug.Log("===============================");
    }

    private void PartTwo() {
        Debug.Log("In Part Two");
        
        // Define each step with conditions and actions in a sequence for Part 2
        var steps = new List<(Func<bool> condition, Action action)> {
            (() => CheckPosition("V122", Position.left), () => SetState(1)),
            (() => CheckCircle("V121"), () => SetState(2)),
            (() => CheckPosition("V119", Position.left), () => SetState(3)),
            (() => CheckPosition("V123", Position.left), () => SetState(4)),
            (() => CheckCircle("V132"), () => SetState(5)),
            (() => CheckTurn("PRV10", 1), () => SetState(6)),
            (() => CheckCircle("V111"), () => SetState(7)),
            (() => CheckTurn("PRV10", 3), CompletePartTwo)
        };

        foreach (var (condition, action) in steps) {
            if (!condition())
                break;
            
            action();
        }
        UpdateStatus();
        Debug.Log("===============================");
    }

    private void PartThree() {
        Debug.Log("In Part Three");
        
        // Define each step with conditions and actions for Part 3
        var steps = new List<(Func<bool> condition, Action action)> {
            (() => CheckPosition("V112", Position.left) && CheckPosition("V113", Position.left), () => SetState(1)),
            (() => !CheckOpen("V115") && !CheckOpen("V116"), () => SetState(2)),
            (() => CheckCircle("V128") && CheckTurn("PRV12", 1), () => SetState(3)),
            (() => CheckPosition("V131", Position.left), () => SetState(4)),
            (() => CheckOpen("V126"), () => SetState(5)),
            (() => CheckPosition("V118", Position.left), () => SetState(6)),
            (() => CheckCircle("V130"), () => SetState(7)),
            (() => CheckTurn("PRV10", 3), () => SetState(8)),
            (() => CheckTurn("PRV10", 1), () => SetState(9)),
            (() => CheckCircle("V111"), () => SetState(10)),
            (() => CheckTurn("PRV10", 3), CompletePartThree)
        };

        foreach (var (condition, action) in steps) {
            if (!condition())
                break;
            
            action();
        }
        UpdateStatus();
        Debug.Log("===============================");
    }

    private void Shutdown() {
        Debug.Log("In Shutdown");
        
        statusUI.text = "Shutdown Procedure";

        // Define each step with conditions and actions for the Shutdown process
        var steps = new List<(Func<bool> condition, Action action)> {
            (() => !CheckCircle("V111") && CheckPosition("V112", Position.top) && CheckPosition("V113", Position.top), () => SetState(1)),
            (() => !CheckOpen("V115") && !CheckOpen("V116") && CheckPosition("V118", Position.top), () => SetState(2)),
            (() => CheckPosition("V122", Position.top) && CheckPosition("V123", Position.top) && CheckPosition("V124", Position.top), () => SetState(3)),
            (() => !CheckOpen("V125") && !CheckOpen("V126"), () => SetState(4)),
            (() => !CheckCircle("V128") && !CheckCircle("V130"), () => SetState(5)),
            (() => CheckPosition("V131", Position.top) && !CheckCircle("V133") && !CheckCircle("V134"), CompleteShutdown)
        };

        foreach (var (condition, action) in steps) {
            if (!condition())
                break;
            
            action();
        }
        UpdateStatus();
        Debug.Log("===============================");
    }

    private void CompletePartOne() {
        state = 9;
        partOneComplete = true;
        statusUI.text = "Part 1 Complete! Press [C] to perform shutdown procedure";
    }

    private void CompletePartTwo() {
        state = 8;
        partTwoComplete = true;
        statusUI.text = "Part 2 Complete! Press [C] to perform shutdown procedure";
    }

    private void CompletePartThree() {
        state = 11;
        partThreeComplete = true;
        statusUI.text = "Part 3 Complete! Press [C] to perform shutdown procedure";
    }

    private void CompleteShutdown() {
        state = 6;
        statusUI.text = "Shutdown Complete! Press [C] to go to next section";
    }

    // reset valves
    private void ResetValves() {
        foreach (var valve in circleValves)
        {
            valve.Reset();
        }

        foreach (var valve in PRVs)
        {
            valve.Reset();
        }

        foreach (var valve in twoWayValves)
        {
            valve.Reset();
        }

        foreach (var valve in threeWayValves)
        {
            valve.Reset();
        }
    }

    // check if a two way valve is open given valve id
    private bool CheckOpen(string id) {
        return Array.Find(twoWayValves, v => v.id == id).open;
    }

    // check if a three way valve is in the right position given valve id and target position
    private bool CheckPosition(string id, Position p)
    {
        bool result = Array.Find(threeWayValves, v => v.id == id).position == p;
        Debug.Log($"In check position, valve {id} is in position {p} : {result}");
        return result;
    }

    // check if a circle valve is open given valve id
    private bool CheckCircle(string id) {
        bool result = Array.Find(circleValves, v => v.id == id).open;
        Debug.Log($"In check circle, valve {id} is open : {result}");
        return result;
    }

    // check if a PRV has at least x number of turns
    private bool CheckTurn(string id, int turn) {
        bool result = Array.Find(PRVs, v => v.id == id).turn >= turn;
        Debug.Log($"In check turn, PRV {id} has at least {turn} turns : {result}");
        return result;
    }

    // update the value of an info gauge
    private void UpdateGaugeValue(string id, int value) {
        InfoGauge target = Array.Find(infoGauges, g => g.id == id);
        target.updateValue(value);
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
                Debug.Log("Strikethrough");
            } else {
                content.fontStyle = FontStyles.Normal;
            }
        }
    }
}
