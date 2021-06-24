using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

/// <summary>
/// Source: https://github.com/playgameservices/play-games-plugin-for-unity
/// </summary>
public class GpsManager : Singleton<GpsManager>
{
    public static Action<bool, string> OnSaveDataLoaded;
    public static Action OnSaveSuccess;
    [SerializeField] private bool disableGPS;
    [SerializeField] private bool showDebugLogs = false;
    private bool isProcessing;
    private bool isPlayerDataLoaded = false;
    public string saveDataLoadedString;
    private const string saveName = "SaveData";
    public ISavedGameMetadata metaData;

    private void Start()
    {
        if (disableGPS)
        {
            OnSaveDataLoaded?.Invoke(false, Player.LoadLocalData());
        }
        else
        {
            GpsSignIn();
            StartCoroutine(LoadLocalSaveAfterDelay());
        }
    }

    #region Init and Login
    public void GpsSignIn()
    {
        //OnSaveDataLoaded?.Invoke(false, Player.LoadLocalData());
        //return;
        ////Will have to edit later
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = showDebugLogs;
        PlayGamesPlatform.Activate();
        Hud.AddHudText?.Invoke("Started Sign in");
        Social.localUser.Authenticate(OnAuthenticate);
    }

    private void OnAuthenticate(bool isSuccess)
    {
        isPlayerDataLoaded = true;
        Hud.AddHudText?.Invoke("**Signed success " + isSuccess);
        if (isSuccess && Social.localUser.authenticated && !isProcessing)//load cloud data
        {
            StartCoroutine(LoadFromCloud());
        }
        else //load local data
        {
            OnSaveDataLoaded?.Invoke(false, Player.LoadLocalData());
        }
    }

    /// <summary>
    /// This is a plan B function which will force load local data incase GPS gives any error
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadLocalSaveAfterDelay()
    {
        yield return new WaitForSeconds(10);
        if (!isPlayerDataLoaded)
        {
            OnSaveDataLoaded?.Invoke(false, "");
        }
    }
    #endregion


    #region show save UI
    public void ShowSelectUI()
    {
        uint maxNumToDisplay = 5;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }


    private void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
        }
        else
        {
            // handle cancel or error
        }
    }
    #endregion


    #region Delete game data
    public void DeleteGameData()
    {
        // Open the file to get the metadata.
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(saveName, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
    }

    private void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(game);
        }
        else
        {
            // handle error
        }
    }
    #endregion


    #region Leaderboards and Achievements
    public void PostScoreToLeaderboard(int score, string lbId)
    {
        Social.ReportScore(score, lbId, (bool success) =>
        {
            Hud.AddHudText?.Invoke("PostScoreToLeaderboard " + success.ToString());
        });
    }

    public void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public void UnlockAchievement(string achievementId)
    {
        Social.ReportProgress(achievementId, 100.0f, (bool success) =>
        {
            Hud.AddHudText?.Invoke("UnlockAchievement " + success.ToString());
        });
    }

    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }
    #endregion


    #region Save/Load game on cloud  
    /// <summary>
    /// Source: https://gist.github.com/IJEMIN/e8f5435ad12faebe42dc47f43bc16346
    /// </summary>
    public void SaveToCloud(string dataToSave)
    {
        if (Social.localUser.authenticated)
        {
            saveDataLoadedString = dataToSave;
            isProcessing = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
                (saveName, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
        }
    }


    #region Save/Load helper functions
    private IEnumerator LoadFromCloud()
    {
        // yield return new WaitUntil(() => isGpsSignedIn);
        isProcessing = true;
        //Hud.SetHudText?.Invoke("Loading game progress from the cloud.");
        ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            saveName, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            OnFileOpenToLoad);
        while (isProcessing)
        {
            yield return null;
        }
        Hud.AddHudText?.Invoke("Done loading CloudData " + saveDataLoadedString);
        OnSaveDataLoaded?.Invoke(true, saveDataLoadedString);
    }

    private void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData == null)
        {
            Hud.AddHudText?.Invoke("No Data saved to the cloud");
        }
        else
        {
            saveDataLoadedString = BytesToString(cloudData);
        }
    }

    private void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            this.metaData = metaData;
            byte[] data = StringToBytes(saveDataLoadedString);
            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);
        }
        else
        {
            Hud.AddHudText?.Invoke("Error opening Saved Game" + status);
        }
    }

    private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            Hud.AddHudText?.Invoke("Error Saving" + status);
        }
        else
        {
            OnSaveSuccess?.Invoke();
        }
        isProcessing = false;
    }

    private void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
            //metaData.
        }
        else
        {
            Hud.AddHudText?.Invoke("Error opening Saved Game" + status);
        }
    }

    private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            Hud.AddHudText?.Invoke("Error Saving" + status);
        }
        else
        {
            ProcessCloudData(bytes);
        }
        isProcessing = false;
    }

    private byte[] StringToBytes(string stringToConvert)
    {
        return Encoding.UTF8.GetBytes(stringToConvert);
    }

    private string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
    #endregion
    #endregion
}