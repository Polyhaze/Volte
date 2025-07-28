using Avalonia.Controls;
using Gommon;
using Volte.UI.Avalonia.ViewModels;

namespace Volte.UI.Avalonia.Controls;

public class VolteControl<TViewModel> : UserControl where TViewModel : BaseModel
{
    public TViewModel ViewModel
    {
        get
        {
            if (DataContext is null)
                return null!;
            
            if (DataContext is not TViewModel viewModel)
                throw new InvalidOperationException(
                    $"Underlying DataContext is not of type {typeof(TViewModel).AsPrettyString()}; " +
                    $"Actual type is {DataContext?.GetType().AsPrettyString()}");

            return viewModel;
        }
        set => DataContext = value;
    }
}