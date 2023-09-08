using System.Collections.Generic;
using UnityEngine;

namespace Ratic
{
    public static class UserData
    {
        public const string SAVE_KEY_USERNAME = "save-key-username";
        public static string Username;
        public static float TicketPrice;
        public static int TournamentId;
        public static string TournamentName;
        public static List<RaticApi.LeaderboardEntry> LeaderboardEntries;
        public static RaticApi.LeaderboardEntry MyLeaderboardEntry;

        static UserData()
        {
            Username = PlayerPrefs.GetString(SAVE_KEY_USERNAME, string.Empty);
        }
    }
}