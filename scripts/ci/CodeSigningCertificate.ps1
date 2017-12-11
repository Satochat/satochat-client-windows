if ($env:CODE_SIGNING_CERTIFICATE) {
    $decoded = [System.Convert]::FromBase64String($env:CODE_SIGNING_CERTIFICATE)
    $decoded | Set-Content -Path .\secrets\CodeSign.pfx -Encoding Byte
}

if ($env:CODE_SIGNING_PASSWORD) {
    $password = ConvertTo-SecureString $env:CODE_SIGNING_PASSWORD -AsPlainText -Force
    Import-PfxCertificate -Password $password -CertStoreLocation Cert:\CurrentUser\My -FilePath .\secrets\CodeSign.pfx | Out-Null
}
