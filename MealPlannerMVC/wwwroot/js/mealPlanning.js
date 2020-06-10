

document.addEventListener("DOMContentLoaded", () => {

    let searchRecipeButton = document.querySelector(".btn_search_recipename");
    searchRecipeButton.addEventListener("click", searchRecipeName);

    //let searchIngredientButton = document.querySelector(".btn_search_ingredient");
    //searchIngredientButton.addEventListener("click", searchIngredientName);

    let searchPricesButton = document.querySelector(".search_prices_button");
    searchPricesButton.addEventListener("click", searchPricesForIngredients);
        
});
window.onload = () => {
    getShoppingList();
    getPlannedRecipes();
};

function getShoppingList() {
    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                console.log("ingredients");
                console.log(xhr.responseText);
                let ingredients = JSON.parse(xhr.response);
                DisplayShoppingList(ingredients);

            }
        }
    }
    xhr.open('GET', 'GetShoppingList', true);
    xhr.send();
}

function getPlannedRecipes() {
    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                               
                let recipes = JSON.parse(xhr.response);
                DisplayPlannedRecipes(recipes);

            }
        }
    }
    xhr.open('GET', 'GetPlannedRecipes', true);
    xhr.send();
}



function searchRecipeName() {

    recipeName = document.querySelector('.search__recipename').value;
    
    let data = new FormData();
    data.append('recipeName', recipeName);

    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                
                console.log("result");
                console.log(this.responseText);
                let recipes = JSON.parse(xhr.response);
                CreateRecipeList(recipes);

            }
        }
    }
    xhr.open('POST', 'SearchRecipeName', true);
    xhr.send(data);
}

function searchIngredientName() {

}

function CreateRecipeList(recipes) {
    

    let element = document.querySelector('.Recipe_list_items');
    while (element.firstChild) {
        element.removeChild(element.lastChild);
    }
        
    for (var i = 0; i < recipes.length; i++) {
        let recipeId = recipes[i].recipeID;
        
        let RecipeListItem = document.createElement('div');
        RecipeListItem.setAttribute('class', 'recipe_list_item');
        RecipeListItem.setAttribute('id', `recipe_item-${recipeId}`);

        let listElementRecipename = document.createElement('li');
        listElementRecipename.innerHTML = recipes[i].recipeName;

        let btnAdd = document.createElement('button');
        btnAdd.setAttribute('id', `button_add-${recipeId}`);
        btnAdd.innerHTML = 'Add to plan';
        btnAdd.addEventListener("click", () => {
            AddToShoppingList(recipeId);
        });

        listElementRecipename.appendChild(btnAdd);

        let listElementIngredient = document.createElement('ul');
        listElementIngredient.setAttribute('class', 'list_of_ingredients');
       
        for (var j = 0; j < recipes[i].ingredients.length; j++) {
            let listItems = document.createElement('li');
            listItems.setAttribute('class','inline_ingredients')
            listItems.innerHTML = recipes[i].ingredients[j].ingredientName;
            listElementIngredient.appendChild(listItems);

        }
        
        RecipeListItem.appendChild(listElementRecipename);
        RecipeListItem.appendChild(listElementIngredient);
        element.appendChild(RecipeListItem);
    }


}

function AddToShoppingList(recipeId) {
    console.log(recipeId);

    let data = new FormData();
    data.append('recipeID', recipeId);

    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                getShoppingList();
                getPlannedRecipes();

            }
        }
    }
    xhr.open('POST', 'AddToPlannedMeals', true);
    xhr.send(data);


}

function DisplayShoppingList(ingredients) {
    let element = document.querySelector('.shopping_list_items');

    while (element.firstChild) {
        element.removeChild(element.lastChild);
    }

    
    for (var i = 0; i < ingredients.length; i++) {
        let ingredientId = ingredients[i].ingredientID;
        let listItem = document.createElement('li');
        listItem.setAttribute('id', `shopping_list_item-${ingredientId}`);
        let delButton = document.createElement('img');
        delButton.setAttribute('id', `del_list_item-${ingredientId}`);
        delButton.setAttribute('src', '/images/trashcan.png');
        delButton.addEventListener("click", () => {
            deleteItemFromShoppingList(ingredientId);
        });
        
        listItem.innerHTML = `${ingredients[i].ingredientName}`;

        listItem.appendChild(delButton);
        element.appendChild(listItem);
    }
}

function DisplayPlannedRecipes(recipes) {
    let element = document.querySelector('.planned_list_items');

    while (element.firstChild) {
        element.removeChild(element.lastChild);
    }


    for (var i = 0; i < recipes.length; i++) {
        let recipeId = recipes[i].recipeID;
        let listItem = document.createElement('li');
        listItem.setAttribute('id', `planned_recipe_list_item-${recipeId}`);
        let delButton = document.createElement('img');
        delButton.setAttribute('id', `del_list_item-${recipeId}`);
        delButton.setAttribute('src', '/images/trashcan.png');
        delButton.addEventListener("click", () => {
            deleteItemFromPlannedRecipes(recipeId);
        });

        listItem.innerHTML = `${recipes[i].recipeName}`;

        listItem.appendChild(delButton);
        element.appendChild(listItem);
    }
}

function deleteItemFromShoppingList(ingredientID) {
    let data = new FormData();
    data.append('ingredientID', ingredientID);

    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                let element = document.querySelector(`#shopping_list_item-${ingredientID}`);
                element.remove();
            }
        }
    }
    xhr.open('POST', 'DeleteFromShoppingList', true);
    xhr.send(data);
}

function deleteItemFromPlannedRecipes(recipeId) {
    let data = new FormData();
    data.append('recipeID', recipeId);

    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                let element = document.querySelector(`#planned_recipe_list_item-${recipeId}`);
                element.remove();
                getShoppingList();

            }
        }
    }
    xhr.open('POST', 'DeleteFromPlannedRecipes', true);
    xhr.send(data);
}

function searchPricesForIngredients() {
    let xhr = new XMLHttpRequest();
    if (xhr != null) {
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                console.log(xhr.responseText);
                let prices = JSON.parse(xhr.response);
                displayPrices(prices);
            }
        }
    }
    xhr.open('POST', 'GetPrices', true);
    xhr.send();
}

function displayPrices(prices) {

    let recipeListElement = document.querySelector('.Recipe_list_items');
    while (recipeListElement.firstChild) {
        recipeListElement.removeChild(recipeListElement.lastChild);
    }
    let element = document.querySelector('.Price_list_items');
    while (element.firstChild) {
        element.removeChild(element.lastChild);
    }

    for (var i = 0; i < prices.length; i++) {
        let listItemID = prices[i].ingredientID;

        let priceListItem = document.createElement('div');
        priceListItem.setAttribute('class', 'price_list_item');
        priceListItem.setAttribute('id', `ingredient_item-${listItemID}`);

        let listElementIngredientname = document.createElement('li');
        listElementIngredientname.innerHTML = "Ingredient name: " + prices[i].ingredientName;
        
        let ulElement = document.createElement('ul');
        listElementIngredientname.appendChild(ulElement);
               
        let detailShop = document.createElement('li');
        detailShop.innerHTML = "Shop name: " + prices[i].shopName;
        ulElement.appendChild(detailShop);

        let detailItemName = document.createElement('li');
        detailItemName.innerHTML = "Product name: " + prices[i].itemName;
        ulElement.appendChild(detailItemName);

        let detailPrice = document.createElement('li');
        detailPrice.innerHTML = "Price: " + prices[i].price + " " + prices[i].currency;
        ulElement.appendChild(detailPrice);




        priceListItem.appendChild(listElementIngredientname);
        
        element.appendChild(priceListItem);
    }
}