#!/bin/bash

dotnet build && dotnet test \
    --verbosity=minimal \
    --collect:"XPlat Code Coverage" \
    --results-directory TestResults \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

dotnet sonarscanner begin /k:"$SONAR_PROJECT" \
    /d:sonar.host.url="$SONAR_HOST" \
    /d:sonar.verbose=true\
    /d:sonar.token="$SONAR_TOKEN" \
    /v:"$GIT_COMMIT" \
    /d:sonar.cs.opencover.reportsPaths="./TestResults/*/*.xml" \
    /d:sonar.exclusions="[**/tests/**/*,**/wwwroot/**]"

dotnet build

dotnet sonarscanner end /d:sonar.token="$SONAR_TOKEN"

exit 0