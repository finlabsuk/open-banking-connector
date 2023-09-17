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

## Deploy website 

Assuming you have permissions to push to the repo `gh-pages` branch, you can update the public website on GitHub Pages using:
```bash
mkdocs gh-deploy -m "Message" -r publicRemote --ignore-version # adjust commit message and remote as required 
```

N.B. The `--ignore-version` flag is used to solve the problem of Mkdocs confusing the tag version with the Mkdocs version which stops the website from being deployed.