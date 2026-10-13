using MauiApp3.Data;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauiApp3.Commands
{
    public class DeletePlayerCommandHandler
    {
        private readonly PlayerRepository _playerRepository;
        public DeletePlayerCommandHandler(PlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public bool Handle(DeletePlayerCommand command)
        {
            Player? player =
                _playerRepository.Players.FirstOrDefault(p => p.Id == command.Id);

            if (player == null)
            {
                return false;
            }

            _playerRepository.Players.Remove(player);
            return true;
        }
    }
}
