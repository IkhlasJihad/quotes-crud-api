FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /usr/app
EXPOSE 80
EXPOSE 5000
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./QuotesAPI.csproj", "src/"]
RUN dotnet restore "src/QuotesAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "QuotesAPI.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "QuotesAPI.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "QuotesAPI.dll"]