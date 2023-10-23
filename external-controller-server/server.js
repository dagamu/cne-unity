const express = require('express') ;
const http = require('http');
const WebSocket = require('ws');
const moment = require('moment')
var colors = require('colors');

colors.enable()

const onConnection = require('./onConnection')

const app = express();
app.use('/', express.static('public'));

//initialize a simple http server
const server = http.createServer(app);

//initialize the WebSocket server instance
const wss = new WebSocket.Server({ server });

var unityClient;

getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() 
};

var devPlayerBot = false;

consoleMessages = []

const newLog = ( text, color ) => {
    consoleMessages.push({text,time:moment().format("hh:mm:ss"), color})
}

wss.on('connection', (ws) => onConnection(ws, newLog, wss) )

//start our server
server.listen(process.env.PORT || 8080, () => {
    console.log(`Server started on port ${server.address().port}. http://localhost:${server.address().port} :)`);
    init()
});

async function init() {
    var initTime = moment()
    while(true){

        var now = new Date()
        console.log( now.toLocaleDateString() + " - " + now.toLocaleTimeString() )

        var initDiff = moment( moment().diff(initTime ) )
        console.log( "Running Time: " + initDiff.format("mm:ss")  )
        //initDiff.hours() + ":" + initDiff.minutes() + ":" + initDiff.seconds()

        console.log()
        console.log("--------------------")
        console.log()

        consoleMessages.forEach( (msg) => {
            console.log(msg.time +": " +colors[msg.color](msg.text))
        })

        await sleep(500);
        console.clear()
    }
  }
  
function sleep(ms) {
return new Promise((resolve) => {
    setTimeout(resolve, ms);
});
}
