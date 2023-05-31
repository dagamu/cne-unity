function mousePressed() {
    touchMode = false
    if (mouseX < width / 2) drag = true
    moveMsg()
  }
  
  function mouseReleased() {
    drag = false
    if( mouseX > width - height/5 && mouseY < height/4 ){
      changeTheme()
    }
  }
  
  function touchStarted() {
  
    touchMode = true
    drag = true
  
    moveMsg()
    let fs = fullscreen();
    if(!fs) fullscreen(!fs)
  }
  
  function touchEnded() {
    drag = false
    if(touches.filter( t => t.x > width - height/5 && t.y < height/4 ).length){
      changeTheme()
    }
  }

  const changeTheme = () => colorTheme = ColorThemes[ ( ColorThemes.indexOf( colorTheme ) + 1) % ColorThemes.length ]
  
  