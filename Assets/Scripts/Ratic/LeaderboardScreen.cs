using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ratic
{
    public class LeaderboardScreen : MonoBehaviour
    {
        [SerializeField] private RaticLeaderboardEntry _leaderboardEntryPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RaticLeaderboardEntry _playerEntry;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _yourRankText;

        private List<RaticLeaderboardEntry> _leaderboardEntries = new();

        private void Awake()
        {
            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            foreach (var raticLeaderboardEntry in _leaderboardEntries)
                Destroy(raticLeaderboardEntry.gameObject);
            _leaderboardEntries.Clear();
            
            _playerEntry.Setup("-", UserData.Username, "-");
            _yourRankText.text = "-";

            if (UserData.MyLeaderboardEntry != null)
            {
                Debug.Log($"[RATIC] My leaderboard data does not exist");
                _playerEntry.Setup(UserData.MyLeaderboardEntry.rank.ToString(), UserData.Username, UserData.MyLeaderboardEntry.score.ToString());
                _yourRankText.text = UserData.MyLeaderboardEntry.rank.ToString();
            }
            
            CreateLeaderboard(UserData.LeaderboardEntries);
        }

        private void CreateLeaderboard(List<RaticApi.LeaderboardEntry> leaderboardEntries)
        {
            foreach (var leaderboardEntry in leaderboardEntries)
            {
                var entry = Instantiate(_leaderboardEntryPrefab, _content);
                entry.Setup(leaderboardEntry.rank.ToString(), leaderboardEntry.username, leaderboardEntry.score.ToString());
                _leaderboardEntries.Add(entry);
            }
        }
    }
}