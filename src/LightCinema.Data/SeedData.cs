using LightCinema.Data.Entities;

namespace LightCinema.Data;

public static class SeedData
{
    public static async Task Seed(ApplicationDbContext dbContext)
    {
        var random = new Random();

        var users = new List<User>
        {
            new()
            {
                Login = "user1",
                Password = "test"
            },
            new()
            {
                Login = "user2",
                Password = "test"
            }
        };

        var hallCount = 2;
        var seatRows = 5;
        var seatNumbers = 5;
        var seats = new List<Seat>(hallCount * seatRows * seatNumbers);

        for (var i = 0; i < hallCount; i++)
        {
            for (var j = 0; j < seatRows; j++)
            {
                for (var k = 0; k < seatNumbers; k++)
                {
                    seats.Add(new Seat
                    {
                        Hall = i,
                        Row = j,
                        Number = k,
                        IsIncreasedPrice = j == seatRows - 1
                    });
                }
            }
        }

        var countries = new List<Country>
        {
            new() { Name = "Россия" },
            new() { Name = "США" },
            new() { Name = "Великобритания" },
            new() { Name = "Германия" },
            new() { Name = "Франция" }
        };

        var genres = new List<Genre>
        {
            new() { Name = "криминал" },
            new() { Name = "драма" },
            new() { Name = "триллер" },
            new() { Name = "ужасы" },
            new() { Name = "романтика" }
        };

        var movies = new List<Movie>
        {
            new()
            {
                Name = "Драйв",
                Descriptions =
                    "Молчаливый водитель спасает девушку от гангстеров. Неонуар с Райаном Гослингом и пульсирующим саундтреком",
                Genres = genres.Take(3).ToList(),
                Countries = new List<Country> { countries[1] },
                Year = 2011,
                AgeLimit = 16,
                PosterLink = "https://avatars.mds.yandex.net/get-kinopoisk-image/1704946/66b27e0c-9f85-424c-bfb0-415bd8475bc8/1920x",
                ImageLink = "https://avatars.mds.yandex.net/get-kinopoisk-image/1946459/af69a221-5921-4186-b700-2197d39e8362/1920x",
            },
            new()
            {
                Name = "Зелёный слоник",
                Descriptions =
                    "Два младших офицера, сидя в одной камере на гауптвахте, вынуждены решать острые социальные и психологические вопросы в небольшом пространстве.",
                Genres = new List<Genre> { genres[2], genres[3] },
                Countries = new List<Country> { countries[0] },
                Year = 1999,
                AgeLimit = 16,
                PosterLink = "https://upload.wikimedia.org/wikipedia/ru/3/34/%D0%97%D0%B5%D0%BB%D1%91%D0%BD%D1%8B%D0%B9_%D1%81%D0%BB%D0%BE%D0%BD%D0%B8%D0%BA.jpg",
                ImageLink = "https://zen4tech.files.wordpress.com/2013/03/exmyzm2vbl8.jpg",
            }
        };

        var sessions = new List<Session>
        {
            new()
            {
                Movie = movies[1],
                Hall = 0,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(1)
            },
            new()
            {
                Movie = movies[1],
                Hall = 1,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(3)
            },
            new()
            {
                Movie = movies[1],
                Hall = 0,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddDays(1)
            },
            new()
            {
                Movie = movies[0],
                Hall = 1,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(2)
            },
            new()
            {
                Movie = movies[0],
                Hall = 1,
                Price = 20,
                IncreasedPrice = 500,
                Start = DateTimeOffset.UtcNow.AddDays(1).AddHours(2)
            },
            new()
            {
                Movie = movies[1],
                Hall = 1,
                Price = 1,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddDays(15)
            }
        };

        var reservations = new List<Reservation>
        {
            new()
            {
                User = users[0],
                Session = sessions[0],
                Seat = seats[0]
            },
            new()
            {
                User = users[0],
                Session = sessions[0],
                Seat = seats[1]
            },
            new()
            {
                User = users[1],
                Session = sessions[0],
                Seat = seats[2]
            }
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.Seats.AddRangeAsync(seats);
        await dbContext.Countries.AddRangeAsync(countries);
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.Movies.AddRangeAsync(movies);
        await dbContext.Sessions.AddRangeAsync(sessions);
        await dbContext.Reservations.AddRangeAsync(reservations);

        await dbContext.SaveChangesAsync();
    }
}