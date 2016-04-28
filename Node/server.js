var express = require('express');
var app = express();
var fs   = require('fs-extra');
var sqlite = require("sqlite3").verbose();
var yaml = require('js-yaml');
var _ = require('underscore');

//Basic utility functions
function is_yaml(filename){
	return filename.split('.').pop() == "yaml";
}

function define_yaml_folder(path, route_name){
	var results = {};
	fs.readdir(path, (err, files) => {
		if (err){
			console.log(err);
		} else {
			_.each(
				_.filter(files, is_yaml),
				(filename) => {
					var key = filename.split('.')[0];
					fs.readFile(path+"/"+filename, 'utf8', (err, contents) => {
						try {
							results[key] = yaml.safeLoad(contents);
						} catch (e) {
							console.log("Problem loading:"+filename, e.reason, "@ line ", e.mark.line, ", column ", e.mark.column);
						}
					});
				}
			);
			//Language Routes
			app.get('/'+route_name, function(req, res) {
				res.send(_.keys(results));
			});

			app.get('/'+route_name+'/:id', function(req, res) {
				if (!_.isUndefined(results[req.params.id])){
					res.send(yaml.safeDump(results[req.params.id]));
				} else {
					res.send("-");
				}
			});
		}
	});
	return results;
}

//Languages
var languages = define_yaml_folder("static/languages", 'language');

//Settings
var settings = define_yaml_folder("static/settings", 'settings');

//Scores
var scores = {
	file: "scores.sqlite"
};
scores.exists = fs.exists(scores.file, (exists)=>{
	var db = new sqlite.Database(scores.file);
	db.serialize(() => {
		db.run('CREATE TABLE IF NOT EXISTS "scores" ("date" DATETIME NOT NULL , "player_ip" TEXT NOT NULL , "yaml_data" TEXT NOT NULL , PRIMARY KEY ("date", "player_ip"))');
		scores.db = db;
		app.get('/scores', (req,res) => {
			db.get("SELECT yaml_data FROM scores", function(err, row){
				console.log(row);
				if (!_.isUndefined(row)){
					res.send(row.yaml_data);
				} else {
					res.send("[]");
				}
			});
		});
		app.post('/scores', (req,res)=>{
		    db.run('INSERT INTO "main"."scores" VALUES (date,player_ip,yaml_data)', {
				date: new Date(),
				player_ip: req.connection.remoteAddress,
				yaml_data:yaml.safeDump(req.params.yaml_data)
			}, function(err, row){
				if (err){
					console.err(err);
					res.status(500);
				}
				else {
					res.status(202);
				}
				res.end();
			});
		});
	});
});


app.listen(8000);
console.log('Listening on port 8000...');