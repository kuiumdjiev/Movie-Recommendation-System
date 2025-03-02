async function getNames() {
    const response = await fetch('https://localhost:7097/api/movie/all');
    const names = await response.json();
    return names;
}
result = [];
getNames().then(x => {
    result = [];
    
    result = x.map(item => ({ id: item.id, name: item.name }));
}).catch(error => {
    console.error(error);
});
   

let search = document.getElementsByName('name')[1]; // Assuming there's only one input with name 'query'
let resultContainer = document.getElementById('result-container');

// Add event listener to update the search input dynamically
search.addEventListener('input', function () {
    let query = search.value.toLowerCase();
    let filteredResults = result.filter(pair => pair.name.toLowerCase().includes(query)).slice(0, 10);
    displayResults(filteredResults);
});

function displayResults(filteredResults) {
    resultContainer.innerHTML = ''; // Clear previous results
    if (filteredResults.length > 0) {
        filteredResults.forEach(object => {
            let resultItem = document.createElement('div');
            resultItem.classList.add('result-item', 'list-group-item', 'list-group-item-action');
            resultItem.textContent = object.name;
            resultItem.addEventListener('click', function () {
                search.value = object.name;
                resultContainer.style.display = 'none';
            });
            resultContainer.appendChild(resultItem);
        });
        resultContainer.style.display = 'block';
    } else {
        resultContainer.style.display = 'none';
    }
}

// Hide the result container when clicking outside
document.addEventListener('click', function (event) {
    if (!search.contains(event.target) && !resultContainer.contains(event.target)) {
        resultContainer.style.display = 'none';
    }
    if (!searchbar.contains(event.target) && !resultbarContainer.contains(event.target)) {
        resultbarContainer.style.display = 'none';
    }
});

// Add some basic styles for the result container and items
let style_down = document.createElement('style');
style_down.innerHTML = `
    .result-container {
        border: 1px solid #ccc;
        max-height: 200px;
        overflow-y: auto;
        position: absolute;
        background-color: white;
        width: 100%;
        z-index: 1000;
        display: none;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }
    .result-item {
        padding: 8px;
        cursor: pointer;
    }
    .result-item:hover {
        background-color: #f0f0f0;
    }
`;
document.head.appendChild(style_down);
