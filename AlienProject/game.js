// global variables
let story = '';
let choices = '';



// function endGame(){
//     // function to print global variable story
// }

function nextPrompt() {

    // get value of user's last choice
    // let lastChoice = document.getElementsByClassName('choice').value;
    // debugger;
    // console.log(lastChoice);

    // // concatenate latest choice to global variable choices
    // choices += lastChoice;

    // local variables
    let prompt = '';
    let option1 = '';
    let option2 = '';


    // switch to handle each different possible combo of decisions
    // switch (choices) {
    //     case 'A':
            prompt = document.getElementById('question').innerHTML;
            // console.log(prompt);
            document.getElementById('question').innerHTML = 'Prompt 2';

            option1 = document.getElementById('choiceALabel').innerHTML;
            // console.log('option1');
            document.getElementById('choiceALabel').innerHTML = 'New Choice 1';

            option1 = document.getElementById('choiceBLabel').innerHTML;
            // console.log('option2');
            document.getElementById('choiceBLabel').innerHTML = 'New Choice 2';
            // break;
        // case 'B':

        // case 'AA':

        // case 'AB':

        // case 'BA':

        // case 'BB':

        // case 'AAA':

        //     endGame();
        // case 'AAB':

        //     endGame();
        // case 'ABA':

        //     endGame();
        // case 'ABB':

        //     endGame();
        // case 'BAA':

        //     endGame();
        // case 'BAB':

        //     endGame();
        // case 'BBA':

        //     endGame();
        // case 'BBB':

        //     endGame();
    // }

}