using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P03_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            INSERT INTO Categories (Name, Description, ImageUrl, IsActive) VALUES
            ('Action','Action movies','category1.jpg',1),
            ('Comedy','Comedy movies','category2.jpg',1),
            ('Drama','Drama movies','category3.jpg',1),
            ('SciFi','Science fiction','category4.jpg',1),
            ('Horror','Horror movies','category5.jpg',1),
            ('Adventure','Adventure movies','category6.jpg',1),
            ('Romance','Romantic movies','category7.jpg',1),
            ('Animation','Animated movies','category8.jpg',1);

            INSERT INTO Cinemas (Name, Location, ImageUrl, Rate, IsActive) VALUES
            ('Cinema City','Cairo','cinema1.jpg',4.5,1),
            ('IMAX Cairo','Cairo','cinema2.jpg',4.8,1),
            ('Galaxy Cinema','Giza','cinema3.jpg',4.2,1),
            ('Point90 Cinema','New Cairo','cinema4.jpg',4.6,1),
            ('Downtown Cinema','Cairo','cinema5.jpg',4.1,1);

            INSERT INTO Actors (FullName, BirthDate, Bio, ImageUrl) VALUES
            ('Robert Downey Jr','1965-04-04','Famous Hollywood actor','actor1.jpg'),
            ('Scarlett Johansson','1984-11-22','American actress','actor2.jpg'),
            ('Leonardo DiCaprio','1974-11-11','Award winning actor','actor3.jpg'),
            ('Tom Hardy','1977-09-15','British actor','actor4.jpg'),
            ('Emma Stone','1988-11-06','American actress','actor5.jpg'),
            ('Christian Bale','1974-01-30','Known for intense roles','actor6.jpg'),
            ('Margot Robbie','1990-07-02','Australian actress','actor7.jpg'),
            ('Ryan Gosling','1980-11-12','Canadian actor','actor8.jpg');

            INSERT INTO Movies (Name, Description, Price, ReleaseDate, MainImage, Status) VALUES
            ('The Last Mission','Elite agents on final mission',80,'2023-05-10','movie1.jpg','Available'),
            ('Future World','Humans fight AI uprising',90,'2024-01-15','movie2.jpg','Available'),
            ('Love in Paris','Romantic story in Paris',60,'2023-02-10','movie3.jpg','Available'),
            ('Haunted Night','Group trapped in haunted house',70,'2023-10-01','movie4.jpg','Available'),
            ('Galaxy War','Intergalactic battle begins',100,'2024-03-20','movie5.jpg','ComingSoon');

            INSERT INTO MovieCategories (MovieId, CategoryId) VALUES
            (1,1),(1,6),
            (2,4),(2,1),
            (3,7),(3,3),
            (4,5),
            (5,4),(5,6);

            INSERT INTO MovieActors (MovieId, ActorId) VALUES
            (1,1),(1,4),
            (2,3),(2,6),
            (3,5),(3,8),
            (4,4),
            (5,1),(5,7);

            INSERT INTO CinemaMovies (CinemaId, MovieId) VALUES
            (1,1),(1,2),(1,3),
            (2,1),(2,4),
            (3,2),(3,3),
            (4,5),
            (5,1),(5,4);

            INSERT INTO MovieImages (ImageUrl, MovieId) VALUES
            ('movie1.jpg',1),
            ('movie2.jpg',1),
            ('movie3.jpg',2),
            ('movie4.jpg',2),
            ('movie5.jpg',3),
            ('movie6.jpg',4),
            ('movie7.jpg',5);

        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            DELETE FROM MovieImages;

            DELETE FROM CinemaMovies;

            DELETE FROM MovieActors;

            DELETE FROM MovieCategories;

            DELETE FROM Movies;

            DELETE FROM Actors;

            DELETE FROM Cinemas;

            DELETE FROM Categories;

        ");
        }
    }
}
