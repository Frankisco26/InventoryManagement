[CmdletBinding()]
Param(
    [string]$context = "Principal",
    [string][AllowNull()]$name,
    [switch]$clear = $false,
    [switch]$update = $false,
    [switch]$rollback = $false,
    [string]$output
)

$context_name = "Contexto" + "$context";
$project = "Data";
$startup_project = "Web";

Push-Location $PSScriptRoot
try
{
    dotnet tool restore | Out-Null

    if ($clear)
    {
        dotnet ef database drop -f --context $context_name --project $project --startup-project $startup_project
        if (Test-Path $project/Migraciones/$context)
        {
            rm -r -fo $project/Migraciones/$context
        }
    }
    elseif ($rollback)
    {
        dotnet ef migrations remove --force --context $context_name --project $project --startup-project $startup_project
    }
    if (![string]::IsNullOrEmpty($output))
    {
        $script_name = [DateTime]::Now.ToString("yyyyMMdd") + "_$context" + "_$output"
        dotnet ef migrations script -i --context $context_name -o $project/Migraciones/$context/Scripts/$script_name.sql --project $project --startup-project $startup_project
        Write-Output "Done: $script_name"
    }
    else
    {
        if (![string]::IsNullOrEmpty($name))
        {
            dotnet ef migrations add $name -o "Migraciones/$context" --context $context_name --project $project --startup-project $startup_project
            if ($update)
            {
                dotnet ef database update $name --context $context_name --project $project --startup-project $startup_project
            }
        }
        else
        {
            if ($update)
            {
                dotnet ef database update --context $context_name --project $project --startup-project $startup_project
            }
        }
    }
}
finally
{
    Pop-Location
}
