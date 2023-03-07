using Domain.Models;
using Service;

namespace MAUI.EmptyHand;

public partial class Game : ContentPage
{
    GameModel gameState;

    string userId;
    //Canvas deckImg;
    bool turnStarted;
    bool playerPlayed;

    PlayerModel actualPlayerInfo;
    PlayerModel enemyPlayerInfo;

    private SignalRService signalRClient;

    public Game(GameModel _gameState, string _userId)
    {
        InitializeComponent();

        userId = _userId;

        turnStarted = false;
        gameState = _gameState;
        playerPlayed = false;
        //UpdateGame(_gameState);
        //turnSoundPlayed = false;

    }

}