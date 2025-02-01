// Конвертировать с использованием Markdig
document.getElementById('convertMarkdigButton').addEventListener('click', async function (event) {
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

// Конвертировать с использованием вашего Markdown парсера
document.getElementById('convertMyMarkdownButton').addEventListener('click', async function (event) {
    event.preventDefault();

    const markdownText = document.getElementById('markdownText').value;

    try {
        const response = await fetch('/MyMarkDown/Render', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ markdownText })
        });

        if (response.ok) {
            const data = await response.json();
            document.getElementById('renderedHtml').textContent = data.html;
        } else {
            document.getElementById('renderedHtml').textContent = document.getElementById('markdownText').value;
        }
    } catch (error) {
        console.error('Ошибка сети или сервера:', error);
        alert('Произошла ошибка при отправке запроса.');
    }
});
