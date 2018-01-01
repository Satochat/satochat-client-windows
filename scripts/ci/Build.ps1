&msbuild /t:publish /p:Configuration=Release
if (-not $?) { throw }

&docker-compose -f docker-compose.yml -f docker-compose.ci.prod.yml build
if (-not $?) { throw }
