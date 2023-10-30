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


const colorStr = [ "blue", "green", "yellow", "red" ]

consoleMessages = []
const newLog = ( text, color="white" ) => {
    consoleMessages.push({text,time:moment().format("hh:mm:ss"), color})
    if( consoleMessages.length > 15 ) consoleMessages.shift()
}

var players = []
const updatePlayers = moveMsg => {
    var msg = moveMsg.split(",")
    var isNew = true
    players.forEach( (p,i) => {
        if(p[0] == msg[0]) {
            players[i] = msg
            isNew = false
        }
    })
    if (isNew) players.push(msg)
}

wss.on('connection', (ws) => onConnection(ws, newLog, wss, updatePlayers) )

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

        console.log()
        console.log("--------------------")
        console.log()

        var lines = [ "", "", "" ]
        players.forEach( (p,i) => {
            
            var dir = [ "···\t",
                        "·+·\t",
                        "···\t"]

            dir[ 1-Math.round(p[2]*1.4)] = setCharAt( 
                dir[1-Math.round(p[2]*1.4)],
                 1+Math.round(p[1]*1.4),
                 "o")

            if( colorStr[i] ){
                 dir.forEach( (l, j) => { if( l ) { lines[j] += l.split("").join("  ").replace("o",colorMsg(colorStr[i])("o")) }  } )
            }
            

        })

        lines.forEach( l => console.log(l))

        console.log()

        var btnLabels = ["Up", "Down", "Left", "Right"]
        var btnLine = ""
        players.forEach( (p, i) => {
            btnLabels.forEach( (b,j) => { 
                btnLine += p[j+3] == "true" ? colorMsg(colorStr[i])( b ): b
                btnLine += " "
            })
            btnLine += "\t"
        })

        console.log(btnLine)
        
        await sleep(500);
        console.clear()
    }
  }

  function setCharAt(str,index,chr) {
    if( !str ) return str
    if(index > str.length-1) return str;
    return str.substring(0,index) + chr + str.substring(index+1);
    } 
    function sleep(ms) {
    return new Promise((resolve) => {
        setTimeout(resolve, ms);
    });


}

function colorMsg(colorStr) { return colors[colorStr] ? colors[colorStr] : colors.white }
