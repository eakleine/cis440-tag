// global variables
let story = '';
let choices = '';



// function endGame(){
//     // function to print global variable story
// }

function nextPrompt() {

    // thing = document.getElementsByTagName('label');
    let getChoice = document.getElementById('A');
    isChoiceAOn = getChoice.value;
    console.log(isChoiceAOn);
    if (isChoiceAOn === 'on'){
        // concat string
        choices += 'A';
        console.log(choices);
    } else {
        // concat string
        choices += 'B';
        console.log(choices);
    }
    // let stuff = document.getElementsByName('radios').values;
    // console.log(stuff)


    // local variables
    let prompt = '';
    let option1 = '';
    let option2 = '';


    // switch to handle each different possible combo of decisions
    switch (choices) {
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