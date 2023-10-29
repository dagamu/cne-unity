const Colors = [
    [0,0,1,0],  //Blue
    [0,1,0,0],  //Green
    [1,1,0,0],  //Yellow
    [1,0,0,0]   //Red
]

const colorStr = [ "blue", "green", "yellow", "red" ]

var connections = []
var devPlayerBot = false;

module.exports = function(ws, newLog, wss, updatePlayers) {

    //connection is up, let's add a simple simple event
    ws.on('message', (message) => {

        //newLog(message.toString())

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

            newLog(`New Player entered: ${newId} [${connections.length}]`, colorStr[colorIndex])

        } else if( message == "UnityConnection"){
            
            let newId = getUniqueID() 
            ws.id = newId
            ws.isUnityClient = true
            unityClient = ws.id

            newLog(`UnityClient established: ${newId}`)

            if( connections.length == 0 ){
                newId = getUniqueID() 
                devPlayerBot = true;
                newLog(`Dev player bot established: ${newId}`)
                connections.push({id:newId, color:Colors[0].join(';')})
            }

            connections.forEach( p => {
                newLog(`Player ${ p.id } entered into Unity`)
                ws.send(`newPlayer:${ p.id },${ p.color }`)
            })
        } else {
            updatePlayers( message.toString().split(":")[1])
        }

        wss.clients.forEach(c => {
            c.send(`${message}`)
            //updatePlayers(connections)
        }
            )
    });

    ws.on('close', () => {
        if( ws.id == undefined ) return;
        if( ws.isUnityClient ) { 
            console.log(`UnityClient left: ${ws.id}`)
            if(devPlayerBot ){
                var bot = connections.shift()
                devPlayerBot = false
                newLog(`Dev player bot killed:`)
            }
            return
        }
        connections.splice(connections.indexOf( ws ), 1)
        newLog(`Player left: ${ws.id} [${connections.length}]`)
        wss.clients.forEach( c => {
            c.send('Delete:' + ws.id)
            
        })
    })
}