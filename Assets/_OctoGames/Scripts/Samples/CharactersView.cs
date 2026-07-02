using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Samples
{
    public class CharactersView : MonoBehaviour
    {
        [SerializeField] private List<Transform> _characters;

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

            gameObject.GetComponent<Text>().text = text;
            Debug.Log(text);
        }
    }
}