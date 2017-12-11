if ($env:STRONG_NAME_KEY) {
    $decoded = [System.Convert]::FromBase64String($env:STRONG_NAME_KEY)
    $decoded | Set-Content -Path .\secrets\StrongNameKey.snk -Encoding Byte
}
