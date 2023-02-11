let http = require("http");
let mysql = require("mysql");
let fs = require("fs");
const { fileURLToPath } = require("url");
let sqlQuery = '';
let output = '';


let httpServer = http.createServer(processRequest);
httpServer.listen(8080);

function processRequest(request, response) {
    
    response.writeHead(200, { 'Content-type': 'text/html' });
    response.write("<head>  <link rel='icon' href='data:;base64,iVBORw0KGgo='> </head>");

    let base = "http://" + request.headers['host'];
    let url = new URL(request.url, base);

    // let searchFilmId = url.searchParams.get("movie");

    sqlQuery = `select * from user_testing;`;     
    initializeDB();

    response.write('<p>Here is the information you were looking for</p>')
    response.write(output);

    // instead of writign output on html we can render a new html file
    // fs.readFile('dashboard.html', function(error, data){
    //     if (error){
    //         res.writeHead(404)
    //         res.write('Error: File not found')
    //     } else {
    //         res.write(data)
    //     }
    //     res.end()
    // })

    response.end();
}

function initializeDB(){
    
    let connectionString = {

        host: "107.180.1.16",
        database: "sprc2023team11",
        port: "3306",
        user: "sprc2023team11",
        password: "sprc2023team11",
    
    };

    console.log(connectionString);

    let con = mysql.createConnection(connectionString);
    console.log("Connecting to database...");

    con.connect(

        function (err) {
    
            if (err) {
                console.log("connection error");
                throw err;
            }
            else
                console.log("Connected to database.");
    
        }
    
    );

    con.query(sqlQuery, processResult);
    con.end();
};

function processResult(err, result) {

    if (err) {
        console.log("connection error");
        throw err;
    }
    else {
        console.log(`There were ${result.length} rows returned`);
        result.forEach(printActor);
        // we might want to store the result in a dictionary and verify 
        // user input with the dictionary key-value pair here
    } 
}


function printActor(record) {

    output = `<p><strong>${record.title}</strong><br><p>${record.description}</p></p>`;

}