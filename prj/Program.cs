using System.CommandLine;
using static prj.ImplementationBundleCommand;
using static prj.ImplementationCreateRstCommand;

var extentionsOfFiles = new string[] { "cs", "cpp", "h", "c", "py", "php", "vb", "java", "js", "ts", "html", "sv", "sql", "json" };
var optionLanguage = new Option<string>("--language", "Languges to include in bundle. A language must be one of the values in the static list.\n" +
    "<|all|c|cpp|cs|h|html|java|js|json|php|py|sln|sql|sv|ts|vb>")
{
    IsRequired = true,
    AllowMultipleArgumentsPerToken = true,
};
optionLanguage.AddAlias("-l");

var optionOutput = new Option<FileInfo>("--output", "File path and name");
optionOutput.AddAlias("-o");
optionOutput.IsRequired = true;

var optionNote = new Option<bool>("--note", "Whether to list the code's source as a comment in the file");
optionNote.AddAlias("-n");

var optionSort = new Option<bool>("--sort-by-file-extension", "The order of copying the code files, default is by the letter of the file name or according to the type of code.");
optionSort.AddAlias("-s");

var optionRemoveEmptyLines = new Option<bool>("--remove-empty-lines", "Whether to delete empty lines from the source code.");
optionRemoveEmptyLines.AddAlias("-r");

var optionAuthor = new Option<string>("--author", "Registering the name of the creator of the file");
optionAuthor.AddAlias("-a");

var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(optionLanguage);
bundleCommand.AddOption(optionOutput);
bundleCommand.AddOption(optionNote);
bundleCommand.AddOption(optionSort);
bundleCommand.AddOption(optionRemoveEmptyLines);
bundleCommand.AddOption(optionAuthor);
bundleCommand.SetHandler((languages, output, note, sort, removeElines, author) =>
{
    DoBundleCommand(languages, extentionsOfFiles, output, note, sort, removeElines, author);
}, optionLanguage, optionOutput, optionNote, optionSort, optionRemoveEmptyLines, optionAuthor);

var createRspCommand = new Command("create-rsp", "Response command of bundle.");
createRspCommand.SetHandler(() =>
{
    DoCreateRspCommand(extentionsOfFiles);
});

var rootCommand = new RootCommand("'prj' is a tool for packaging code from different files in one file.\n" +
    "Running the root command will create a file that contains all of the desired code that you want to pack.");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);
rootCommand.InvokeAsync(args);
