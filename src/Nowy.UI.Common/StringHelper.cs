using System.Globalization;
using System.Text;

namespace Nowy.UI.Common;

public static class StringHelper
{
    public static Random Random => _rand;

    private static readonly Random _rand = new();
    private static readonly IReadOnlyList<char> _vovels = new[] { 'a', 'e', 'i', 'o', 'u', 'y', };

    public static string MakeRandomText(int count_words_min = 5, int count_words_max = 25, int count_characters_min = 3, int count_characters_max = 12)
    {
        bool last_character_was_vovel = false;

        char choose_next_character()
        {
            int attempts_left = 100;
            char c;
            do
            {
                c = (char)( 'a' + ( _rand.Next() % ( 'z' - 'a' ) ) );
                if (_vovels.Contains(c) != last_character_was_vovel)
                    break;
            } while (attempts_left-- >= 0);

            last_character_was_vovel = _vovels.Contains(c);

            return c;
        }

        StringBuilder sb = new();
        bool is_beginning_of_sentence = true;
        int count_words = _rand.Next() % ( count_words_max - count_words_min ) + count_words_min;
        for (int word_index = 0; word_index < count_words; word_index++)
        {
            if (word_index != 0)
            {
                if (_rand.Next() % 100 > 95)
                {
                    sb.Append(".");
                    is_beginning_of_sentence = true;
                }

                sb.Append(' ');
            }

            int count_characters = _rand.Next() % ( count_characters_max - count_characters_min ) + count_characters_min;
            for (int character_index = 0; character_index < count_characters; character_index++)
            {
                bool is_uppercase = is_beginning_of_sentence && ( word_index == 0 || ( _rand.Next() % 100 > 90 ) );
                char c = choose_next_character();
                if (is_uppercase)
                    c = char.ToUpper(c);
                sb.Append(c);
                is_beginning_of_sentence = false;
            }
        }

        sb.Append(".");
        return sb.ToString();
    }

    public static string MakeRandomUuid()
    {
        return Guid.NewGuid().ToString("D");
    }

    public static string RemoveDiacritics(string s)
    {
        string normalizedString = s.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    private static IFormatProvider inv
        = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

    public static string? ToStringInvariant<T>(this T obj, string? format = null)
    {
        return ( format is null )
            ? FormattableString.Invariant($"{obj}")
            : string.Format(inv, $"{{0:{format}}}", obj);
    }

    public static string MoveHouseNumberToEnd(string? street)
    {
        street ??= string.Empty;

        string[] a = street.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < a.Length; i++)
        {
            if (a[0].Any(char.IsDigit))
            {
                a = a.Skip(1).Concat(new[] { a[0], }).ToArray();
            }
        }

        street = a.Join(" ");

        return street;
    }
}
