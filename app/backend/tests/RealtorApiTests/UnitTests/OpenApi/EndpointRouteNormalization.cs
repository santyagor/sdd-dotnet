using System.Text.RegularExpressions;

namespace RealtorApiTests.UnitTests.OpenApi;

public static class EndpointRouteNormalization
{
    private static readonly Regex RouteConstraintRegex = new(
        "\\{(?<name>[^}:]+)(?::[^}]+)+\\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string Normalize(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);

        return RouteConstraintRegex.Replace(route.Trim(), "{$1}");
    }
}
