using System.ComponentModel.DataAnnotations;

namespace Ex1.Common.Requests.Author
{
    public class UpdateAuthorRequest
    {
        [Required(ErrorMessage = "Mã tác giả không được để trống")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Tên tác giả không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Tiểu sử không được vượt quá 500 ký tự.")]
        public string? Bio { get; set; }
    }
}
