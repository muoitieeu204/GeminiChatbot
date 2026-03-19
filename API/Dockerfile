# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files first (this helps with caching)
COPY ["GeminiChatbot/GeminiChatbot.csproj", "GeminiChatbot/"]
RUN dotnet restore "GeminiChatbot/GeminiChatbot.csproj"

# Copy the rest of the code and build it
COPY . .
WORKDIR "/src/GeminiChatbot"
RUN dotnet build "GeminiChatbot.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "GeminiChatbot.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render uses the PORT environment variable; ASP.NET Core 8.0+ 
# listens on 8080 by default, which Render handles automatically.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GeminiChatbot.dll"]