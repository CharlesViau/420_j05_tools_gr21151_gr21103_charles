using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exercise9
{
    public class BetProgram : MonoBehaviour
    {
        private List<string> _matches;

        private void Start()
        {
            _matches = new List<string>
            {
                "1vsA",
                "2vsB",
                "3vsC",
                "4vsD",
                "5vsE"
            };

            ShowPossibilities();
        }

        private void ShowPossibilities()
        {
            foreach (var match in _matches)
            {
                var result = "";
                var cpt = _matches.Count;
                PowerUp(cpt, ref result);
            }
        }
        

        private void PowerUp(int cpt, ref string result)
        {
            foreach (var outcome in Enum.GetNames(typeof(Outcomes)))
            {
                if (cpt != 0)
                {
                    result += Result((Outcomes) Enum.Parse(typeof(Outcomes), outcome));
                    cpt--;
                    PowerUp(cpt, ref result);
                }

                Debug.Log(result);
            }
        }

        private string Result(Outcomes outcomes)
        {
            return outcomes switch
            {
                Outcomes.Win => "Win ",
                Outcomes.Defeat => "Lose ",
                Outcomes.Null => "Null ",
                _ => throw new ArgumentOutOfRangeException(nameof(outcomes), outcomes, null)
            };
        }
    }

    public enum Outcomes
    {
        Win,
        Defeat,
        Null
    }
}