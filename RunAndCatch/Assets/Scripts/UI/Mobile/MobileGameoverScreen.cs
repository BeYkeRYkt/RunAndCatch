using Photon.Pun;
using UnityEngine.UI;

public class MobileGameoverScreen : UIScreen
{
    public static string ID = "ui_mobile_game_over_menu";

    // ping text
    public Text pingText;

    // gameover text
    public Text gameoverText;

    // reason text
    public Text reasonText;

    // leave button text
    public Text leaveButtonText;

    public void Update()
    {
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

    public override void Initialize()
    {
        mId = ID;

        UpdateUI();

        // update game over text
        gameoverText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_game_over_text").ToUpper();

        // update reason text
        reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_reason_catched_text") + ": " + RoomManager.Instance.HunterNickname;

        // update leave button text
        leaveButtonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_leave_text").ToUpper();
    }

    public override void OnShowScreen()
    {
        base.OnShowScreen();
        UpdateUI();
    }

    public void OnLeaveButtonPressed()
    {
        UIManager uiManager = UIManager.Instance;
        uiManager.CloseGUI();
        ClientNetworkManager manager = FindObjectOfType<ClientNetworkManager>();
        manager.Leave();
    }

    public void UpdateUI(EndReason reason)
    {
        switch (reason)
        {
            case EndReason.FEW_PLAYERS:
                reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_reason_few_players_text");
                break;
            case EndReason.HUNTER_LEAVED:
                reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_reason_hunter_leaved_text");
                break;
            case EndReason.HUNTER_WIN:
                if (GameManager.Instance.playerRole == PlayerRole.HUNTER)
                {
                    reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_you_win_text");
                }
                else
                {
                    reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_reason_hunter_win_text");
                }
                break;
            case EndReason.VICTIMS_WIN:
                if (GameManager.Instance.playerRole == PlayerRole.VICTIM && !GameManager.Instance.isPlayerDead)
                {
                    reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_you_win_text");
                }
                else
                {
                    reasonText.text = LanguageManager.Instance.langReader.getString("ui_game_over_menu_reason_victims_win_text");
                }
                break;
            default:
                reasonText.text = "UNKNOWN_MESSAGE";
                break;
        }
    }
}
