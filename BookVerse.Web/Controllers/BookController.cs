using BookVerse.DataModels;
using BookVerse.Services.Core.Contracts;
using BookVerse.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookVerse.Web.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = GetUserId();

            var model = await _bookService.GetAllBooksIndexAsync(userId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _bookService.GetBookCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userId = GetUserId();

            await _bookService.AddBookToDBAsync(viewModel, userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks()
        {
            string userId = GetUserId();

            var model = await _bookService.GetAllMyBooksAsync(userId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();

            var model = await _bookService.GetBookDetailsAsync(id, userId);
            return View(model);
        }

        public async Task<IActionResult> AddToMyBooks(int id) 
        {
            string userId = GetUserId();

            if (await _bookService.CheckBookForAlreadyTakenIdAsync(id, userId) == false)
            {
                MyBookViewModel? myDesiredBook = await _bookService.GetForAddingToMyBooksByIdAsync(id);

                if (myDesiredBook == null)
                {
                    return RedirectToAction("Details", new { id });
                }

                await _bookService.AddBookToMyDesiredBooksAsync(userId, myDesiredBook);
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Remove(int id)
        {
            string userId = GetUserId();

            MyBookViewModel? book = await _bookService.GetForAddingToMyBooksByIdAsync(id);

            if (book == null)
            {
                return RedirectToAction("MyBooks");
            }

            await _bookService.RemoveBookFromMyCollection(userId, book);
            return RedirectToAction("MyBooks");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetUserId();

            Book book = await _bookService.FindBookToDeleteOrEditById(id);

            if (book == null || book.PublisherId != userId)
            {
                return RedirectToAction("Details");
            }

            BookDeleteViewModel deleteModel = new BookDeleteViewModel();

            deleteModel.Id = book.Id;
            deleteModel.Title = book.Title; 
            deleteModel.Publisher = book.Publisher.UserName;           

            return View(deleteModel);
        }

        public async Task<IActionResult> ConfirmDelete(BookDeleteViewModel model)
        {
            await _bookService.SoftDeleteBookFromDB(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            BookEditViewModel model = await _bookService.GetBookViewModelToEditAsync(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            string? userId = GetUserId();

            if (model.PublisherId != userId)
            {
                return RedirectToAction("Index");
            }
           
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Book book = await _bookService.FindBookToDeleteOrEditById(model.Id);

            if (book == null)
            {
                return RedirectToAction("Index");
            }

            var userId = GetUserId();

            if (model.PublisherId != userId)
            {
                return RedirectToAction("Index");
            }

            await _bookService.EditBookAsync(model);
            return RedirectToAction("Index");
        }
















    }
}
