//MIT License
//
//Copyright (c) 2024 - Jose Batista-Neto
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SplitJson;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("SplitJson - Split a JSON file into multiple smaller files.");

        if (args.Length == 0 || args[0] == "/?")
        {
            ShowHelp();
            return;
        }

        if (args.Length != 2)
        {
            Console.WriteLine("\nInvalid number of arguments.");
            ShowHelp();
            return;
        }

        // show parameter values
        Console.WriteLine($"\nSource File: {args[0]}");
        Console.WriteLine($"Number of files to split: {args[1]}");

        // check if file exists
        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.WriteLine("\nSource File does not exist.");
            return;
        }

        int numberOfFiles;
        if (!int.TryParse(args[1], out numberOfFiles) || numberOfFiles <= 0)
        {
            Console.WriteLine("\nInvalid number of files.");
            return;
        }

        await SplitJsonFile(filePath, numberOfFiles);
    }

    static void ShowHelp()
    {
        Console.WriteLine("\nUsage: SplitJson.exe <sourceJsonFilePath> <numberOfFiles>");
        Console.WriteLine("     <sourceJsonFilePath>: Path to the source JSON file.");
        Console.WriteLine("     <numberOfFiles>: Number of smaller files to split the JSON into.");
        Console.WriteLine("\nExample: SplitJson.exe \"C:\\path\\to\\file.json\" 5");
        Console.WriteLine("\nThis will split the file.json into 5 smaller files and put them in the same folder as the input json.");
    }

    static async Task SplitJsonFile(string filePath, int numberOfFiles)
    {
        try
        {
            // Create an output file stream for each output file
            List<StreamWriter> writers = new List<StreamWriter>();

            // Track first object for each file
            List<bool> isFirstObject = new List<bool>();

            // Initialize the list of StreamWriters and isFirstObject flags
            for (int i = 0; i < numberOfFiles; i++)
            {
                StreamWriter writer = new StreamWriter(File.Create($"{filePath.ToLower().Replace(".json", "")}_split_{i + 1}.json"));
                await writer.WriteAsync("["); // Start JSON array
                writers.Add(writer);
                isFirstObject.Add(true); // Initialize as true for each file
            }

            // Open the JSON file as a stream so the file can be huge and read asynchronously
            await using FileStream stream = File.OpenRead(filePath);

            // Deserialize/Serialize JSON options
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Ignore null properties
            };

            // Deserialize JSON as a async stream of objects
            IAsyncEnumerable<MailBoxInfo?> objects = JsonSerializer.DeserializeAsyncEnumerable<MailBoxInfo>(stream, jsonSerializerOptions);

            // Iterate through the objects and write them to the output files
            int counter = 0;
            await foreach (MailBoxInfo? obj in objects)
            {
                // throw exception if obj is null
                if (obj == null)
                    throw new Exception("Mailbox Object is null");

                // Determine which output file to write to based on the counter
                int fileIndex = counter % numberOfFiles;
                if (!isFirstObject[fileIndex])
                    await writers[fileIndex].WriteAsync(","); // Add comma before next object
                else
                    isFirstObject[fileIndex] = false; // Update flag after first object is written

                // Serialize the object to JSON and write it to the output file
                string json = JsonSerializer.Serialize(obj, jsonSerializerOptions);
                await writers[fileIndex].WriteAsync(json);

                // Log progress
                Console.WriteLine($"Wrote object {counter} to file {fileIndex + 1}");
                counter++;
            }

            // Close the output files
            foreach (StreamWriter writer in writers)
            {
                await writer.WriteAsync("]"); // End JSON array
                writer.Close();
            }

            Console.WriteLine("\nJSON file split successfully.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}