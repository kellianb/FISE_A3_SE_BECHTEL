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

#### Core

```mermaid
classDiagram
    class JobManager {
        -IBackupTransactionExecutor _transactionExecutor
        -IBackupTransactionBuilder _transactionBuilder
        +uint MaxJobs
        +List~Job~ Jobs
        +string DefaultJobFilePath$
        +JobManager()
        +JobManager(IBackupTransactionExecutor, IBackupTransactionBuilder, uint?)
        +AddJob(Job job) JobManager
        +AddJobs(List~Job~ jobs) JobManager
        +AddJobsFromFile(string? filePath) JobManager
        +RemoveJobByIndex(int jobIndex) JobManager
        +RemoveByIndices(HashSet~int~ jobIndices) JobManager
        +RemoveAll() JobManager
        +RunByIndices(HashSet~int~ jobIndices) BackupCommand
        +RunAll() BackupCommand
        +ExportAll(string? filePath) JobManager
        +ExportByIndices(HashSet~int~ jobIndices, string? filePath) JobManager
        -BuildBackupCommand(List~Job~ concernedJobs) BackupCommand
        -BuildBackupCommand(Job job) BackupCommand
    }

    class Job {
        +string Name
        +string SourcePath
        +string TargetPath
        +bool Recursive
        +bool Differential
        +EncryptionType? EncryptionType
        +string? EncryptionKey
        +string? FileMask
        +Job(string sourcePath, string targetPath, bool recursive, bool differential, string? name, EncryptionType? encryptionType, string? encryptionKey, string? fileMask)
        +Job()
    }

    class BackupCommand {
        -IBackupTransactionExecutor _executor
        -Progress~BackupProgress~ _progress
        -IProgress~BackupProgress~ _progressReport
        -BackupTransaction _transaction
        +List~string~ JobNames
        -CancellationTokenSource _cancellationTokenSource
        -SemaphoreSlim _semaphore
        -ProgramFilter? _programFilter
        -bool _disposed
        +BackupCommandState State
        +long TotalFileSize
        +long RemainingFileSize
        +long TotalFileCount
        +long RemainingFileCount
        +long TotalDirectoryCount
        +long RemainingDirectoryCount
        +event EventHandler~BackupProgress~ ProgressChanged
        +Start() void
        +Pause() void
        +Execute() void
        +SetProgramFilter(ProgramFilter? programFilter) BackupCommand
        +GetConcernedFiles() Dictionary~FileChangeType, string[]~
        +GetConcernedDirectories() Dictionary~DirectoryChangeType, string[]~
        -ExecuteAsync() Task
        -ShouldCancel() void
        -UpdateProgress(string currentItem, CurrentOperationType? operationType) void
        +Dispose() void
    }

    class ProgramFilter {
        +List~string~ BannedPrograms
        +HashSet~string~ RunningProcesses$
        +ThrowIfBannedProgramDetected() void
    }

    class BannedProgramRunningException {
        +string BannedProgramName
        +BannedProgramRunningException(string message, string bannedProgramName)
    }

    class BackupTransaction {
        +List~FileChange~ FileChanges
        +List~DirectoryChange~ DirectoryChanges
        +AddDirectoryChange(DirectoryChange change) BackupTransaction
        +AddFileChange(FileChange change) BackupTransaction
        +GetConcernedFolders(DirectoryChangeType changeType) string[]
        +GetTotalCopiedFileSize() long
        +GetConcernedFiles(FileChangeType changeType) string[]
    }

    class IBackupTransactionBuilder {
        <<interface>>
        +Build(Job job) BackupTransaction
        +Build(List~Job~ jobs) BackupTransaction
    }

    class BackupTransactionBuilder {
        +Build(Job job) BackupTransaction
        +Build(List~Job~ jobs) BackupTransaction
        -AddJobToTransaction(Job job, BackupTransactionEditor editor, FileCompare fileCompare) BackupTransactionEditor
    }

    class BackupTransactionEditor {
        -BackupTransaction _transaction
        -FileMask _fileMask
        -IEncryptor? _encryptor
        +New()$ BackupTransactionEditor
        +WithMaskAndEncryptor(FileMask fileMask, IEncryptor encryptor)$ BackupTransactionEditor
        +FromTransaction(BackupTransaction transaction, FileMask fileMask, IEncryptor encryptor)$ BackupTransactionEditor
        +SetMask(FileMask mask) BackupTransactionEditor
        +SetEncryptor(IEncryptor encryptor) BackupTransactionEditor
        +Get() BackupTransaction
        +AddDirectoryCreation(string path) BackupTransactionEditor
        +AddDirectoryDeletion(DirectoryInfo targetDirectory) BackupTransactionEditor
        +AddFileCreation(FileInfo sourceFile, string targetFilePath) BackupTransactionEditor
        +AddFileUpdate(FileInfo sourceFile, FileInfo targetFile) BackupTransactionEditor
        +AddFileDeletion(FileInfo targetFile) BackupTransactionEditor
    }

    class IBackupTransactionExecutor {
        <<interface>>
        +delegate CancelCallback()
        +delegate ProgressCallback(string, CurrentOperationType?)
        +Execute(BackupTransaction transaction) void
        +ExecuteAsync(BackupTransaction transaction, CancelCallback shouldCancel, ProgressCallback updateProgress, CancellationToken cancellationToken) Task
    }

    class BackupTransactionExecutor {
        +Execute(BackupTransaction transaction) void
        +ExecuteAsync(BackupTransaction transaction, CancelCallback shouldCancel, ProgressCallback updateProgress, CancellationToken cancellationToken) Task
        -ExecuteDirectoryChange(DirectoryChange change) void
        -ExecuteFileChangeAsyncWithRetry(FileChange change, CancellationToken cancellationToken) Task
        -ExecuteFileChangeAsync(FileChange change, CancellationToken cancellationToken) Task
    }

    class FileSystemChange {
        <<abstract>>
        +string TargetPath
        +Equals(FileSystemChange other) bool
        +GetHashCode() int
    }

    class FileChange {
        +FileChangeType ChangeType
        +long FileSize
        +string? SourcePath
        +IEncryptor? Encryptor
        +Creation(string sourcePath, string targetPath, long fileSize, IEncryptor encryptor)$ FileChange
        +Modification(string sourcePath, string targetPath, long fileSize, IEncryptor encryptor)$ FileChange
        +Deletion(string targetPath)$ FileChange
        +Equals(FileChange other) bool
        +GetHashCode() int
    }

    class DirectoryChange {
        +DirectoryChangeType ChangeType
        +Creation(string targetPath)$ DirectoryChange
        +Deletion(string targetPath)$ DirectoryChange
        +Equals(DirectoryChange other) bool
        +GetHashCode() int
    }

    class ICompare {
        <<interface>>
        +Compare(BackupTransactionEditor transaction) BackupTransactionEditor
    }

    class DirectoryCompare {
        -FileCompare _compare
        -bool _differential
        -bool _recursive
        -DirectoryInfo _sourceDirectory
        -string _targetDirectoryPath
        +Compare(BackupTransactionEditor transaction) BackupTransactionEditor
        -Full(BackupTransactionEditor transaction, DirectoryInfo sourceDirectory, string targetDirectoryPath, bool recursive) BackupTransactionEditor
        -Differential(BackupTransactionEditor transaction, DirectoryInfo sourceDirectory, string targetDirectoryPath, bool recursive) BackupTransactionEditor
        -DiffDirectoryFiles(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, BackupTransactionEditor transaction) BackupTransactionEditor
        -FullDirectoryFiles(DirectoryInfo sourceDirectory, string targetDirectoryPath, BackupTransactionEditor transaction) BackupTransactionEditor
    }

    class SingleFileCompare {
        -FileInfo sourceFile
        -string targetFilePath
        -bool differential
        -FileCompare compare
        +Compare(BackupTransactionEditor transaction) BackupTransactionEditor
        -Differential(BackupTransactionEditor transaction) BackupTransactionEditor
        -Full(BackupTransactionEditor transaction) BackupTransactionEditor
    }

    class FileCompare {
        -IEncryptor? encryptor
        +FileCompare(IEncryptor? encryptor)
        +AreFilesEqual(FileInfo file1, FileInfo file2) bool
        -CalculateFileHash(Stream stream)$ string
    }

    class FileMask {
        +Dictionary~FileMaskEffect, List~FileMaskingStrategy~~ Masks
        +FileMask()
        +FileMask(Dictionary~FileMaskEffect, List~FileMaskingStrategy~~ masks)
        -AddMask(FileMaskEffect effect, FileMaskingStrategy strategy) void
        +ShouldCopy(FileInfo file) bool
        +ShouldEncrypt(FileInfo file) bool
        -ApplyMask(FileMaskEffect effect, FileInfo file) bool
    }

    class FileMaskBuilder {
        -FileMask _fileMask
        -FileMaskBuilder(FileMask? fileMask)
        +New()$ FileMaskBuilder
        +Empty()$ FileMask
        +From(FileMask fileMask)$ FileMaskBuilder
        +FromString(string serialized)$ FileMaskBuilder
        +ValidateSerialized(string serialized)$ bool
        +Build() FileMask
        +BuildSerialized() string
        +MaxFileSize(long maxSize, FileMaskEffect effect) FileMaskBuilder
        +MinFileSize(long minSize, FileMaskEffect effect) FileMaskBuilder
        +AllowedExtensions(List~string~ extensions, FileMaskEffect effect) FileMaskBuilder
        +BannedExtensions(List~string~ extensions, FileMaskEffect effect) FileMaskBuilder
    }

    class FileMaskingStrategy {
        +IsOk(FileInfo file) bool
    }

    class MaxFileSizeStrategy {
        +long MaxSize
        +MaxFileSizeStrategy(long maxSize)
        +IsOk(FileInfo file) bool
    }

    class MinFileSizeStrategy {
        +long MinSize
        +MinFileSizeStrategy(long minSize)
        +IsOk(FileInfo file) bool
    }

    class AllowedFileExtensionsStrategy {
        +List~string~ AllowedExtensions
        +AllowedFileExtensionsStrategy(List~string~ extensions)
        +IsOk(FileInfo file) bool
    }

    class BannedFileExtensionsStrategy {
        +List~string~ BannedExtensions
        +BannedFileExtensionsStrategy(List~string~ extensions)
        +IsOk(FileInfo file) bool
    }

    class JobFileExporter {
        <<static>>
        +ExportJobsToFile(List~Job~ jobs, string? filePath) void
    }

    class JobFileLoader {
        <<static>>
        +LoadJobsFromFile(string? filePath) List~Job~
    }

    class JsonJobSerializer {
        <<static>>
        +Serialize(List~Job~ jobs) string
    }

    class JsonJobDeserializer {
        <<static>>
        +Deserialize(TextReader reader) List~Job~
    }

    class JsonBackupUtilSerializerContext {
        <<partial>>
    }

    class SelectionStringParser {
        <<static>>
        -string ExpressionSeparator$
        -string RangeSeparator$
        +Parse(string expression) HashSet~int~
        -ParsePart(string expressionPart)$ List~int~
    }

    class Config {
        <<static>>
        +string AppName$
        +uint DefaultMaxJobCount$
        +string AppDir$
        +string DefaultJobFilePath$
        +string SettingsFilePath$
        +string LoggingDirectory$
    }

    class Logging {
        <<static>>
        +Init() void
        -GetLogger()$ ILogger
    }

%% Enums and Structs
    class BackupCommandState {
        <<enumeration>>
        NotStarted
        Running
        Paused
        PausedBannedProgram
        Finished
    }

    class FileChangeType {
        <<enumeration>>
        Create
        Modify
        Delete
    }

    class DirectoryChangeType {
        <<enumeration>>
        Create
        Delete
    }

    class CurrentOperationType {
        <<enumeration>>
        CreatingDirectories
        CopyingPriorityFiles
        CopyingFiles
    }

    class FileMaskEffect {
        <<enumeration>>
        Copy
        Encrypt
        Prioritise
    }

    class BackupProgress {
        <<struct>>
        +BackupCommandState State
        +CurrentOperationType? Type
        +long TotalFileSize
        +long CompletedFileSize
        +long TotalDirectoryCount
        +long CompletedDirectoryCount
        +long TotalFileCount
        +long CompletedFileCount
        +long CompletedPercentage
        +string CurrentItem
    }

    JobManager ..> Job
    JobManager *-- BackupCommand
    JobManager -- IBackupTransactionExecutor
    JobManager -- IBackupTransactionBuilder
    JobManager -- JobFileExporter
    JobManager -- JobFileLoader

    BackupCommand -- BackupTransaction
    BackupCommand -- IBackupTransactionExecutor
    BackupCommand -- ProgramFilter
    BackupCommand -- BackupProgress
    BackupCommand -- BackupCommandState

    ProgramFilter -- BannedProgramRunningException

    BackupTransactionBuilder ..|> IBackupTransactionBuilder
    BackupTransactionBuilder ..> BackupTransaction
    BackupTransactionBuilder -- BackupTransactionEditor
    BackupTransactionBuilder -- DirectoryCompare
    BackupTransactionBuilder -- SingleFileCompare
    BackupTransactionBuilder -- FileCompare

    BackupTransactionExecutor ..|> IBackupTransactionExecutor
    BackupTransactionExecutor ..> BackupTransaction
    BackupTransactionExecutor -- FileChange
    BackupTransactionExecutor -- DirectoryChange

    BackupTransaction *-- FileChange
    BackupTransaction *-- DirectoryChange

    BackupTransactionEditor ..> BackupTransaction
    BackupTransactionEditor -- FileMask
    BackupTransactionEditor ..> FileChange
    BackupTransactionEditor ..> DirectoryChange

    FileChange --|> FileSystemChange
    DirectoryChange --|> FileSystemChange
    FileChange ..> FileChangeType
    DirectoryChange ..> DirectoryChangeType

    DirectoryCompare ..|> ICompare
    SingleFileCompare ..|> ICompare
    DirectoryCompare -- FileCompare
    SingleFileCompare -- FileCompare

    FileMaskBuilder ..> FileMask
    FileMask -- FileMaskEffect
    FileMask *-- FileMaskingStrategy
    MaxFileSizeStrategy --|> FileMaskingStrategy
    MinFileSizeStrategy --|> FileMaskingStrategy
    AllowedFileExtensionsStrategy --|> FileMaskingStrategy
    BannedFileExtensionsStrategy --|> FileMaskingStrategy

    JobFileExporter -- JsonJobSerializer
    JobFileLoader -- JsonJobDeserializer
    JsonJobSerializer -- JsonBackupUtilSerializerContext
    JsonJobDeserializer -- JsonBackupUtilSerializerContext

    BackupProgress *-- BackupCommandState
    BackupProgress *-- CurrentOperationType

    SelectionStringParser -- JobManager
    
```

#### ViewModels

```mermaid
classDiagram
    class ICommand {
        <<interface>>
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        +CanExecuteChanged EventHandler
    }

    class INotifyPropertyChanged {
        <<interface>>
        +PropertyChanged PropertyChangedEventHandler
    }

    class INotifyDataErrorInfo {
        <<interface>>
        +GetErrors(string? propertyName) IEnumerable
        +HasErrors bool
        +ErrorsChanged EventHandler~DataErrorsChangedEventArgs~
    }

    class CommandBase {
        <<abstract>>
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void*
        +CanExecuteChanged EventHandler
        #OnCanExecuteChanged() void
    }

    class AsyncCommandBase {
        <<abstract>>
        -_isExecuting bool
        -IsExecuting bool
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        +ExecuteAsync(object? parameter) Task*
    }

    class ViewModelBase {
        <<abstract>>
        +PropertyChanged PropertyChangedEventHandler
        #OnPropertyChanged([CallerMemberName] string? propertyName) void
        +Dispose() void*
    }

    class NavigateCommand~TViewModel~ {
        -navigationService NavigationService~TViewModel~
        +Execute(object? parameter) void
    }

    class CreateJobCommand~TViewModel~ {
        -_jobCreationViewModel JobCreationViewModel
        -_jobStore JobStore
        -_navigationService NavigationService~TViewModel~
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class CreateTransactionsForSelectedJobsCommand {
        -_jobListingViewModel JobListingViewModel
        -_jobStore JobStore
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class DeleteSelectedJobsCommand {
        -_jobListingViewModel JobListingViewModel
        -_jobStore JobStore
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class ExportJobsCommand {
        -_jobListingViewModel JobListingViewModel
        -_jobStore JobStore
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class LoadJobsCommand {
        -_jobListingViewModel JobListingViewModel
        -_jobStore JobStore
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class ApplySettingsCommand~TViewModel~ {
        -_settingsViewModel SettingsViewModel
        -_localizationService LocalizationService
        -_programFilterStore ProgramFilterStore
        -_navigationService NavigationService~TViewModel~
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class RunTransactionCommand {
        -_transactionViewModel TransactionViewModel
        -_run Action
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class PauseTransactionCommand {
        -_transactionViewModel TransactionViewModel
        -_pause Action
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class DeleteSelectedTransactionsCommand {
        -_transactionListingViewModel TransactionListingViewModel
        -_backupCommandStore BackupCommandStore
        +CanExecute(object? parameter) bool
        +Execute(object? parameter) void
        -OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class NavigationStore {
        -_currentViewModel ViewModelBase?
        +CurrentViewModel ViewModelBase?
        +CurrentViewModelChanged Action?
        -OnCurrentViewModelChanged() void
    }

    class JobStore {
        -_backupCommandStore BackupCommandStore
        -_jobManager JobManager
        -_jobFilePath string
        -_propertyNameToErrorsDictionary Dictionary~string, List~string~~
        +Jobs List~Job~
        +JobFilePath string
        +CanAccessJobFile bool
        +PropertyChanged PropertyChangedEventHandler
        +ErrorsChanged EventHandler~DataErrorsChangedEventArgs~
        +AddJob(Job job) void
        +RemoveByIndices(HashSet~int~ indices) void
        +RemoveAll() void
        +RunByIndices(HashSet~int~ indices) void
        +RunAll() void
        +LoadJobs() void
        +ExportAll() void
        +GetErrors(string? propertyName) IEnumerable
        -AddError(string errorMessage, string propertyName) void
        -ClearErrors(string propertyName) void
        #OnPropertyChanged([CallerMemberName] string? propertyName) void
        -OnErrorsChanged(string propertyName) void
    }

    class BackupCommandStore {
        -_programFilterStore ProgramFilterStore
        +BackupCommands List~BackupCommand~
        +PropertyChanged PropertyChangedEventHandler
        +AddBackupCommand(BackupCommand backupCommand) void
        +RunByIndex(int index) void
        +PauseByIndex(int index) void
        +RemoveByIndices(List~int~ indices) void
        #OnPropertyChanged([CallerMemberName] string? propertyName) void
    }

    class ProgramFilterStore {
        +ProgramFilter ProgramFilter
        +BannedPrograms List~string~
        +ProgramFilterChanged Action?
        -OnProgramFilterChanged() void
    }

    class NavigationService~TViewModel~ {
        -navigationStore NavigationStore
        -viewModelSource Func~TViewModel~
        +Navigate() void
    }

    class LocalizationService {
        <<singleton>>
        -s_instance Lazy~LocalizationService~
        +Instance LocalizationService$
        +this[string key] string
        +PropertyChanged PropertyChangedEventHandler
        +SetCulture(CultureInfo culture) void
        +GetCurrentCulture() CultureInfo
        +GetSupportedCultures() List~CultureInfo~
    }

    class MainViewModel {
        -_navigationStore NavigationStore
        +CurrentViewModel ViewModelBase?
        +Dispose() void
        -OnCurrentViewModelChanged() void
    }

    class HomeViewModel {
        +JobListingViewModel JobListingViewModel
        +TransactionListingViewModel TransactionListingViewModel
        +OpenJobCreationCommand ICommand
        +OpenSettingsCommand ICommand
    }

    class JobCreationViewModel {
        -_propertyNameToErrorsDictionary Dictionary~string, List~string~~
        -_name string
        -_sourcePath string
        -_targetPath string
        -_recursive bool
        -_differential bool
        -_encryptionType EncryptionTypeOptions
        -_encryptionKey string
        -_fileMask string
        +Name string
        +SourcePath string
        +TargetPath string
        +Recursive bool
        +Differential bool
        +EncryptionType EncryptionTypeOptions
        +EncryptionKey string
        +FileMask string
        +CanCreateJob bool
        +SubmitCommand ICommand
        +CancelCommand ICommand
        +HasErrors bool
        +ErrorsChanged EventHandler~DataErrorsChangedEventArgs~
        +GetErrors(string? propertyName) IEnumerable
        -AddError(string errorMessage, string propertyName) void
        -ClearErrors(string propertyName) void
        -OnErrorsChanged(string propertyName) void
    }

    class JobListingViewModel {
        -_jobStore JobStore
        -_propertyNameToErrorsDictionary Dictionary~string, List~string~~
        +Jobs ObservableCollection~JobViewModel~
        +SelectedJobIndices HashSet~int~
        +JobFilePath string
        +CanAccessJobFile bool
        +LoadJobsCommand ICommand
        +ExportJobsCommand ICommand
        +CreateTransactionsForSelectedJobsCommand ICommand
        +DeleteSelectedJobsCommand ICommand
        +HasErrors bool
        +ErrorsChanged EventHandler~DataErrorsChangedEventArgs~
        +LoadJobViewModels() void
        +GetErrors(string? propertyName) IEnumerable
        -DisposeJobViewModels() void
        -OnJobStorePropertyChanged(object sender, PropertyChangedEventArgs e) void
        -OnJobViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
        -AddError(string errorMessage, string propertyName) void
        -ClearErrors(string propertyName) void
        -OnErrorsChanged(string propertyName) void
    }

    class JobViewModel {
        -job Job
        -_isSelected bool
        +Name string
        +SourcePath string
        +TargetPath string
        +Recursive bool
        +Differential bool
        +EncryptionType EncryptionTypeOptions
        +EncryptionKey string
        +FileMask string
        +IsSelected bool
    }

    class TransactionListingViewModel {
        -_backupCommandStore BackupCommandStore
        +Transactions ObservableCollection~TransactionViewModel~
        +SelectedTransactionIndices List~int~
        +DeleteSelectedTransactionsCommand ICommand
        +LoadTransactionViewModels() void
        +Dispose() void
        -DisposeTransactionViewModels() void
        -OnBackupCommandStorePropertyChanged(object sender, PropertyChangedEventArgs e) void
        -OnTransactionViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) void
    }

    class TransactionViewModel {
        -_command BackupCommand
        -_isSelected bool
        -_jobNames string
        -_totalFileSize long
        -_completedFileSize long
        -_totalDirectoryCount long
        -_completedDirectoryCount long
        -_totalFileCount long
        -_completedFileCount long
        -_completedPercentage long
        -_currentItem string
        -_state BackupCommandState
        -_currentOperationType CurrentOperationType?
        +IsSelected bool
        +JobNames string
        +TotalFileSize long
        +CompletedFileSize long
        +FileSizeProgress string
        +TotalDirectoryCount long
        +CompletedDirectoryCount long
        +DirectoryCountProgress string
        +TotalFileCount long
        +CompletedFileCount long
        +FileCountProgress string
        +CompletedPercentage long
        +TotalPercentage long
        +CurrentItem string
        +State BackupCommandState
        +CurrentOperationType CurrentOperationType?
        +RunTransactionCommand ICommand
        +PauseTransactionCommand ICommand
        +Dispose() void
        -OnBackupCommandProgressChanged(object sender, BackupProgress e) void
    }

    class SettingsViewModel {
        -_localizationService LocalizationService
        -_programFilterStore ProgramFilterStore
        -_propertyNameToErrorsDictionary Dictionary~string, List~string~~
        -_selectedLanguage CultureInfo
        -_bannedPrograms string
        +AvailableLanguages List~CultureInfo~
        +SelectedLanguage CultureInfo
        +BannedPrograms string
        +CanSaveSettings bool
        +ApplyCommand ICommand
        +CancelCommand ICommand
        +HasErrors bool
        +ErrorsChanged EventHandler~DataErrorsChangedEventArgs~
        +GetErrors(string? propertyName) IEnumerable
        -AddError(string errorMessage, string propertyName) void
        -ClearErrors(string propertyName) void
        -OnErrorsChanged(string propertyName) void
    }

    class EncryptionTypeOptions {
        <<enumeration>>
        Xor
        None
    }

    class EncryptionTypeOptionsUtils {
        <<static>>
        +To(EncryptionTypeOptions type) EncryptionType?$
        +From(EncryptionType? type) EncryptionTypeOptions$
    }

    ICommand <|-- CommandBase
    CommandBase <|-- AsyncCommandBase
    CommandBase <|-- NavigateCommand
    CommandBase <|-- CreateJobCommand
    CommandBase <|-- CreateTransactionsForSelectedJobsCommand
    CommandBase <|-- DeleteSelectedJobsCommand
    CommandBase <|-- ExportJobsCommand
    CommandBase <|-- LoadJobsCommand
    CommandBase <|-- ApplySettingsCommand
    CommandBase <|-- RunTransactionCommand
    CommandBase <|-- PauseTransactionCommand
    CommandBase <|-- DeleteSelectedTransactionsCommand

    ViewModelBase <|-- MainViewModel
    ViewModelBase <|-- HomeViewModel
    ViewModelBase <|-- JobCreationViewModel
    ViewModelBase <|-- JobListingViewModel
    ViewModelBase <|-- JobViewModel
    ViewModelBase <|-- TransactionListingViewModel
    ViewModelBase <|-- TransactionViewModel
    ViewModelBase <|-- SettingsViewModel

    INotifyPropertyChanged <|-- ViewModelBase
    INotifyPropertyChanged <|-- LocalizationService
    INotifyPropertyChanged <|-- BackupCommandStore
    INotifyPropertyChanged <|-- JobStore

    INotifyDataErrorInfo <|-- JobCreationViewModel
    INotifyDataErrorInfo <|-- JobListingViewModel
    INotifyDataErrorInfo <|-- SettingsViewModel
    INotifyDataErrorInfo <|-- JobStore

    MainViewModel o-- NavigationStore

    HomeViewModel *-- JobListingViewModel
    HomeViewModel *-- TransactionListingViewModel
    HomeViewModel o-- NavigateCommand

    NavigationService o-- NavigationStore
    NavigationService *-- ViewModelBase

    JobStore o-- BackupCommandStore

    BackupCommandStore o-- ProgramFilterStore

    NavigateCommand ..> NavigationService

    CreateJobCommand ..> JobCreationViewModel
    CreateJobCommand ..> JobStore
    CreateJobCommand -- NavigationService

    CreateTransactionsForSelectedJobsCommand ..> JobListingViewModel
    CreateTransactionsForSelectedJobsCommand ..> JobStore

    DeleteSelectedJobsCommand ..> JobListingViewModel
    DeleteSelectedJobsCommand ..> JobStore

    ExportJobsCommand ..> JobListingViewModel
    ExportJobsCommand ..> JobStore

    LoadJobsCommand ..> JobListingViewModel
    LoadJobsCommand ..> JobStore

    ApplySettingsCommand ..> SettingsViewModel
    ApplySettingsCommand -- LocalizationService
    ApplySettingsCommand -- ProgramFilterStore
    ApplySettingsCommand -- NavigationService

    RunTransactionCommand ..> TransactionViewModel
    PauseTransactionCommand ..> TransactionViewModel

    DeleteSelectedTransactionsCommand ..> TransactionListingViewModel
    DeleteSelectedTransactionsCommand ..> BackupCommandStore

    JobCreationViewModel *-- CreateJobCommand
    JobCreationViewModel *-- NavigateCommand

    JobListingViewModel ..> JobStore
    JobListingViewModel o-- JobViewModel
    JobListingViewModel *-- LoadJobsCommand
    JobListingViewModel *-- ExportJobsCommand
    JobListingViewModel *-- CreateTransactionsForSelectedJobsCommand
    JobListingViewModel *-- DeleteSelectedJobsCommand

    TransactionViewModel *-- RunTransactionCommand
    TransactionViewModel *-- PauseTransactionCommand

    TransactionListingViewModel ..> BackupCommandStore
    TransactionListingViewModel o-- TransactionViewModel
    TransactionListingViewModel *-- DeleteSelectedTransactionsCommand

    SettingsViewModel -- LocalizationService
    SettingsViewModel -- ProgramFilterStore
    SettingsViewModel *-- ApplySettingsCommand
    SettingsViewModel *-- NavigateCommand

    EncryptionTypeOptionsUtils ..> EncryptionTypeOptions
    JobCreationViewModel -- EncryptionTypeOptions
    JobViewModel -- EncryptionTypeOptions
```

#### CLI
```mermaid
classDiagram

        class DisplayChanges {
            <<static>>
            +DisplayFileChanges(Dictionary~FileChangeType, string[]~) string
            +DisplayDirectoryChanges(Dictionary~DirectoryChangeType, string[]~) string
        }

        class DisplayJobs {
            <<static>>
            -DisplayOne(Job, int) string
            +Display(List~Job~) string
        }

        class Formatting {
            <<static>>
            +string Indent
            +string BoldOn
            +string Reset
        }

        class CreateJobCommand {
            <<static>>
            +Build() Command
            -CommandHandler(FileSystemInfo, FileSystemInfo, bool, bool, string?, FileSystemInfo?)
        }

        class LoadJobsCommand {
            <<static>>
            +Build() Command
            -CommandHandler(FileSystemInfo?)
        }

        class RemoveJobCommand {
            +Build() Command
            -CommandHandler(FileSystemInfo)
        }

        class RunJobCommand {
            <<static>>
            +Build() Command
            -CommandHandler(FileSystemInfo, FileSystemInfo, bool, bool)
        }

        class Program {
            -Main(string[]) int
            -SetLocale(OptionResult, Option~string~)
        }
    
    LoadJobsCommand ..> DisplayJobs
    LoadJobsCommand ..> DisplayChanges
    RemoveJobCommand ..> DisplayJobs
    RunJobCommand ..> DisplayChanges

    DisplayJobs ..> Formatting
    DisplayChanges ..> Formatting

    Program *-- CreateJobCommand
    Program *-- LoadJobsCommand
    Program *-- RemoveJobCommand
    Program *-- RunJobCommand
```

#### Crypto

```mermaid
classDiagram
        class IEncryptor {
            <<interface>>
            +Encrypt(string) string
        }

        class XorEncryptor {
            <<internal>>
            -string _key
            +XorEncryptor(string)
            +Encrypt(string) string
        }

        class EncryptorBuilder {
            <<static>>
            +New(EncryptionType, string) IEncryptor
        }

        class EncryptionType {
            <<enumeration>>
            Xor
        }

    XorEncryptor ..|> IEncryptor
    EncryptorBuilder *-- IEncryptor
    EncryptorBuilder *-- XorEncryptor
    EncryptorBuilder ..> EncryptionType
    XorEncryptor -- EncryptionType
```

#### UI

```mermaid
classDiagram

class App {
-IServiceProvider _serviceProvider
-Mutex _mutex
+OnStartup(StartupEventArgs)
+OnExit(ExitEventArgs)
-SingleInstance() bool
-CreateMainViewModel(IServiceProvider) MainViewModel
-CreateJobListingViewModel(IServiceProvider) JobListingViewModel
-CreateJobCreationViewModel(IServiceProvider) JobCreationViewModel
-CreateSettingsViewModel(IServiceProvider) SettingsViewModel
-CreateHomeViewModel(IServiceProvider) HomeViewModel
-CreateTransactionListingViewModel(IServiceProvider) TransactionListingViewModel
-CreateNavigationService~T~(IServiceProvider) NavigationService~T~
-CreateJobStore(IServiceProvider) JobStore
-CreateBackupCommandStore(IServiceProvider) BackupCommandStore
}

    class MainWindow {
        +MainWindow()
    }

    class HomeView {
        +HomeView()
    }

    class JobCreationView {
        +JobCreationView()
    }

    class JobListingView {
        +JobListingView()
        -Selector_OnSelectionChanged(object, SelectionChangedEventArgs)
    }

    class SettingsView {
        +SettingsView()
    }

    class TransactionListingView {
        +TransactionListingView()
        -Selector_OnSelectionChanged(object, SelectionChangedEventArgs)
    }

    App *-- MainWindow
    MainWindow *-- HomeView
    MainWindow *-- JobCreationView
    MainWindow *-- JobListingView
    MainWindow *-- SettingsView
    MainWindow *-- TransactionListingView
```

#### I18n

```mermaid
classDiagram
    class I18N {
        -List~CultureInfo~ s_supportedCultures$
        -ResourceManager s_resourceManager$
        +GetLocalizedMessage(string key)$ string
        +SetCulture(CultureInfo culture)$ void
        +GetCurrentCulture()$ CultureInfo
        +GetSupportedCultures()$ List~CultureInfo~
    }

    class I18NKeyAttribute {
        +string Key
        +I18NKeyAttribute(string key)
    }

    class I18nEnumExtensions {
        +GetI18nKey(Enum value)$ string
        +GetLocalizedMessage(Enum value)$ string
    }

    I18nEnumExtensions -- I18N
    I18NKeyAttribute <.. I18nEnumExtensions
    I18NKeyAttribute --|> Attribute 
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
