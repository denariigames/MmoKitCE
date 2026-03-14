using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if !DISABLE_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class Builder
    {
        public static void Build(BuildTarget target, StandaloneBuildSubtarget subTarget)
        {
            string outputPath = $"./Builds/{target}";
            string exeName = "Build.exe";
            string bundleVersion = PlayerSettings.bundleVersion;
            string addressableProfileName = "Default";
            bool developmentMode = false;

            string versionCode = "";
            string keystoreName = PlayerSettings.Android.keystoreName;
            string keystorePass = PlayerSettings.Android.keystorePass;
            string keyaliasName = PlayerSettings.Android.keyaliasName;
            string keyaliasPass = PlayerSettings.Android.keyaliasPass;
            bool buildAppBundle = EditorUserBuildSettings.buildAppBundle;

            bool cleanContent = false;
            bool purgeCache = false;
            bool generateMapServerDockerfile = false;
            int mapServerPortInDockerfile = 6000;

            switch (target)
            {
                case BuildTarget.Android:
                    versionCode = PlayerSettings.Android.bundleVersionCode.ToString();
                    break;
                case BuildTarget.iOS:
                    versionCode = PlayerSettings.iOS.buildNumber;
                    break;
            }

            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].ToLower().Equals("-outputpath") && i + 1 < args.Length)
                    outputPath = args[i + 1];
                if (args[i].ToLower().Equals("-exename") && i + 1 < args.Length)
                    exeName = args[i + 1];
                if (args[i].ToLower().Equals("-bundleversion") && i + 1 < args.Length)
                    bundleVersion = args[i + 1];
                if (args[i].ToLower().Equals("-versioncode") && i + 1 < args.Length)
                    versionCode = args[i + 1];
                if (args[i].ToLower().Equals("-keystorename") && i + 1 < args.Length)
                    keystoreName = args[i + 1];
                if (args[i].ToLower().Equals("-keystorepass") && i + 1 < args.Length)
                    keystorePass = args[i + 1];
                if (args[i].ToLower().Equals("-keyaliasname") && i + 1 < args.Length)
                    keyaliasName = args[i + 1];
                if (args[i].ToLower().Equals("-keyaliaspass") && i + 1 < args.Length)
                    keyaliasPass = args[i + 1];
                if (args[i].ToLower().Equals("-addressableprofilename") && i + 1 < args.Length)
                    addressableProfileName = args[i + 1];
                if (args[i].ToLower().Equals("-buildappbundle") && i + 1 < args.Length)
                    buildAppBundle = bool.Parse(args[i + 1]);
                if (args[i].ToLower().Equals("-developmentmode") && i + 1 < args.Length)
                    developmentMode = bool.Parse(args[i + 1]);
                if (args[i].ToLower().Equals("-cleancontent") && i + 1 < args.Length)
                    cleanContent = bool.Parse(args[i + 1]);
                if (args[i].ToLower().Equals("-purgecache") && i + 1 < args.Length)
                    purgeCache = bool.Parse(args[i + 1]);
                if (args[i].ToLower().Equals("-generatemapserverdockerfile") && i + 1 < args.Length)
                    generateMapServerDockerfile = bool.Parse(args[i + 1]);
                if (args[i].ToLower().Equals("-mapserverportindockerfile") && i + 1 < args.Length)
                    mapServerPortInDockerfile = int.Parse(args[i + 1]);
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogError("No output path");
                return;
            }

            switch (target)
            {
                case BuildTarget.Android:
                    PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
                    PlayerSettings.Android.keystoreName = keystoreName;
                    PlayerSettings.Android.keystorePass = keystorePass;
                    PlayerSettings.Android.keyaliasName = keyaliasName;
                    PlayerSettings.Android.keyaliasPass = keyaliasPass;
                    EditorUserBuildSettings.buildAppBundle = buildAppBundle;
                    break;
                case BuildTarget.iOS:
                    PlayerSettings.iOS.buildNumber = versionCode;
                    break;
            }

            PlayerSettings.bundleVersion = bundleVersion;

            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                    Debug.Log($"Add {EditorBuildSettings.scenes[i].path} to scenes in build list.");
                }
            }

#if UNITY_2021_1_OR_NEWER
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                target = target,
                options = developmentMode ? BuildOptions.Development : BuildOptions.None,
                subtarget = (int)subTarget,
                locationPathName = Path.Combine(outputPath, exeName),
                scenes = scenes.ToArray(),
            };
#else
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                target = target,
                options = BuildOptions.None,
                locationPathName = Path.Combine(outputPath, exeName),
                scenes = scenes.ToArray(),
            };
#endif
#if !DISABLE_ADDRESSABLES
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            string profileId = settings.profileSettings.GetProfileId(addressableProfileName);
            settings.activeProfileId = profileId;
            AddressableAssetSettings.PlayerBuildOption preChangeBuildOption = settings.BuildAddressablesWithPlayerBuild;
            settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
            if (cleanContent)
                CleanAddressablePlayerContent();
            if (purgeCache)
                PurgeBuildCache();
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (generateMapServerDockerfile)
                GenerateMapServerDockerfile(outputPath, exeName, mapServerPortInDockerfile);
            Debug.Log($"Build {target}, {subTarget}, v.{bundleVersion}({versionCode}), aa profile: {addressableProfileName}\nResult: {report.summary.result}");
            settings.BuildAddressablesWithPlayerBuild = preChangeBuildOption;
#endif
            EditorApplication.Exit(0);
        }

        public static void BuildWindows64Server()
        {
            Build(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Server);
        }

        public static void BuildLinux64Server()
        {
            Build(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Server);
        }

        public static void BuildAndroid()
        {
            Build(BuildTarget.Android, StandaloneBuildSubtarget.Player);
        }

        public static void BuildiOS()
        {
            Build(BuildTarget.iOS, StandaloneBuildSubtarget.Player);
        }

        public static void CleanAddressablePlayerContent()
        {
#if !DISABLE_ADDRESSABLES
            AddressableAssetSettings.CleanPlayerContent();
            Debug.Log("Cleaned previous Addressables build output.");
#endif
        }

        public static void PurgeBuildCache()
        {
            BuildCache.PurgeCache(false);
            Debug.Log("Purged global SBP build cache.");
        }
        public static void GenerateMapServerDockerfile(string buildPath, string exeName, int port = 6000)
        {
            string shFilePath = Path.Combine(buildPath, "run.sh");
            string shFileContent = $@"#!/bin/bash

# Command to run your Unity server executable
EXECUTABLE=""./{exeName}""
ARGS=""-batchmode -nographics -logfile /dev/stdout -startMapServer""

while true; do
    echo ""Starting server...""
    $EXECUTABLE $ARGS
    EXIT_CODE=$?
    echo ""Server exited with code $EXIT_CODE. Restarting in 2 seconds...""
    sleep 2
done
";
            File.WriteAllText(
                shFilePath,
                shFileContent.Replace("\r\n", "\n"), // force LF
                new System.Text.UTF8Encoding(false) // no BOM
            );
            Debug.Log($"Sh generated at: {shFilePath}");

            string dockerFilePath = Path.Combine(buildPath, "Dockerfile");
            string dockerFileContent = $@"# Use a lightweight base image with the necessary dependencies
FROM ubuntu:20.04

# Set environment variables
ENV DEBIAN_FRONTEND=noninteractive

# Install necessary packages
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libsm6 \
    libxi6 \
    libxcursor1 \
    libxrandr2 \
    libxinerama1 \
    libglu1-mesa \
    libnss3 \
    libgcc1 \
    libgconf-2-4 \
    libnss3 \
    libxss1 \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /unity-server

# Copy the Unity build to the container
COPY . /unity-server

# Ensure the server binary has execution permissions
RUN chmod +x /unity-server/{exeName}
RUN chmod +x /unity-server/run.sh

# Convert CRLF -> LF just in case
RUN sed -i 's/\r$//' /unity-server/run.sh

# Expose any ports the server needs to communicate on
EXPOSE {port}/tcp {port}/udp

# `mapPort` map-server port
ENV mapPort={port}

# Define the command to run the server
ENTRYPOINT [""./run.sh""]";

            File.WriteAllText(dockerFilePath, dockerFileContent);
            Debug.Log($"Dockerfile generated at: {dockerFilePath}");
        }
    }
}
