let ingredientCount = 2;
let stepCount = 2;

document.addEventListener("DOMContentLoaded", () => {

    let newIngredientButton = document.querySelector(".new_ingredient_line__btn");
    newIngredientButton.addEventListener("click", newIngredient);
    let deleteIngredientButton = document.querySelector(".delete__btn");
    deleteIngredientButton.addEventListener("click", deleteIngredient);
    
    let newStepButton = document.querySelector(".new_step_line__btn");
    newStepButton.addEventListener("click", newStep);

    let postButton = document.querySelector(".Post__btn");
    postButton.addEventListener("click", postRecipe);
});

function deleteIngredient(count) {
    console.log(count);
    let ingredientRow = document.getElementById(`ingredient-${count}`);
    //while (ingredientRow.firstChild) {
    //    ingredientRow.removeChild(ingredientRow.lastChild);
    //}
    ingredientRow.remove();
}

function deleteStep(count) {
    console.log(count);
    let stepRow = document.getElementById(`step-${count}`);
    //while (stepRow.firstChild) {
    //    stepRow.removeChild(stepRow.lastChild);
    //}
    stepRow.remove();
}


function newIngredient() {

    let rowNumber = ingredientCount;
    element = document.querySelector('.ingredients_list');

    let ingredientForm = document.createElement('div');
    ingredientForm.setAttribute('class', 'ingredient_item');
    ingredientForm.setAttribute('id', `ingredient-${ingredientCount}`);


    
    //html = `<div class="ingredient_item" id="ingredient-%count%"><input type = "text" class="add__ingredient_quantity" placeholder = "Quantity" onkeypress = "this.style.width = ((this.value.length + 3) * 10) + 'px';" ><input type="text" class="add__ingredient_measure" placeholder="Measure unit" onkeypress="this.style.width = ((this.value.length + 3) * 10) + 'px';"><input type="text" class="add__ingredient_name" placeholder="Ingredient name" onkeypress="this.style.width = ((this.value.length + 3) * 10) + 'px';"><button class="delete__btn" >-</button></div>`
    //newHtml = html.replace('%count%', count);
    
    //element.insertAdjacentHTML('beforeend', newHtml);

    let quantity_input = document.createElement('input');
    quantity_input.setAttribute('class', 'add__ingredient_quantity');
    quantity_input.setAttribute('placeholder', 'Quantity');

    let measure_input = document.createElement('input');
    measure_input.setAttribute('class', 'add__ingredient_measure');
    measure_input.setAttribute('placeholder', 'Measure unit');

    let name_input = document.createElement('input');
    name_input.setAttribute('class', 'add__ingredient_name');
    name_input.setAttribute('placeholder', 'Ingredient name');

    let deleteButton = document.createElement('button');
    deleteButton.setAttribute('id', `delete__btn-${rowNumber}`);
    deleteButton.innerHTML = '-';
    deleteButton.addEventListener("click", () => { deleteIngredient(rowNumber)});

    ingredientForm.appendChild(quantity_input);
    ingredientForm.appendChild(measure_input);
    ingredientForm.appendChild(name_input);
    ingredientForm.appendChild(deleteButton);

    element.appendChild(ingredientForm);



    ingredientCount++;
}

function newStep() {

    let rowNumber = stepCount;
    element = document.querySelector('.recipe_step_list');

    let stepForm = document.createElement('div');
    stepForm.setAttribute('class', 'recipe_step');
    stepForm.setAttribute('id', `step-${rowNumber}`);

    let step_text_input = document.createElement('textarea');
    step_text_input.setAttribute('class', 'step_text');
    step_text_input.setAttribute('id', `step_text-${rowNumber}`);
        
    step_text_input.setAttribute('row', 4)
    
    
    let deleteButton = document.createElement('button');
    deleteButton.setAttribute('id', `delete__btn-${rowNumber}`);
    deleteButton.innerHTML = '-';
    deleteButton.addEventListener("click", () => { deleteStep(rowNumber) });

    stepForm.appendChild(step_text_input);
    
    stepForm.appendChild(deleteButton);

    element.appendChild(stepForm);



    stepCount++;
}

function postRecipe() {
    let recipeName, recipeDescription;
    let recipeSteps = [];
    let recipeIngredients = [];

    var data = new FormData();

    recipeName = document.querySelector('.add__recipename').value;
    recipeDescription = document.querySelector('.add__description').value;

    var steps = document.querySelectorAll("[id^=step_text]");

    for (var i = 0; i < steps.length; i++) {
        
        let stepnumber = i + 1;
        let steptext = steps[i].value;
        recipeSteps.push({
            key: stepnumber,
            value: steptext
        });
        
    }
    var ingredients = document.querySelectorAll("[id^=ingredient-]");
    for (var i = 0; i < ingredients.length; i++) {
        let ingredient_line = [];
        let ingredient_quantity = ingredients[i].querySelector('.add__ingredient_quantity').value;
        let ingredient_measure = ingredients[i].querySelector('.add__ingredient_measure').value;
        let ingredient_name = ingredients[i].querySelector('.add__ingredient_name').value;

        ingredient_line.push({
            key: 'ingredientQuantity',
            value: ingredient_quantity
        });
        ingredient_line.push({
            key: 'ingredientMeasure',
            value: ingredient_measure
        });
        ingredient_line.push({
            key: 'ingredientName',
            value: ingredient_name
        });

        recipeIngredients.push({
            key: i + 1,
            value: ingredient_line
        });

    }
    console.log(recipeIngredients);
    console.log(recipeSteps);


    data.append('recipeName',recipeName);
    data.append('recipeDescription',recipeDescription);
    data.append('recipeIngredients',recipeIngredients);
    data.append('recipeSteps',recipeSteps);
    let xhr = new XMLHttpRequest();
    xhr.open('POST', 'Slot/TaskToSlot', true);
    xhr.send(data);


    //var recipe = {
    //    name: 'recept',
    //    description: 'leírás',
    //    recipeSteps: {
    //        step1: {
    //            stepnumber: 1,
    //            steptext:'1. lépés'
    //        },
    //        step2: {
    //            stepnumber: 2,
    //            steptext: '2. lépés'
    //        }
    //    }
    //};
    

    //data.append('recipeName', recipeName);
    //data.append('recipeDescription', recipeDescription);
    //for (var pair of data.entries()) {
    //    console.log(pair[0] + ', ' + pair[1]);
    //}

    //console.log(recipe);

}