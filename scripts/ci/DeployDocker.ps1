Write-Output "Uploading image..."
$env:DOCKER_PASSWORD | docker login -u $env:DOCKER_USERNAME --password-stdin
if (-not $?) { throw }

&docker-compose -f docker-compose.yml -f docker-compose.ci.prod.yml push web
if (-not $?) { throw }
