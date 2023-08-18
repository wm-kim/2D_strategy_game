using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Api;
using Unity.Services.CloudSave.Model;

namespace Deck;

public class Class1
{
    private const int RequiredDeckSize = 5;
    private const int MaxDeckCount = 10;
    private const string DeckSaveKey = "decks";
    private const string CurrentDeckSaveKey = "currentDeckId";
    
    private readonly CloudSaveDataApi m_cloudSaveDataApi;
    
    public Class1(CloudSaveDataApi cloudSaveDataApi)
    {
        m_cloudSaveDataApi = new CloudSaveDataApi(new HttpApiClient());
    }
    
    [CloudCodeFunction("SaveDeckData")]
    public async Task SaveDeckData(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            CheckValidKeyAndValue(key, value, DeckSaveKey);
            
            // convert the value to a list of ints and check if it's valid
            DeckDTO deckDTO = JsonConvert.DeserializeObject<DeckDTO>(value.ToString());
            if (deckDTO != null && !IsValidDeckList(deckDTO.CardIds)) 
                throw new Exception("Invalid deck list");
            
            // get the current decks
            var decksObject = await GetData(context, gameApiClient, key);
            if (decksObject == null)
            {
                // await SaveData(context, gameApiClient, key, new List<DeckDTO> {deckDTO});
                
                // use service token to save data
                deckDTO.Id = 0;
                Dictionary<int, DeckDTO> decks = new Dictionary<int, DeckDTO> {{deckDTO.Id, deckDTO}};
                await SaveDecks(context, decks);
            }
            else
            {
                Dictionary<int, DeckDTO> decks = JsonConvert.DeserializeObject<Dictionary<int, DeckDTO>>(decksObject.ToString());
                // check if player can add more decks
                if (decks.Count >= MaxDeckCount)
                {
                    throw new Exception("Max deck count reached");
                }
                else
                {
                    // assign an available id to the deck
                    deckDTO.Id = GetAvailableDeckId(decks);
                    
                    // add the deck to the dictionary
                    decks.Add(deckDTO.Id, deckDTO);
                    
                    // use service token to save data
                    await SaveDecks(context, decks);
                }
            }
            
            // save the current deck id
            await SaveCurrentDeckId(context, deckDTO.Id);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    // TODO - also needs to validate card ids
    
    [CloudCodeFunction("SelectPlayerDeck")]
    public async Task SelectPlayerDeck(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            CheckValidKeyAndValue(key, value, CurrentDeckSaveKey);
            
            // convert the value to an int and check if it's valid
            int deckId = JsonConvert.DeserializeObject<int>(value.ToString());
            if (deckId < 0 || deckId >= RequiredDeckSize) 
                throw new Exception("Invalid deck id");
            
            // get the current decks
            var decksObject = await GetData(context, gameApiClient, "decks");
            if (decksObject == null)
            {
                throw new Exception("No decks found");
            }
            else
            {
                Dictionary<int, DeckDTO> decks = JsonConvert.DeserializeObject<Dictionary<int, DeckDTO>>(decksObject.ToString());
                // check if the deck exists
                if (!decks.ContainsKey(deckId))
                {
                    throw new Exception("Deck not found");
                }
                else
                {
                    await SaveCurrentDeckId(context, deckId);
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    [CloudCodeFunction("DeletePlayerDeck")]
    public async Task DeletePlayerDeck(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            CheckValidKeyAndValue(key, value, DeckSaveKey);
            
            // convert the value to an int and check if it's valid
            int deckId = JsonConvert.DeserializeObject<int>(value.ToString());
            if (deckId < 0 || deckId >= RequiredDeckSize) 
                throw new Exception("Invalid deck id");
            
            // get the current decks
            var decksObject = await GetData(context, gameApiClient, key);
            if (decksObject == null)
            {
                throw new Exception("No decks found");
            }
            else
            {
                Dictionary<int, DeckDTO> decks = JsonConvert.DeserializeObject<Dictionary<int, DeckDTO>>(decksObject.ToString());
                // check if the deck exists
                if (!decks.ContainsKey(deckId))
                {
                    throw new Exception("Deck not found");
                }
                else
                {
                    // remove the deck from the dictionary
                    decks.Remove(deckId);
                    
                    // use service token to save data
                    await SaveDecks(context, decks);
                }
            }
            
            // get the current deck id
            var currentDeckIdObject = await GetData(context, gameApiClient, CurrentDeckSaveKey);
            if (currentDeckIdObject == null) return;
            
            int currentDeckId = JsonConvert.DeserializeObject<int>(currentDeckIdObject.ToString());
            
            // check if the current deck id is the same as the deleted deck id
            if (currentDeckId == deckId)
            {
                // save the current deck id as -1
                await SaveCurrentDeckId(context, -1);
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    [CloudCodeFunction("GetPlayerDecks")]
    public async Task<List<DeckDTO>> GetPlayerDecks(IExecutionContext context, IGameApiClient gameApiClient, List<string> playerIds)
    {
        try
        {
            // get the current deck id of each player
            List<int> currentDeckIds = new List<int>();
            foreach (var playerId in playerIds)
            {
                var currentDeckIdObject = await GetDataFromServer(context, gameApiClient, playerId, CurrentDeckSaveKey);
                if (currentDeckIdObject == null)
                {
                    throw new Exception("No current deck id found for player " + playerId);
                }
                else
                {
                    int currentDeckId = JsonConvert.DeserializeObject<int>(currentDeckIdObject.ToString());
                    currentDeckIds.Add(currentDeckId);
                }
            }
            
            // get the current decks of each player
            List<DeckDTO> decksList = new List<DeckDTO>();
            foreach (var playerId in playerIds)
            {
                var decksObject = await GetDataFromServer(context, gameApiClient, playerId, DeckSaveKey);
                if (decksObject == null)
                {
                    throw new Exception("No decks found for player " + playerId);
                }
                else
                {
                    Dictionary<int, DeckDTO> decks = JsonConvert.DeserializeObject<Dictionary<int, DeckDTO>>(decksObject.ToString());
                    // check if the deck exists
                    
                    var playerIdIndex = playerIds.IndexOf(playerId);
                    if (!decks.ContainsKey(currentDeckIds[playerIdIndex]))
                    {
                        throw new Exception($"Deck {currentDeckIds[playerIdIndex]} not found for player {playerId}");
                    }
                    else
                    {
                        decksList.Add(decks[currentDeckIds[playerIdIndex]]);
                    }
                }
            }
            
            return decksList;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    private bool IsValidDeckList(List<int> deckList) => deckList.Count is RequiredDeckSize;
    
    private int GetAvailableDeckId(Dictionary<int, DeckDTO> decks)
    {
        bool[] isIdUsed = new bool[RequiredDeckSize];

        // Mark the IDs that are already in use
        foreach (var deck in decks)
        {
            int id = deck.Key;
            if (id >= 0 && id < RequiredDeckSize)
            {
                isIdUsed[id] = true;
            }
        }

        for (int i = 0; i < isIdUsed.Length; i++)
        {
            if (!isIdUsed[i]) return i;
        }

        // If no ID is available, return -1, but this should never happen
        return -1;
    }
    
    private void CheckValidKeyAndValue(string key, object value, string expectedKey)
    {
        // first check if the key is valid
        if (key != expectedKey)
            throw new Exception("Invalid key");
        
        // then check if the value is valid
        if (value == null)
            throw new Exception("value is null");
    }
    
    private async Task SaveCurrentDeckId(IExecutionContext context, int deckId)
    {
        await m_cloudSaveDataApi.SetItemAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
            new SetItemBody(CurrentDeckSaveKey, deckId));
    }
    
    private async Task SaveDecks(IExecutionContext context, Dictionary<int, DeckDTO> decks)
    {
        await m_cloudSaveDataApi.SetItemAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
            new SetItemBody(DeckSaveKey, decks));
    }
    
    private async Task<object?> GetData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new List<string> {key});
            return result.Data.Results.FirstOrDefault()?.Value;
        }
        catch (ApiException ex)
        {
            throw;
        }
    }
    
    /// <summary>
    /// Gets the player data from the server.
    /// </summary>
    private async Task<object?> GetDataFromServer(IExecutionContext context, IGameApiClient gameApiClient, string playerId, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(context, context.ServiceToken, context.ProjectId, playerId, new List<string> {key});
            return result.Data.Results.FirstOrDefault()?.Value;
        }
        catch (ApiException ex)
        {
            throw;
        }
    }
}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}