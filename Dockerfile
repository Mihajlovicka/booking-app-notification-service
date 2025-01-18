FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY NotificationService/*.csproj NotificationService/
RUN dotnet restore NotificationService/NotificationService.csproj 


COPY NotificationService/ NotificationService/
WORKDIR /source/NotificationService
RUN dotnet build -c release --no-restore

# test
FROM build AS test
COPY NotificationService.Tests/*.csproj NotificationService.Tests/
RUN dotnet restore NotificationService.Tests/NotificationService.Tests.csproj

WORKDIR /source/NotificationService.Tests
COPY NotificationService.Tests/ .
ENTRYPOINT ["dotnet", "test", "--logger:console;verbosity=detailed"]


FROM build AS publish
WORKDIR /source/NotificationService
RUN dotnet publish -c release --no-build -o /app

# final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.dll"]