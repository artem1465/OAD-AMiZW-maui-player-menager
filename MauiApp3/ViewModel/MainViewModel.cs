using MauiApp3.Commands;
using MauiApp3.Model;
using MauiApp3.Queries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace MauiApp3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GetPlayerByIdQueryHandler _getPlayerByIdQueryHandler;
        private readonly GetPlayersQueryHandler _getPlayersQueryHandler;
        private readonly AddPlayerCommandHandler _addPlayerCommandHandler;
        private readonly DeletePlayerCommandHandler _deletePlayerCommandHandler;
        private readonly EditPlayerCommandHandler _editPlayerCommandHandler;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Player> Players { get; } = new();
        public string PlayerName { get; set; } = "";
        public string PlayerLevel { get; set; } = "";
        public string PlayerId { get; set; } = "";

        public string DeletePlayerId { get; set; } = "";
        public string EditPlayerId { get; set; } = "";
        public string EditPlayerName { get; set; } = "";
        public string EditPlayerLevel { get; set; } = "";

        private string _searchResult = "";
        private string _deleteResult = "";
        private string _editResult = "";

        public string SearchResult
        {
            get
            {
                return _searchResult;
            }

            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        public string DeleteResult
        {
            get
            {
                return _deleteResult;
            }

            set
            {
                _deleteResult = value;
                OnPropertyChanged();
            }
        }

        public string EditResult
        {
            get
            {
                return _editResult;
            }

            set
            {
                _editResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPlayerCommandButton { get; }
        public ICommand FindPlayerCommandButton { get; }
        public ICommand DeletePlayerCommandButton { get; }
        public ICommand EditPlayerCommandButton { get; }

        public MainViewModel(
            GetPlayerByIdQueryHandler getPlayerByIdQueryHandler,
            GetPlayersQueryHandler getPlayersQueryHandler,
            AddPlayerCommandHandler addPlayerCommandHandler,
            DeletePlayerCommandHandler deletePlayerCommandHandler,
            EditPlayerCommandHandler editPlayerCommandHandler)
        {
            _getPlayerByIdQueryHandler = getPlayerByIdQueryHandler;
            _getPlayersQueryHandler = getPlayersQueryHandler;
            _addPlayerCommandHandler = addPlayerCommandHandler;
            _deletePlayerCommandHandler = deletePlayerCommandHandler;
            _editPlayerCommandHandler = editPlayerCommandHandler;

            AddPlayerCommandButton = new Command(AddPlayer);
            FindPlayerCommandButton = new Command(FindPlayer);
            DeletePlayerCommandButton = new Command(DeletePlayer);
            EditPlayerCommandButton = new Command(EditPlayer);

            LoadPlayers();
        }

        private void DeletePlayer()
        {
            if (!int.TryParse(DeletePlayerId, out int playerId))
            {
                DeleteResult = "Niepoprawne ID";
                return;
            }

            DeletePlayerCommand command = new DeletePlayerCommand(playerId);
            bool deleted = _deletePlayerCommandHandler.Handle(command);

            DeleteResult = deleted ? "Usunięto gracza" : "Nie znaleziono gracza";
            LoadPlayers();
        }

        private void EditPlayer()
        {
            if (!int.TryParse(EditPlayerId, out int playerId))
            {
                EditResult = "Niepoprawne ID";
                return;
            }

            if (!int.TryParse(EditPlayerLevel, out int playerLevel))
            {
                EditResult = "Niepoprawny poziom";
                return;
            }

            if (string.IsNullOrWhiteSpace(EditPlayerName))
            {
                EditResult = "Podaj nazwę gracza";
                return;
            }

            EditPlayerCommand command =
                new EditPlayerCommand(playerId, EditPlayerName.Trim(), playerLevel);

            bool edited = _editPlayerCommandHandler.Handle(command);

            EditResult = edited ? "Zedytowano gracza" : "Nie znaleziono gracza";
            LoadPlayers();
        }

        private void AddPlayer()
        {
            if (!int.TryParse(PlayerLevel, out int playerLevel))
            {
                return;
            }
            AddPlayerCommand command = new AddPlayerCommand(PlayerName, playerLevel);
            _addPlayerCommandHandler.Handle(command);

            LoadPlayers();
        }

        private void FindPlayer()
        {
            if (!int.TryParse(PlayerId, out int playerId))
            {
                SearchResult = "Niepoprawne ID";
                return;
            }

            GetPlayerByIdQuery query =
                new GetPlayerByIdQuery(playerId);

            Player? player =
                _getPlayerByIdQueryHandler.Handle(query);

            if (player == null)
            {
                SearchResult = "Nie znaleziono gracza";
                return;
            }

            SearchResult =
                $"{player.Name} - poziom {player.Level}";
        }

        private void LoadPlayers()
        {
            GetPlayersQuery query = new GetPlayersQuery();
            List<Player> players = _getPlayersQueryHandler.Handle(query);

            Players.Clear();

            foreach (Player player in players)
            {
                Players.Add(player);
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string?  propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
