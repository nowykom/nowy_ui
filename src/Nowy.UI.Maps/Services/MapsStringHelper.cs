namespace Nowy.UI.Maps.Services;

public static class MapsStringHelper
{
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
