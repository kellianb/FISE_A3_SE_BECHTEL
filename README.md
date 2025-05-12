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
    participant #58;Program
    participant rootCommand
    participant #58;SingleJobCommand
    participant command
    participant jobManager
    participant transaction
    participant #58;BackupCommand
    participant #58;DisplayChanges
    participant #58;I18N

    activate User
    User->>+#58;Program: Enter run command with arguments
    #58;Program->>+rootCommand: Retrieve command
    rootCommand->>+#58;SingleJobCommand: Transmit command
    #58;SingleJobCommand->>+command: Build command arguments source and target paths, type and name of backup
    command-->>#58;SingleJobCommand: Set command handler

    alt no error
        #58;SingleJobCommand->>+jobManager: Build backup command
        jobManager->>+transaction: Build job with given information
        transaction->>+#58;BackupCommand: Build backup command
        #58;BackupCommand-->>transaction: Return backup command
        transaction-->>-jobManager: Return backup command
        jobManager-->>-#58;SingleJobCommand: Return backup command
        #58;SingleJobCommand->>+#58;DisplayChanges: Retrieve changes of concerned files and directories
        #58;DisplayChanges->>+#58;I18N: Get messages in the right language
        activate #58;I18N
        #58;I18N-->>#58;DisplayChanges: Return messages
        #58;DisplayChanges-->>-#58;SingleJobCommand: Return changes
        #58;SingleJobCommand->>I18N: Get message of change confirmation in the right language
        #58;I18N-->>#58;SingleJobCommand: Return message
        #58;SingleJobCommand-->>User: Ask for confirmation of changes
        alt user confirm changes
            User->>#58;SingleJobCommand: Confirm changes
            #58;SingleJobCommand->>#58;BackupCommand: Execute backup command
            #58;BackupCommand-->>#58;SingleJobCommand: Respond with success
            #58;SingleJobCommand->>I18N: Get message of success in the right language
            #58;I18N-->>#58;SingleJobCommand: Return message
            #58;SingleJobCommand-->>User: Send message of success
        else user cancel changes
            User->>#58;SingleJobCommand: Cancel changes
        end
    else error
        #58;BackupCommand-->>#58;SingleJobCommand: Send a message of error
        deactivate #58;BackupCommand
        #58;SingleJobCommand->>I18N: Get message of error in the right language
        #58;I18N-->>-#58;SingleJobCommand: Return message
        deactivate #58;I18N
        #58;SingleJobCommand-->>User: Send message of error
        end
        #58;SingleJobCommand-->>-rootCommand: Return command
        rootCommand-->>-#58;Program: Invoke run command
        #58;Program-->>-User: Display command messages in console
        deactivate command
    deactivate User
```

#### Run job(s) from JSON file

```mermaid
sequenceDiagram
    actor User
    participant #58;Program
    participant rootCommand
    participant #58;LoadJobsCommand
    participant command
    participant jobManager
    participant transaction
    participant #58;BackupCommand
    participant #58;JobFileLoader
    participant #58;JsonDeserializer
    participant #58;DisplayChanges
    participant #58;I18N
    participant #58;SelectionStringParser


    activate User
    User->>+#58;Program: Enter load command with path argument
    #58;Program->>+rootCommand: Retrieve command
    rootCommand->>+#58;LoadJobsCommand: Transmit command
    #58;LoadJobsCommand->>+command: Build command with path argument
    command-->>#58;LoadJobsCommand: Set command handler

    alt no error
        #58;LoadJobsCommand->>+jobManager: Load jobs from JSON file
        jobManager-->>#58;JobFileLoader: Load jobs from JSON file
        #58;JobFileLoader->>+#58;JsonDeserializer: Parse JSON file
        #58;JsonDeserializer-->>-#58;JobFileLoader: Return jobs parsed into a list
        #58;JobFileLoader-->>jobManager: Return jobs list
        jobManager-->>#58;LoadJobsCommand: Return jobs manager
        #58;LoadJobsCommand-->User: Display jobs list and ask which jobs to run
        User->>#58;LoadJobsCommand: Select jobs indexes to run
        #58;LoadJobsCommand->>+#58;SelectionStringParser: Parse selection string
        #58;SelectionStringParser-->>-#58;LoadJobsCommand: Return selected jobs indexes
        #58;LoadJobsCommand->>jobManager: Build backup command for indexes
        jobManager->>+transaction: Build job(s) with given information
        transaction->>+#58;BackupCommand: Build backup command
        #58;BackupCommand-->>transaction: Return backup command
        transaction-->>-jobManager: Return backup command
        jobManager-->>-#58;LoadJobsCommand: Return backup command
        #58;LoadJobsCommand->>+#58;DisplayChanges: Retrieve changes of concerned files and directories
        #58;DisplayChanges ->> +#58;I18N: Get messages in the right language
        #58;I18N-->>#58;DisplayChanges: Return messages
        #58;DisplayChanges-->>-#58;LoadJobsCommand: Return changes
        #58;LoadJobsCommand->>I18N: Get message of change confirmation in the right language
        #58;I18N-->>#58;LoadJobsCommand: Return message
        #58;LoadJobsCommand-->>User: Ask for confirmation of changes
        alt user confirm changes
            User->>#58;LoadJobsCommand: Confirm changes
            #58;LoadJobsCommand->>#58;BackupCommand: Execute backup command
            #58;BackupCommand-->>#58;LoadJobsCommand: Respond with success
            #58;LoadJobsCommand->>I18N: Get message of success in the right language
            #58;I18N-->>#58;LoadJobsCommand: Return message
            #58;LoadJobsCommand-->>User: Send message of success
        else user cancel changes
            User->>#58;LoadJobsCommand: Cancel changes
        end
    else error
        #58;BackupCommand-->>#58;LoadJobsCommand: Send a message of error
        deactivate #58;BackupCommand
        #58;LoadJobsCommand->>I18N: Get message of error in the right language
        #58;I18N-->>-#58;LoadJobsCommand: Return message
        #58;LoadJobsCommand-->>User: Send message of error
        end
        #58;LoadJobsCommand-->>-rootCommand: Return command
        rootCommand-->>-#58;Program: Invoke run command
        #58;Program-->>-User: Display command messages in console
        deactivate command
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
