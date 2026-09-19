using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace Alborz.WinUI.ViewModels.Common;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    protected void ReportError(Exception exception)
    {
        ErrorMessage = exception is ArgumentException or InvalidOperationException or KeyNotFoundException
            ? exception.Message : "The operation failed. Check the database connection and try again.";
    }
}

