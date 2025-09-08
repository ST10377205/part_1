// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// DOMContentLoaded to load javascript when html is loaded to browser
document.addEventListener("DOMContentLoaded", (event) => {

    // function to toggle password visibility
    function togglePasswordVisibility(targetId) {
        const input = document.getElementById(targetId);
        const toggle = document.querySelector(`[data-target="${targetId}"]`);
        const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
        input.setAttribute('type', type);
        toggle.classList.toggle('bi-eye');
        toggle.classList.toggle('bi-eye-slash');
    }

    // event listener to the togglePasswordVisibility function
    document.querySelectorAll('.password-toggle').forEach(toggle => {
        toggle.addEventListener('click', () => {
            const targetId = toggle.getAttribute('data-target');
            togglePasswordVisibility(targetId);
        });
    });

    // function to calculate total amount
    function total_calculate() {
        const sessions = parseFloat(document.getElementById('Sessions').value) || 0;
        const hours = parseFloat(document.getElementById('Hours').value) || 0;
        const rate = parseFloat(document.getElementById('Rate').value) || 0;
        const total = sessions * hours * rate;
        document.getElementById('price').value = isNaN(total) ? '' : total.toFixed(2);
    }
});
