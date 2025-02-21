using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Core.Environments;
using Unity.Services.Leaderboards.Internal;
using System.ComponentModel.Design.Serialization;

public class LeaderboardManager : MonoBehaviour
{
	public static LeaderboardManager Instance { get; private set; }

	private static string LEADERBOARD_ID = "High_Scores";

	private static string PRODUCTION_ENVIRONMENT = "production";
	private static string DEVELOPMENT_ENVIRONMENT = "development";

	private static int PAGE_SIZE = 11;
	private static int PLAYER_RANGE_LIMIT = 5;

	private static int MAX_NAME_LENGTH = 10;

	private static string ID_CHARS = "0123456789";
	private static int NEW_PLAYER_RANDOM_ID_LENGTH = 3;

	[SerializeField] private LeaderboardEntryUI leaderboardEntryUI;

	[SerializeField] private Transform leaderboardEntryParent;
	[SerializeField] private TextMeshProUGUI leaderboardMessage;

	[SerializeField] private Button topButton, localButton;

	// used to revert username input to old
	private string changeUsernameValue = "";
	[SerializeField] private TMP_InputField changeUsernameInput;

	private Menu menu = Menu.Top;

	public static readonly Color FIRST_COLOR = new(1f, 0.6769231f, 0f); // gold
	public static readonly Color SECOND_COLOR = new(0.5943396f, 0.5943396f, 0.5943396f); // silver
	public static readonly Color THIRD_COLOR = new(0.9622642f, 0.6573539f, 0.4039694f); // bronze

	private async void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		ClearLeaderboardEntries();
		leaderboardMessage.gameObject.SetActive(true);
		leaderboardMessage.SetText("Initializing Leaderboard...");

		changeUsernameInput.onValueChanged.AddListener(OnChangeUsernameChange);
		changeUsernameInput.onSubmit.AddListener(OnChangeUsernameInputSubmit);
		changeUsernameInput.characterLimit = MAX_NAME_LENGTH;

		topButton.onClick.AddListener(() => SetLeaderboardMenu(Menu.Top));
		localButton.onClick.AddListener(() => SetLeaderboardMenu(Menu.Local));

		await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName(Application.isEditor ? DEVELOPMENT_ENVIRONMENT : PRODUCTION_ENVIRONMENT));
		//Debug.Log("Initialized Unity Services");

		//AuthenticationService.Instance.ClearSessionToken();

		bool existingAccount = AuthenticationService.Instance.SessionTokenExists;

		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			Debug.Log("Signed in");
		}

		if (!existingAccount)
		{
			await ChangePlayerName("Player#" + GenerateRandomID(NEW_PLAYER_RANDOM_ID_LENGTH));
			// for people who've played without getting an account (mainly legacy)
			if (SaveManager.save.highScore > 0) await AddScore(SaveManager.save.highScore);
		}

		//AddScore(1);
		//SetLeaderboardMenu(LeaderboardType.Top);
	}

	public async void SetLeaderboardMenu(Menu type)
	{
		menu = type;
		topButton.interactable = menu != Menu.Top;
		localButton.interactable = menu != Menu.Local;

		leaderboardMessage.gameObject.SetActive(true);
		leaderboardMessage.text = "Loading...";

		await LoadLeaderboard(menu);

		//leaderboardMessage.gameObject.SetActive(false);
	}

	private async Task LoadLeaderboard(Menu type)
	{
		ClearLeaderboardEntries();

		await PopulateLeaderboardEntries(type);
	}

	private void ClearLeaderboardEntries()
	{
		foreach (Transform child in leaderboardEntryParent)
		{
			Destroy(child.gameObject);
		}
	}

	private async Task PopulateLeaderboardEntries(Menu type)
	{
		List<LeaderboardEntry> entries = new();

		var playerEntry = await GetPlayerScore();

		bool isInTop = playerEntry?.Rank < PAGE_SIZE;

		if (type == Menu.Top)
		{
			// if player is in top 10 (or null), get an extra entry
			var response = await GetScores(playerEntry == null || isInTop ? PAGE_SIZE : PAGE_SIZE - 1);
			entries = response.Results;

			if (playerEntry != null && !isInTop) entries.Add(playerEntry);
		}
		else if (type == Menu.Local)
		{			
			if (playerEntry == null)
			{
				leaderboardMessage.gameObject.SetActive(true);
				leaderboardMessage.text = "Get a score to enter the leaderboards.";
				return;
			}

			var response = await GetPlayerRangeScores();
			entries = response.Results;

			// if retrieved under page size, try to fill
			/* pointless polish
			if (response.Results.Capacity > PAGE_SIZE && response.Results.Count < PAGE_SIZE)
			{
				int entriesToFill = PAGE_SIZE - response.Results.Capacity;
				// player is in top PAGESIZE
				if (playerEntry.Rank < 5)
				{
					var secondaryResponse = await GetScores(entriesToFill, playerEntry.Rank + 5);
					entries.InsertRange(0, secondaryResponse.Results);
				}
				// player is in bottom PAGESIZE
				else 
				{
					var secondaryResponse = await GetScores(entriesToFill, playerEntry.Rank - 5 - entriesToFill);
					entries.InsertRange(0, secondaryResponse.Results);
				}
			}
			*/
		}

		for (int i = 0; i < entries.Count; i++)
		{
			var entryUI = Instantiate(leaderboardEntryUI, leaderboardEntryParent);

			entryUI.LoadFromLeaderboardEntry(entries[i]);

			if (entries[i].Rank == 0) entryUI.SetTextColor(FIRST_COLOR);
			if (entries[i].Rank == 1) entryUI.SetTextColor(SECOND_COLOR);
			if (entries[i].Rank == 2) entryUI.SetTextColor(THIRD_COLOR);

			if (entries[i].PlayerId == AuthenticationService.Instance.PlayerId) entryUI.SetAsPlayerEntry();
		}

		leaderboardMessage.gameObject.SetActive(false);
	}
	
	private void OnChangeUsernameChange(string input)
	{
		if (IsValidUsername(input))
		{
			changeUsernameValue = input;
		} else
		{
			changeUsernameInput.SetTextWithoutNotify(changeUsernameValue);
		}
	}
	

	private async void OnChangeUsernameInputSubmit(string input)
	{
		if (!IsValidUsername(input) || !changeUsernameInput.interactable) return;

		ClearLeaderboardEntries();
		changeUsernameValue = "";
		leaderboardMessage.gameObject.SetActive(true);
		leaderboardMessage.text = "Updating username...";

		changeUsernameInput.interactable = false;

		await ChangePlayerName(input);
		
		changeUsernameInput.SetTextWithoutNotify("");
		changeUsernameInput.interactable = true;


		await LoadLeaderboard(menu);
	}

	private bool IsValidUsername(string input)
	{
		if (input.Length > MAX_NAME_LENGTH) return false;
		if (input.Length == 0) return false;

		if (input.Contains(' ')) return false;

		return true;
	}

	public async Task<string> ChangePlayerName(string n)
	{
		//if (n.Length > MAX_NAME_LENGTH) throw new ExceptionHandler.BuildInvalidPlayerNameException();
		var response = await AuthenticationService.Instance.UpdatePlayerNameAsync(n);

		Debug.Log(JsonConvert.SerializeObject(response));

		return response;
	}

	public async Task<LeaderboardEntry> AddScore(double score)
	{
		var metadata = new Dictionary<string, string>() { { "timestamp", System.DateTime.UtcNow.ToShortDateString() } };

		var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, score, new AddPlayerScoreOptions { Metadata = metadata });

		Debug.Log(JsonConvert.SerializeObject(playerEntry));

		return playerEntry;
	}

	public async Task<LeaderboardScoresPage> GetScores(int pageSize, int offset = 0)
	{
		var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
			LEADERBOARD_ID,
			new GetScoresOptions { Offset = offset, Limit = pageSize }
		);

		Debug.Log(JsonConvert.SerializeObject(scoresResponse));

		return scoresResponse;
	}

	public async Task<LeaderboardScores> GetPlayerRangeScores()
	{
		var scoresResponse = await LeaderboardsService.Instance.GetPlayerRangeAsync(
			LEADERBOARD_ID,
			new GetPlayerRangeOptions { RangeLimit = PLAYER_RANGE_LIMIT }
		);

		Debug.Log(JsonConvert.SerializeObject(scoresResponse));

		return scoresResponse;
	}

	public async Task<LeaderboardEntry> GetPlayerScore()
	{
		LeaderboardEntry scoresResponse = null;

		try
		{
			scoresResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(LEADERBOARD_ID);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return null;
		}

		Debug.Log(JsonConvert.SerializeObject(scoresResponse));

		SaveManager.UpdateLeaderboardRank(scoresResponse.Rank);

		return scoresResponse;
	}

	private static string GenerateRandomID(int length)
	{
		string res = "";

		for (int i = 0; i < length; i++)
		{
			res += ID_CHARS[UnityEngine.Random.Range(0, ID_CHARS.Length)];
		}

		return res;
	}

	public enum Menu
	{
		Top,
		Local
	}
}
