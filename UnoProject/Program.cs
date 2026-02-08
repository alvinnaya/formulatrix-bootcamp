using System;
using System.Collections.Generic;
using Cards;
using Players;
using Deck;
using Controller;
using Fleck;
using System.Text.Json;
using Dto;


        
        //membuat game object awal saat server pertama kali dijalankan

        Deck.Deck deck = new Deck.Deck();
        InitDeck(deck);
        Shuffle(deck.Cards);
        DiscardPile discardPile = new DiscardPile();
        //awalnya player kosong, nanti diisi pas ada command createplayer
        GameController game = new GameController( new List<IPlayer>(),deck, discardPile);


        //menjalankan server websocket
        List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        WebSocketServer server = new WebSocketServer("ws://0.0.0.0:5000");

  
        // ini adalah event-event tambahan
        game.GameStarted += () => BroadcastGameState("gamestart");
        game.CardPlayed += (p, c) => BroadcastGameState($"playcard {c}");
        game.CardDrawn += (p, c) => BroadcastGameState($"drawcard {c}");
        game.TurnStarted += p => BroadcastGameState($"turnstart {p.Name}");
        game.TurnEnded += p => BroadcastGameState($"turnend {p.Name}");
        game.DirectionChanged += d => BroadcastGameState($"direction {d}");
        game.CurrentColorChanged += c => BroadcastGameState($"color {c}");
        game.UnoCalled += p => BroadcastJson("info", new { message = $"{p.Name} called UNO!" });
        game.UnoPenaltyApplied += p => BroadcastJson("info", new { message = $"{p.Name} penalty +2" });
        game.GameEnded += p => BroadcastJson("GameEnd", new { winner = $"{p.Name} has win", gameEnd= game.IsGameOver});
    

    //memulai server websocket
        server.Start(socket =>
        {
            //saat ada client yang connect ia akan menyimpan koneksinya di list allSockets
            socket.OnOpen = () =>
            {
                Console.WriteLine("Client connected!");
                 allSockets.Add(socket);
                 //kirim state game saat client connect
                 BroadcastGameState("state");
                
            };
            //saat ada client yang disconnect ia akan menghapus koneksinya dari list allSockets
            socket.OnClose = () =>
            {
                Console.WriteLine("Client disconnected");
                allSockets.Remove(socket);
            };
            //saat ada pesan dari client, ia akan memanggil fungsi HandleMessage untuk memproses pesan tersebut secara asynchronous
            socket.OnMessage = async message =>
            {
                Console.WriteLine("Received: " + message);
                await HandleMessage(socket, message);
            };
        });

        Console.WriteLine("WebSocket server running on ws://localhost:5000");
        Console.ReadLine(); // biar server tetap jalan


//fungsi untuk menangani pesan dari client
async Task HandleMessage(IWebSocketConnection socket, string message)
    {
        string[] parts = message.Split(" ");
        switch (parts[1].ToLower())
        {     
            //command createplayer di game controller
            case "createplayer":
            {
                //filter input messege format awalan harus "Game"
                if(parts[0] != "Game" )
                {
                    SendJson(socket, "error", new { message = "invalid input" });
                    break;
                }
                //filter input messege harus ada jumlah player
                if (parts.Length < 3)
                {
                    SendJson(socket, "error", new { message = "missing number of players" });
                    break;
                }
                //filter jika game sudah dimulai tidak bisa membuat player baru
                if(game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game already started" });
                    break;
                }

                //cek apakah input jumlah player valid integer
                if (int.TryParse(parts[2], out int numOfPlayers))
                {
                    //membuat list player baru
                    List<IPlayer> newPlayerList =  new List<IPlayer>();
                    //memanipulasi list player dengan menggunakan fungsi createplayer, fungsi ini akan menambahkan player ke list sesuai jumlah yang diinput
                    CreatePlayers(numOfPlayers, newPlayerList);
                    // Console.WriteLine($"Created {numOfPlayers} players.");
                    // Console.WriteLine(newPlayerList.Count);
                    // newPlayerList.ForEach(p => Console.WriteLine(p.Name));

                    //mengganti list player di game controller dengan list player yang baru dibuat
                    game.ChangePlayers(newPlayerList);
                    //mengirim info ke client bahwa player berhasil dibuat
                    SendJson(socket, "info", new { message = $"there was {numOfPlayers} Players" });
                }
                break;

                
            }
            //command start di game 
            case "start":{
                

                //filter tidak bisa start game jika sudah dimulai
                if(game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game already started" });
                    break;
                }
                //filter tidak bisa start game jika player belum di set
                if (game.Players == null || game.Players.Count == 0)
                {
                    SendJson(socket, "error", new { message = "Players not set" });
                    break;
                }
                //filter awalan input messege harus "Game"
                if(parts[0] == "Game")
                {
                
                    //this is the main problem
                //memberikan 7 kartu ke setiap player
                foreach (IPlayer p in game.Players)
                for (int i = 0; i < 7; i++)
                {
                    game.DrawCard(p);
                }
            
                //memulai game
                game.StartGame();

                //mengirim state game awal ke semua client
                BroadcastGameState("game start");


               
             
                }
                   break;
            }
            //command reset di game 
            case "reset":
            {
                //filter awalan input messege harus "Game"
                if (parts[0] != "Game")
                {
                    SendJson(socket, "error", new { message = "invalid input" });
                    break;
                }

                //membuat ulang deck dan discard pile baru sebagai mekanisme reset game
                Deck.Deck newDeck = new Deck.Deck();
                InitDeck(newDeck);
                Shuffle(newDeck.Cards);
                IDiscardPile newDiscard = new DiscardPile();

                //mereset game dengan deck dan discard pile yang baru
                game.ResetGame(newDeck, newDiscard);

                //mengirim info ke client bahwa game telah di reset
                SendJson(socket, "info", new { message = "game reset" });
                break;
            }
            //command untuk mendapatkan state game saat ini yang akan dikirim ke client
            case "getcurrentstate":
            {
                //filter untuk memastikan game sudah dimulai
                 if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }

                //mendapatkan kartu terakhir yang dimainkan
                ICard lastCard = game.GetLastPlayedCard();
                //membuat object anonim yang berisi state game saat ini, 
                //yang berisi info kartu terakhir, giliran player saat ini, jumlah kartu setiap player, warna saat ini, dan status akhir game
                 GameStateDTO gameState = new GameStateDTO
                {
                    LastCard = lastCard.ToString(),
                    CurrentPlayer = game.GetCurrentPlayer().Name,
                    AllPlayers = GetPlayersCardCount(game),
                    CurrentColor = game.CurrentColor.ToString(),
                    GameEnd = game.IsGameOver,
                    IsGameStarted = game.IsGameStarted
                };

                SendJson(socket, "gameState", gameState);
                break;
                
            }
            //command untuk mendapatkan kartu yang dimiliki player yang diminta
            case "getcard":
            {
                //filter untuk memastikan game sudah dimulai
                if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }

                //mendapatakan player berdasarkan nama yang dikirim dari client
                IPlayer player = game.GetPlayerByName(parts[0]);
                //filter jika player tidak ditemukan
                if(player == null)
                {
                    SendJson(socket, "error", new { message = "Player not found" });
                    break;
                }

                //mendapatkan kartu yang dimiliki player
                IReadOnlyList<ICard> hand = game.GetPlayerCards(player);
                ICard lastCard = game.GetLastPlayedCard();

                // membuat object anonim yang berisi state player yang diminta
                var state = new
                {
                    lastCard = lastCard.ToString(),
                    player = player.Name,
                    //retturn list yang berisi informasi tentang kartu yang dimiliki player
                    hand = BuildHandState(hand)
                };
                //return state ke client yang meminta
                SendJson(socket, "playerState", state);
            

                break;
            }  
            //command draw di game untuk mengambil kartu berdasarkan player saat ini
            case "draw":
            {       //filter untuk memastikan game sudah dimulai
                    if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }


                    //mendapatkan player yang sedang mendapat giliran
                    IPlayer currentPlayer = game.GetCurrentPlayer();
                    //filter untuk memastikan player yang meminta draw adalah player yang sedang mendapat giliran
                    if(currentPlayer.Name == parts[0])
                    {   
                        //melakukan draw card untuk player saat ini
                        game.DrawCard(currentPlayer);
                        //mengakhiri giliran player saat ini
                        game.Nexturn();
                        //mendapatkan kartu yang dimiliki player saat ini sebelum draw
                        IReadOnlyList<ICard> hand = game.GetPlayerCards(currentPlayer);
                        //mendapatkan kartu terakhir yang dimainkan setelah draw
                        ICard lastCard = game.GetLastPlayedCard();
                        //mendapatkan player selanjutnya setelah giliran player saat ini berakhir
                        IPlayer nextPlayer = game.GetCurrentPlayer();

                        //membuat object anonim yang berisi state player saat ini setelah draw
                     var playerState = new
                    {   //kartu terakhir yang dimainkan setelah draw
                        lastCard = lastCard.ToString(),
                        //nama player saat ini sebelum draw atau yang melakukan draw
                        player = currentPlayer.Name,
                        //retturn list yang berisi informasi tentang kartu yang dimiliki player yang melakukan draw setelah draw
                        hand = BuildHandState(game.GetPlayerCards(currentPlayer))
                    };

                    // Kirim ke client
                    //mengirim state game state setelah draw ke semua client
                    BroadcastGameState($"{currentPlayer} draw card");
                    //mengirim state player yang melakukan draw ke client yang bersangkutan
                    SendJson(socket, "playerState", playerState);

             
                
                    }


            }
                
           
                    

                   
                
                break;

            case "play":
            {       //filter untuk memastikan game sudah dimulai
                    if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }
                
                    //mendapatkan player yang sedang mendapat giliran
                    IPlayer currentPlayer = game.GetCurrentPlayer();
                    //mendapatkan kartu yang dimiliki player saat ini
                    IReadOnlyList<ICard> hand = game.GetPlayerCards(currentPlayer);
                    //mengambil index kartu yang akan dimainkan dari input message
                    Int32 idx = int.Parse(parts[2]);
                    //mmendapatkan kartu terakhir yang dimainkan sebelum play card
                    ICard lastCard = game.GetLastPlayedCard();

                    //filter untuk memastikan player yang meminta play adalah player yang sedang mendapat giliran
                    if(currentPlayer.Name == parts[0])
                    {
                         ICard card = hand[idx];
                    //memastikan kartu yang dimainkan valid sesuai aturan uno
                    if (game.IsCardValid(card))
                    {
                        
                        //jika kartu yang dimainkan adalah wild card, maka harus ada warna yang dipilih di format messagenya
                        if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
                        {
                           //memastikan input message ada warna yang dipilih dan mengkonversi string warna ke enum CardColor
                            if (!Enum.TryParse<CardColor>(parts[3], true, out CardColor chosenColor) ||
                                !Enum.IsDefined(typeof(CardColor), chosenColor))
                            {
                                SendJson(socket, "error", new { message = "invalid color for wild card" });
                                break;
                            }
                            //mengatur warna saat ini di game controller sesuai warna yang dipilih
                            game.SetCurrentColor(chosenColor);

                        }
                        //memainkan kartu yang dipilih
                        game.PlayCard(currentPlayer, card);
                    
                        //bila kartu yang dimainkan membuat player hanya memiliki 1 kartu tersisa maka akan 
                        // delay selama 3 detik sebelum melanjutkan giliran untuk memberikan waktu kepada player untuk memanggil command "uno"
                        if(hand.Count == 1)
                        {    
                            //meamngirim state player dan game setelah play card tapi sebelum giliran dilanjutkan
                             var playerStateUno = new
                            {
                                lastCard = game.GetLastPlayedCard().ToString(),
                                currentPlayer = currentPlayer.Name,
                                hand = BuildHandState(game.GetPlayerCards(currentPlayer))
                            };

                           
                             //mengirim GameState ke semua client yang isinya adalah 
                        
                            //player state dikirim ke client yang bersangkutan
                            SendJson(socket, "playerState", playerStateUno);
                                         
                            //mengirim GameState ke semua client yang isinya adalah 
                            BroadcastGameState($"{currentPlayer} played {card} ");

                            //delay 3 detik
                             await Task.Delay(3000);
                          
                            
                        }
                        //melanjutkan giliran ke player selanjutnya lalu mengirim state player dan game
                        //setelah play card dan giliran dilanjutkan
                        game.Nexturn();
                        IPlayer nextPlayer = game.GetCurrentPlayer();
                         var playerState = new
                        {
                            lastCard = game.GetLastPlayedCard().ToString(),
                            currentPlayer = currentPlayer.Name,
                            hand = BuildHandState(game.GetPlayerCards(currentPlayer))
                        };
                // Kirim ke client untuk player state dan semua client untuk game state
                SendJson(socket, "playerState", playerState);
                BroadcastGameState($"{currentPlayer} has played card {lastCard}");
                            }
                    else
                    {
                        SendJson(socket, "error", new { message = "invalid card" });
                    }
                    }
                  
                    
                
                break;

                
            }
            //command uno di game untuk memanggil uno saat player memiliki 1 kartu tersisa
            //tapi uno dapat dipanggil kapan saja selama game berjalan
            case "uno":
            {
                //filter untuk memastikan game sudah dimulai
                if(!game.IsGameStarted)
                {
                    SendJson(socket, "error", new { message = "Game not started" });
                    break;
                }
                    //mendapatkan nama player yang memanggil uno dari input message
                   string playerName = parts[0];
                   //mendapatkan object player berdasarkan nama yang dikirim
                   IPlayer currentPlayer = game.GetPlayerByName(playerName);
                    //memanggil fungsi calluno di game controller untuk player yang ada di message
                      game.CallUno(currentPlayer);
                      //mengirim info ke semua client bahwa player ini telah memanggil uno
                        BroadcastJson("info", new { message = $"{currentPlayer.Name} called UNO!" } );

                         //mengirim GameState ke semua client yang isinya adalah 
                        BroadcastGameState($"{currentPlayer} call uno");
                  
                   
                
                break;
                
            }

            
            default:
                SendJson(socket, "error", new { message = "Unknown command" });
                break;
        }
    }

//fungsi untuk mengirim state game saat ini ke semua client
void BroadcastGameState(string action)
        {
            if (!game.IsGameStarted) return;

            ICard lastCard = game.GetLastPlayedCard();
            GameStateDTO gameState = new GameStateDTO   
            {
                LastCard = lastCard?.ToString() ?? string.Empty,
                CurrentPlayer = game.GetCurrentPlayer().Name,
                //mendapatkan jumlah kartu setiap player
                AllPlayers = GetPlayersCardCount(game),
                //action yang dilakukan pemain
                Action   = action,
                //mendapatkan informasi warna kartu saat ini
                CurrentColor = game.CurrentColor.ToString(),
                //status apakah game telah berakhir
                GameEnd = game.IsGameOver,

            };

            BroadcastJson("gameState", gameState);
        }
//fungsi untuk mengirim pesan ke semua client
void Broadcast(string message)
{
    foreach (IWebSocketConnection s in allSockets.ToList()) // ToList() biar aman kalau ada disconnect
    {
        s.Send(message);
    }
}
//mengirim pesan json ke client yang mengirim pesan
void SendJson(IWebSocketConnection socket, string type, object data)
{
    var payload = new { type, data };
    socket.Send(JsonSerializer.Serialize(payload));
}
//mengirim pesan ke semua client dalam format json
void BroadcastJson(string type, object data)
{
    var payload = new { type, data };
    Broadcast(JsonSerializer.Serialize(payload));
}

//membuat list object yang berisi nama player dan jumlah kartu yang dimilikinya    
List<PlayerCardCountDTO> GetPlayersCardCount(GameController game)
{
    // hitung jumlah kartu player dari fungsi yang sudah ada di game object
    var playerCounts = game.GetPlayerCardCounts();

    // Buat list object, bukan JSON string
    List<PlayerCardCountDTO> players = playerCounts.Select(kvp => new PlayerCardCountDTO
    {
        Name = kvp.Key,
        CardCount = kvp.Value
    }).ToList(); // cast ke object supaya bisa digabung dengan object anonim lain

    return players;
}

//retturn list yang berisi informasi tentang setiap kartu yang dimiliki player
List<object> BuildHandState(IReadOnlyList<ICard> hand)
{
    return hand
        .Select((c, idx) => new
        {
            index = idx,
            card = c.ToString(),
            cardColor = c.Color?.ToString() ?? string.Empty,
            cardType = c.Type.ToString()
        })
        .ToList<object>();
}

//menambahkan kartu-kartu ke deck
void InitDeck(IDeck deck)
    {
        foreach (CardColor color in Enum.GetValues<CardColor>())
        {
            deck.Cards.Push(new Card(CardType.Number0, color));
            deck.Cards.Push(new Card(CardType.Number1, color));
            deck.Cards.Push(new Card(CardType.Number2, color));
            deck.Cards.Push(new Card(CardType.Number3, color));
            deck.Cards.Push(new Card(CardType.Number4, color));
            deck.Cards.Push(new Card(CardType.Number5, color));
            deck.Cards.Push(new Card(CardType.Number6, color));
            deck.Cards.Push(new Card(CardType.Number7, color));
            deck.Cards.Push(new Card(CardType.Number8, color));
            deck.Cards.Push(new Card(CardType.Number9, color));

            deck.Cards.Push(new Card(CardType.Skip, color));
            deck.Cards.Push(new Card(CardType.Reverse, color));
            deck.Cards.Push(new Card(CardType.DrawTwo, color));
        }

        deck.Cards.Push(new Card(CardType.Wild));
        deck.Cards.Push(new Card(CardType.WildDrawFour));
    }

//mengacak urutan kartu di deck
void Shuffle<T>(Stack<T> stack)
    {
        // memindahkan elemen Stack ke List untuk diacak
        List<T> list = new List<T>(stack);
        // membangun ulang Stack dengan urutan acak
        Random rnd = new Random();
        stack.Clear();

        while (list.Count > 0)
        {   
            //mendapatkan index acak dari jumlah elemen di list
            int i = rnd.Next(list.Count);
            //mmemindahkan elemen dari list ke stack
            stack.Push(list[i]);
            //menghapus elemen yang sudah dipindahkan dari list
            list.RemoveAt(i);
        }
    }

//membuat player berdasarkan jumlah input yang diberikan dan menambahkannya ke list player
void CreatePlayers(int jumlah, List<IPlayer> players)
    {
      
        for (int i = 1; i <= jumlah; i++)
        {
            players.Add(new Player($"Player{i}"));
        }
       
    }

// void DrawCard(IPlayer player)
// {
//         game.DrawCard(player);
//         game.Nexturn();
// }




         
        
   

        // game start and events
       

        // ===== EVENTS =====
        //game.GameStarted += () => Console.WriteLine("Game Started");
        // game.TurnStarted += p => Console.WriteLine($"\nTurn: {p}");
        // game.CardPlayed += (p, c) => Console.WriteLine($"{p} played {c}");
        // game.CardDrawn += (p, c) => Console.WriteLine($"{p} drew {c}");
        // game.GameEnded += p => Console.WriteLine($"\nWINNER: {p}");
        // foreach (var p in players)
        //     for (int i = 0; i < 7; i++)
        //     {
        //         game.DrawCard(p);
        //     }
      
        // game.StartGame();
        
        // Console.WriteLine($"First card: {game.GetLastPlayedCard()}");

        

        // ===== GAME LOOP =====
        // while (!game.IsGameOver)
        // {

        //     var player = game.GetCurrentPlayer();
        //     var hand = game.GetPlayerCards(player);
        //     Console.WriteLine($"Last card: {game.GetLastPlayedCard()}");
        //     Console.WriteLine($"\n{player}'s turn");

        //     Console.WriteLine("\nHand:");
        //     for (int i = 0; i < hand.Count; i++)
        //         Console.WriteLine($"{i}. {hand[i]}");

        //     Console.Write("Index kartu / d (draw): ");
        //     var input = Console.ReadLine();

        //     if (input == "d")
        //     {
        //         DrawCard(player);
             
                
        //     }

        //     if (int.TryParse(input, out int idx) &&
        //         idx >= 0 && idx < hand.Count)
        //     {
        //         var card = hand[idx];
        //         if (game.IsCardValid(card))
        //         {
        //             game.PlayCard(player, card);

        //             if (hand.Count == 1)
        //             {
        //                 Console.Write("UNO? (y/n): ");
        //                 if (Console.ReadLine() == "y")
        //                     game.CallUno(player);
        //             }
        //             game.Nexturn();
        //         }
        //         else
        //         {
        //             Console.WriteLine("Invalid card");
        //         }
        //     }
        // }
    

    // ===== HELPERS =====
