using TMPro;
using UnityEngine;

namespace DEAL
{
    public class BetManHandDeck : BaseDeck
    {
        [SerializeField]
        private TMP_Text multiplierText;
        private const string multiplierKey = "Multiplier";
        [SerializeField]
        private TMP_Text baseScoreText;
        private const string baseScoreKey = "BaseScore";
        [SerializeField]
        private TMP_Text totalScoreText;
        private const string totalScoreKey = "TotalScore";
        [SerializeField]
        private TMP_Text sumScoreText;
        private const string sumScoreKey = "SumScore";

        protected override void OnLocalCustomPropertyChanged(string key, GenericProperty value)
        {
            base.OnLocalCustomPropertyChanged(key, value);
            switch (key)
            {
                case multiplierKey:
                    SetText(multiplierText, value.ToString());
                    break;
                case baseScoreKey:
                    SetText(baseScoreText, value.ToString());
                    break;
                case totalScoreKey:
                    SetText(totalScoreText, value.ToString());
                    break;
                case sumScoreKey:
                    SetText(sumScoreText, value.ToString());
                    break;
            }
        }
        
        private void SetText(TMP_Text field, string newValue)
        {
            if (field != null)
            {
                // 需要加動畫演出的話改這裡
                field.text = newValue;
            }
        }
    }
}
