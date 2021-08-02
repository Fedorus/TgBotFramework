FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["examples/WebhookTest/WebhookTest.csproj", "WebhookTest/"]
RUN dotnet restore "examples/WebhookTest/WebhookTest.csproj"
COPY . .
WORKDIR "/src/WebhookTest"
RUN dotnet build "WebhookTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebhookTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebhookTest.dll"]
