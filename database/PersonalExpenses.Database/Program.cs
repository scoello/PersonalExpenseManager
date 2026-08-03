using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

var connectionString = args.FirstOrDefault()
    ?? Environment.GetEnvironmentVariable("PERSONAL_EXPENSES_SQLSERVER")
    ?? "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True";
var scriptsDirectory = Path.Combine(AppContext.BaseDirectory, "Scripts");
var scripts = Directory.GetFiles(scriptsDirectory, "*.sql", SearchOption.AllDirectories)
    .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
    .ThenBy(path => Path.GetRelativePath(scriptsDirectory, path), StringComparer.OrdinalIgnoreCase)
    .ToArray();
if (scripts.Length == 0) throw new InvalidOperationException($"No SQL scripts were found in {scriptsDirectory}.");

await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
foreach (var path in scripts)
{
    Console.WriteLine($"Applying {Path.GetRelativePath(scriptsDirectory, path)}...");
    var batches = Regex.Split(await File.ReadAllTextAsync(path), @"^\s*GO\s*(?:--.*)?$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    foreach (var batch in batches.Where(value => !string.IsNullOrWhiteSpace(value)))
    {
        await using var command = new SqlCommand(batch, connection) { CommandTimeout = 120 };
        await command.ExecuteNonQueryAsync();
    }
}
Console.WriteLine("PersonalExpenses SQL Server database is ready.");
