using Service_Health.Core.Models;

namespace Service_Health.Core;

public interface IDataAccess
{
    void Save(CheckResult result);

    IReadOnlyList<CheckResult> GetLatest();

    IReadOnlyList<CheckResult> GetHistory(string key);
}