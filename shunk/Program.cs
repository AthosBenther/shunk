using System;
using System.IO;
using ImageMagick;

class Program
{
    // Specify the directory to process
    static string currentDirectory = Directory.GetCurrentDirectory();
    static void Main(string[] args)
    {

        Log("Shunk running path: " + currentDirectory);
        // Get all .tga files in the specified directory
        string[] tgaFiles = Directory.GetFiles(currentDirectory, "*.tga");

        foreach (var file in tgaFiles)
        {
            // Check if the file should be ignored
            if (file.Contains("_r") || file.Contains("_g") || file.Contains("_b"))
            {
                Log($"Ignoring file: {file}");
                continue;
            }

            try
            {
                // Load the image
                using (MagickImage image = new MagickImage(file))
                {
                    // Separate the channels
                    var channels = image.Separate(Channels.RGB).ToList();

                    // Extract the file name without extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                    // Save the red channel image
                    using (MagickImage redChannel = (MagickImage)channels[0])
                    {
                        string redChannelPath = Path.Combine(currentDirectory, $"{fileNameWithoutExtension}_r.tga");
                        redChannel.Write(redChannelPath);
                    }

                    // Save the green channel image
                    using (MagickImage greenChannel = (MagickImage)channels[1])
                    {
                        string greenChannelPath = Path.Combine(currentDirectory, $"{fileNameWithoutExtension}_g.tga");
                        greenChannel.Write(greenChannelPath);
                    }

                    // Save the blue channel image
                    using (MagickImage blueChannel = (MagickImage)channels[2])
                    {
                        string blueChannelPath = Path.Combine(currentDirectory, $"{fileNameWithoutExtension}_b.tga");
                        blueChannel.Write(blueChannelPath);
                    }
                }

                Log($"Channels extracted and saved for {file}");
            }
            catch (Exception ex)
            {
                Log($"An error occurred while processing {file}: {ex.Message}");
            }
        }
    }

    static void Log(string Log)
    {
        string logFile = Path.Combine(currentDirectory, "shunk.log");
        string finalLog = DateTime.Now.ToString() + ": " + Log + '\n';
        File.AppendAllText(logFile, finalLog);
        Console.WriteLine(Log);
    }
}