public class HotelApi : IApi
{
    public void Register(WebApplication app)
    {
        app.MapGet("/hotels", Get)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .WithName("GetAllHotels")
            .WithTags("Getters");

        app.MapGet("/hotels/search/name/{query}", GetByName)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .Produces<List<Hotel>>(StatusCodes.Status404NotFound)
            .WithName("SearchHotel")
            .WithTags("Getters")
            .ExcludeFromDescription();

        app.MapGet("/hotels/{id}", GetById)
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .WithName("GetHotel")
            .WithTags("Getters");

        app.MapPost("/hotels", Create)
            .Accepts<Hotel>("application/json")
            .Produces<Hotel>(StatusCodes.Status201Created)
            .WithName("CreateHotel")
            .WithTags("Creators");

        app.MapPut("hotels", Update)
            .Accepts<Hotel>("application/json")
            .Produces<Hotel>(StatusCodes.Status201Created)
            .WithName("UpdateHotel")
            .WithTags("Updaters");
            
        app.MapDelete("hotels/{id}", Delete)
            .Produces<Hotel>(StatusCodes.Status201Created)
            .WithName("DeleteHotel")
            .WithTags("Deleters");
    }

    [Authorize] 
    private async Task<IResult> Get(IHotelRepository repository) => Results.Ok(await repository.GetHotelsAsync());
    [Authorize] 
    private async Task<IResult> GetById(int id,IHotelRepository repository) => 
            await repository.GetHotelAsync(id) is Hotel hotel
            ? Results.Ok(hotel)
            : Results.NotFound();
    [Authorize] 
    private async Task<IResult> GetByName(string query, IHotelRepository repository) => 
            await repository.GetHotelsAsync(query) is IEnumerable<Hotel> hotels
            ? Results.Ok(hotels)
            : Results.NotFound(Array.Empty<Hotel>());
    [Authorize] 
    private async Task<IResult> Create([FromBody] Hotel hotel, IHotelRepository repository)
    {
        await repository.InsertHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{hotel.Id}", hotel);
    }
    [Authorize] 
    private async Task<IResult> Update([FromBody] Hotel hotel, IHotelRepository repository)
    {
        await repository.UpdateHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.NoContent();
    }
    [Authorize] 
    private async Task<IResult> Delete(int id, IHotelRepository repository)
    {
        await repository.DeleteHotelAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    }
}