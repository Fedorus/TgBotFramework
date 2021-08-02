FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["examples/echoBotExample/echoBotExample.csproj", "echoBotExample/"]
RUN dotnet restore "examples/echoBotExample/echoBotExample.csproj"
COPY . .
WORKDIR "/src/echoBotExample"
RUN dotnet build "echoBotExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "echoBotExample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "echoBotExample.dll"]
