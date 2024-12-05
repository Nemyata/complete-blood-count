window.onload = function () {
    document.body.classList.add('animation_hiding');

    window.setTimeout(function () {
        document.body.classList.add('animation');
        document.body.classList.remove('animation_hiding');
    }, 600);
}

function startLoading() {
    //const loader = document.getElementsByClassName("animationLoader");

    document.body.classList.remove('animation');
    //document.body.classList.add('animation_hiding');
}

function stopLoading() {
    document.body.classList.add('animation');
    document.body.classList.remove('animation_hiding');
}