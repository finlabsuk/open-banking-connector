This file contains notes in relation to Rider/ReSharper DotSettings file `FinnovationLabs.OpenBanking.Library.Connector.sln.DotSettings`.

# Rider/ReSharper DotSettings file

The DotSettings file is used to save team-shared settings.

These are primarily code style and other settings which configure a profile for automated code clean-up.

## Re-generating file

You can delete the DotSettings file and re-build from scratch by saving Rider settings using Rider's Save option *Solution "FinnovationLabs.OpenBanking.Library.Connector.sln.DotSettings" team-shared*.

Before doing this, please check and clean-up the contents of your other Rider settings layers (select "Manage Layers" from Rider settings) to ensure intended team-shared settings are not included in other settings layers by mistake.

The Rider settings shown below need to be configured. Please look at the previous (deleted) version of the DotSettings file to help correctly configure them.

#### > Editor / Inspection Settings / Generated Code

Please specify the directories which are excluded from code clean=up due to being automatically generated (e.g. OpenAPI-sourced types and DB migrations).

#### > Editor / Code Style / C#

Please specify preferred code style settings.

#### > Editor / File Header Template

Please specify the file header template.

#### > Editor / Code Cleanup

Create a new code clean-up profile:

1. Duplicate "Built-in: Full Cleanup" and rename to e.g. "Full Cleanup (Updated)"
2. Remove the following from "Remove redundancies & apply optimisations / C#":
    1. "Make auto-property get-only, if possible"
3. Add "Update file header (copyright) / C# / Update file header"
4. Set new profile as default for silent clean-up

## Updating code style preferences

Sometimes code style preferences require adjustment.

The easiest way to do this is to find a code sample where there is an issue. Then in Rider select the sample and use the following two context actions under "Reformat and cleanup":
- "Configure code style..." which shows settings which can affect the selected code with a preview function to see the effect of changes
- "Detect code style settings..." which shows suggested settings changes that capture the impact of edits you've made (this does not always make the best suggestions in the author's experience)

Once the desired settings are determined, save Rider settings using Rider's Save option *Solution "FinnovationLabs.OpenBanking.Library.Connector.sln.DotSettings" team-shared*. This will update the DotSettings file.

## Notes on some ReSharper code style settings

#### > Editor / Code Style / C# / Line Breaks and Wrapping / Arrangement of Initialisers

It seems setting *Wrap object and collection initializer* to *Chop Always* negates the effect of *Place simple array, object and collection on single line*. This caused some confusion. The workaround is to use *Chop if long or multiline* instead and limit elements per line to 1. It's a pity this setting affects list as well as object initialisers. 
 