using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace DocumentApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentItemsController : ControllerBase
    {
        private readonly DocumentContext _context;

        public DocumentItemsController(DocumentContext context)
        {
            _context = context;
        }

        // GET: api/DocumentItems
        [HttpGet]
        [RequiredScope("documents.read")]
        public async Task<ActionResult<IEnumerable<DocumentItem>>> GetDocumentItems()
        {
            var objectIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var userId = User.FindFirst(objectIdClaim)?.Value.ToString();
            if (_context.DocumentItems == null)
            {
                return NotFound();
            }
            if (userId == null)
            {
                return NotFound();
            }
            return await _context.DocumentItems.Where(x => x.UserId == userId).ToListAsync();
        }

        // GET: api/DocumentItems/5
        [HttpGet("{id}")]
        [RequiredScope("documents.read")]
        public async Task<ActionResult<DocumentItem>> GetDocumentItem(long id)
        {
            var objectIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var userId = User.FindFirst(objectIdClaim)?.Value.ToString();

            if (_context.DocumentItems == null)
            {
                return NotFound();
            }
            if (userId == null)
            {
                return NotFound();
            }
            var documentItem = await _context.DocumentItems.FindAsync(id);
            if (documentItem == null)
            {
                return NotFound();
            }

            if (documentItem.UserId == userId)
            {
                return documentItem;
            }
            else
            {
                return NotFound();
            }
        }

        // PUT: api/DocumentItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [RequiredScope("documents.write")]
        public async Task<IActionResult> UpdateDocumentItem(long id, DocumentItem documentItem)
        {
            if (id != documentItem.Id)
            {
                return BadRequest();
            }

            var documentItemToUpdate = await _context.DocumentItems.FindAsync(id);
            if (documentItemToUpdate == null)
            {
                return NotFound();
            }

            documentItemToUpdate.DocumentType = documentItem.DocumentType;
            documentItemToUpdate.DocumentDate = documentItem.DocumentDate;
            documentItemToUpdate.UpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DocumentItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/DocumentItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [RequiredScope("documents.write")]
        public async Task<ActionResult<DocumentItem>> CreateDocumentItem(DocumentItem documentItem)
        {
            var objectIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var documentItemToCreate = new DocumentItem
            {
                UserId = string.IsNullOrEmpty(User.FindFirst(objectIdClaim)?.Value.ToString())
                    ? ""
                    : User.FindFirst(objectIdClaim)!.Value.ToString(),
                DocumentDate = documentItem.DocumentDate,
                UpdatedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                DocumentType = documentItem.DocumentType,
                Notes = documentItem.Notes,
            };

            _context.DocumentItems.Add(documentItemToCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDocumentItem),
                new { id = documentItemToCreate.Id },
                documentItemToCreate
            );
        }

        // DELETE: api/DocumentItems/5
        [HttpDelete("{id}")]
        [RequiredScope("documents.write")]
        public async Task<IActionResult> DeleteDocumentItem(long id)
        {
            if (_context.DocumentItems == null)
            {
                return NotFound();
            }
            var documentItem = await _context.DocumentItems.FindAsync(id);
            if (documentItem == null)
            {
                return NotFound();
            }

            _context.DocumentItems.Remove(documentItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentItemExists(long id)
        {
            return (_context.DocumentItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
