# MOJEWIDEŁO Backend recommendation engine

## Dependencies

- Python 3.11

  ```
  https://www.python.org/downloads/
  ```

- Poetry

  Linux

  ```
  curl -sSL https://install.python-poetry.org | python3 -
  ```

  Windows (PowerShell)

  ```
  (Invoke-WebRequest -Uri https://install.python-poetry.org -UseBasicParsing).Content | py -
  ```

- PIP packages
  ```
  poetry install
  ```

## How to run

```
poetry run start
```

## How to use Poetry

- `poetry shell` - activate virtual environment
- `poetry add <name>` - add PIP package (use this instead of `pip install <name>`)

## VS Code virtual environment configuration

If type checking or automatic imports are not working, you need to point VS Code to the right virtual environment.

1. `poetry shell`
1. `poetry env info`
1. Copy `Executable` path
1. `Ctrl+Shift+P` -> `Python: Select interpreter`
1. Paste path

## Code formatting

This project uses Black for code formatting and isort for organizing imports.

Run Black using:

- (Recommended) VS Code extension
- With command line
  ```
  black .
  ```

Run isort using:

- (Recommended) Automatically in VS Code on save:

  Add to `settings.json`:

  ```JSON
  "editor.codeActionsOnSave": {
    "source.organizeImports": true
  }
  ```

- In VS Code, manually

  `Ctrl+Shift+P` -> `Organize imports`

- With command line
  ```
  isort .
  ```
