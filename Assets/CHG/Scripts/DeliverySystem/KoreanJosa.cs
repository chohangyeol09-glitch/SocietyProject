using System.Text;

namespace CHG.Scripts.DeliverySystem
{
    public static class KoreanJosa
    {
        private static readonly string[][] Tokens =
        {
            new[] { "을(를)", "을", "를" },
            new[] { "를(을)", "을", "를" },
            new[] { "이(가)", "이", "가" },
            new[] { "가(이)", "이", "가" },
            new[] { "은(는)", "은", "는" },
            new[] { "는(은)", "은", "는" },
            new[] { "와(과)", "과", "와" },
            new[] { "과(와)", "과", "와" },
            new[] { "(으)로", "으로", "로" },
            new[] { "으로(로)", "으로", "로" },
        };

        public static string Resolve(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder(text.Length);
            int index = 0;

            while (index < text.Length)
            {
                string[] matched = FindToken(text, index);
                if (matched == null)
                {
                    sb.Append(text[index]);
                    index++;
                    continue;
                }

                char prev = sb.Length > 0 ? sb[sb.Length - 1] : '\0';
                bool hasFinal = HasFinalConsonant(prev, out bool isRieul);
                bool isRoJosa = matched[1] == "으로";

                sb.Append(isRoJosa
                    ? (hasFinal && !isRieul ? matched[1] : matched[2])
                    : (hasFinal ? matched[1] : matched[2]));

                index += matched[0].Length;
            }

            return sb.ToString();
        }

        private static string[] FindToken(string text, int index)
        {
            foreach (string[] token in Tokens)
            {
                string pattern = token[0];
                if (index + pattern.Length > text.Length)
                    continue;

                if (string.CompareOrdinal(text, index, pattern, 0, pattern.Length) == 0)
                    return token;
            }

            return null;
        }

        public static bool HasFinalConsonant(char character, out bool isRieul)
        {
            isRieul = false;
            if (character < 0xAC00 || character > 0xD7A3)
                return false;

            int finalIndex = (character - 0xAC00) % 28;
            isRieul = finalIndex == 8;
            return finalIndex != 0;
        }
    }
}
