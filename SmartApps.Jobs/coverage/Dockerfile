FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev default-jdk ca-certificates-java \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN dotnet tool install -g dotnet-sonarscanner

ENV PATH="${PATH}:/root/.dotnet/tools"

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

COPY ["/SmartApps.Jobs.sln", "/app/SmartApps.Jobs.sln"]
COPY ["/src/Infraestructure/Data/Infraestructure.Data/Infraestructure.Data.csproj", "/app/src/Infraestructure/Data/Infraestructure.Data/Infraestructure.Data.csproj"]
COPY ["/src/Infraestructure/Data/Infraestructure.Data.IntegrationTests/Infraestructure.Data.IntegrationTests.csproj", "/app/src/Infraestructure/Data/Infraestructure.Data.IntegrationTests/Infraestructure.Data.IntegrationTests.csproj"]
COPY ["/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch/Infraestructure.Elasticsearch.csproj", "/app/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch/Infraestructure.Elasticsearch.csproj"]
COPY ["/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch.UnitTests/Infraestructure.Elasticsearch.UnitTests.csproj", "/app/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch.UnitTests/Infraestructure.Elasticsearch.UnitTests.csproj"]
COPY ["/src/Infraestructure/Migrations/Infraestructure.Migrations/Infraestructure.Migrations.csproj", "/app/src/Infraestructure/Migrations/Infraestructure.Migrations/Infraestructure.Migrations.csproj"]
COPY ["/src/Infraestructure/RabbitMq/Infraestructure.RabbitMq/Infraestructure.RabbitMq.csproj", "/app/src/Infraestructure/RabbitMq/Infraestructure.RabbitMq/Infraestructure.RabbitMq.csproj"]
COPY ["/src/Infraestructure/Utils/Infraestructure.Utils/Infraestructure.Utils.csproj", "/app/src/Infraestructure/Utils/Infraestructure.Utils/Infraestructure.Utils.csproj"]
COPY ["/src/Infraestructure/Utils/Infraestructure.Utils.UnitTests/Infraestructure.Utils.UnitTests.csproj", "/app/src/Infraestructure/Utils/Infraestructure.Utils.UnitTests/Infraestructure.Utils.UnitTests.csproj"]
COPY ["/src/Services/Jobs/Jobs.Api/Jobs.Api.csproj", "/app/src/Services/Jobs/Jobs.Api/Jobs.Api.csproj"]
COPY ["/src/Services/Jobs/Jobs.Application/Jobs.Application.csproj", "/app/src/Services/Jobs/Jobs.Application/Jobs.Application.csproj"]
COPY ["/src/Services/Jobs/Jobs.Domain/Jobs.Domain.csproj", "/app/src/Services/Jobs/Jobs.Domain/Jobs.Domain.csproj"]
COPY ["/src/Services/Jobs/Jobs.Infraestructure/Jobs.Infraestructure.csproj", "/app/src/Services/Jobs/Jobs.Infraestructure/Jobs.Infraestructure.csproj"]
COPY ["/src/Services/Jobs/Jobs.IntegrationTests/Jobs.IntegrationTests.csproj", "/app/src/Services/Jobs/Jobs.IntegrationTests/Jobs.IntegrationTests.csproj"]
COPY ["/src/Services/Jobs/Jobs.UnitTests/Jobs.UnitTests.csproj", "/app/src/Services/Jobs/Jobs.UnitTests/Jobs.UnitTests.csproj"]
RUN dotnet restore /app/SmartApps.Jobs.sln

COPY . .

RUN chmod +x coverage/entrypoint.sh

ENTRYPOINT ["coverage/entrypoint.sh"]