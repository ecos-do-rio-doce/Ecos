using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Ecos
{
    public class EndgameScreen : MonoBehaviour
    {
        [Title("UI")]
        [SerializeField] private TMP_Text scoreText;

        [Title("Events")]
        [SerializeField] private UnityEvent onStartDisplaying;

        [Title("Animation")]
        [SerializeField] private float startDelay = 1f;
        [SerializeField] private float lineDelay = 0.25f;
        [SerializeField] private float countTweenDuration = 0.35f;
        [SerializeField] private float pointsTweenDuration = 0.5f;
        [SerializeField] private bool playOnEnable = true;

        [Title("Formatting")]
        [SerializeField] private int labelColumnWidth = 28;
        [SerializeField] private int valueColumnWidth = 10;
        [SerializeField] private bool useThousandsSeparator = true;
        [SerializeField] private bool useTabsBetweenColumns = true;

        private readonly List<ScoreLine> lines = new();
        private Coroutine routine;

        private void OnDisable()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
        }

        [Button]
        public void Play()
        {
            FindFirstObjectByType<WorldFishing>().ChangeState(new EndGameState());

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(PlayRoutine());
        }

        public void RefreshAndPlay() => Play();

        private IEnumerator PlayRoutine()
        {
            onStartDisplaying?.Invoke();

            BuildLines(PlayerScore.Instance);

            int[] displayedCounts = new int[lines.Count];
            int[] displayedValues = new int[lines.Count];

            scoreText.text = BuildText(lines, displayedCounts, displayedValues);

            if (startDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(startDelay);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                ScoreLine line = lines[i];

                if (line.HasCount)
                {
                    yield return TweenCount(i, line.CountValue, displayedCounts, displayedValues);
                }

                yield return TweenValue(i, line.Value, displayedCounts, displayedValues);

                if (lineDelay > 0f)
                {
                    yield return new WaitForSecondsRealtime(lineDelay);
                }
            }

            routine = null;
        }

        private IEnumerator TweenCount(int index, int targetCount, int[] displayedCounts, int[] displayedValues)
        {
            float elapsed = 0f;

            while (elapsed < countTweenDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / countTweenDuration);

                displayedCounts[index] = Mathf.RoundToInt(Mathf.Lerp(0f, targetCount, t));
                scoreText.text = BuildText(lines, displayedCounts, displayedValues);

                yield return null;
            }

            displayedCounts[index] = targetCount;
            scoreText.text = BuildText(lines, displayedCounts, displayedValues);
        }

        private IEnumerator TweenValue(int index, int targetValue, int[] displayedCounts, int[] displayedValues)
        {
            float elapsed = 0f;

            while (elapsed < pointsTweenDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / pointsTweenDuration);

                displayedValues[index] = Mathf.RoundToInt(Mathf.Lerp(0f, targetValue, t));
                scoreText.text = BuildText(lines, displayedCounts, displayedValues);

                yield return null;
            }

            displayedValues[index] = targetValue;
            scoreText.text = BuildText(lines, displayedCounts, displayedValues);
        }

        private void BuildLines(PlayerScore playerScore)
        {
            lines.Clear();

            List<AttemptInfos> attempts = playerScore.allAttempts ?? new List<AttemptInfos>();
            if (attempts.Count == 0)
            {
                lines.Add(new ScoreLine("Total", 0));
                return;
            }

            int scorePerPerfectCatch = playerScore.ScorePerPerfectCatch;
            float modifierPerCorrectFish = playerScore.ModifierPerCorrectFish;
            float modifierPerWrongFish = playerScore.ModifierPerWrongFish;

            int correctCatchPoints = 0;
            int wrongCatchPoints = 0;
            int perfectBonusPoints = 0;

            int correctCatchCount = 0;
            int wrongCatchCount = 0;
            int perfectCount = 0;

            float missedTimingsSum = 0f;
            float wrongClicksSum = 0f;

            foreach (AttemptInfos attempt in attempts)
            {
                int basePoints = attempt.fishDef != null ? attempt.fishDef.PointsValue : 0;
                bool wasCorrect = attempt.shouldBeCaptured == attempt.wasCaptured;
                bool wasPerfect = wasCorrect && attempt.missedTimings <= 0 && attempt.wrongClicks <= 0;
                bool wasWrongCatch = attempt.shouldBeCaptured != attempt.wasCaptured;

                if (wasCorrect)
                {
                    correctCatchCount++;
                    correctCatchPoints += Mathf.Max(0, Mathf.RoundToInt(basePoints * modifierPerCorrectFish));

                    if (wasPerfect)
                    {
                        perfectCount++;
                        perfectBonusPoints += scorePerPerfectCatch;
                    }
                }
                else if (wasWrongCatch)
                {
                    wrongCatchCount++;
                    wrongCatchPoints += Mathf.Max(0, Mathf.RoundToInt(basePoints * modifierPerWrongFish));
                }

                missedTimingsSum += Mathf.Max(0, attempt.missedTimings);
                wrongClicksSum += Mathf.Max(0, attempt.wrongClicks);
            }

            float averageMissedTimings = attempts.Count > 0 ? missedTimingsSum / attempts.Count : 0f;
            int missedTimingBonusPoints = 0;


            missedTimingBonusPoints = Mathf.RoundToInt(
                playerScore.MissedTimingRewardCurve.Evaluate(averageMissedTimings)
            );


            float averageWrongClicks = attempts.Count > 0 ? wrongClicksSum / attempts.Count : 0f;
            int wrongClicksBonusPoints = 0;
            wrongClicksBonusPoints = Mathf.RoundToInt(
                playerScore.WrongClicksRewardCurve.Evaluate(averageWrongClicks)
            );


            int totalPoints = correctCatchPoints + wrongCatchPoints + perfectBonusPoints + missedTimingBonusPoints + wrongClicksBonusPoints;

            lines.Add(new ScoreLine("Pescas corretas", correctCatchPoints, correctCatchCount));
            lines.Add(new ScoreLine("Pescas erradas", wrongCatchPoints, wrongCatchCount));
            lines.Add(new ScoreLine("Velocidade", missedTimingBonusPoints));
            lines.Add(new ScoreLine("Precisão", wrongClicksBonusPoints));
            lines.Add(new ScoreLine("Capturas Perfeitas", perfectBonusPoints, perfectCount));
            lines.Add(new ScoreLine("\nTotal", totalPoints));
        }

        private string BuildText(List<ScoreLine> lines, int[] displayedCounts, int[] displayedValues)
        {
            var sb = new StringBuilder(256);

            for (int i = 0; i < lines.Count; i++)
            {
                int count = (displayedCounts != null && i < displayedCounts.Length) ? displayedCounts[i] : 0;
                int value = (displayedValues != null && i < displayedValues.Length) ? displayedValues[i] : 0;
                sb.AppendLine(FormatLine(lines[i], count, value));
            }

            return sb.ToString();
        }

        private string FormatLine(ScoreLine line, int count, int value)
        {
            string label = line.Label;

            if (line.HasCount)
            {
                label += $" ({count})";
            }

            label = label.PadRight(Mathf.Max(0, labelColumnWidth));

            string valueText = useThousandsSeparator ? value.ToString("N0") : value.ToString();
            valueText = valueText.PadLeft(Mathf.Max(0, valueColumnWidth));

            if (useTabsBetweenColumns)
            {
                return $"{label}\t{valueText}";
            }

            return $"{label} {valueText}";
        }

        [Serializable]
        private struct ScoreLine
        {
            public string Label;
            public int Value;
            public int CountValue;
            public bool HasCount;

            public ScoreLine(string label, int value, int countValue)
            {
                Label = label;
                Value = value;
                CountValue = countValue;
                HasCount = true;
            }

            public ScoreLine(string label, int value)
            {
                Label = label;
                Value = value;
                CountValue = 0;
                HasCount = false;
            }
        }
    }
}