const webSocket = new WebSocket(document.baseURI.replace('http','ws'));

const proccessByType = {
  
    'ID': ( data, messge ) => {
      gameID = data
      console.log('ID: ' + gameID)

    },
    'Color': ( data, message ) => {

      colorData = message[1].split(';')
      let colorList = colorData.map( r => parseInt(r)*160)
      colorList[3] = 0.1
      playerColor = `rgba(${colorList.join(',')})`
      console.log(playerColor)

    },
    'ModelChange': ( data, message ) => {
        console.log(data)
        data.forEach( (m,i) => { if(m==gameID) playerThumbnail = data[i+1]} )
    }
  }

webSocket.onmessage = (event) => {

    let message = event.data.split(':')
    let type = message[0]
    let data = message[1] ? message[1].split(',') : ''

    if( type!="Move") console.log(message)
  
    if (proccessByType[type]) proccessByType[type]( data, message )

  };

  webSocket.addEventListener("open", () => {
    console.log("We are connected");
    webSocket.send('createPlayer')
  });