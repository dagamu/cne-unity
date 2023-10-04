var drag,
     touchMode,
     cnv,
     themeImg,
     gameID

const ColorThemes = [
  {
    bg: 33,
    stroke: 25,
    btn: 40,
    text: 50,
    pressed: 100,
  
  },
  {
    bg: 255,
    stroke: 40,
    btn: 230,
    text: 170,
    pressed: 150,
  
  }
]

var colorTheme = ColorThemes[0]
var playerColor = colorTheme.btn;
const userAgent = navigator.userAgent.toLowerCase();
var isTablet = /(ipad|tablet|(android(?!.*mobile))|(windows(?!.*phone)(.*touch))|kindle|playbook|silk|(puffin(?!.*(IP|AP|WP))))/.test(userAgent);

const moveMsg = () => webSocket.send('Move:' + [gameID, controlMessage].toString() )
function windowResized () { resizeCanvas(windowWidth, windowHeight) }

function preload(){
   themeImg = loadImage('https://static-00.iconduck.com/assets.00/dark-theme-icon-512x512-185rlszm.png')
}

function setup() {

  cnv = createCanvas(windowWidth - 10, windowHeight - 10);
  windowResized()

  drag = false
  touchMode = userAgent.indexOf("win64") != -1 ? false : windowWidth < 900

}

function draw() {
  background( colorTheme.bg );
  noFill()

  let prop = width / height

  var rsp = {
    Joystick: {
      size:     isTablet ? width * 0.2  :
                prop > 2.5 ? height * 4 / 5 : width / 2.7,
      center:   isTablet ? [ width * 0.15, height / 2 ] : [ width / 4, height / 2 ],
      handler:  isTablet ?  width * 0.08 :
                prop > 2.5 ? height / 3 : width / 7
                
    },
    btns: {
      size: isTablet ? width / 10 :
            prop < 2 ? width / 6 :
            prop > 2.5 ? height / 2.5 : width / 8,
      xOffset:  isTablet ? height / 10:
                prop < 1.6 || prop > 2.5 ? width / 8 :
                prop < 2 ? height / 4 :
                height / 4,
    }
  };

  if( isTablet ){
    push()
    noStroke()
    fill(colorTheme.btn)
    rect( width *0.3, 0, width*0.4, height)
    pop()
  }

  stroke( playerColor )
  strokeWeight(6)
  circle(...rsp.Joystick.center, rsp.Joystick.size)

  //Joystick
  strokeWeight(4)
  stroke( colorTheme.stroke )
  fill(colorTheme.btn )

  let pos

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


  } else pos = drag ? [mouseX, mouseY] : rsp.Joystick.center
  

  circle( ...pos, rsp.Joystick.handler )

  controlMessage = [
    round(
      (pos[0] - rsp.Joystick.center[0]) / (rsp.Joystick.center[0]), 2),   // Joystick X 0
    round(
      (rsp.Joystick.center[1] - pos[1]) / (rsp.Joystick.center[0]), 2), // Joystick X 1
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

  btns = !isTablet ? [
    [ width * 3 / 4, height / 4     ],                    //Up
    [ width * 3 / 4, height * 3 / 4 ],                    //Down
    [ width * 3 / 4 - rsp.btns.xOffset, height / 2 ],     //Left
    [ width * 3 / 4 + rsp.btns.xOffset, height / 2 ],     //Right

  ] : [
    [ width * 0.9, height * 1.1 / 3     ],                    //Up
    [ width * 0.9, height * 1.1 * 2 / 3 ],                    //Down
    [ width * 0.9 - rsp.btns.xOffset, height * 1.1 / 2 ],     //Left
    [ width * 0.9 + rsp.btns.xOffset, height * 1.1 / 2 ],     //Right
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
  textAlign(LEFT)
  text('Id: ' + gameID, height / 6, height / 10)

  moveMsg()
  tint(255, 70)
  image( themeImg, width - height/5.8, height/16, height/6, height/6 );

}

