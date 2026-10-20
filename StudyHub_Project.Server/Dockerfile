FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["StudyHub_Project.Server.csproj", "./"]

RUN dotnet restore "StudyHub_Project.Server.csproj"

COPY . .



RUN dotnet publish "StudyHub_Project.Server.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app


ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080


COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "StudyHub_Project.Server.dll"]