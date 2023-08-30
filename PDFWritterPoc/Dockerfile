FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PDFWritterPoc/PDFWritterPoc.csproj", "PDFWritterPoc/"]
RUN dotnet restore "PDFWritterPoc/PDFWritterPoc.csproj"
COPY . .
WORKDIR "/src/PDFWritterPoc"
RUN dotnet build "PDFWritterPoc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PDFWritterPoc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PDFWritterPoc.dll"]
