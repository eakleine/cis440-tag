// global variables
let story = '';
let choices = '';

var currentChoice = ''
var choiceHistory = ''


function endGame(){
    // concat to story one last time
    story+= document.getElementById('question').innerHTML;
    console.log(story);

    // function to print story for user
    document.getElementById('question').innerHTML = `Recap:\n${story}`;

    // document.createElement(span);
    // span.innerHTML(story);
    document.getElementById("gameButton").innerHTML = "Thanks for playing!"
    document.getElementById("gameButton").src = 'index.html'

    // close app? or just link button to home page?

}

function nextPrompt() {

    story+= document.getElementById('question').innerHTML;
    console.log(story);

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
    switch (choiceHistory) {
        case 'A':
            document.getElementById('question').innerHTML = 'You arrive at a space bar and \
            order a drink to scope out potential candidates, hoping to overhear some political \
            talk that might intrigue a conversation.  You meet some people willing to raid with you, \
            what is your next plan of action?';

            // choice 1 leads to death
            document.getElementById('choiceALabel').innerHTML = 'Finish getting drunk and run \
            the invaders over while you and your new band of warriors are filled with liquid courage.';

            document.getElementById('choiceBLabel').innerHTML = 'Train over the next eon of \
            space and time and prepare yourselves to win a battle.';
            break;

        case 'B':
            document.getElementById('question').innerHTML = 'You sign yourself up for the space\
             version of Love Island, classily renamed Love Planet, determined this Netflix special \
             will align you with the perfect mate.  After you meet some fine alien ladies, you are\
              faced with a few choices.';

            document.getElementById('choiceALabel').innerHTML = "One alien begins developing feelings \
            for you. If you match with her you’ll be sure to stay on the show till the end of the season.";

            document.getElementById('choiceBLabel').innerHTML = 'You begin developing feelings for an\
             alien on the show. But they are more focused on the cameras than you, do you risk getting\
             voted off for true love?';
            break;

        case 'AA':
            document.getElementById('question').innerHTML = 'This was unwise.  The battle was over\
             in 6 minutes, and none of your members survived.  Your silly alien head stands on a stake\
              outside the Tleilaxu alien base.';
            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "You're not very good at this!"
            endGame();
            break;

        case 'AB':
            document.getElementById('question').innerHTML = 'You train until you are some of\
             the most skilled warriors in the galaxy';

            document.getElementById('choiceALabel').innerHTML = 'You fly through an asteroid \
            cluster to get back quicker before the Tleilaxu can secure their hold on the planet.';

            document.getElementById('choiceBLabel').innerHTML = "You go around the asteroid \
            cluster to be safe - you're the best fighters in the galaxy after all.";
            break;

        case 'BA':
            document.getElementById('question').innerHTML = "After going on a date with this alien\
             and matching you start to notice something odd about her. Her accent isn’t \
             recognizable and she never tells you where she’s from. Until in your sleep she puts a\
              knife to your neck. She’s a Tleilaxu spy! You gurgle on your blood pulling your last deaths."

            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "Too Bad!"
            break;

            endGame();
        case 'BB':
            document.getElementById('question').innerHTML = "You match with this alien but their eyes\
             are elsewhere, always on their phone talking about how many NeuroLink followers they’ll \
            get. But at night you hear them cry themselves to sleep. What do you do?";

            document.getElementById('choiceALabel').innerHTML = 'Confront them about why they are crying \
            themselves to sleep. Their on Planet Love after all!';

            document.getElementById('choiceBLabel').innerHTML = "Ditch this shallow alien and find true \
            love with another alien on the show.";
            break;

            case 'ABA':
            prompt = document.getElementById('question').innerHTML;
            document.getElementById('question').innerHTML = 'You get through the asteroid \
            cluster unharmed. Landing back on {your planet}. You take the fight to the Tleilaxu \
            base as their are building a large castle. Using the construction as distraction you \
            are able to sneak up to the Tleilaxu emperor and assassinate him. You broadcast your \
            victory and lead a global uprising! Finally freeing {your planet}!';

            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "Congrats!"

            endGame();

            break;

        case 'ABB':
            prompt = document.getElementById('question').innerHTML;
            document.getElementById('question').innerHTML = "After a long flight around the asteroid\
             field back to {your planet} you land near the Tleilaxu castle. As you run to it you \
             realize your like 1000 years old at this point. You trip over a rock and fall. You scream\
              out “Help I’ve fallen and can’t get up!”. And the Tleilaxu are more than eager to help.";

            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "Bummer Summer!";

            endGame();

            break;

        case 'BBA':
            prompt = document.getElementById('question').innerHTML;
            document.getElementById('question').innerHTML = "After slapping you for eavesdropping on \
            them they admit that their planet had been invaded by the Tleilaxu. You explain that the \
            same happened to your planet. After trauma bonding you get married and live happily ever after.";

            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "What a lovely couple!";

            endGame();

            break;

            case 'BBB':
            prompt = document.getElementById('question').innerHTML;
            document.getElementById('question').innerHTML = "Sadly what you didn’t realize is that you were a \
            pity cast for Love Planet, they thought having an alien from {your planet} would boost ratings. So \
            no other alien is interested in you, and you die a loner at old age.";

            document.getElementById('choiceALabel').remove();
            document.getElementById('A').remove();

            document.getElementById('choiceBLabel').remove();
            document.getElementById('B').remove();

            document.getElementById("gameButton").innerHTML = "Forever Alone!";

            endGame();

            break;
    }

}