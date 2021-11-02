#Update documentation

Here are the steps required to:

- update current documentation using markdown 

- add new documentation

##Update documentation
To update the current documentation:

- edit the required files in the `/docs` repo folder

- Any changes made must be applied to the nav section in the `mkdocs.yml`. (e.g. file/folder rename or file structure changes)

- Links to content in the repo must be relative links. For example:

```
[Page](./page.md)
```

- The next step is to [generate docs website and deploy documentation.](./deploy-documentation.md)

##Add new documentation

To add new documentation:

- create new folder in `/docs` repo folder

- create new `README.md` file inside the folder

- create `file.md` file


In `mkdocs.yml`, the nav section must include your changes:

```yml
nav:
- SectionTitle:
    - folder/file.md
    - PageTitle: file.md
```

The next step is to [generate docs website and deploy documentation.](./deploy-documentation.md)