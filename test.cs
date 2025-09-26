name: Build and SAST Scan

on:
  push:
    branches:
      - main
  pull_request:
    branches:
      - main

jobs:
  sonarcloud-sast:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout Code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0 # Fetch all history for accurate 'Clean as You Code' analysis

    # --- SonarQube Prerequisites ---
    - name: Set up JDK 17 (Required by SonarScanner)
      uses: actions/setup-java@v4
      with:
        distribution: 'temurin'
        java-version: '17'

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x' 

    - name: Install SonarScanner for .NET
      run: dotnet tool install --global dotnet-sonarscanner

    # --- 1. BEGIN ANALYSIS ---
    - name: Begin SonarQube Analysis
      env:
        # Use the secret you created in GitHub
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }} 
      run: |
        dotnet sonarscanner begin \
          /k:"supersamoan808_my-first-project" \
          /o:"supersamoan808" \
          /d:sonar.token="$SONAR_TOKEN" \
          /d:sonar.host.url="https://sonarcloud.io"

    # --- 2. BUILD THE PROJECT ---
    - name: Build the .NET Solution
      # Use your specific build command, e.g., for a solution file (.sln)
      run: dotnet build --no-incremental

    # --- 3. END ANALYSIS ---
    - name: End SonarQube Analysis and Upload Results
      env:
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }} 
      run: |
        dotnet sonarscanner end \
          /d:sonar.token="$SONAR_TOKEN"
          
    # You can add tests/coverage reporting steps here if needed