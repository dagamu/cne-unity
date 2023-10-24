const express = require('express') ;
const http = require('http');
const WebSocket = require('ws');

const Colors = [
    [0,0,1,0],  //Blue
    [0,1,0,0],  //Green
    [1,1,0,0],  //Yellow
    [1,0,0,0]   //Red
]

const app = express();
app.use('/', express.static('public'));

//initialize a simple http server
const server = http.createServer(app);

//initialize the WebSocket server instance
const wss = new WebSocket.Server({ server });

var connections = []
var unityClient;

getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() 
};

wss.on('connection', (ws) => {

    //connection is up, let's add a simple simple event
    ws.on('message', (message) => {

        //console.log(message.toString())

        if(message == 'createPlayer'){

            let newId = getUniqueID()  
            ws.id = newId
            
            let colorIndex = 0
            ws.color = Colors[colorIndex].join(';')
            while( connections.filter( c => c.color == ws.color ).length ){
                colorIndex++
                ws.color = Colors[colorIndex].join(';')
            }

            ws.send("ID:"+ws.id);
            ws.send("Color:"+ws.color)

            wss.clients.forEach(c => c.send(`newPlayer:${newId},${ ws.color }`))
            
            connections.push(ws)

            console.log(`New Player entered: ${newId} [${connections.length}]`)

        } else if( message == "UnityConnection"){
            
            let newId = getUniqueID() 
            ws.id = newId
            ws.isUnityClient = true
            unityClient = ws.id

            console.log(`UnityClient established: ${newId}`)

            connections.forEach( p => {
                console.log(`Player ${ p.id } entered into Unity`)
                ws.send(`newPlayer:${ p.id },${ p.color }`)
            })

        }


        wss.clients.forEach(c => 
            c.send(`${message}`))
    });

    ws.on('close', () => {
        if( ws.isUnityClient ) return
        connections.splice(connections.indexOf( ws ), 1)
        console.log(`Player left: ${ws.id} [${connections.length}]`)
        wss.clients.forEach( c => {
            c.send('Delete:' + ws.id)
            
        })
    })

    
});

//start our server
server.listen(process.env.PORT || 8080, () => {
    console.log(`Server started on port ${server.address().port}. http://localhost:${server.address().port} :)`);
});

