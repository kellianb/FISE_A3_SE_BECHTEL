# Software engineering project

| Project Members |
|-----------------|
| Laura GIESE     |
| Kellian BECHTEL |
| Evan CAUMARTIN  |

## How to make changes

- Clone the project: `git pull <repo url>`
- Create a new feature branch: `git checkout -b 'feature/<name-of-your-feature>'`
- Commit your changes: `git commit -m '<Tell us what you did and why here>'`
- Push you changes: `git push`
- Open a PR on github


## How to solve merge conflicts

Prefer git rebases over merges

Note: When done rebasing, you have to push using `git push --force`

## UML Diagrams

### Use Case Diagram

![use_case.svg](assets%2Fuse_case.svg)

### Sequence Diagram

#### Create and edit backup job

```mermaid
sequenceDiagram
    actor User
    participant #58;BackupUtilCli

    activate User
    User->>+#58;BackupUtilCli: Give source and target paths, type and name of backup
    alt no error
        #58;BackupUtilCli->>+transaction: Build job with given information
        #58;BackupUtilCli-->>User: Ask for confirmation of changes
        alt user confirm changes
            User->>#58;BackupUtilCli: Confirm changes
            #58;BackupUtilCli->>transaction: Execute backup transaction
            transaction-->>#58;BackupUtilCli: Respond with success
            #58;BackupUtilCli-->>User: Send message of success
        else user cancel changes
            User->>#58;BackupUtilCli: Cancel changes
        end
    else error
        transaction-->>#58;BackupUtilCli: Send a message of error
        deactivate transaction
        #58;BackupUtilCli-->>User: Send a message of error
        deactivate #58;BackupUtilCli
        deactivate User
    end
```

#### Select language

```mermaid
    sequenceDiagram
        actor User
        participant #58;I18n

        activate User
        User->>+#58;I18n: Select language
        deactivate #58;I18n
        deactivate User
```

#### Run one or all jobs

```mermaid
sequenceDiagram
    actor User
    participant #58;SingleFileHandler

    activate User
    alt single job
        User->>+#58;SingleFileHandler: Run job
        deactivate #58;SingleFileHandler
        else all jobs
        User->>+#58;DirectoryHandler: Run all jobs
        deactivate #58;DirectoryHandler
        deactivate User
        end


```

#### Delete job

```mermaid
```

### Class Diagram

TODO

### Activity Diagram
