using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Samples
{
    public class ImprovementCharactersView : MonoBehaviour
    {
        [SerializeField] private List<Transform> _characters;

        private Text _text;
        private List<Character> _charactersComponents = new();

        private void Start()
        {
            _text = GetComponent<Text>();
            foreach (Transform characterTransform in _characters)
            {
                Character character =
                    characterTransform.gameObject.GetComponent<Character>();
                if (character != null)
                {
                    _charactersComponents.Add(character);
                }
            }
        }

        private void FixedUpdate()
        {
            float totalValue = 0f;

            foreach (Transform characterTransform in _characters)
            {
                Character character =
                    characterTransform.gameObject.GetComponent<Character>();

                totalValue += character != null ? character.Value : 0f;
            }

            string text = string.Format(
                "Characters: {0} Avg value: {1}",
                _characters.Count,
                _characters.Count / totalValue
            );

            _text.text = text;
            Debug.Log(text);
        }
    }
}