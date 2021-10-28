#Update documentation

Here are the steps required to:

- update current documentation using markdown 

- add new documentation

##Update documentation
To update the current documentation:

- go to `/docs` repo folder

- edit folder name in `/docs`

- edit the `README.md` file inside the folder

- edit the `file.md` file

Any changes made must be applied to the nav section in the `mkdocs.yml`. (e.g. file/folder rename or file structure changes)

Links to content in the repo must be relative links. For example:

```
[update documentation](./update-documentation.md)
```

The next step is to [generate docs website and deploy documentation.](./deploy-documentation.md)

##Add new documentation

To add new documentation:

- go to `/docs` repo folder

- create new folder in `/docs`

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