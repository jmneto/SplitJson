# SplitJson

SplitJson is a sample command-line utility designed to split a large JSON file into multiple smaller JSON files. This tool is particularly useful for handling large datasets that need to be divided for easier processing or distribution. The program takes a source JSON file and the number of smaller files you want to split it into as inputs, and efficiently distributes the JSON objects across the specified number of output files.

# Features:
- Command-line interface for easy integration with scripts and automation workflows.
- Ability to handle very large JSON files using asynchronous streaming to minimize memory usage.
- Customizable number of output files based on user requirements.
- Each output file is a valid JSON array, making it ready for further processing or analysis.
- Robust error handling to manage and report issues during the file splitting process.

# Usage:

The utility is invoked from the command line with two arguments: the path to the source JSON file and the number of smaller files to split into. For example:

```
SplitJson.exe "C:\path\to\file.json" 5
```

This command will split 'file.json' into 5 smaller JSON files and save them in the same directory as the source file.

This tool is ideal for developers, data scientists, and system administrators who work with large JSON datasets and require a reliable way to segment data into manageable parts. Whether you're preprocessing data for machine learning, distributing workload across systems, or simply organizing data, SplitJson provides a straightforward solution.

# Note

The code is not generic. For efficiency the Type existing in the Json array must be manually defined as a class inside the code. 

