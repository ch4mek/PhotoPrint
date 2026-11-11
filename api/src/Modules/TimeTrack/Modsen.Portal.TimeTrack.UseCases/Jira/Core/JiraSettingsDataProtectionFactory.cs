using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Core;

internal interface IJiraSettingsDataProtectionFactory
{
    IDataProtectionService Create();
}

internal sealed class JiraSettingsDataProtectionFactory : IJiraSettingsDataProtectionFactory
{
    private readonly IDataProtectionFactory _protectionFactory;
    private const string _jiraSettingScope = "JiraSetting";

    public JiraSettingsDataProtectionFactory(IDataProtectionFactory protectionFactory)
    {
        _protectionFactory = protectionFactory;
    }

    public IDataProtectionService Create()
    {
        return _protectionFactory.Create(_jiraSettingScope);
    }
}
