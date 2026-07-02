using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Samples
{
    public class CharactersView : MonoBehaviour
    {
        private const string TextFormat = "Characters: {0} Avg value: {1}";

        [SerializeField] private List<Transform> _characters;
        [SerializeField] private float _updateIntervalSeconds = 1f;

        private Text _text;
        private float _timer;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            _timer = 0f;
            UpdateView();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _updateIntervalSeconds)
                return;

            _timer = 0f;
            UpdateView();
        }

        private void UpdateView()
        {
            string text = GetText();
            SetText(text);
            LogText(text);
        }

        private string GetText()
        {
            var totalCharacters = 0;
            if (_characters != null)
            {
                totalCharacters = _characters.Count;
            }
            
            return string.Format(
                TextFormat,
                totalCharacters,
                GetAverageValue());
        }

        private void SetText(string text)
        {
            if (_text)
                _text.text = text;
        }

        // Debug.Log is expensive (allocations, stack trace). [Conditional] strips calls from release builds.
        // In production, prefer a dedicated logger with levels and no-op in hot paths.
        [Conditional("DEVELOPMENT_BUILD")]
        private static void LogText(string text)
        {
            UnityEngine.Debug.Log(text);
        }

        private float GetAverageValue()
        {
            if (_characters == null || _characters.Count == 0)
                return 0f;

            float totalValue = 0f;

            foreach (Transform characterTransform in _characters)
            {
                if (characterTransform == null)
                    continue;

                Character character = characterTransform.GetComponent<Character>();
                totalValue += character != null ? character.Value : 0f;
            }

            return totalValue / _characters.Count;
        }
    }
}
