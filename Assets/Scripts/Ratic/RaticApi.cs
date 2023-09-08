using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace Ratic
{
    public static class UnityWebRequestExtensions
    {
        public static string ToCurl(this UnityWebRequest request, Dictionary<string, string> header)
        {
            string curlCommand = "curl -X '" + request.method + "' \\\n";
            curlCommand += "'" + request.url + "' \\\n";

            foreach (var keyValuePair in header)
            {
                string headerValue = request.GetRequestHeader(keyValuePair.Key);
                curlCommand += "  -H '" + keyValuePair.Key + ": " + headerValue + "' \\\n";
            }

            if (request.method == UnityWebRequest.kHttpVerbPOST || request.method == UnityWebRequest.kHttpVerbPUT)
            {
                byte[] bodyData = request.uploadHandler.data;
                if (bodyData != null && bodyData.Length > 0)
                {
                    string body = System.Text.Encoding.UTF8.GetString(bodyData);
                    curlCommand += "  -d '" + body + "'";
                }
            }

            return curlCommand;
        }
    }

    public class RaticApi
    {
        private const string BaseURL = "https://tournament-api.ratic.network";
        private const string ListTournament = "/cash_tournament/list";
        private const string JoinTournament = "/cash_tournament/join_tournament";
        private const string SubmitScore = "/cash_tournament/submit_score";
        private const string StripeWebhook = "/cash_tournament/stripe_webhook";
        private const string ListLeaderboardEntries = "/cash_tournament/leaderboard";
        private const string GetMyLeaderboardData = "/cash_tournament/leaderboard/rank";

        private static IEnumerator MakeRequest(Uri uri, string method = "GET",
            Dictionary<string, string> headers = null,
            string data = null, Action<string> callback = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>
                {
                    { "accept", "application/json" },
                    { "Content-Type", "application/json" }
                };
            }

            using (UnityWebRequest request = CreateWebRequest(uri, method, headers, data))
            {
                request.timeout = 5;
                Debug.Log($"CURL:\n{request.ToCurl(headers)}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"An error occurred: {request.error}");
                    var emptyResponse = new EmptyResponse
                    {
                        message = "request timed out",
                        success = false
                    };
                }
                else
                {
                    Debug.Log($"Response content: {request.downloadHandler.text}");
                    callback?.Invoke(request.downloadHandler.text);
                }
            }
        }

        private static UnityWebRequest CreateWebRequest(Uri uri, string method,
            Dictionary<string, string> headers,
            string data)
        {
            UnityWebRequest request;
            if (method == "POST")
                request = UnityWebRequest.Post(uri, data);
            else if (method == "PUT")
                request = UnityWebRequest.Put(uri, data);
            else if (method == "DELETE")
                request = UnityWebRequest.Delete(uri);
            else
                request = UnityWebRequest.Get(uri);

            request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(data));
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

            return request;
        }

        #region RequestBodies

        public class TournamentEntriesBody
        {
            public int tournament_id { get; set; }
            public int limit { get; set; }
            public int offset { get; set; }
        }

        public class TournamentlistBody
        {
            public int pageNumber { get; set; }
            public int pageSize { get; set; }
            public string sortField { get; set; }
            public string sortOrder { get; set; }
            public object filter { get; set; }
        }

        public class GetMyLeaderboardDataBody
        {
            public int tournament_id { get; set; }
            public string username { get; set; }
        }

        public class JoinTournamentBody
        {
            public int tournament_id { get; set; }
            public string username { get; set; }
        }

        public class SubmitScoreBody
        {
            public string username { get; set; }
            public int tournament_id { get; set; }
            public int score { get; set; }
            public int score_type { get; set; }
        }

        #endregion

        #region RequestResponses

        public class EmptyResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public bool active { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string description { get; set; }
            public string status { get; set; }
            public string start_time { get; set; }
            public string end_time { get; set; }
            public float ticket_price { get; set; }
            public string studio_uuid { get; set; }
            public int created_by { get; set; }
            public string stripe_key { get; set; }
        }

        public class GetTournamentResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public int total_count { get; set; }
            public List<Result> result { get; set; }
        }

        public class PaidLeaderboardResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public int total_count { get; set; }
            public List<LeaderboardEntry> result { get; set; }
        }

        public class LeaderboardEntry
        {
            public string username { get; set; }
            public object tournament_id { get; set; }
            public object reward { get; set; }
            public object xoxoday_link { get; set; }
            public int rank { get; set; }
            public int score { get; set; }
        }

        public class GetMyLeaderboardResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public LeaderboardEntry result { get; set; }
        }

        #endregion




        public static IEnumerator JoinFreeTournament(string userName)
        {
            var data = new TournamentlistBody
            {
                pageNumber = 1,
                pageSize = 100,
                sortField = "id",
                sortOrder = "desc",
                filter = null
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{ListTournament}");
            yield return MakeRequest(uri, "POST", null, serializedData,
                joinResponse =>
                {
                    Debug.Log($"[RATIC] JoinFreeTournament response:{joinResponse}");
                });
        }

        public static IEnumerator GetPaidLeaderboard(int tournamentId, int limit, int offset, Action<PaidLeaderboardResponse> callback)
        {
            var data = new TournamentEntriesBody()
            {
                tournament_id = tournamentId,
                limit = limit,
                offset = offset
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{ListLeaderboardEntries}");
            yield return MakeRequest(uri, "POST", null, serializedData,
                paidLeaderboardResponse =>
                {
                    Debug.Log($"[RATIC] GetPaidLeaderboard response:{paidLeaderboardResponse}");
                    callback?.Invoke(JsonConvert.DeserializeObject<PaidLeaderboardResponse>(paidLeaderboardResponse));
                });
        }

        public static IEnumerator GetFreeLeaderboard()
        {
            var data = new TournamentlistBody
            {
                pageNumber = 1,
                pageSize = 100,
                sortField = "id",
                sortOrder = "desc",
                filter = null
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{ListTournament}");
            yield return MakeRequest(uri, "POST", null, serializedData,
                leaderboardResponse =>
                {
                    Debug.Log($"[RATIC] GetFreeLeaderboard response:{leaderboardResponse}");
                });
        }

        public static IEnumerator GetMyLeaderboard(int tournamentId, string userName, Action<GetMyLeaderboardResponse> callback)
        {
            var data = new GetMyLeaderboardDataBody
            {
                tournament_id = tournamentId,
                username = userName
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{GetMyLeaderboardData}");
            yield return MakeRequest(uri, "POST", null, serializedData,
                myLeaderboardResponse =>
                {
                    Debug.Log($"[RATIC] GetMyLeaderboard response:{myLeaderboardResponse}");
                    callback?.Invoke(JsonConvert.DeserializeObject<GetMyLeaderboardResponse>(myLeaderboardResponse));
                });
        }

        public static IEnumerator SubmitPaidScore(int tournamentId, int score, string username)
        {
            var data = new SubmitScoreBody
            {
                tournament_id = tournamentId,
                score = score,
                username = username,
                score_type = 1
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{SubmitScore}");
            yield return MakeRequest(uri, "PUT", null, serializedData,
                joinResponse =>
                {
                    Debug.Log($"[RATIC] SubmitPaidScore response:{joinResponse}");
                });
        }

        public static IEnumerator SubmitFreeScore(string score)
        {
            var data = new TournamentlistBody
            {
                pageNumber = 1,
                pageSize = 100,
                sortField = "id",
                sortOrder = "desc",
                filter = null
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var baseUri = new Uri(BaseURL);
            var uri = new Uri(baseUri, $"{ListTournament}");
            yield return MakeRequest(uri, "POST", null, serializedData,
                joinResponse =>
                {
                    Debug.Log($"[RATIC] SubmitFreeScore response:{joinResponse}");
                });
        }
    }
}