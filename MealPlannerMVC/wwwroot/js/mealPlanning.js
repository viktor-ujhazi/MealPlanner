

document.addEventListener("DOMContentLoaded", () => {

    let searchRecipeButton = document.querySelector(".btn_search_recipename");
    searchRecipeButton.addEventListener("click", searchRecipeName);

    let searchIngredientButton = document.querySelector(".btn_search_ingredient");
    searchIngredientButton.addEventListener("click", searchIngredientName);

        
});
window.onload = () => {
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
};


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

    //let result = document.createElement('div');
    //result.setAttribute('class', '.search_results')
    
    //var para = document.createElement("p");
    //var node = document.createTextNode("Results: ");
    //para.appendChild(node);
    //result.appendChild(para);
   
    //element.appendChild(result)

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

                console.log("ingredients");
                console.log(xhr.responseText);
                let ingredients = JSON.parse(xhr.response);
                DisplayShoppingList(ingredients);

            }
        }
    }
    xhr.open('POST', 'AddToShoppingList', true);
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