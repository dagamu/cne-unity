var drag;
var touchMode;
var cnv;
var btns;

var colorTheme = {
  bg: 33,
  stroke: 25,
  btn: 40,
  text: 50,
  pressed: 100,

}
var colorSliders = {};
var playerColor = colorTheme.btn;

const webSocket = new WebSocket(document.baseURI.replace('http','ws'));
var gameID;

webSocket.onmessage = (event) => {

  let message = event.data.split(':')
  let type = message[0]
  let data = message[1] ? message[1].split(',') : ''

  var proccessByType = {

    'ID': () => {
      gameID = data
      console.log('ID: ' + gameID)

    },
    'Color': () => {

      colorData = message[1].split(';')
      let colorList = colorData.map( r => parseInt(r)*160)
      colorList[3] = 0.1
      playerColor = `rgba(${colorList.join(',')})`
      console.log(playerColor)

    }
  }

  if (proccessByType[type]) proccessByType[type]()
  

};

webSocket.addEventListener("open", () => {
  console.log("We are connected");
  webSocket.send('createPlayer')
});

function moveMsg() {
  webSocket.send('Move:' + [gameID, controlMessage].toString())
}

function showSliders(){
  for( cslider in colorSliders ){
    colorSliders[cslider].style( 'visibility', 'visible')
  }   
}

function setup() {

  cnv = createCanvas(windowWidth - 10, windowHeight - 10);
  windowResized()

  drag = false
  touchMode = windowWidth < 900;

  for( c in colorTheme ){
    let newColorSlider = createSlider(0, 255, colorTheme[c] );
    newColorSlider.position(10, colorSliders.length*20 + 10);
    newColorSlider.style('width', '80px');
    newColorSlider.style('visibility', 'hidden');
    colorSliders[c] = newColorSlider  
  }

}

function draw() {
  background( colorTheme.bg );
  noFill()

  for( c in colorSliders ) colorTheme[c] = colorSliders[c].value()
  

  let prop = width / height

  var rsp = {
    Joystick: {
      size: prop > 2.5 ? height * 4 / 5 :
             width / 2.7,
      center: [ width / 4, height / 2 ],
      handler: prop > 2.5 ? height / 3 :
              width / 7
    },
    btns: {
      size: prop < 2 ? width / 6 :
            prop > 2.5 ? height / 2.5:
            width / 8,
      xOffset: prop < 1.6 || prop > 2.5 ? width / 8:
                prop < 2 ? height / 4:
                height / 4,
    }
  };

  stroke( playerColor )
  strokeWeight(6)
  circle(...rsp.Joystick.center, rsp.Joystick.size)

  //Joystick
  strokeWeight(4)
  stroke( colorTheme.stroke )
  fill(colorTheme.btn )

  var pos

  if (touchMode) {
    pos = rsp.Joystick.center
    if (drag) {
      touches.some(t => {
        if (t.x < width / 2) {
          pos = [t.x, t.y]
          return true
        }
        return false
      })
    }


  } else {
    pos = drag ? [mouseX, mouseY] : rsp.Joystick.center
  }

  circle(...pos, rsp.Joystick.handler)


  controlMessage = [
    round((pos[0] - rsp.Joystick.center[0]) / (rsp.Joystick.center[0]), 2),   // Joystick X 0
    round((rsp.Joystick.center[1] - pos[1]) / (rsp.Joystick.center[0]), 2), // Joystick X 1
    false, // btnUp 2
    false,  // btnDown 3
    false,  // btnLeft 4
    false // btnRight 5
  ]

  //Buttons
  translate(-width / 24, 0)
  strokeWeight(6)
  fill( colorTheme.btn )

  var btnSize = rsp.btns.size
  const checkButton = (x, y) => 
    touches.some( t => dist(x, y, t.x, t.y) < btnSize * 3 / 5 )
    || dist(x, y, mouseX, mouseY) < btnSize * 3 / 5 && mouseIsPressed


  btns = [
    [ width * 3 / 4, height / 4     ],                    //Up
    [ width * 3 / 4, height * 3 / 4 ],                    //Down
    [ width * 3 / 4 - rsp.btns.xOffset, height / 2 ],     //Left
    [ width * 3 / 4 + rsp.btns.xOffset, height / 2 ],     //Right

  ]

  btns.forEach( (btn, i) => {
    
    let pressed = checkButton( btn[0]-width / 24, btn[1])
    controlMessage[i+2] = pressed

    push()
    strokeWeight(4)
    stroke( colorTheme.stroke )
    if( pressed ) {
      stroke( colorTheme.pressed )
      moveMsg()
    }
    circle( ...btn, btnSize)
    pop()
  })

  noStroke()
  textSize(height / 15)
  fill( colorTheme.text )
  text('Id: ' + gameID, height / 10, height / 10)

  if( !controlMessage.every( e => !e ) ){
    moveMsg()
  }

}

function mousePressed() {
  touchMode = false
  if (mouseX < width / 2) drag = true
  moveMsg()
}

function mouseReleased() {
  drag = false
}

function touchStarted() {
  touchMode = true
  drag = true

  moveMsg()
}

function touchEnded() {
  drag = false
}

/* full screening will change the size of the canvas */
function windowResized() {

  resizeCanvas(windowWidth, windowHeight)


}
