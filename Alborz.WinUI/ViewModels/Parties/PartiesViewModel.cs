using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Alborz.Application.Features.Parties.Commands;
using Alborz.Application.Features.Parties.Queries;

namespace ProjectName.WinUI.ViewModels;

public partial class PartiesViewModel : ObservableObject
{

    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region <Collections>

    public ObservableCollection<PartyDto> Parties { get; } = new();

    #endregion

    #region <Observable Properties>

    [ObservableProperty]
    private string _searchName = string.Empty;

    [ObservableProperty]
    private string _searchPhone = string.Empty;

    [ObservableProperty]
    private PartyDto? _selectedParty;

    #endregion

    #region <Constructor>

    public PartiesViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = LoadPartiesAsync();
    }

    #endregion

    #region <Commands & Methods>

    [RelayCommand]
    public async Task LoadPartiesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetPartiesQuery(SearchName, SearchPhone));

        Parties.Clear();
        foreach (var party in result)
        {
            Parties.Add(party);
        }
    }

    public async Task AddPartyAsync(string name, string phone, bool isSupplier, bool isCustomer)
    {
        using var scope = _scopeFactory.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IMediator>()
            .Send(new CreatePartyCommand(name, phone, isSupplier, isCustomer));

        await LoadPartiesAsync();
    }

    public async Task UpdatePartyAsync(int id, string name, string phone, bool isSupplier, bool isCustomer)
    {
        using var scope = _scopeFactory.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IMediator>()
            .Send(new UpdatePartyCommand(id, name, phone, isSupplier, isCustomer));

        await LoadPartiesAsync();
    }

    #endregion
}