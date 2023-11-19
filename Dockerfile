FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Projeyi kopyala ve ba��ml�l�klar� yeniden y�kle
COPY ["AspNetCoreWebAppTest/AspNetCoreWebAppTest.csproj", "."]
RUN dotnet restore

# Projeyi derle
COPY AspNetCoreWebAppTest/. .
RUN dotnet build "AspNetCoreWebAppTest.csproj" -c Release -o /app/build

# Yay�nlama a�amas�
FROM build AS publish
RUN dotnet publish "AspNetCoreWebAppTest.csproj" -c Release -o /app/publish

# �al��ma a�amas�: Yay�nlanm�� uygulamay� �al��t�r�n
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreWebAppTest.dll"]
