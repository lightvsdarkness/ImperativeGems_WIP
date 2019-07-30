using System.Collections;
using System.Collections.Generic;
using IG.CGrid;
using TMPro;
using UnityEngine;


public enum CellIndication {
    Nothing,
    CellName,
    EnergyMax,
    EnergyCurrent,
    EnergyFlowIn
}

public class UICellIndication : MonoBehaviour {

    public string IndicatedText; 
    [Space]
    public CellIndication IndicatedInfo;
    [Space]
    public TextMeshProUGUI TextIndicator;

    private void Start() {
        if (TextIndicator == null)
            TextIndicator = GetComponent<TextMeshProUGUI>();
    }


    //private void Update() {

    //}

    public void Indicate(CellLogic logic) {
        if(IndicatedInfo == CellIndication.Nothing) {
            IndicatedText = "";
            TextIndicator.enabled = false;
            return;
        }
        else
        TextIndicator.enabled = true;


        if (IndicatedInfo == CellIndication.CellName)
            IndicatedText = logic.gameObject.name;
        else if (logic.Data == null) {
            Debug.LogError("Can't indicate Data because it's null", this);
        }
        else if(IndicatedInfo == CellIndication.EnergyMax)
            IndicatedText = logic.Data.CellStateFormT.EnergyMaxStored.ToString("G3");
        else if (IndicatedInfo == CellIndication.EnergyCurrent)
            IndicatedText = logic.Data.EnergyCurrent.ToString("G3");
        else if (IndicatedInfo == CellIndication.EnergyFlowIn)
            IndicatedText = logic.Data.EnergyReceived.EnergyFlowInExternal.ToString("G3");

        else {
            IndicatedText = "";
            TextIndicator.enabled = false;
        }
        TextIndicator.text = IndicatedText;
    }
    //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
}
