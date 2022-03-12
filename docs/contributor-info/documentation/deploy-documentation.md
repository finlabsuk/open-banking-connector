# Deploy documentation

Here are the steps required to:

- prepare for deployment 

- deploy documentation to website

## Pre-requisites

To publish documentation to the website, you will need to have permission to push to the `gh-pages` branch of the repo.

You will need to install Python 3 and Material for MkDocs (mkdocs-material).

Assuming you have installed Python, you can install Material for MkDocs as follows:

```powershell
pip install mkdocs-material # if mkdocs-material not installed
pip install mkdocs-git-revision-date-plugin
```

## Prepare for deployment

### Preview website

To deploy the website locally for inspection and testing, please `cd` to the OBC repo root and run the following command:
```
mkdocs serve
```
This will return the URL you can use to see the website.


### Decide on docs version and create a tag

Everytime website is regenerated, point links to a new tag:

- decide on a version number

- create tag based on version number

```
git tag "docs_v1.0"
```

- push tag to github
```
git push publicRemote docs_v1.0
```


Github tags are used to create a version number for a new release. This separates different versions of code and links depending on the version. 

### Create overview page (temporary change - do not commit)

For the overview page on the website, an overview folder with a `README.md` was created for the website heading feature. This is not compatible with Github so this folder was added to `.gitignore` to stop it from being committed.

There is a separate project `/docs/README.md` file which is visible on github.


### Convert code links to absolute paths (temporary change - do not commit)

Relative paths are normally used for links to code in the docs. Example:
```
(../../../../src/..../BankConfigurationMethods.cs#39)
```

However, before running `mkdocs` to generate the website, relative links that go outside the `docs` folder must be converted to absolute links:
```
(https://github.com/finlabsuk/open-banking-connector/blob/docs_v1.0/src/..../BankConfigurationMethods.cs#39).
```

Here is a regular expression search and replace that may help to speed this conversion up (we used this in Visual Studio Code)

Search:
```
\(\.\.\/[\.\/]*?src
```

Replace:
```
(https://github.com/finlabsuk/open-banking-connector/blob/docs_v1.5/src
```

*Please do not commit these changes to links.*

## Deploy to website 

Assuming you have permissions to push to the repo `gh-pages` branch, you can update the public website on GitHub Pages using:
```powershell
mkdocs gh-deploy -m "Deploy Docs using Tag v1.4" -r publicRemote --ignore-version # adjust commit message and remote as required 
```

N.B. The ```--ignore-version```
flag is used to solve the problem of Mkdocs confusing the tag version with the Mkdocs version which stops the website from being deployed. 