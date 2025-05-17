    // script.js

    document.addEventListener('DOMContentLoaded', () => {
        // Chọn tất cả các tiêu đề có nút toggle
        const headers = document.querySelectorAll('.item-header');

        headers.forEach(header => {
            header.addEventListener('click', () => {
                // Toggle class 'active' trên header
                header.classList.toggle('active');

                // Chọn phần nội dung tương ứng (ngay sau header)
                const content = header.nextElementSibling;

                // Điều chỉnh style.maxHeight để tạo hiệu ứng expand/collapse
                if (header.classList.contains('active')) {
                    // Mở rộng nội dung: Đặt max-height bằng chiều cao thực của nội dung
                    // Cộng thêm một chút để đảm bảo hiển thị hết và có padding
                    content.style.maxHeight = content.scrollHeight + 30 + "px"; // scrollHeight là chiều cao nội dung
                } else {
                    // Thu gọn nội dung: Đặt max-height về 0
                    content.style.maxHeight = "0";
                }

                // Cập nhật ký hiệu trên nút toggle (+/- hoặc +/rotated +)
                const toggleBtn = header.querySelector('.toggle-btn');
                if (header.classList.contains('active')) {
                    // Ký hiệu có thể được điều khiển bằng CSS rotate như trong style.css
                    // Hoặc bạn có thể đổi text content: toggleBtn.textContent = '-';
                } else {
                    // toggleBtn.textContent = '+';
                }
            });
        });

        // Prism.js sẽ tự động highlight các block code khi trang load.
        // Không cần gọi hàm đặc biệt ở đây trừ khi bạn thêm code động.
    });


function runOp(type) {
    const countInput = document.getElementById("count");
    const resultPre = document.getElementById("result");
    const count = parseInt(countInput.value);

    const chartContainerId = `chart-${type}`;
    let chartContainer = document.getElementById(chartContainerId); // Khai báo chartContainer ở ngoài

    // Ẩn tất cả các canvas khác và hủy biểu đồ cũ trên chúng
    document.querySelectorAll('.item-content canvas').forEach(canvas => {
        // Chỉ xử lý các canvas khác không phải canvas hiện tại
        if (canvas.id !== chartContainerId) {
             if (canvas.style.display !== 'none') {
                 canvas.style.display = 'none'; // Ẩn canvas
             }
             // Hủy chart cũ trên các canvas ẩn đi
             const existingChartToDestroy = Chart.getChart(canvas);
             if (existingChartToDestroy) {
                 existingChartToDestroy.destroy(); // Hủy chart cũ để giải phóng bộ nhớ
                 console.log(`Destroyed old chart on canvas: ${canvas.id}`);
             }
        }
    });


    if (isNaN(count) || count <= 0) {
        resultPre.innerText = "Lỗi: Vui lòng nhập số phần tử hợp lệ (> 0).";
        resultPre.style.color = "red";
        // Đảm bảo ẩn chart nếu input không hợp lệ
        if (chartContainer) {
            chartContainer.style.display = 'none';
        }
        return;
    }

    // Xóa nội dung cũ và hiển thị trạng thái tải
    resultPre.innerText = "Đang xử lý...";
    resultPre.style.color = "#000000";
    // Ẩn chart cũ khi đang tải cái mới (nếu nó đã hiển thị)
     if (chartContainer && chartContainer.style.display !== 'none') {
        chartContainer.style.display = 'none';
    }


    fetch(`http://localhost:5106/api/Simulate/${type}/${count}`, {
        method: 'POST'
    })
    .then(res => {
        if (!res.ok) {
            return res.text().then(text => { throw new Error(`HTTP error! status: ${res.status}, message: ${text}`); });
        }
        return res.json();
    })
    .then(data => {
        console.log("Kết quả API:", data);

        // >>>>>> Đảm bảo chỉ xóa text loading ở đây <<<<<<
        resultPre.innerText = ''; // Xóa text "Đang xử lý..."
        resultPre.style.color = "#000"; // Reset màu (nếu cần)
         // >>>>>> Kết thúc xóa text <<<<<<


        // Lấy lại hoặc tạo mới canvas
        chartContainer = document.getElementById(chartContainerId);

        if (!chartContainer) {
            console.log("Canvas container chưa tồn tại, đang tạo mới...");
            chartContainer = document.createElement('canvas');
            chartContainer.id = chartContainerId;
            // >>>>>> Set kích thước cho canvas <<<<<<
            chartContainer.width = 600; // Ví dụ: Chiều rộng
            chartContainer.height = 600; // Ví dụ: Chiều cao
            // >>>>>> Thêm border cho canvas để dễ debug <<<<<<
            chartContainer.style.border = '2px solid blue'; // Border màu xanh đậm

            // Chèn canvas vào sau resultPre
            resultPre.parentNode.insertBefore(chartContainer, resultPre.nextSibling);
             console.log("Canvas container vừa được tạo và chèn.");


        } else {
             console.log("Canvas container đã tồn tại, đang cập nhật hoặc vẽ lại...");
             // Nếu đã tồn tại, đảm bảo nó hiển thị
             chartContainer.style.display = 'block'; // Hiển thị lại canvas

             // Hủy biểu đồ cũ trên canvas nếu nó đã tồn tại
             const existingChart = Chart.getChart(chartContainer);
             if (existingChart) {
                 existingChart.destroy();
                 console.log(`Destroyed existing chart on canvas: ${chartContainerId}`);
             }
        }

        // Đảm bảo canvas có context 2D hợp lệ trước khi vẽ
        const ctx = chartContainer.getContext('2d');
        if (!ctx) {
             console.error("Không lấy được context 2D từ canvas!");
             resultPre.innerText = "Lỗi hiển thị biểu đồ: Không lấy được context canvas.";
             resultPre.style.color = "red";
             return; // Dừng xử lý nếu không có context
        }


        // Đảm bảo parse số từ chuỗi "X.YY ms"
        const listTime = parseFloat(data.listTime.replace(" ms", ""));
        const arrayListTime = parseFloat(data.arrayListTime.replace(" ms", ""));

        console.log("listTime (number):", listTime);
        console.log("arrayListTime (number):", arrayListTime);


        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['List<T>', 'ArrayList'],
                datasets: [{
                    label: `Thời gian thực hiện (${data.operation})`,
                    data: [listTime, arrayListTime],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.6)', // Màu xanh dương
                        'rgba(255, 99, 132, 0.6)' // Màu đỏ
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: false, // False để kiểm soát kích thước bằng width/height attributes
                maintainAspectRatio: false, // False để kiểm soát tỷ lệ khung hình bằng width/height attributes
                 scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Thời gian (ms)'
                        }
                    }
                },
                plugins: {
                    title: {
                        display: true,
                        text: `So sánh thời gian ${data.operation} (${count} phần tử})`
                    },
                     tooltip: {
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += context.parsed.y.toFixed(2) + ' ms';
                                }
                                return label;
                            }
                        }
                    }
                }
            }
        });

    })
    .catch(error => {
        console.error("Lỗi gọi API:", error);
        resultPre.innerText = "Lỗi gọi API: " + error.message;
        resultPre.style.color = "red";

        // Nếu có lỗi, ẩn chart đi và hủy nó (nếu đã tồn tại)
        if (chartContainer) {
             chartContainer.style.display = 'none';
             const existingChartOnError = Chart.getChart(chartContainer);
             if (existingChartOnError) {
                 existingChartOnError.destroy();
             }
         }
    });
}