using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown DropDownSaved;
    public TMP_Text DropDownError;
    public TMP_Dropdown DropDownResolutions;
    public Toggle FullScreenToggle;

    private string pathPrefix;
    private void Awake()
    {
        pathPrefix = Application.persistentDataPath + "/Saved/";
        if (PlayerPrefs.HasKey("res"))
        {
            SelectResolution(PlayerPrefs.GetInt("res"));
        }
        else
        {
            int currRes = currResolution();
            if(currRes != -1)
                SelectResolution(currRes);
        }
        if (PlayerPrefs.HasKey("isfull"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("isfull") == 1 ? true : false;
        }
        else
        {
            PlayerPrefs.SetInt("isfull", Screen.fullScreen ? 1 : 0);
        }
    }

    public void StartGame()
    {
        LoadGame(null);
        if (!Directory.Exists(pathPrefix))
            Directory.CreateDirectory(pathPrefix);
    }

    public void LoadGame(string filePath)
    {
        GameManager.FileWithData = filePath;
        SceneManager.LoadScene("Game");
    }

    public void OnDropDownLoad()
    {
        if(!Directory.Exists(pathPrefix))
            Directory.CreateDirectory(pathPrefix);
        DropDownSaved.ClearOptions();
        DropDownError.text = "";
        List<string> names = InOut.GetAllFileNames(pathPrefix);
        if (names.Count == 0)
            DropDownError.text = "There are no saved files.";
        else
        {
            DropDownSaved.AddOptions(names);
        }
    }

    public void OnDropDownChanged(int val)
    {
        var data = DropDownSaved.options[val];
        string path = $"{pathPrefix}{data.text}.obdat";
        if (!File.Exists(path))
        {
            OnDropDownLoad();
            DropDownError.text = "File wasn't found. Possible that it was removed.";
        }
    }

    public void Delete()
    {
        if (DropDownSaved.options.Count == 0)
            DropDownError.text = "First select a file to delete.";
        else
        {
            var data = DropDownSaved.options[DropDownSaved.value];
            string path = $"{pathPrefix}{data.text}.obdat";
            if (File.Exists(path))
                File.Delete(path);
            OnDropDownLoad();
        }
    }

    public void LoadFromDropDown()
    {
        if (DropDownSaved.options.Count == 0)
        {
            DropDownError.text = "There are no files to load.";
            return;
        }
        string path = $"{pathPrefix}{DropDownSaved.options[DropDownSaved.value].text}.obdat";
        if (!File.Exists(path))
        {
            OnDropDownLoad();
            DropDownError.text = "File wasn't found. Possible that it was removed.";
        }
        else
        {
            GameManager.FileWithData = DropDownSaved.options[DropDownSaved.value].text;
            SceneManager.LoadScene("Game");
        }
    }

    private int currResolution()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if(Screen.width == Screen.resolutions[i].width && Screen.height == Screen.resolutions[i].height)
            {
                return i;
            }
        }
        return -1;
    }

    public void OnOptionsLoad()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }
        DropDownResolutions.ClearOptions();
        DropDownResolutions.AddOptions(options);
        DropDownResolutions.value = PlayerPrefs.GetInt("res");
        FullScreenToggle.isOn = PlayerPrefs.GetInt("isfull") == 1 ? true : false;
    }

    public void SelectResolution(int val)
    {
        Resolution res = Screen.resolutions[val];
        bool fullsc = PlayerPrefs.HasKey("isfull") ? (PlayerPrefs.GetInt("isfull") == 1 ? true : false) : true;
        Screen.SetResolution(res.width, res.height, fullsc);
        PlayerPrefs.SetInt("res", val);
    }

    public void ToggleFullscreen(bool val)
    {
        Screen.fullScreen = val;
        PlayerPrefs.SetInt("isfull", val ? 1 : 0);
    }

    public void Quit()
    {
        Application.Quit(0);
    }
}