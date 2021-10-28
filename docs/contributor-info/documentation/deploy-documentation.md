#Deploy documentation

Here are the steps required to:

- prepare for deployment 

- deploy documentation to website

##Prepare for deployment

###Generate Docs website (Mkdocs)

To generate the docs website based on the contents of a local open-banking-connector repo (i.e. latest edits), you will need to install Python 3 and Material for MkDocs (mkdocs-material).

Assuming you have installed Python, you can install Material for MkDocs as follows:
```powershell
pip install mkdocs-material # if mkdocs-material not installed
```
###Preview website
To deploy the website locally for inspection and testing, please `cd` to the OBC repo root and run the following command:
```
mkdocs serve
```
This will return the URL you can use to see the website.


### Tags
Everytime website is regenerated, point links to a new tag:

- decide on a version number

- create tag based on version number

```
git tag "docs_v1.0"
```

- push tag to github



Github tags are used to create a version number for a new release. This separates different versions of code and links depending on the version. 

### Overview page on website

For the overview page on the website, an overview folder with a `README.md` was created for the website heading feature. This is not compatible with Github so this folder was added to `.gitignore` to stop it from being committed.

There is a separate project `/docs/README.md` file which is visible on github.


### Link Path change from relative to absolute

Whilst making local changes it is recommended to use relative links:
```
(../../../../src/..../BankConfigurationMethods.cs#39)
```

However, before deploying links must be changed to absolute links:
```
(https://github.com/finlabsuk/open-banking-connector/blob/docs_v1.0/src/..../BankConfigurationMethods.cs#39).
```

Do a search and replace to speed this conversion up. (visual studio code)

Search:
```
\(./.*?/src
```

Replace:
```
https://github.com/finlabsuk/open-banking-connector/blob/docs_v1.0/src
```


##Deploy to website 
If you have permissions, you can update the public website on GitHub Pages using:
```powershell
mkdocs gh-deploy -m "Test deployment" -r publicRemote # adjust commit message and remote as required
```