// global variables
let story = '';
let choices = '';

var currentChoice = ''
var choiceHistory = ''
		

// function endGame(){
//     // function to print global variable story
// }
    
function nextPrompt() {
    buttonA = document.getElementById('A');
    buttonB = document.getElementById('B');

    currentChoice = buttonA.checked ? buttonA.value : buttonB.checked ? buttonB.value : '';
    choiceHistory += currentChoice;
    console.log("currentChoice: " + currentChoice);
    console.log("choiceHistory: " + choiceHistory);


    // local variables
    let prompt = '';
    let option1 = '';
    let option2 = '';


    // switch to handle each different possible combo of decisions
    switch (currentChoice) {
        case 'A':
            prompt = document.getElementById('question').innerHTML;
            // console.log(prompt);
            document.getElementById('question').innerHTML = 'Prompt 2';

            option1 = document.getElementById('choiceALabel').innerHTML;
            // console.log('option1');
            document.getElementById('choiceALabel').innerHTML = 'A Choice 1';

            option1 = document.getElementById('choiceBLabel').innerHTML;
            // console.log('option2');
            document.getElementById('choiceBLabel').innerHTML = 'A Choice 2';
            break;
        case 'B':
            // console.log(prompt);
            document.getElementById('question').innerHTML = 'Prompt 3';

            option1 = document.getElementById('choiceALabel').innerHTML;
            // console.log('option1');
            document.getElementById('choiceALabel').innerHTML = 'B Choice 1';

            option1 = document.getElementById('choiceBLabel').innerHTML;
            // console.log('option2');
            document.getElementById('choiceBLabel').innerHTML = 'B Choice 2';
            break;
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
    }

}