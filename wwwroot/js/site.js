document.addEventListener("DOMContentLoaded", () => {
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('success') === 'True') {
        Swal.fire({
            title: '✅ Changes Saved!',
            text: 'The new content has been successfully updated.',
            icon: 'success',
            confirmButtonColor: '#795548',
            confirmButtonText: 'OK'
        }).then(() => {
            // Remove the query param from URL after showing alert
            window.history.replaceState({}, document.title, window.location.pathname);
        });
    } else if (urlParams.get('error') === 'True') {
        Swal.fire({
            title: '⚠️ Error!',
            text: 'There was a problem saving the data. Please try again.',
            icon: 'error',
            confirmButtonColor: '#795548'
        });
    }

    // simple stagger for fade-in blocks
    document.querySelectorAll(".fade-in").forEach((el, idx) => {
        el.style.animationDelay = `${0.12 * idx}s`;
    });
});