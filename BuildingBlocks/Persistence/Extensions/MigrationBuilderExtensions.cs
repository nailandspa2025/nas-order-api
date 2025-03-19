
using Microsoft.EntityFrameworkCore.Migrations;

namespace BuildingBlocks.Persistence.Extensions;

public static class MigrationBuilderExtensions
{
    public static void SqlFromFile(this MigrationBuilder migrationBuilder, string relativeFilePath)
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilePath);
        string script = File.ReadAllText(file);

        Console.WriteLine($"Executing script {file}:");
        Console.WriteLine(script);

        migrationBuilder.Sql(script);
    }
}

