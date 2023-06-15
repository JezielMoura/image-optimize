FROM mcr.microsoft.com/dotnet/sdk:8.0-preview-jammy as sdk
WORKDIR /src
COPY . /src

RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview-jammy as runtime
WORKDIR /app
COPY --from=sdk src/publish /app
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ImageOptimize.dll"]