param (
    [string]$RootPath = ".",
    [string]$OutputPath = ".\attributes-tree.md"
)

function Get-AttributeTree {
    param (
        [string]$Path,
        [int]$Depth
    )

    $result = @()
    $items = Get-ChildItem -Path $Path -Force | Sort-Object PSIsContainer, Name

    $attributeFiles = $items | Where-Object { -not $_.PSIsContainer -and $_.Name -like "*Attribute.cs" }
    $childDirs = $items | Where-Object { $_.PSIsContainer }

    if ($attributeFiles.Count -gt 0 -or ($childDirs.Count -gt 0)) {
        # Get relative path
        $relativePath = Resolve-Path $Path | ForEach-Object {
            $_.Path.Substring((Resolve-Path $RootPath).Path.Length).TrimStart('\')
        }

        if ($attributeFiles.Count -gt 0) {
            $result += ""
            # Fix: Correctly handling the relative path using concatenation
            $result += "- " + $relativePath -replace '\\', '/'
            foreach ($file in $attributeFiles) {
                # Correct file name handling
                $result += "  - " + $file.Name
            }
        }

        foreach ($child in $childDirs) {
            $childResult = Get-AttributeTree -Path $child.FullName -Depth ($Depth + 1)
            if ($childResult.Count -gt 0) {
                $result += $childResult
            }
        }
    }

    return $result
}

# Execute the function and save to markdown file
$result = Get-AttributeTree -Path (Resolve-Path $RootPath).Path -Depth 0
if ($result.Count -gt 0) {
    $result | Out-File -FilePath $OutputPath -Encoding utf8
    Write-Host "✅ Markdown 文件已生成：$OutputPath"
} else {
    Write-Host "⚠️ 没有找到任何 Attribute.cs 文件"
}
