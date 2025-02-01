function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

function updateButtonStates() {
    const token = getCookie('token');
    document.getElementById('logoutButton').disabled = !token;
    document.getElementById('loginButton').disabled = !!token;
    document.getElementById('registerButton').disabled = !!token;
    document.getElementById('getDocumentsButton').disabled = !token;
    document.getElementById('saveButton').disabled = !token;
}

document.addEventListener('DOMContentLoaded', updateButtonStates);