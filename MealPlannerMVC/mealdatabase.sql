
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS recipes CASCADE;
DROP TABLE IF EXISTS ingredients CASCADE;
DROP TABLE IF EXISTS measurement_quantities CASCADE;
DROP TABLE IF EXISTS measurement_units CASCADE;
DROP TABLE IF EXISTS recipe_ingredients CASCADE;
DROP TABLE IF EXISTS steps CASCADE;
DROP TABLE IF EXISTS shopping_list CASCADE;
DROP TABLE IF EXISTS shop_inventory CASCADE;
DROP TABLE IF EXISTS user_inventory CASCADE;
DROP TABLE IF EXISTS planned_meals CASCADE;


CREATE TABLE users(
	"user_id" SERIAL PRIMARY KEY,
	username TEXT ,
	email TEXT UNIQUE NOT NULL,
	"password" TEXT NOT NULL,
	city TEXT,
	address TEXT,
	user_role TEXT DEFAULT 'user'

);

CREATE TABLE recipes(
	recipe_id SERIAL PRIMARY KEY,
	recipe_name TEXT,
	description TEXT,
	"user_id" INTEGER NOT NULL REFERENCES users("user_id")
	
);
	
CREATE TABLE ingredients(
	ingredient_id SERIAL PRIMARY KEY,
	ingredient_name TEXT
);
	
CREATE TABLE measurement_quantities(
	measurement_quantity_id SERIAL PRIMARY KEY,
	quantity_amount FLOAT

);
	
CREATE TABLE measurement_units(
	measurement_unit_id SERIAL PRIMARY KEY,
	measurement_unit_text TEXT --UNIQUE

);
CREATE TABLE steps(
	step_number INT,
	step_text TEXT,
	recipe_id INTEGER NOT NULL REFERENCES recipes(recipe_id)
);

CREATE TABLE recipe_ingredients(
	recipe_id INTEGER NOT NULL REFERENCES recipes(recipe_id),
	measurement_quantity_id INTEGER NOT NULL REFERENCES measurement_quantities(measurement_quantity_id),
	measurement_unit_id INTEGER NOT NULL REFERENCES measurement_units(measurement_unit_id),
	ingredient_id INTEGER NOT NULL REFERENCES ingredients(ingredient_id)
	
	
);

CREATE TABLE shopping_list(
	"user_id" INTEGER NOT NULL REFERENCES users("user_id"),
	ingredient_id INTEGER NOT NULL REFERENCES ingredients(ingredient_id)
	
);

CREATE TABLE planned_meals(
	"user_id" INTEGER NOT NULL REFERENCES users("user_id"),
	recipe_id INTEGER NOT NULL REFERENCES recipes(recipe_id)
	
);

CREATE TABLE shop_inventory(
	item_id SERIAL PRIMARY KEY,
	ingredient_id INTEGER,
	item_name TEXT,
	shop_id INTEGER NOT NULL REFERENCES users("user_id"),
	price FLOAT,
	currency TEXT
	
);

CREATE TABLE user_inventory(
	"user_id" INTEGER NOT NULL REFERENCES users("user_id"),
	ingredient_id INTEGER,
	measurement_quantity_id INTEGER,
	measurement_unit_id INTEGER
);

CREATE OR REPLACE FUNCTION delete_items_from_shopping_list() RETURNS TRIGGER AS $$
DECLARE ingredient_ids INTEGER[];
BEGIN
	SELECT ARRAY(
	SELECT shopping_list.ingredient_id FROM shopping_list
	WHERE user_id = OLD.user_id
	except
	SELECT recipe_ingredients.ingredient_id FROM recipe_ingredients
	JOIN planned_meals on recipe_ingredients.recipe_id = planned_meals.recipe_id 
		and planned_meals.user_id = OLD.user_id
	) INTO ingredient_ids;
	
	IF array_upper(ingredient_ids,1)>0 THEN
	
	FOR i IN 1.. array_length(ingredient_ids, 1)
	 
	LOOP
    	
	DELETE FROM shopping_list WHERE ingredient_id = ingredient_ids[i] AND user_id = OLD.user_id;
	
  	END LOOP;
	END IF;
	
	RETURN NULL;
    
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION check_item_in_shopping_list() RETURNS TRIGGER AS $$
DECLARE count_ingredient INTEGER;
BEGIN

	SELECT COUNT(*) FROM shopping_list WHERE "user_id" = NEW.user_id AND ingredient_id = NEW.ingredient_id INTO count_ingredient;
    IF count_ingredient<1 THEN
		Raise notice 'number: %', count_ingredient;
        RETURN NEW;
    END IF;
	Raise notice 'number: %', count_ingredient;
	Return NULL;
    
END;
$$ LANGUAGE plpgsql;

/****************************************/
CREATE OR REPLACE FUNCTION add_to_shopping_list_after_add_meal() RETURNS TRIGGER AS $$
DECLARE ingredient_ids INTEGER[];
BEGIN

	SELECT ARRAY(SELECT ingredient_id FROM recipe_ingredients WHERE recipe_id = NEW.recipe_id) INTO ingredient_ids;
    PERFORM add_to_shopping_list(NEW.user_id, ingredient_ids);
	
	RETURN NULL;
    
END;
$$ LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION add_to_shopping_list(u_id Integer, i_id INTEGER[]) RETURNS void AS $$
BEGIN
		
	FOR i IN 1.. array_length(i_id, 1)
	 
	LOOP
    	
	INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (u_id, i_id[i]);
	
  	END LOOP;

END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION add_recipe(r_name TEXT, r_description TEXT, u_id INTEGER) RETURNS void AS $$
	
BEGIN
    
	INSERT INTO recipes (recipe_name, description, "user_id") VALUES (r_name, r_description, u_id);
		
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_steps_to_recipe(s_number INTEGER, s_text TEXT, r_id Integer) RETURNS void AS $$
	
BEGIN
    
	INSERT INTO steps (step_number, step_text, recipe_id) VALUES (s_number, s_text, r_id);
		
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_ingredients_to_recipe(i_name TEXT, i_quantity FLOAT, i_measurement_unit TEXT, r_id Integer) RETURNS void AS $$
	DECLARE i_name_id_exist INTEGER;
	DECLARE i_measurement_unit_id_exist INTEGER;
	DECLARE i_name_id INTEGER;
	DECLARE i_quantity_id INTEGER;
	DECLARE i_measurement_unit_id INTEGER;
BEGIN
    SELECT ingredient_id FROM ingredients WHERE i_name = ingredients.ingredient_name into i_name_id_exist;
        
	IF i_name_id_exist > 0 THEN
        i_name_id = i_name_id_exist;
    ELSE
		INSERT INTO ingredients (ingredient_name) VALUES (i_name) RETURNING ingredient_id into i_name_id;
	
	END IF;
	
	SELECT measurement_unit_id FROM measurement_units WHERE i_measurement_unit = measurement_units.measurement_unit_text into i_measurement_unit_id_exist;
	
	IF i_measurement_unit_id_exist > 0 THEN
        i_measurement_unit_id = i_measurement_unit_id_exist;
	ELSE
		INSERT INTO measurement_units (measurement_unit_text) VALUES (i_measurement_unit) RETURNING measurement_unit_id into i_measurement_unit_id;
		
	END IF;
	
	
	
	INSERT INTO measurement_quantities (quantity_amount) VALUES (i_quantity) RETURNING measurement_quantity_id into i_quantity_id;
	INSERT INTO recipe_ingredients (recipe_id,measurement_quantity_id, measurement_unit_id, ingredient_id) VALUES(r_id, i_quantity_id, i_measurement_unit_id, i_name_id);
	
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION check_ingredient(i_name TEXT) RETURNS INTEGER AS $$
DECLARE i_name_id_exist INTEGER;
BEGIN
	SELECT ingredient_id FROM ingredients WHERE i_name = ingredients.ingredient_name into i_name_id_exist;
        
	IF i_name_id_exist > 0 THEN
        RETURN i_name_id_exist;
    ELSE
		INSERT INTO ingredients (ingredient_name) VALUES (i_name) RETURNING ingredient_id into i_name_id_exist;
		RETURN i_name_id_exist;
	END IF;

END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION new_recipe(r_name TEXT, r_description TEXT, u_id INTEGER, step_numbers INT[] , steps TEXT[], i_name TEXT[], i_quantity FLOAT[], i_measurement_unit TEXT[]) RETURNS void AS $$
	DECLARE new_recipe_id INTEGER;
	
BEGIN
	INSERT INTO recipes (recipe_name, description, "user_id") VALUES (r_name, r_description, u_id) RETURNING recipe_id into new_recipe_id;
	
	
	
	FOR i IN 1.. array_length(step_numbers, 1)
	 
	LOOP
    	
	INSERT INTO steps (step_number, step_text, recipe_id) VALUES (step_numbers[i], steps[i], new_recipe_id);
	
  	END LOOP;
	
	
	FOR i IN 1.. array_length(i_name, 1)
	 
	LOOP
    	
	PERFORM add_ingredients_to_recipe(i_name[i], i_quantity[i], i_measurement_unit[i], new_recipe_id );
	
  	END LOOP;
		

END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_price_for_ingredient(u_id INTEGER) RETURNS void AS $$

BEGIN

select distinct on (shop_inventory.ingredient_id) shop_inventory.ingredient_id, shop_inventory.item_id, min(price) as price, shop_inventory.item_name, shop_inventory.shop_id, shop_inventory.currency
	from shop_inventory
join shopping_list on shopping_list.ingredient_id = shop_inventory.ingredient_id
where shopping_list.user_id = u_id
Group by shop_inventory.ingredient_id, shop_inventory.item_id, shop_inventory.item_name, shop_inventory.shop_id, shop_inventory.currency;

END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION check_item_in_planned_meals() RETURNS TRIGGER AS $$
DECLARE check_exist INTEGER;
BEGIN
	SELECT COUNT(*) FROM planned_meals WHERE user_id = NEW.user_id AND recipe_id = NEW.recipe_id INTO check_exist;
	IF check_exist > 0 THEN
	RETURN NULL;
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER check_item_in_shopping_list_trigger
BEFORE INSERT ON shopping_list
FOR EACH ROW EXECUTE FUNCTION check_item_in_shopping_list();

CREATE TRIGGER add_to_shopping_list_after_add_meal_trigger
AFTER INSERT ON planned_meals
FOR EACH ROW EXECUTE FUNCTION add_to_shopping_list_after_add_meal();


CREATE TRIGGER delete_items_from_shopping_list_trigger
AFTER DELETE ON planned_meals
FOR EACH ROW EXECUTE FUNCTION delete_items_from_shopping_list();

CREATE TRIGGER check_item_in_planned_meals_trigger
BEFORE INSERT ON planned_meals
FOR EACH ROW EXECUTE FUNCTION check_item_in_planned_meals();


INSERT INTO users (username, email, password, user_role) VALUES ('admin', 'admin@admin.com', 'admin', 'admin');
INSERT INTO users (username, email, password) VALUES ('user1', 'user1@users.com', 'user1');
INSERT INTO users (username, email, password, user_role) VALUES ('SampleShop', 'sample@shop.com', 'sample', 'shop');
INSERT INTO users (username, email, password, user_role) VALUES ('Mari Néni kisbótja', 'mari@shop.com', 'mari', 'shop');


SELECT new_recipe('Palacsinta', 'Recipe 1 description', 2, ARRAY [1, 2, 3, 4, 5, 6, 7], 
	ARRAY ['Tegyük egy keverőtálba a lisztet, tojást, vaníliás cukrot és a sót. ',
	'Robotgép segítségével kezdjük el keverni, majd folyamatosan adjuk hozzá a tejet és végül az olvasztott vajat. Addig keverjük, míg sima palacsintatésztát nem kapunk.',
	'Takarjuk le, és hűtőszekrényben pihentessük 2 órán át.  A tészta akkor jó, ha állaga a főzőtejszín sűrűségéhez hasonlít. (Ha szükséges, kis tejjel higíthatjuk.)',
	'Forrósítsuk fel a teflon palacsintasütőnket. (Tipp: én minden egyes palacsinta kisütése előtt hajszálvékonyan egy ecset segítségével pár csepp olajat szét szoktam kenni.)',
	'Vegyük le a tűzről a sütőnket, és kanalazzunk bele a tésztából (kb. fél merőkanálnyit).  A palacsintasütő megdöntésével és forgatásával egyenletesen és vékonyan oszlassuk szét a tésztát az edény alján.',
	'Közepes lángon süssük a palacsintát 45-60 másodpercig, ill. amíg el nem válik a serpenyőtől. Lazítsuk fel a palacsintát, fordítsuk meg, és a másik oldalát is süssük kb. 30 mp-ig.',
	'Borítsuk tányérra. A kész palacsintákat halmozzuk egymásra, és takarjuk le konyharuhával, amíg az összeset kisütjük.'
	],
	ARRAY['finomliszt', 'tojás', 'vaníliás cukor', 'só', 'tej', 'vaj', 'napraforgó olaj'],
	ARRAY[125, 2, 20, 1, 250, 40, 6],
	ARRAY['g', 'db', 'g', 'csipet', 'ml', 'g', 'teáskanál']
	
	
	);


/*

SELECT add_recipe('Palacsinta', 'Recipe 1 description');

SELECT add_steps_to_recipe(1, 'Tegyük egy keverőtálba a lisztet, tojást, vaníliás cukrot és a sót. ', 1);
SELECT add_steps_to_recipe(2, 'Robotgép segítségével kezdjük el keverni, majd folyamatosan adjuk hozzá a tejet és végül az olvasztott vajat. Addig keverjük, míg sima palacsintatésztát nem kapunk.', 1);
SELECT add_steps_to_recipe(3, 'Takarjuk le, és hűtőszekrényben pihentessük 2 órán át.  A tészta akkor jó, ha állaga a főzőtejszín sűrűségéhez hasonlít. (Ha szükséges, kis tejjel higíthatjuk.)', 1);
SELECT add_steps_to_recipe(4, 'Forrósítsuk fel a teflon palacsintasütőnket. (Tipp: én minden egyes palacsinta kisütése előtt hajszálvékonyan egy ecset segítségével pár csepp olajat szét szoktam kenni.)', 1);
SELECT add_steps_to_recipe(5, 'Vegyük le a tűzről a sütőnket, és kanalazzunk bele a tésztából (kb. fél merőkanálnyit).  A palacsintasütő megdöntésével és forgatásával egyenletesen és vékonyan oszlassuk szét a tésztát az edény alján.', 1);
SELECT add_steps_to_recipe(6, 'Közepes lángon süssük a palacsintát 45-60 másodpercig, ill. amíg el nem válik a serpenyőtől. Lazítsuk fel a palacsintát, fordítsuk meg, és a másik oldalát is süssük kb. 30 mp-ig.', 1);
SELECT add_steps_to_recipe(7, 'Borítsuk tányérra. A kész palacsintákat halmozzuk egymásra, és takarjuk le konyharuhával, amíg az összeset kisütjük.', 1);

SELECT add_ingredients_to_recipe('finomliszt', 125, 'g', 1);
SELECT add_ingredients_to_recipe('tojás', 2, 'db', 1);
SELECT add_ingredients_to_recipe('vaníliás cukor', 20, 'g', 1);
SELECT add_ingredients_to_recipe('só', 1, 'csipet', 1);
SELECT add_ingredients_to_recipe('tej', 250, 'ml', 1);
SELECT add_ingredients_to_recipe('vaj', 40, 'g', 1);
SELECT add_ingredients_to_recipe('napraforgó olaj', 6, 'teáskanál', 1);
*/

SELECT add_recipe('Madártej', 'Recipe 2 description', 2);

SELECT add_steps_to_recipe(1, 'A tojásfehérjét egy csipet sóval kezdjük el kemény habbá verni, majd adjuk hozzá a két evőkanál porcukrot, és verjük teljesen keményre.', 2);
SELECT add_steps_to_recipe(2, 'A tejhez adjuk hozzá a vaníliarúd kikapart magjait, majd forraljuk fel fel, és kanállal szaggassuk bele a tojáshabból galuskákat. Két-három percig főzzük, majd óvatosan for- dítsuk meg, újabb két-három perc, és már szedhetjük is ki egy üres tálba.', 2);
SELECT add_steps_to_recipe(3, 'A tojássárgáját egy csipet sóval és a cukorral keverjük jó habosra, öntsük hozzá egy keveset a langyos tejből, majd azonnal keverjük simára. Adjuk hozzá a maradék tejet, keverjük el, és öntsd vissza az egészet az edénybe.)', 2);
SELECT add_steps_to_recipe(4, 'Alacsony lángon, folyamatosan kevergetve addig főzzük, amíg szépen be nem sűrűsödik, majd hagyjuk kihűlni. Tegyük a tetejére a galuskákat, és az egészet rakjuk be a hűtőbe legalább 2-3 órával azelőtt, hogy tálalni szeretnénk.', 2);

SELECT add_ingredients_to_recipe('tojásfehérje', 4, 'db', 2);
SELECT add_ingredients_to_recipe('só', 1, 'csipet', 2);
SELECT add_ingredients_to_recipe('porcukor', 2, 'evőkannál', 2);
SELECT add_ingredients_to_recipe('tej', 800, 'ml', 2);
SELECT add_ingredients_to_recipe('vanília', 1, 'db', 2);
SELECT add_ingredients_to_recipe('tojássárgája', 4, 'db', 2);
SELECT add_ingredients_to_recipe('cukor', 70, 'g', 2);

INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (5, 'Tej 2,8%', 3, 240, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (1, 'Valamilyen liszt', 3, 180, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (6, 'Tea vaj', 3, 165, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (2, 'Tojás M', 3, 30, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (12, 'Kristálycukor', 3, 240, 'HUF');

INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (5, 'Tej 1,4%', 4, 220, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (1, 'Liszt', 4, 185, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (6, 'Vaj', 4, 160, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (2, 'Tojás Házi', 4, 32, 'HUF');
INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (12, 'Kristálycukor', 4, 240, 'HUF');


INSERT INTO planned_meals ("user_id", recipe_id) VALUES (2, 2);




/*
INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (2, 1);
INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (2, 2);
INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (2, 1);
INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (2, 2);
INSERT INTO shopping_list ("user_id", ingredient_id) VALUES (2, 4);
*/


