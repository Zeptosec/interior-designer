using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static string FileWithData;
    public bool IsPaused = false;
    public GameObject PausePanel;
    public delegate void Pause(bool state);
    public event Pause OnPause;
    public TMP_InputField InputField;
    public TMP_Text SaveText;
    public Slider SensitivitySlider;
    public TMP_Text SensitivityText;
    private string pathPrefix;

    private void Awake()
    {
        pathPrefix = Application.persistentDataPath + "/Saved/";
        
        if (instance == null)
            instance = this;
        else Debug.LogError("There can only be one GameManager object");
    }

    void Start()
    {
        LoadFromDataFile();
        Selection.instance.OnPanelToggle += Locker;
        ObjectEditor.instance.OnPanelChange += Locker;
        OnPause += UpdateSensitivitySlider;
        LockCursor();
    }

    public void UpdateSensitivitySlider(bool b = true)
    {
        if (b)
        {
            SensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 1.5f);
            SensitivityText.text = string.Format("Sensitivity: {0, 3:f1}",SensitivitySlider.value);
        }
    }

    private void LoadFromDataFile()
    {
        if(FileWithData != null)
        {
            List<SaveData> data = InOut.Load(pathPrefix + FileWithData + ".obdat");
            for (int i = 0; i < data.Count; i++)
            {
                GameObject obj = (GameObject)Resources.Load(data[i].ObjectName);
                var o = Instantiate(obj);
                o.transform.position = new Vector3(data[i].PosX, data[i].PosY, data[i].PosZ);
                o.transform.rotation = Quaternion.Euler(data[i].RotX, data[i].PosY, data[i].PosZ);
                o.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(data[i].R, data[i].G, data[i].B, data[i].A);
                o.transform.SetParent(transform);
            }
        }
    }

    public void OnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Locker(bool val)
    {
        if (val)
            UnlockCursor();
        else LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ObjectEditor.instance.TryToTurnOffColorPanel()) { }
            else if (PausePanel.transform.GetChild(1).gameObject.activeSelf)
            {
                PausePanel.transform.GetChild(1).gameObject.SetActive(false);
                PausePanel.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (ObjectEditor.instance.EditingPanel.activeSelf)
            {
                ObjectEditor.instance.BackToGame();
            }
            else if (Selection.instance.Selected != null || ObjectEditor.instance.Selected != null)
                Selection.instance.Select(null);
            else TogglePause();
        }
    }

    public void OnValChange(string str)
    {
        if (File.Exists($"{pathPrefix}{str}.obdat"))
        {
            SaveText.text = "File already exists. Saving will override the existing file.";
        }
        else
        {
            SaveText.text = "";
        }
    }

    public void OnSavePanelLoad()
    {
        if (FileWithData != null)
            InputField.text = FileWithData;
        else InputField.text = "";
        OnValChange(InputField.text);
    }

    public void Save()
    {
        if (InputField.text.Length > 0)
        {
            string file = $"{pathPrefix}{InputField.text}.obdat";
            try
            {
                InOut.Save(file, transform);
                FileWithData = InputField.text;
                SaveText.text = "Saved successfully.";
            }
            catch (InvalidDataException e)
            {
                SaveText.text = e.Message;
            }
        }
        else
        {
            SaveText.text = "File name wasn't specified.";
        }
    }

    public void TogglePause()
    {
        if (Selection.instance.SelectionPanel.activeSelf)
            Selection.instance.Toggle();
        else
        {
            IsPaused = !PausePanel.activeSelf;
            OnPause?.Invoke(!PausePanel.activeSelf);
            PausePanel.SetActive(!PausePanel.activeSelf);
            Locker(PausePanel.activeSelf);
        }
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
