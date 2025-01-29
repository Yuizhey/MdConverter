const markdownInput = document.getElementById('markdownText');
const fileNameInput = document.getElementById('documentName');
const saveButton = document.getElementById('saveButton');
let existingFileNames = [];


function validateSaveButton() {
    const markdownText = markdownInput.value.trim();
    const fileName = fileNameInput.value.trim();

    const isFileNameDuplicate = existingFileNames.includes(fileName.toLowerCase());

    if (markdownText && fileName && !isFileNameDuplicate) {
        saveButton.disabled = false;
    } else {
        saveButton.disabled = true;
    }
}


markdownInput.addEventListener('input', validateSaveButton);
fileNameInput.addEventListener('input', validateSaveButton);


document.getElementById('getDocumentsButton').addEventListener('click', async function () {
    try {
        const token = document.cookie.split('; ').find(row => row.startsWith('token'))?.split('=')[1];

        const response = await fetch('/Document/GetAllDocumentsByUserName', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const documents = await response.json();
            existingFileNames = documents.map(doc => doc.name.toLowerCase()); // Список существующих файлов
            validateSaveButton(); // Проверяем кнопку
        } else {
            alert('Ошибка при получении документов');
        }
    } catch (error) {
        console.error('Ошибка при запросе документов:', error);
        alert('Произошла ошибка при загрузке документов.');
    }
});


saveButton.addEventListener('click', async function () {
    const markdownText = markdownInput.value.trim();
    const fileName = fileNameInput.value.trim();

    if (!markdownText || !fileName || existingFileNames.includes(fileName.toLowerCase())) {
        alert("Ошибка: проверьте введенные данные.");
        return;
    }

    try {
        const token = document.cookie.split('; ').find(row => row.startsWith('token'))?.split('=')[1];

        const response = await fetch('/Document/CreateDocument', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify({
                Name: fileName,
                MarkdownText: markdownText
            })
        });

        if (response.ok) {
            alert('Документ успешно сохранен!');
            existingFileNames.push(fileName.toLowerCase()); // Добавляем в список существующих
            validateSaveButton(); // Перепроверяем кнопку
        } else {
            alert('Ошибка при сохранении документа');
        }
    } catch (error) {
        console.error('Ошибка при сохранении документа:', error);
        alert('Произошла ошибка при сохранении документа.');
    }
});


document.getElementById('logoutButton').addEventListener('click', async function () {
    try {
        const response = await fetch('/Account/Logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        if (response.ok) {
            document.cookie = 'token=; Max-Age=0; path=/';
            document.getElementById('logoutButton').disabled = true;
            location.href = '/index.html';
        } else {
            alert('Ошибка при выходе');
        }
    } catch (error) {
        console.error('Ошибка сети или сервера:', error);
        alert('Произошла ошибка при выходе.');
    }
});


async function loadDocumentContent(documentName) {
    try {
        const token = document.cookie.split('; ').find(row => row.startsWith('token'))?.split('=')[1];

        const response = await fetch(`/Document/GetDocumentContent?documentName=${documentName}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const { content } = await response.json();
            document.getElementById('markdownText').value = content;
            document.getElementById('documentName').value = documentName;
        } else {
            // Если документ не найден, очищаем поля
            document.getElementById('markdownText').value = "";
            document.getElementById('documentName').value = "";
            alert('Документ не найден или был удален.');
        }
    } catch (error) {
        console.error('Ошибка при загрузке содержимого документа:', error);
        document.getElementById('markdownText').value = "";
        document.getElementById('documentName').value = "";
        alert('Произошла ошибка при загрузке содержимого документа.');
    }
}



async function deleteDocument(documentId) {
    try {
        const token = document.cookie.split('; ').find(row => row.startsWith('token'))?.split('=')[1];

        const response = await fetch(`/Document/DeleteDocument?documentId=${documentId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            alert('Документ успешно удален');
            document.getElementById('getDocumentsButton').click(); // Обновляем список документов

            // Если больше нет документов, очищаем поля
            if (document.getElementById('documentsList').children.length === 0) {
                document.getElementById('markdownText').value = "";
                document.getElementById('documentName').value = "";
            }
        } else {
            alert('Ошибка при удалении документа');
        }
    } catch (error) {
        console.error('Ошибка при удалении документа:', error);
        alert('Произошла ошибка при удалении документа.');
    }
}



document.getElementById('getDocumentsButton').addEventListener('click', async function () {
    try {
        const token = document.cookie.split('; ').find(row => row.startsWith('token'))?.split('=')[1];

        const response = await fetch('/Document/GetAllDocumentsByUserName', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const documents = await response.json();
            const list = document.getElementById('documentsList');
            list.innerHTML = ''; // Очищаем текущий список

            existingFileNames = documents.map(doc => doc.name.toLowerCase());
            validateSaveButton(); // Проверяем кнопку

            documents.forEach(doc => {
                const listItem = document.createElement('li');
                listItem.textContent = doc.name;

                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Удалить';
                deleteButton.className = 'delete-button';
                deleteButton.addEventListener('click', async () => {
                    if (confirm(`Вы уверены, что хотите удалить документ "${doc.name}"?`)) {
                        await deleteDocument(doc.id);
                    }
                });

                listItem.addEventListener('click', async () => {
                    await loadDocumentContent(doc.name);
                });

                listItem.appendChild(deleteButton);
                list.appendChild(listItem);
            });
        } else {
            alert('Ошибка при получении документов');
        }
    } catch (error) {
        console.error('Ошибка при запросе документов:', error);
        alert('Произошла ошибка при загрузке документов.');
    }
});


document.getElementById('downloadButton').addEventListener('click', function () {
    const htmlContent = document.getElementById('renderedHtml').value;
    const documentName = fileNameInput.value || "renderedMarkdown";

    const blob = new Blob([htmlContent], { type: 'text/html' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${documentName}.html`;
    link.click();
});


document.getElementById('openInNewWindowButton').addEventListener('click', function () {
    const htmlContent = document.getElementById('renderedHtml').value;

    const newWindow = window.open('', '_blank');
    newWindow.document.write(`
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Отрендеренный Markdown</title>
        </head>
        <body>
            ${htmlContent}
        </body>
        </html>
    `);
    newWindow.document.close();
});
