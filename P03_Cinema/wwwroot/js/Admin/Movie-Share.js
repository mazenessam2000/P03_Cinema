document.addEventListener("DOMContentLoaded", function () {

    console.log("Movie chart running...");

    const ctx = document.getElementById("moviesChart");

    if (!ctx) {
        console.log("moviesChart NOT FOUND");
        return;
    }

    new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ["Avengers", "Dune", "Joker", "Fast X", "Interstellar"],
            datasets: [{
                data: [410, 365, 298, 250, 190],
                backgroundColor: [
                    "#0d6efd",
                    "#198754",
                    "#dc3545",
                    "#ffc107",
                    "#0dcaf0"
                ]
            }]
        }
    });

});