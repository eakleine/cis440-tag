let http = require("http");
let mysql = require("mysql");
let fs = require("fs");
const { fileURLToPath } = require("url");
let sqlQuery = '';
let output = '';
let account = [];
let password = [];
let searchAccount = '';
let searchPassword = '';
let file = '';


let httpServer = http.createServer(processRequest);
httpServer.listen(8080);

function processRequest(request, response) {
    
    response.writeHead(200, { 'Content-type': 'text/html' });
    response.write("<head>  <link rel='icon' href='data:;base64,iVBORw0KGgo='> </head>");

    let base = "http://" + request.headers['host'];
    let url = new URL(request.url, base);

    searchAccount = url.searchParams.get("account");
    searchPassword = url.searchParams.get("password");

    sqlQuery = `select * from user_testing;`;     
    initializeDB();
    login_verification();

    if (file =='dashboard.html'){
        fs.readFile('dashboard.html', function(error, data){
            if (error){
                response.writeHead(404)
                response.write('Error: File not found')
            } else {
                response.write(data)
                response.write(output);
            }
            response.end()
        })
    } else {
        fs.readFile('login.html', function(error, data){
            if (error){
                response.writeHead(404)
                response.write('Error: File not found')
            } else {
                response.write(data)
                response.write(output);
            }
            response.end()
        })
    }

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
        for (let i = 0; i < result.length; i++) { 
            console.log(result[i].account, result[i].password)
            account.push(result[i].account) 
            password.push(result[i].password)
        }
        
        console.log(account)
        console.log(password)

    } 
}

function login_verification(){

    console.log(searchAccount)
    console.log(searchPassword)

    for (let i = 0; i < account.length; i++){
        if (searchAccount == account[i]){
            if (searchPassword == password[i]){
                // output = `<p>Account exists</p>`
                file = 'dashboard.html'
                output = " ";
            }
            else{
                file = 'login.html'
                output = `<p>Password is not correct</p>`
            }
            break;
        }
        else {
            file = 'login.html'
            output = `<p>Account does not exist</p>`+ `<p>Do you want to create an account?</p>`;
        }
    }
}


