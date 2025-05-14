# Software engineering project

[![Build and test (linux-x64)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-linux-x64.yml/badge.svg)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-linux-x64.yml)
[![Build and test (win-x64)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-win-x64.yml/badge.svg)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-win-x64.yml)

| Project Members |
|-----------------|
| Laura GIESE     |
| Kellian BECHTEL |
| Evan CAUMARTIN  |

## CLI usage guide

```
Usage:
  BackupUtil.Cli [command] [options]

Options:
  -l, --locale <locale>  Locale of the application, example: 'fr-FR', 'en-GB', defaults to OS locale
  --version              Show version information
  -?, -h, --help         Show help and usage information

Commands:
  create <source-path> <target-path>  Create a backup job
  load <job-file-path>                Load backup jobs from a file and execute them []
  remove                              Remove a backup job
  run <source-path> <target-path>     Run a backup job
```

To use this CLI application, you need to open the CLI at the path "{location of your application}\FISE_A3_SE_BECHTEL\BackupUtil.Cli\bin\Debug\net9.0". 
Then you will be able to run the commands above with the chosen options.
You need to create a backup job with a source and a target path before loading it.
You can also directly run a backup job by specifying the source and target paths.
Once you have loaded available jobs, you can run them by selecting the job index, then confirm your choice.
The backup job will then be executed and the files will be copied from the source to the target path, and all changes will be displayed in the CLI.
The log file, generated in the local application data folder, will contain the details of the changes ("C:\Users\Utilisateur\AppData\Local\EasySave\Logs" on Windows).

You can also chose your language by using the -l option.


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
    participant Filesystem

    activate User
    User->>+CLI: Enters command: run <source-path> <target> ...

        CLI->>+jobManager: Build backup command
        jobManager-->>-CLI: Return backup command
        CLI-->>User: Ask for confirmation of changes
        alt user confirms changes
            User->>CLI: Confirm changes
            CLI->>+#58;BackupCommand: Execute backup command
            #58;BackupCommand->>+Filesystem: Execute file changes
            Filesystem-->>-#58;BackupCommand: Respond with success
            #58;BackupCommand-->>CLI: Confirmation
            CLI-->>User: Confirmation
        else user cancels changes
            User->>CLI: Cancel changes
            CLI-->>User: Confirmation
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
    participant Filesystem

    activate User
    User->>+CLI: Enters command: load <path> ...

        CLI->>+jobManager: Load jobs from JSON file
        jobManager->>+ Filesystem: Read JSON file
        Filesystem-->>-jobManager: Return JSON file contents
        jobManager-->>CLI: Confirmation
        CLI->>jobManager: Get jobs list
        jobManager-->>CLI: Return jobs list
        CLI-->>User: Display job list and ask for which jobs to run
        User->>CLI: Select jobs to run
        CLI->>jobManager: Build backup command from selected jobs
        jobManager-->>-CLI: Return backup command
        CLI-->>User: Ask for confirmation of changes
        alt user confirms changes
            User->>CLI: Confirm changes
            CLI->>+#58;BackupCommand: Execute backup command
            #58;BackupCommand->>+Filesystem: Execute file changes
            Filesystem-->>-#58;BackupCommand: Respond with success
            #58;BackupCommand-->>CLI: Confirmation
            CLI-->>User: Confirmation
        else user cancels changes
            User->>CLI: Cancel changes
            CLI-->>User: Confirmation

end

    deactivate User
```

#### Create and save a Job to a JSON file

```mermaid
sequenceDiagram
    actor User
    participant CLI
    participant jobManager
    participant Filesystem

    activate User
    User->>+CLI: Enters command: create -o <path> ...
        CLI->>+jobManager: Load jobs from JSON file
        jobManager->>+ Filesystem: Read JSON file
        Filesystem-->>-jobManager: Return JSON file contents
        jobManager-->>CLI: Confirmation
        CLI->>jobManager: Add new job from user
        jobManager-->>CLI: Confirmation
        CLI->>+jobManager: Save all jobs to JSON file
        jobManager->>+ Filesystem: Write JSON to file
        Filesystem-->>-jobManager: Confirmation
        jobManager-->>CLI: Confirmation
        CLI-->>User: Confirmation


    deactivate User
```

#### Select language

```mermaid
    sequenceDiagram
        actor User
        participant CLI
        participant I18n

        activate User
        activate I18n
        User->>+CLI: Enters command with a language option
        CLI->>I18n: Set selected language
        I18n-->>CLI: Confirmation
        loop For each message
            CLI->>I18n: Get message with selected language
            I18n-->>CLI: Return message
            CLI-->>-User: Display message in selected language
        end
        deactivate I18n
        deactivate User
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
    <<interface>>
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
    <<interface>>
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

class JobFileExporter {
    +void ExportJobsToFile(List[Job], string?)
}

class JsonSerializer {
    string Serialize(List[Job] jobs)
}

JobManager *-- Job
FileChange --|> FileSystemChange
DirectoryChange --|> FileSystemChange
BackupTransaction *-- FileChange
BackupTransaction *-- DirectoryChange
BackupTransactionExecutor --|> IBackupTransactionExecutor

BackupTransactionBuilder --|> IBackupTransactionBuilder
BackupTransactionBuilder ..> BackupTransaction : instantiate

JobManager *--IBackupTransactionBuilder
JobManager *--IBackupTransactionExecutor
BackupCommand *-- BackupTransaction
BackupCommand *-- IBackupTransactionExecutor

JobManager ..> BackupCommand : instantiate 

JobManager ..> JobFileLoader

JobFileLoader ..> Job : instantiate
JobFileLoader ..> JsonDeserializer

JobManager ..JobFileExporter

JobFileExporter ..> JsonSerializer
```

### Activity Diagram

![use_case.svg](assets/activity.svg)
