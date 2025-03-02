async function getNames() {
    const response = await fetch('https://localhost:7097/api/movie/all');
    const names = await response.json();
    return names;
}

let result = [];
getNames().then(x => {
    result = x.map(item => ({ id: item.id, name: item.name }));
}).catch(error => {
    console.error(error);
});

let searchbar = document.getElementsByName('name')[0]; // Assuming there's only one input with name 'name'
let resultbarContainer = document.getElementById('resultbar-container');

// Add event listener to update the search input dynamically
searchbar.addEventListener('input', function () {
    let query = searchbar.value.toLowerCase();
    let filteredResults = result.filter(pair => pair.name.toLowerCase().includes(query)).slice(0, 10);
    displaySeacrchBarResults(filteredResults);
});

function displaySeacrchBarResults(filteredResults) {
    resultbarContainer.innerHTML = ''; // Clear previous results
    if (filteredResults.length > 0) {
        filteredResults.forEach(object => {
            let resultItem = document.createElement('div');
            resultItem.classList.add('result-item', 'list-group-item', 'list-group-item-action');
            resultItem.textContent = object.name;
            resultItem.addEventListener('click', function () {
                searchbar.value = object.name;
                resultbarContainer.style.display = 'none';
            });
            resultbarContainer.appendChild(resultItem);
        });
        resultbarContainer.style.display = 'block';
    } else {
        resultbarContainer.style.display = 'none';
    }
}

// Hide the result container when clicking outside
document.addEventListener('click', function (event) {
    if (!searchbar.contains(event.target) && !resultbarContainer.contains(event.target)) {
        resultbarContainer.style.display = 'none';
    }
});

// Add some basic styles for the result container and items
const style = document.createElement('style');
style.innerHTML = `
    .resultbar-container {
        border: 1px solid #ccc;
        max-height: 200px;
        overflow-y: auto;
        position: absolute;
        background-color: white;
        width: calc(100% - 70%);
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
document.head.appendChild(style);
