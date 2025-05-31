# Software engineering project

[![Build and test (linux-x64)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-linux-x64.yml/badge.svg)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-linux-x64.yml)
[![Build and test (win-x64)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-win-x64.yml/badge.svg)](https://github.com/kellianb/FISE_A3_SE_BECHTEL/actions/workflows/build-and-test-win-x64.yml)

| Project Members |
|-----------------|
| Laura GIESE     |
| Kellian BECHTEL |
| Evan CAUMARTIN  |


## Getting started

To get the CLI, you can either download it from the build artifacts of the "Build and test" GitHub action corresponding to your platform, or compile it yourself.

To compile the app, Clone this repository and run `dotnet build`.

To run backup jobs, you can either directly specify all parameters as arguments (`BackupUtil.Cli run ...`)
or load a list of backup jobs from a json file (`BackupUtil.Cli load ...`) and pick which to run.

You need to create a backup job before being able to load and run it.
Backup jobs can be created using `Backuputil.Cli create ...`.

If no filepath is specified, backup jobs are saved to, removed and loaded from:

- Linux: `~/.locale/share/EasySave/Jobs/BackupJobs.json`
- Windows: `~\AppData\Local\EasySave\Jobs\BackupJobs.json`

A daily log file containing details about all changes made by this utility can be found at:

- Linux: `~/.locale/share/EasySave/Logs`
- Windows: `~\AppData\Local\EasySave\Logs`

Currently, two languages are supported: French and English.
If your OS language is among them, the CLI will default to it.

You can also manually set your language by using the `-l ` option.

### CLI usage guide

```
Usage:
  BackupUtil.Cli [command] [options]

Options:
  -l, --locale <locale>  Locale of the application, example: 'fr-FR', 'en-GB', defaults to OS locale
  --version              Show version information
  -?, -h, --help         Show help and usage information

Commands:
  create <source-path> <target-path>  Create a backup job and write it to a file
  load                                Load backup jobs from a file and execute them
  remove                              Remove a backup job from a file
  run <source-path> <target-path>     Run a backup job
```

## UML Diagrams

### Use Case Diagram

![use_case.svg](assets/use_case.svg)

### Sequence Diagram

#### Run a job

```mermaid
sequenceDiagram
    actor User
    participant GUI
    participant RunTransactionCommand
    participant _backupCommandStore
    participant #58;BackupCommand
    participant Filesystem

    activate User
    User->>+GUI: Click on run
    GUI->>+RunTransactionCommand: Check form validity
    RunTransactionCommand->>_backupCommandStore: Run by index
    _backupCommandStore->>+#58;BackupCommand: Run job
    #58;BackupCommand->>+Filesystem: Execute file changes
    Filesystem-->>-#58;BackupCommand: Respond with success
    #58;BackupCommand-->>GUI: Confirmation
    GUI-->>User: Confirmation
    deactivate User
```

#### Create a new Job

```mermaid
sequenceDiagram
    actor User
    participant GUI
    participant SubmitCommand
    participant _jobCreationViewModel
    participant _jobStore
    participant _jobManager

    activate User
    User->>+GUI: Click on add new job and fill in form
        GUI->>+SubmitCommand: Check form validity
        SubmitCommand->>_jobCreationViewModel: Retrieve form data
        _jobCreationViewModel-->>SubmitCommand: Instantiate job with data
        SubmitCommand->>_jobStore: Add job
        _jobStore-->>_jobManager: Add job to jobs list
        _jobManager-->>_jobStore: Confirmation
        _jobStore-->>GUI: Trigger jobs list update
        GUI-->>-User: Display updated jobs list
    deactivate User
```

#### Select language

```mermaid
    sequenceDiagram
        actor User
        participant GUI
        participant ApplyCommand
        participant _localizationService
        participant I18n

        activate User
        activate I18n
        User->>+GUI: Select language
        GUI->>ApplyCommand: Check validity
        ApplyCommand->>_localizationService: Set culture
        _localizationService->>I18n: Set culture
        _localizationService-->>GUI: Trigger language change
        loop For each label
            GUI->>I18n: Get label with selected language
            I18n-->>GUI: Return label
            GUI-->>-User: Display label in selected language
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
+bool Recursive
+bool Differential
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
+List~FileChange~ FileChanges
+List~DirectoryChange~ DirectoryChanges
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
+List~Job~ Jobs
+JobManager LoadJobsFromFile(string)
+BackupCommand GetBackupCommandForIndexes(HashSet~int~)
+BackupCommand BuildBackupCommand(List~Job~)
+BackupCommand BuildBackupCommand(Job concernedJobs)
}

class IBackupTransactionExecutor {
<<interface>>
+Execute(BackupTransaction)
+Task ExecuteAsync(BackupTransaction, CancelCallback, ProgressCallback, CancellationToken)
}

class BackupTransactionExecutor {
+void Execute(BackupTransaction)
+Task ExecuteAsync(BackupTransaction, CancelCallback, ProgressCallback, CancellationToken)
}

class BackupCommand {
-IBackupTransactionExecutor Receiver
-BackupTransaction Transaction
+void Execute()
+void Start()
+void Pause()
+long GetTotalCopiedFileSize()
+Dictionary~FileChangeType, string[]~ GetConcernedFiles()
+Dictionary~DirectoryChangeType, string[]~ GetConcernedDirectories()
}

class BackupCommandStore {
+List~BackupCommand~ BackupCommands
+void AddBackupCommand(BackupCommand)
+void RunByIndex(int)
+void PauseByIndex(int)
+void RemoveByIndices(List~int~)
}

class ProgramFilterStore {
+ProgramFilter ProgramFilter
}

class ProgramFilter {
+void ThrowIfBannedProgramDetected()
}

class JobFileLoader {
+List~Job~ LoadJobsFromFile(string)
}

class JsonDeserializer {
List~Job~ Deserialize(TextReader reader)
}

class JobFileExporter {
+void ExportJobsToFile(List~Job~, string?)
}

class JsonSerializer {
string Serialize(List~Job~ jobs)
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

BackupCommandStore *-- BackupCommand
BackupCommandStore *-- ProgramFilterStore
ProgramFilterStore *-- ProgramFilter

JobManager ..> BackupCommand : instantiate

JobManager ..> JobFileLoader

JobFileLoader ..> Job : instantiate
JobFileLoader ..> JsonDeserializer

JobManager ..JobFileExporter

JobFileExporter ..> JsonSerializer
```


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

## Contributing
### How to make changes

- Clone the project: `git pull <repo url>`
- Create a new feature branch: `git checkout -b 'feature/<name-of-your-feature>'`
- Commit your changes: `git commit -m '<Tell us what you did and why here>'`
- Push you changes: `git push`
- Open a PR on github

### How to solve merge conflicts

Prefer git rebases over merges

Note: When done rebasing, you have to push using `git push --force`
