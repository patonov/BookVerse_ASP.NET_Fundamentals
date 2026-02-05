using BookVerse.DataModels;
using BookVerse.ViewModels;

namespace BookVerse.Services.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookIndexViewModel>> GetAllBooksIndexAsync(string? userId);

        Task<BookCreateViewModel> GetBookCreateViewModelAsync();
        Task AddBookToDBAsync(BookCreateViewModel model, string userId);

        Task<IEnumerable<MyBookViewModel>> GetAllMyBooksAsync(string userId);

        Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId);

        Task<bool> CheckBookForAlreadyTakenIdAsync(int id, string userId);

        Task<MyBookViewModel> GetForAddingToMyBooksByIdAsync(int id);
        Task AddBookToMyDesiredBooksAsync(string userId, MyBookViewModel myDesiredBook);

        Task RemoveBookFromMyCollection(string userId, MyBookViewModel myBook);

        Task<Book> FindBookToDeleteOrEditById(int id);

        Task SoftDeleteBookFromDB(BookDeleteViewModel model);

        Task<BookEditViewModel> GetBookViewModelToEditAsync(int id);
        Task EditBookAsync(BookEditViewModel model);
    }
}
