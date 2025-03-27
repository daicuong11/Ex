using System.ComponentModel.DataAnnotations;

namespace Ex1.Common.Requests.Book
{
    public class CreateBookRequest
    {
        [Required(ErrorMessage = "Tiêu đề sách không được để trống.")]
        [StringLength(255, ErrorMessage = "Tiêu đề sách không được vượt quá 255 ký tự.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Giá sách không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sách phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tác giả.")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã tác giả không hợp lệ.")]
        public int? AuthorId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày xuất bản không hợp lệ.")]
        public DateTime? PublishedDate { get; set; }
    }
}
