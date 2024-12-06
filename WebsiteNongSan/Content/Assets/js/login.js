// https://bytewebster.com/
// https://bytewebster.com/
// https://bytewebster.com/
const signupBtn = document.getElementById("signup-btn");
const signinBtn = document.getElementById("signin-btn");
const mainContainer = document.querySelector(".con");

signupBtn.addEventListener("click", () => {
    mainContainer.classList.toggle("change");
});
signinBtn.addEventListener("click", () => {
    mainContainer.classList.toggle("change");
});
