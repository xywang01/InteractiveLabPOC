using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterValues : MonoBehaviour
{
    public Text FIT16;
    public Text PI16;
    public Text TI16;
    public Text FT17;
    public Text FT18;
    public Text TE42;
    public Text TE43;
    public Text TE44;
    public Text TE45;
    public Text TE46;

    public Text[] meterList;

    // Start is called before the first frame update
    public MeterValues()
    {
        // FIT16.text = "60.0";
        // PI16.text = "4.4";
        // TI16.text = "105.7";
        // FT17.text = "3.3";
        // FT18.text = "5.6";
        // TE42.text = "4.9";
        // TE43.text = "32.5";
        // TE44.text = "6.3";
        // TE45.text = "18.5";
        // TE46.text = "9.4";
        // meterList = new Text[] {FIT16, PI16, TI16, FT17, FT18, TE42, TE43, TE44, TE45, TE46};
    }

    private float nextActionTime = 0.0f;
    private float period = 3.0f;

    void Update () {
        // fluctuate the value within +/- 3 of the previous value
        if (Time.time > nextActionTime) {
            nextActionTime += period;
            foreach(Text meterText in meterList) {
                float newVal = Single.Parse(meterText.text) + UnityEngine.Random.Range(-3, 3);

                if (newVal > 0) {
                    meterText.text = newVal.ToString();
                }
            }
        }
    }

    public void changeValue(int id, string val) {
        Debug.Log(meterList.ToString());
        Debug.Log(meterList[id].text.ToString());
        meterList[id].text = val;
    }
}
