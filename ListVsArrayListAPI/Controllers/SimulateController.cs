using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic; // Cần import List<T>
using System.Diagnostics; // Cần import Stopwatch

namespace ListVsArrayListAPI.Controllers
{
    [Route("api/[controller]")] // Base route cho Controller này là /api/Simulate
    [ApiController] // Thuộc tính này giúp Controller xử lý API tốt hơn (validation, binding...)
    public class SimulateController : ControllerBase
    {
        [HttpPost("add/{count:int}")] // Endpoint POST: /api/Simulate/add/{count}
        public IActionResult Add(int count)
        {
            // Sử dụng Stopwatch để đo thời gian
            var swList = Stopwatch.StartNew();
            List<string> list = new();
            for (int i = 0; i < count; i++)
            {
                list.Add("value" + i);
            }
            swList.Stop(); // Dừng đo thời gian cho List

            var swArrayList = Stopwatch.StartNew();
            ArrayList arrayList = new();
            for (int i = 0; i < count; i++)
            {
                arrayList.Add("value" + i);
            }
            swArrayList.Stop(); // Dừng đo thời gian cho ArrayList

            // Trả về kết quả dạng JSON
            return Ok(new
            {
                Operation = "Add",
                ListTime = swList.Elapsed.TotalMilliseconds.ToString("F2") + " ms", // Dùng TotalMilliseconds cho độ chính xác cao hơn và format 2 số thập phân
                ArrayListTime = swArrayList.Elapsed.TotalMilliseconds.ToString("F2") + " ms"
            });
        }

        [HttpPost("insert/{count:int}")] // Endpoint POST: /api/Simulate/insert/{count}
        public IActionResult Insert(int count)
        {
             // Đảm bảo count đủ lớn để có vị trí mid hợp lệ
            if (count <= 1)
            {
                 // Xử lý trường hợp count quá nhỏ, không thể chèn giữa
                 // Có thể trả về BadRequest hoặc một thông báo lỗi phù hợp
                 return BadRequest("Count must be greater than 1 for Insert operation.");
            }

            int mid = count / 2;

            // Khởi tạo List và ArrayList với dữ liệu ban đầu
            List<string> list = new();
            ArrayList arrayList = new();
            for (int i = 0; i < count; i++)
            {
                list.Add("initial_value");
                arrayList.Add("initial_value");
            }

            var swList = Stopwatch.StartNew();
            list.Insert(mid, "inserted_value"); // Chèn vào giữa
            swList.Stop();

            var swArrayList = Stopwatch.StartNew();
            arrayList.Insert(mid, "inserted_value"); // Chèn vào giữa
            swArrayList.Stop();

             // Trả về kết quả dạng JSON
            return Ok(new
            {
                Operation = $"Insert at index {mid}",
                ListTime = swList.Elapsed.TotalMilliseconds.ToString("F2") + " ms",
                ArrayListTime = swArrayList.Elapsed.TotalMilliseconds.ToString("F2") + " ms"
            });
        }

        [HttpPost("get/{count:int}")] // Endpoint POST: /api/Simulate/get/{count}
        public IActionResult Get(int count)
        {
            // Đảm bảo count đủ lớn để có vị trí mid hợp lệ
            if (count <= 0)
            {
                 return BadRequest("Count must be greater than 0 for Get operation.");
            }

            int mid = count / 2; // Vị trí giữa

            // Khởi tạo List và ArrayList với dữ liệu ban đầu
            List<string> list = new();
            ArrayList arrayList = new();
            for (int i = 0; i < count; i++)
            {
                list.Add("initial_value");
                arrayList.Add("initial_value");
            }
            // Đảm bảo vị trí mid có tồn tại sau khi thêm
             if (mid >= count) mid = count -1;
             if (mid < 0) mid = 0; // Đảm bảo mid hợp lệ ngay cả khi count = 1

            var swList = Stopwatch.StartNew();
            var valueList = list[mid]; // Lấy phần tử
            swList.Stop();

            var swArrayList = Stopwatch.StartNew();
            var valueArrayList = arrayList[mid]; // Lấy phần tử
            swArrayList.Stop();

             // Trả về kết quả dạng JSON
            return Ok(new
            {
                Operation = $"Get element at index {mid}",
                ListTime = swList.Elapsed.TotalMilliseconds.ToString("F2") + " ms",
                ArrayListTime = swArrayList.Elapsed.TotalMilliseconds.ToString("F2") + " ms"
            });
        }

        [HttpPost("remove/{count:int}")] // Endpoint POST: /api/Simulate/remove/{count}
        public IActionResult Remove(int count)
        {
             // Đảm bảo count đủ lớn để có phần tử để xóa
            if (count <= 0)
            {
                 return BadRequest("Count must be greater than 0 for Remove operation.");
            }

            // Khởi tạo List và ArrayList với dữ liệu ban đầu
            List<string> list = new();
            ArrayList arrayList = new();
            for (int i = 0; i < count; i++)
            {
                list.Add("initial_value");
                arrayList.Add("initial_value");
            }

            var swList = Stopwatch.StartNew();
            list.RemoveAt(count - 1); // Xóa phần tử cuối cùng
            swList.Stop();

            var swArrayList = Stopwatch.StartNew();
            arrayList.RemoveAt(count - 1); // Xóa phần tử cuối cùng
            swArrayList.Stop();

            // Trả về kết quả dạng JSON
            return Ok(new
            {
                Operation = $"Remove last element (index {count - 1})",
                ListTime = swList.Elapsed.TotalMilliseconds.ToString("F2") + " ms",
                ArrayListTime = swArrayList.Elapsed.TotalMilliseconds.ToString("F2") + " ms"
            });
        }
    }
}