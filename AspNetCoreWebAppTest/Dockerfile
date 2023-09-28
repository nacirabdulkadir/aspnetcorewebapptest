FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Projeyi kopyala ve baðýmlýlýklarý yeniden yükle
COPY ["AspNetCoreWebAppTest.csproj", "."]
RUN dotnet restore

# Projeyi derle
COPY . .
WORKDIR "/src"
RUN dotnet build "AspNetCoreWebAppTest.csproj" -c Release -o /app/build

# Yayýnlama aþamasý
FROM build AS publish
RUN dotnet publish "AspNetCoreWebAppTest.csproj" -c Release -o /app/publish

# Çalýþma aþamasý: Yayýnlanmýþ uygulamayý çalýþtýrýn
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreWebAppTest.dll"]