using CommunityToolkit.Mvvm.ComponentModel;

namespace Volte.UI.Avalonia.ViewModels;

public class BaseModel : ObservableObject
{
    protected void OnPropertiesChanged(string firstPropertyName, params ReadOnlySpan<string> propertyNames)
    {
        OnPropertyChanged(firstPropertyName);
        foreach (string propertyName in propertyNames)
        {
            OnPropertyChanged(propertyName);
        }
    }
}