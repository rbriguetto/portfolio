FROM mcr.microsoft.com/dotnet/sdk:7.0 AS server-builder
WORKDIR /server

COPY ["/src/Infraestructure/Data/Infraestructure.Data/Infraestructure.Data.csproj", "/server/src/Infraestructure/Data/Infraestructure.Data/Infraestructure.Data.csproj"]
COPY ["/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch/Infraestructure.Elasticsearch.csproj", "/server/src/Infraestructure/Elasticsearch/Infraestructure.Elasticsearch/Infraestructure.Elasticsearch.csproj"]
COPY ["/src/Infraestructure/Migrations/Infraestructure.Migrations/Infraestructure.Migrations.csproj", "/server/src/Infraestructure/Migrations/Infraestructure.Migrations/Infraestructure.Migrations.csproj"]
COPY ["/src/Infraestructure/RabbitMq/Infraestructure.RabbitMq/Infraestructure.RabbitMq.csproj", "/server/src/Infraestructure/RabbitMq/Infraestructure.RabbitMq/Infraestructure.RabbitMq.csproj"]
COPY ["/src/Infraestructure/Utils/Infraestructure.Utils/Infraestructure.Utils.csproj", "/server/src/Infraestructure/Utils/Infraestructure.Utils/Infraestructure.Utils.csproj"]
COPY ["/src/Services/Jobs/Jobs.Api/Jobs.Api.csproj", "/server/src/Services/Jobs/Jobs.Api/Jobs.Api.csproj"]
COPY ["/src/Services/Jobs/Jobs.Application/Jobs.Application.csproj", "/server/src/Services/Jobs/Jobs.Application/Jobs.Application.csproj"]
COPY ["/src/Services/Jobs/Jobs.Domain/Jobs.Domain.csproj", "/server/src/Services/Jobs/Jobs.Domain/Jobs.Domain.csproj"]
COPY ["/src/Services/Jobs/Jobs.Infraestructure/Jobs.Infraestructure.csproj", "/server/src/Services/Jobs/Jobs.Infraestructure/Jobs.Infraestructure.csproj"]
RUN dotnet restore /server/src/Services/Jobs/Jobs.Api/Jobs.Api.csproj

COPY . .

RUN dotnet build
RUN dotnet publish -c Release ./src/Services/Jobs/Jobs.Api/Jobs.Api.csproj -o /publish/

FROM mcr.microsoft.com/dotnet/aspnet:7.0
RUN sed -i 's/TLSV1.2/TLSv1.1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=0/g' /etc/ssl/openssl.cnf
EXPOSE 80
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=server-builder /publish .
ENTRYPOINT ["dotnet", "Jobs.Api.dll"]