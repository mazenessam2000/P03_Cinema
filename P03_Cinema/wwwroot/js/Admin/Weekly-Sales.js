document.addEventListener("DOMContentLoaded", function () {

    console.log("Weekly chart running...");

    const ctx = document.getElementById("salesChart");

    if (!ctx) {
        console.log("salesChart NOT FOUND");
        return;
    }

    new Chart(ctx, {
        type: "line",
        data: {
            labels: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            datasets: [{
                label: "Tickets Sold",
                data: [320, 450, 510, 620, 900, 1200, 1400],
                borderColor: "#0d6efd",
                backgroundColor: "rgba(13,110,253,0.2)",
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }
            }
        }
    });

});