using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tab
{
    public string tabID;         // small id, e.g. "Audio"
    public Button headerButton;  // tab header button
    public GameObject content;   // the panel to show/hide
}

public class TabController : MonoBehaviour
{
    public List<Tab> tabs = new List<Tab>();
    public string startTabID = ""; // optional: set default tab id
    public Color activeHeaderColor = Color.white;
    public Color inactiveHeaderColor = Color.grey;

    private Tab currentTab;

    void Start()
    {
        // wire up buttons
        foreach (var t in tabs)
        {
            var tabCopy = t;
            if (tabCopy.headerButton != null)
                tabCopy.headerButton.onClick.AddListener(() => ShowTab(tabCopy.tabID));
        }

        // choose start tab (try PlayerPrefs -> startTabID -> first tab)
        string saved = PlayerPrefs.GetString("lastTab", "");
        if (!string.IsNullOrEmpty(saved)) startTabID = saved;
        if (string.IsNullOrEmpty(startTabID) && tabs.Count > 0) startTabID = tabs[0].tabID;

        ShowTab(startTabID);
    }

    public void ShowTab(string id)
    {
        foreach (var t in tabs)
        {
            bool match = t.tabID == id;
            if (t.content != null) t.content.SetActive(match);

            // update header visuals (if assigned)
            if (t.headerButton != null)
            {
                var img = t.headerButton.GetComponent<Image>();
                if (img != null)
                    img.color = match ? activeHeaderColor : inactiveHeaderColor;

                // optional: change text color if using Text component
                var txt = t.headerButton.GetComponentInChildren<Text>();
                if (txt != null) txt.color = match ? activeHeaderColor : inactiveHeaderColor;
            }

            if (match) currentTab = t;
        }

        // remember last tab
        PlayerPrefs.SetString("lastTab", id);
        PlayerPrefs.Save();
    }

    // optional: allow switching by keyboard, e.g. Tab + Left/Right
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Cycle(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Cycle(1);
    }

    void Cycle(int dir)
    {
        if (tabs.Count == 0) return;
        int idx = tabs.IndexOf(currentTab);
        if (idx < 0) idx = 0;
        idx = (idx + dir + tabs.Count) % tabs.Count;
        ShowTab(tabs[idx].tabID);
    }
}
