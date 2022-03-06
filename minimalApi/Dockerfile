#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
RUN ls
#COPY ["minimalApi.csproj", "minimalApi/"]
#RUN dotnet restore "minimalApi/minimalApi.csproj"
COPY . .
RUN ls
RUN dotnet restore "minimalApi.csproj"
#WORKDIR "/src/minimalApi"
RUN ls
RUN dotnet build "minimalApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "minimalApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_ReadyToRun=0 
ENV DOTNET_TieredPGO=1 
ENV DOTNET_TC_QuickJitForLoops=1
ENTRYPOINT ["dotnet", "minimalApi.dll"]