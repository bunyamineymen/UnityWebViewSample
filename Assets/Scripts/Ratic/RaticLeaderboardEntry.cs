using TMPro;
using UnityEngine;

namespace Ratic
{
    public class RaticLeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rankAndNameText;
        [SerializeField] private TMP_Text _score;
        
        public void Setup(string rank, string name, string score)
        {
            _rankAndNameText.text = $"{rank}.{name}";
            _score.text = $"{score}";
        }
    }
}