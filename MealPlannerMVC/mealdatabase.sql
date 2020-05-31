
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS recipes CASCADE;
DROP TABLE IF EXISTS ingredients CASCADE;
DROP TABLE IF EXISTS measurement_quantities CASCADE;
DROP TABLE IF EXISTS measurement_units CASCADE;
DROP TABLE IF EXISTS recipe_ingredients CASCADE;
DROP TABLE IF EXISTS steps CASCADE;
DROP TABLE IF EXISTS shops CASCADE;
DROP TABLE IF EXISTS shop_store CASCADE;
DROP TABLE IF EXISTS user_store CASCADE;

CREATE TABLE users(
	"user_id" SERIAL PRIMARY KEY,
	username TEXT ,
	email TEXT UNIQUE NOT NULL,
	"password" TEXT NOT NULL,
	user_role TEXT DEFAULT 'user'

);

CREATE TABLE recipes(
	recipe_id SERIAL PRIMARY KEY,
	recipe_name TEXT,
	description TEXT
	
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

CREATE TABLE shops(
	shop_id SERIAL PRIMARY KEY,
	shop_name TEXT,
	shop_location TEXT

);
/*
CREATE TABLE shop_store(
	id
	ingredient_id
	shop_id
	name
	price
	

);

CREATE TABLE user_store(
);
*/

CREATE OR REPLACE FUNCTION add_recipe(r_name TEXT, r_description TEXT) RETURNS void AS $$
	
BEGIN
    
	INSERT INTO recipes (recipe_name, description) VALUES (r_name, r_description);
		
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_steps_to_recipe(s_number INTEGER, s_text TEXT, r_id Integer) RETURNS void AS $$
	
BEGIN
    
	INSERT INTO steps (step_number, step_text, recipe_id) VALUES (s_number, s_text, r_id);
		
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_ingredients_to_recipe(i_name TEXT, i_quantity FLOAT, i_measurement_unit TEXT, r_id Integer) RETURNS void AS $$
	DECLARE i_name_id INTEGER;
	DECLARE i_quantity_id INTEGER;
	DECLARE i_measurement_unit_id INTEGER;
BEGIN
    
	INSERT INTO ingredients (ingredient_name) VALUES (i_name) RETURNING ingredient_id into i_name_id;
	INSERT INTO measurement_quantities (quantity_amount) VALUES (i_quantity) RETURNING measurement_quantity_id into i_quantity_id;
	INSERT INTO measurement_units (measurement_unit_text) VALUES (i_measurement_unit) RETURNING measurement_unit_id into i_measurement_unit_id;
	
	INSERT INTO recipe_ingredients (recipe_id,measurement_quantity_id, measurement_unit_id, ingredient_id) VALUES(r_id, i_quantity_id, i_measurement_unit_id, i_name_id);
	
END;
$$ LANGUAGE plpgsql;


INSERT INTO users (username, email, password, user_role) VALUES ('admin', 'admin@admin.com', 'admin', 'admin');
INSERT INTO users (username, email, password) VALUES ('user1', 'user1@users.com', 'user1');

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


SELECT add_recipe('Madártej', 'Recipe 2 description');

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

