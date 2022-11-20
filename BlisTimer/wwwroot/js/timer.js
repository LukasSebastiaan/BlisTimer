﻿
const time_el = document.querySelector('.watch .time');
const startButton = document.getElementById("start");
const stopButton = document.getElementById("stop");
const pauseButton = document.getElementById("pause");

let seconds = 0;
let interval = null;

// Event Listeners
startButton.addEventListener('click', start);
stopButton.addEventListener('click', stop);
pauseButton.addEventListener('click', pause);


// Timer updates
function timer() {
    seconds++;

    //Format the time 
    let hrs = Math.floor(seconds / 3600);
    let mins = Math.floor((seconds - (hrs * 3600)) / 60);
    let secs = seconds % 60;

    if (secs < 10) secs = "0" + secs;
    if (mins < 10) mins = "0" + mins;
    if (hrs < 10) hrs = "0" + hrs;

    time_el.innerText = `${hrs}:${mins}:${secs}`;
}

//start button
function start() {
    if (interval) {
        return;
    }

    interval = setInterval(timer, 1000);
}

//pause button
function pause() {
    clearInterval(interval);
    interval = null;
}

//stop button
function stop() {
    if(seconds > 0){
        clearInterval(interval);
        interval = null;

        openPopup();
    }
}

//confirm button
function confirm() {
    closePopup();
    seconds = 0;
    time_el.innerText = "00:00:00";
}

//cancel button
function cancel() {
    closePopup();
    start();
}


//Popup message
let popup = document.getElementById("popup");

function openPopup() {
    popup.classList.add("open-popup");
}

function closePopup() {
    popup.classList.remove("open-popup");
}
