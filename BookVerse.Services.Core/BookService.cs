using BookVerse.Data;
using BookVerse.DataModels;
using BookVerse.Services.Core.Contracts;
using BookVerse.ViewModels;
using Microsoft.EntityFrameworkCore;
using static BookVerse.GCommon.ValidationConstants;

namespace BookVerse.Services.Core
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookIndexViewModel>> GetAllBooksIndexAsync(string? userId)
        {
            
            IEnumerable<BookIndexViewModel> model = await _context.Books.Where(b => b.IsDeleted == false)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .Select(b => new BookIndexViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    CoverImageUrl = b.CoverImageUrl,
                    Genre = b.Genre.Name,
                    SavedCount = b.UsersBooks.Count,
                    IsSaved = b.UsersBooks.Any(ub => ub.BookId == b.Id && ub.UserId == userId),
                    IsAuthor = userId != null && b.PublisherId == userId,
                }).ToListAsync();

            return model;
        }

        public async Task<BookCreateViewModel> GetBookCreateViewModelAsync()
        {
            var model = new BookCreateViewModel();

            var genres = await _context.Genres
                .Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                })
                .ToListAsync();

            model.Genres = genres;
            return model;
        }

        public async Task AddBookToDBAsync(BookCreateViewModel model, string userId)
        {
            if (!DateTime.TryParseExact(model.PublishedOn, PublishedOnFormat, null, System.Globalization.DateTimeStyles.None, out var publishedOnDate))
            {
                throw new InvalidOperationException("Invalid date format");
            }

            Book book = new Book()
            {
                Title = model.Title,
                Description = model.Description,
                CoverImageUrl = model.CoverImageUrl,
                PublishedOn = publishedOnDate,
                GenreId = model.GenreId,
                PublisherId = userId,
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MyBookViewModel>> GetAllMyBooksAsync(string userId)
        {
            IEnumerable<MyBookViewModel> model = await _context
               .UsersBooks
               .Where(ub => ub.UserId == userId)
               .Include(ub => ub.Book)
               .Select(ub => new MyBookViewModel
               {
                   Id = ub.BookId,
                   Title = ub.Book.Title,
                   Genre = ub.Book.Genre.Name,
                   CoverImageUrl = ub.Book.CoverImageUrl,
               })
               .ToListAsync();

            return model;
        }

        public async Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId)
        {
            var model = await _context.Books.Where(b => b.Id == id)
                    .Select(b => new BookDetailsViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Description = b.Description,
                        CoverImageUrl = b.CoverImageUrl,
                        Publisher = b.Publisher.UserName,
                        PublishedOn = b.PublishedOn,
                        Genre = b.Genre.Name,
                        IsAuthor = userId != null && b.PublisherId == userId,
                    }).FirstOrDefaultAsync();
                        
            return model;
        }

        public async Task<MyBookViewModel> GetForAddingToMyBooksByIdAsync(int id)
        {
            MyBookViewModel? myBook = await _context.Books.Where(b => b.Id == id)
                    .Select(b => new MyBookViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Genre = b.Genre.Name,
                        CoverImageUrl= b.CoverImageUrl,
                    }).FirstOrDefaultAsync();

            return myBook;
        }

        public async Task<bool> CheckBookForAlreadyTakenIdAsync(int id, string userId)
        {
            return await _context.UsersBooks.AnyAsync(ub => ub.UserId == userId && ub.BookId == id);
        }

        public async Task AddBookToMyDesiredBooksAsync(string userId, MyBookViewModel myDesiredBook)
        {
            bool isExisted = await _context.UsersBooks
                    .AnyAsync(ub => ub.UserId == userId && ub.BookId == myDesiredBook.Id);

            if (!isExisted)
            {
                UserBook userBook = new UserBook()
                {
                    UserId = userId,
                    BookId = myDesiredBook.Id,
                };

                await _context.UsersBooks.AddAsync(userBook);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveBookFromMyCollection(string userId, MyBookViewModel myBook)
        {
            var userBook = await _context.UsersBooks
                    .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == myBook.Id);

            if (userBook != null)
            {
                _context.UsersBooks.Remove(userBook!);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Book> FindBookToDeleteOrEditById(int id)
        {
            Book book = await _context.Books.FindAsync(id); 
            return book;
        }

        public async Task SoftDeleteBookFromDB(BookDeleteViewModel model)
        {
            Book? book = await _context.Books
                    .FindAsync(model.Id);

            if (book != null)
            {
                book.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BookEditViewModel> GetBookViewModelToEditAsync(int id)
        {
            var genres = await _context.Genres
                    .Select(g => new GenreViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                    })
                    .ToListAsync();

            var model = await _context.Books.Where(b => b.Id == id)
                .Select(b => new BookEditViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    CoverImageUrl = b.CoverImageUrl,
                    PublishedOn = b.PublishedOn.ToString("dd-MM-yyyy"),
                    GenreId = b.GenreId,
                    Genres = genres,
                    PublisherId = b.PublisherId,
                })
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task EditBookAsync(int id, BookEditViewModel model)
        {
            if (!DateTime.TryParseExact(model.PublishedOn, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None,
               out var timeOfPublishing))
            {
                throw new InvalidOperationException("Invalid date format");
            }

            var target = await _context.Books.FindAsync(id);

            var genres = await _context.Genres.ToListAsync();

            target!.Id = model.Id;
            target.Title = model.Title;
            target.Description = model.Description;
            target.CoverImageUrl = model.CoverImageUrl;
            target.PublishedOn = timeOfPublishing;
            target.GenreId = model.GenreId;
            target.Genre = genres.FirstOrDefault(g => g.Id == model.GenreId)!;
            target.PublisherId = model.PublisherId;

            await _context.Books.AddAsync(target);
            await _context.SaveChangesAsync();
        }



    }
}
