# Deploy documentation

Here are the steps required to:

- prepare for deployment 

- deploy documentation to website

## Pre-requisites

To publish documentation to the website, you will need to have permission to push to the `gh-pages` branch of the repo.

You will require Python 3 and Material for MkDocs (mkdocs-material).

Assuming you have installed Python, you can install Material for MkDocs as follows:

```bash
pip install mkdocs-material
pip install mkdocs-git-revision-date-localized-plugin 
pip install mkdocs-render-swagger-plugin
```

## Prepare for deployment

### Preview website

To deploy the website locally for inspection and testing, please `cd` to the repo root and run the following command:
```
mkdocs serve
```
This will return the URL you can use to preview the website. Commit and push any corrections or updates required.

### Convert code links to absolute paths (temporary change - do not commit)

Relative paths are used for links to other repo files in the docs. However, before running `mkdocs` to generate the website, relative links that go outside the `docs` folder must be converted to absolute links.

If no version tag is available for absolute links, create and push a git tag with a new version number for the docs:
```bash
git tag "docs_v1.0"
git push publicRemote docs_v1.0
```
So, for example,
```
(../../../../src/..../BankConfigurationMethods.cs#39)
```
must be converted to

```
(https://github.com/finlabsuk/open-banking-connector/blob/tag/src/..../BankConfigurationMethods.cs#39).
```

Here is a regular expression search and replace that may be used to do this in (we used this in Visual Studio Code.

Search:
```
\(\.\.\/[\.\/]*?src
```

Replace (with correct tag):
```
(https://github.com/finlabsuk/open-banking-connector/blob/tag/src
```

*Note: Please do not commit these changes to links. They can be discarded once the website is deployed.*

## Deploy website 

Assuming you have permissions to push to the repo `gh-pages` branch, you can update the public website on GitHub Pages using:
```bash
mkdocs gh-deploy -m "Message" -r publicRemote --ignore-version # adjust commit message and remote as required 
```

N.B. The `--ignore-version` flag is used to solve the problem of Mkdocs confusing the tag version with the Mkdocs version which stops the website from being deployed.