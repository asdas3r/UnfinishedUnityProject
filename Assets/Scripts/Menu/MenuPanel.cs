using UnityEngine;

[System.Serializable]
public class MenuPanel
{
    [SerializeField] public string name;
    [SerializeField] public GameObject panelObject;
    [SerializeField] public MenuPanel backPanel { get; private set; }
    [SerializeField] public GameObject firstToSelect;

    public void SetBackPanel(MenuPanel panel)
    {
        backPanel = panel;
    }
}
