using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTabSwitcher : MonoBehaviour
{
    public GameObject upgradesTab;
    public GameObject equipmentTab;
    public GameObject barTab;

    public void SwitchToEquipment()
    {
        equipmentTab.SetActive( true );
        upgradesTab.SetActive( false );
        barTab.SetActive( false );
    }

    public void SwitchToUpgrades()
    {
        upgradesTab.SetActive( true );
        equipmentTab.SetActive( false );
        barTab.SetActive( false );
    }

    public void SwitchToBar()
    {
        upgradesTab.SetActive( false );
        equipmentTab.SetActive( false );
        barTab.SetActive( true );
    }
}
