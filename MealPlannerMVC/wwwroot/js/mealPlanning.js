

document.addEventListener("DOMContentLoaded", () => {

    let searchRecipeButton = document.querySelector(".btn_search_recipename");
    searchRecipeButton.addEventListener("click", searchRecipeName);

    let searchIngredientButton = document.querySelector(".btn_search_ingredient");
    searchIngredientButton.addEventListener("click", searchIngredientName);

        
});

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
    

    let element = document.querySelector('.Recipe_list');
    while (element.firstChild) {
        element.removeChild(element.lastChild);
    }

    let result = document.createElement('div');
    result.setAttribute('class', '.search_results')
    
    var para = document.createElement("p");
    var node = document.createTextNode("Results: ");
    para.appendChild(node);
    result.appendChild(para);
   
    element.appendChild(result)

    for (var i = 0; i < recipes.length; i++) {
        let recipeId = recipes[i].recipeID;
        
        let RecipeListItem = document.createElement('div');
        RecipeListItem.setAttribute('class', 'recipe_list_item');
        RecipeListItem.setAttribute('id', `recipe_item-${recipeId}`);

        let paragraphRecipename = document.createElement('p');
        paragraphRecipename.innerHTML = recipes[i].recipeName;
        RecipeListItem.appendChild(paragraphRecipename);

        let btnAdd = document.createElement('button');
        btnAdd.setAttribute('id', `button_add-${recipeId}`);
        btnAdd.innerHTML = 'Add to plan';
        btnAdd.addEventListener("click", () => {
            AddToShoppingList(recipeId);
        });



        RecipeListItem.appendChild(btnAdd);
             


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
                ModifyShoppingList(ingredients);

            }
        }
    }
    xhr.open('POST', 'GetIngredientsByRecipeID', true);
    xhr.send(data);


}

function ModifyShoppingList(ingredients) {
    let element = document.querySelector('.shopping_list_items');

    for (var i = 0; i < ingredients.length; i++) {
        let listItem = document.createElement('li');
        listItem.innerHTML = `${ingredients[i].ingredientName}`;
        element.appendChild(listItem);

    }
}