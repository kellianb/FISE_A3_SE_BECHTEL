using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;
using BackupUtil.Cli.Command;
using BackupUtil.Core.Util;
using BackupUtil.I18n;

namespace BackupUtil.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        Logging.Init();

        Option<string> localeOption = new(["--locale", "-l"],
            "Locale of the application, example: 'fr-FR', 'en-GB', defaults to OS locale");

        localeOption.AddValidator(result => SetLocale(result, localeOption));

        RootCommand rootCommand =
        [
            CreateJobCommand.Build(),
            LoadJobsCommand.Build(),
            RemoveJobCommand.Build(),
            RunJobCommand.Build()
        ];

        rootCommand.AddGlobalOption(localeOption);

        return rootCommand.Invoke(args);
    }


    private static void SetLocale(OptionResult result, Option<string> localeOption)
    {
        if (result.GetValueForOption(localeOption) is not string locale)
        {
            return;
        }

        CultureInfo culture;

        try
        {
            culture = CultureInfo.GetCultureInfo(locale);
        }
        catch
        {
            Console.WriteLine("Invalid locale");
            return;
        }

        if (!I18N.GetSupportedCultures().Contains(culture))
        {
            Console.WriteLine("Unsupported locale");
            return;
        }

        I18N.SetCulture(new CultureInfo(locale));
    }
}
