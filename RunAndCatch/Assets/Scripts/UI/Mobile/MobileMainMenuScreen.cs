using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MobileMainMenuScreen : UIScreen
{
    public static string ID = "ui_mobile_main_menu";

    // ping text
    public Text pingText;

    // language depend labels
    public Text nicknameLabel;
    public Text customizeLabel;
    public Text startLabel;
    public Text settingsLabel;

    // nickname field
    public InputField nicknameField;

    // Update is called once per frame
    void Update()
    {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            // Quit the application
            Application.Quit();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // update ping text
        if (ClientLobbyManager.Instance.IsConnected())
        {
            pingText.text = LanguageManager.Instance.langReader.getString("ui_ping_text") + ": " + PhotonNetwork.GetPing() + " ms";
        }
    }

    public override void OnShowScreen()
    {
        base.OnShowScreen();
        UpdateUI();
    }

    public override void Initialize()
    {
        mId = ID;

        // set random nickname
        string nickname = PlayerPrefs.GetString("nickname");
        if(nickname == null || nickname.Equals(""))
        {
            nickname = "default" + Random.Range(0, 100);
        }

        PhotonNetwork.NickName = nickname;
        nicknameField.text = PhotonNetwork.NickName;

        nicknameLabel.text = LanguageManager.Instance.langReader.getString("ui_main_menu_nickname_text") + ":";
        customizeLabel.text = LanguageManager.Instance.langReader.getString("ui_main_menu_customize_text").ToUpper();
        startLabel.text = LanguageManager.Instance.langReader.getString("ui_main_menu_start_game_text").ToUpper();
        settingsLabel.text = LanguageManager.Instance.langReader.getString("ui_main_menu_settings_text").ToUpper();
    }

    public void OnCustomizeButtonPress()
    {
        UIManager uiManager = UIManager.Instance;
    }

    public void OnPlayButtonPress()
    {
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        uiManager.OpenGUI(MobilePlayMenuScreen.ID);
    }

    public void OnSettingsButtonPress()
    {
        UIManager uiManager = UIManager.Instance;
    }

    public void OnNicknameValueChangedEnd()
    {
        // update nickname
        PhotonNetwork.NickName = nicknameField.text;
        PlayerPrefs.SetString("nickname", PhotonNetwork.NickName);
    }
}
