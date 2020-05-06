using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public Text hMultikillText;
    public Text hKillAmountText;
    public Text hCoinAmountText;
    public Text hTreasureChestAmountText;
    public Text totalKillsText;

    public void Initialize()
    {
        DataManager dataManager = DataManager.singleton;
        hMultikillText.text = "Biggest multikill: " + dataManager.GetSaveValue( ESave_Values_Keys.HIGHEST_KILLSTREAK ).ToString();
        hKillAmountText.text = "Highest kill amount: " + dataManager.GetSaveValue( ESave_Values_Keys.HIGHEST_KILLSAMOUNT ).ToString();
        hCoinAmountText.text = "Most coins earned: " + dataManager.GetSaveValue( ESave_Values_Keys.HIGHEST_COINSAMOUNT ).ToString();
        hTreasureChestAmountText.text = "Most treasure chests stolen: " + dataManager.GetSaveValue( ESave_Values_Keys.HIGHEST_TREASURECHESTAMOUNT ).ToString();
        totalKillsText.text = "Total kills: " + dataManager.GetSaveValue( ESave_Values_Keys.KILLS_AMOUNT ).ToString();
    }

}
