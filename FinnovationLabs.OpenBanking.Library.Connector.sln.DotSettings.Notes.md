This file contains notes in relation to settings file FinnovationLabs.OpenBanking.Library.Connector.sln.DotSettings.

# How the code cleanup profile was created.

Since this is not obvious from the file, the following steps were used to create the code cleanup profile in Visual Studio/ReSharper.

1. Duplicate and edit "Built-in: Full Cleanup"
2. Rename to "Full Cleanup With Updates"
2. Make changes to C# settings
    a. Add "Update file header"
    b. Remove "Code Styles: Apply code bdy style (expression body vs. block body)"

# Modification to file layout patterns

Changed to remove unit test method sorting:

1. Go to Code Editing / C# / File Layout / Patterns / NUnit Test Fixtures
2. Set Test Methods / Sort By to "None"
