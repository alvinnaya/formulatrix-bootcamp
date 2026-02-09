using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Controllers;
using System.Text.Json;
using Dto;
using Microsoft.AspNetCore.SignalR;
using GameControllerNamespace;
using helperFunction;



        Deck.Deck deck = new Deck.Deck();
        Helper.InitDeck(deck);
        Helper.Shuffle(deck.Cards);
        DiscardPile discardPile = new DiscardPile();
    
        //awalnya player kosong, nanti diisi pas ada command createplayer
        GameController game = new GameController( new List<IPlayer>(),deck, discardPile);

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<GameController>(game);

        builder.Services.AddCors(options =>
        {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        var app = builder.Build();

        app.UseRouting();
        app.UseCors(); 

        app.MapControllers();              
        app.MapHub<GameHub>("/gamehub");   

        app.Run();
   