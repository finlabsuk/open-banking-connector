# Update documentation

To update documentation:

- edit files in the repo `./docs` folder
- update the `nav` section in the repo `./mkdocs.yml` file to specify sections and links.

In the docs, please ensure links to other repo files are relative links. For example:

```markdown
[Page](./page.md)
```

Once merged to `master`, docs are [deployed](./deploy-documentation.md) automatically with the next release.
