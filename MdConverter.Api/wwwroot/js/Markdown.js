document.getElementById('markdownForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    
    const markdownText = document.getElementById('markdownText').value;

    try {
        const response = await fetch('/MarkDown/Render', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ markdownText })
        });
        
        if (response.ok) {
            const data = await response.json();
            
            document.getElementById('renderedHtml').textContent = data.html;
        } else {
            const errorMessage = await response.text();
            alert(`Ошибка: ${errorMessage}`);
        }
    } catch (error) {
        console.error('Ошибка сети или сервера:', error);
        alert('Произошла ошибка при отправке запроса.');
    }
});
