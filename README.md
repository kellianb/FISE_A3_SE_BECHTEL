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

![use_case.svg](assets/use_case.svg)

### Sequence Diagram

#### Create and run a single job

```mermaid
sequenceDiagram
    actor User
    participant CLI
    participant jobManager
    participant #58;BackupCommand
    participant #58;DisplayChanges

    activate User
    User->>+CLI: Enters command: run <source-path> <target>

    alt no error
        CLI->>+jobManager: Build backup command
        jobManager-->>-CLI: Return backup command
        CLI->>+#58;DisplayChanges: Retrieve changes of concerned files and directories
        #58;DisplayChanges-->>-CLI: Return changes
        CLI-->>User: Ask for confirmation of changes
        alt user confirm changes
            User->>CLI: Confirm changes
            CLI->>+#58;BackupCommand: Execute backup command
            #58;BackupCommand-->>CLI: Respond with success
            CLI-->>User: Send message of success
        else user cancel changes
            User->>CLI: Cancel changes
        end
    else error
        #58;BackupCommand-->>CLI: Send a message of error
        deactivate #58;BackupCommand
        CLI-->>User: Send message of error
        end
    deactivate User
```

#### Run job(s) from JSON file

```mermaid
sequenceDiagram
    actor User
    participant CLI
    participant jobManager
    participant #58;BackupCommand
    participant #58;DisplayChanges


    activate User
    User->>+CLI: Enters command: load <path>

    alt no error
        CLI->>+jobManager: Load jobs from JSON file
        jobManager-->>CLI: Return jobs manager
        CLI-->User: Display jobs list and ask which jobs to run
        User->>CLI: Select jobs indexes to run
        CLI->>jobManager: Build backup command for indexes
        jobManager-->>-CLI: Return backup command
        CLI->>+#58;DisplayChanges: Retrieve changes of concerned files and directories
        #58;DisplayChanges-->>-CLI: Return changes
        CLI-->>User: Ask for confirmation of changes
        alt user confirm changes
            User->>CLI: Confirm changes
            CLI->>+#58;BackupCommand: Execute backup command
            #58;BackupCommand-->>CLI: Respond with success
            CLI-->>User: Send message of success
        else user cancel changes
            User->>CLI: Cancel changes
        end
    else error
        #58;BackupCommand-->>CLI: Send a message of error
        deactivate #58;BackupCommand
        CLI-->>User: Send message of error
        end
    deactivate User
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

#### Delete job

```mermaid
```

### Class Diagram

```mermaid
classDiagram

class Job {
    +string Name
    +string SourcePath
    +string TargetPath
    +bool recursive
    +bool differential
}

class FileSystemChange {
    +string TargetPath
}

class FileChange {
    +string? SourcePath
    +long FileSize
    +FileChangeType ChangeType
}

class DirectoryChange {
    +DirectoryChangeType ChangeType
}


class BackupTransaction {
    +List[FileChange] FileChanges
    +List[DirectoryChange] DirectoryChanges
    +string[] GetConcernedFolders(DirectoryChangeType)
    -BackupTransaction AddDirectoryChange(DirectoryChange)
    +BackupTransaction AddDirectoryCreation(string)
    +BackupTransaction AddDirectoryDeletion(DirectoryInfo)
    +long GetTotalCopiedFileSize()
    +string[] GetConcernedFiles(FileChangeType)
    -BackupTransaction AddFileChange(FileChange)
    +BackupTransaction AddFileCreation(FileInfo, string)
    +BackupTransaction AddFileUpdate(FileInfo, FileInfo)
    +BackupTransaction AddFileDeletion(FileInfo)
}

class IBackupTransactionBuilder {
    +BackupTransaction Build(Job)
    +BackupTransaction Build(Job[])
}

class BackupTransactionBuilder {
    +BackupTransaction Build(Job)
    +BackupTransaction Build(Job[])
}

class JobManager {
    -IBackupTransactionBuilder _transactionBuilder
    -IBackupTransactionExecutor _transactionExecutor
    +List[Job] Jobs
    +JobManager LoadJobsFromFile(string)
    +BackupCommand GetBackupCommandForIndexes(HashSet[int])
    +BackupCommand BuildBackupCommand(List[Job])
    +BackupCommand BuildBackupCommand(Job concernedJobs)
}

class IBackupTransactionExecutor {
    +Execute(BackupTransaction)
}

class BackupTransactionExecutor {
    +Execute(BackupTransaction)
}

class BackupCommand {
    -IBackupTransactionExecutor receiver
    -BackupTransaction transaction
    +Execute()
    +long GetTotalCopiedFileSize()
    +Dictionary[FileChangeType, string[]] GetConcernedFiles()
    +Dictionary[DirectoryChangeType, string[]] GetConcernedDirectories()
}

class JobFileLoader {
    +List[Job] LoadJobsFromFile(string)
}

class JsonDeserializer {
    List[Job] Deserialize(TextReader reader)
}

JobManager *-- Job
FileChange --|> FileSystemChange
DirectoryChange --|> FileSystemChange
BackupTransaction *-- FileChange
BackupTransaction *-- DirectoryChange
BackupTransactionExecutor --|> IBackupTransactionExecutor

BackupTransactionBuilder --|> IBackupTransactionBuilder
BackupTransactionBuilder ..> BackupTransaction : instantiates

JobManager *--IBackupTransactionBuilder
JobManager *--IBackupTransactionExecutor
BackupCommand *-- BackupTransaction
BackupCommand *-- IBackupTransactionExecutor

JobManager ..> BackupCommand : instantiates

JobManager ..> JobFileLoader
JobFileLoader ..> Job : instantiates
JobFileLoader ..> JsonDeserializer
```

### Activity Diagram

TODO: WIP

```mermaid
    graph TD;
        Z[Launch EasySave] --> A[Create or edit a backup job];
        Z --> E[Load jobs];
        Z --> O[Select language];
        subgraph createEditJob
            A --> N{Less than 5 jobs already existing?};
            N -- Yes --> B{Confirm changes?};
            N -- No --> D[Cancel changes];
            B -- Yes --> C[Add job to the list];
            B -- No --> D;
            C --> M[End];
            D --> M;
        end
        subgraph runJob
            E --> F[Display jobs information];
            F --> G[Ask user to select jobs to execute];
            G --> H[Display modification information];
            H--> I{Confirm modifications ?};
            I -- Yes --> J[Execute jobs];
            I -- No --> K[Cancel modifications];
            J --> L[End];
            K --> L;

        end
        subgraph selectLanguage
            O --> P[End];

        end
        M --> Z;
        L --> Z;
        P --> Z;

```
