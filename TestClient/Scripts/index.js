//import { AuthenticationData } from "./AuthenticationData";

// DEFINE
const USER_ID = 'USER_ID';
const USERNAME = 'USERNAME';
const ACCOUNT = 'ACCOUNT';

const PATTERN = "===";
const PATTERN_ITEM = "---";
const PATTERN_END_LINE = "\n";
const PATTERN_ITEM_INCORRECT = ",,,";

const TRUE = "TRUE";
const FALSE = "FALSE";

const AUTHENTICATION = "AUTHENTICATION";
const AUTHENTICATION_RESPONSE = "AUTHENTICATION_RESPONSE";

const ADD_QUEST = "ADD_QUEST";
const ADD_QUEST_RESPONSE = "ADD_QUEST_RESPONSE";

const GENERATE_TEST = "GENERATE_TEST";
const GENERATE_TEST_RESPONSE = "GENERATE_TEST_RESPONSE";

const LOAD_TEST = "LOAD_TEST";
const LOAD_TEST_RESPONSE = "LOAD_TEST_RESPONSE";

const LOAD_TEST_QUEST_ALL = "LOAD_TEST_QUEST_ALL";
const LOAD_TEST_QUEST_ALL_RESPONSE = "LOAD_TEST_QUEST_ALL_RESPONSE";

const SUBMIT_ANWSER = "SUBMIT_ANWSER";
const SUBMIT_ANWSER_RESPONSE = "SUBMIT_ANWSER_RESPONSE";

const REGISTER = "REGISTER";
const REGISTER_RESPONSE = "REGISTER_RESPONSE";

// INIT
var testList = new Array();
var questList = new Array();
var testId;
var totalQuestion;
// END INIT

class AuthenticationData {
    constructor(username, password) {
        this.username = username;
        this.password = password;
    }
}

class AccountDTO {
    constructor(id, accountName, username, password, roleId, isActive) {
        this.id = id;
        this.accountName = accountName;
        this.username = username;
        this.password = password;
        this.roleId = roleId;
        this.isActive = isActive;
    }
}

class QuestionDTO {
    constructor(id, authorId, quest, correct, incorrects) {
        this.id = id;
        this.authorId = authorId;
        this.quest = quest;
        this.correct = correct;
        this.incorrects = incorrects;  // array
    }
}

class TestDTO {
    constructor(id, totalQuestion) {
        this.id = id;
        this.totalQuestion = totalQuestion;
    }
}


class ServerResponse {
    constructor(msg) {
        this.type = msg.split(PATTERN)[0];
        this.result = msg.split(PATTERN)[1];
        this.msg = msg.split(PATTERN)[2];
    }
}

class TestResponse {
    constructor(msg) {
        // line one is test
        // other question
        let items = msg.split(PATTERN_END_LINE);
        for (let i = 0; i < items.length; i++) {
            let questItems = items[i].split(PATTERN_ITEM);
            switch (i) {
                case 0:
                    this.testId = parseInt(items[i].split(PATTERN_ITEM)[0]);
                    this.totalQuestion = parseInt(items[i].split(PATTERN_ITEM)[1]);
                    break;
                default:
                    let incorrects = new Array();
                    for (let i = 4; i < questItems.length; i++) {
                        incorrects.push(questItems[i]);
                    }

                    this.quests.push(new QuestionDTO(
                        parseInt(questItems[0]),
                        parseInt(questItems[1]),
                        questItems[2],
                        questItems[3],
                        incorrects
                    ));
                    break;
            }
        }
    }
}

// END DEFINE

var SERVER_HOST = "127.0.0.1";
var PORT = 11111;

const webSocket = new WebSocket(`ws://${SERVER_HOST}:${PORT}/authentication`);

webSocket.onopen = (event) => {
    console.log("open");
}

webSocket.onmessage = (event) => {
    let message = event.data;

    let response = new ServerResponse(message);

    switch (response.type) {
        case AUTHENTICATION_RESPONSE:
        case REGISTER_RESPONSE:
            if (response.result === TRUE) {
                let account = extractAccount(response.msg);
                console.log(account.accountName);
                localStorage.setItem(ACCOUNT, JSON.stringify(account));

                localStorage.setItem(USER_ID, account.id);

                location.href = "../index.html";
            } else {
                if (response.type === AUTHENTICATION)
                    alert("Tên đăng nhập hoặc mật khẩu không chính xác !!!");
                else
                    alert("Tên đăng nhập đã tồn tại !!!");

            }
            break;

        case ADD_QUEST_RESPONSE:
            if (response.result === TRUE) {
                alert("Thêm câu hỏi thành công.");
                addQuestion();
            } else {
                alert("Thêm câu hỏi thất bại !!!");
            }

            break;

        case GENERATE_TEST_RESPONSE:
            if (response.result === TRUE) {
                alert("Tạo mới đề kiểm tra thành công.");
            } else {
                alert("Tạo mới đề kiểm tra thất bại !!!");
            }
            break;

        case LOAD_TEST_RESPONSE:
            if (response.result === TRUE) {

                this.testList = new Array();

                let testId = 0, totalQuestion = 0;
                let tests = response.msg.split(PATTERN_END_LINE);
                for (let i = 0; i < tests.length; i++) {
                    testId = parseInt(tests[i].split(PATTERN_ITEM)[0]);
                    totalQuestion = parseInt(tests[i].split(PATTERN_ITEM)[1]);

                    addToTestShow(testId, totalQuestion);
                }

                loadTestShow();
            }
            break;

        case LOAD_TEST_QUEST_ALL_RESPONSE:
            if (response.result === TRUE) {

                // load questList
                let quests = response.msg.split(PATTERN_END_LINE);
                let id = 0, authorId = 0;
                let quest = "";
                for (let i = 0; i < quests.length; i++) {

                    let items = quests[i].split(PATTERN_ITEM);
                    let incorrects = new Array();

                    for (let j = 0; j < items.length; j++) {
                        switch (j) {
                            case 0: id = parseInt(items[j]); break;
                            case 1: authorId = parseInt(items[j]); break;
                            case 2: quest = items[j]; break;
                            default: incorrects.push(items[j]); break;
                        }

                    }

                    this.questList.push(new QuestionDTO(id, authorId, quest, "", incorrects));


                }

                // render test detail
                testRenderQuest();

                // open test
                openTestDialog();
            }

            break;

        case SUBMIT_ANWSER_RESPONSE:
            if (response.result === TRUE) {
                alert(response.msg);
                openTestDialog();
            }
            break;

        default:
            console.log("NONE");
            break;
    }
}

webSocket.onerror = (event) => {
    console.log(event.message);
}

var sendMessage = (request) => {
    let message = `${AUTHENTICATION}${PATTERN}${request.username}${PATTERN_ITEM}${request.password}`;
    if (webSocket.readyState === WebSocket.OPEN) {
        webSocket.send(message);
        console.log(`Successfully send message (${message}).`);
    } else {
        console.log("ERROR send message !!!");
    }
}

// method
var parseBool = (str) => {
    switch (str.toLowerCase()) {
        case "0": case "false": return false;
        case "1": case "true": return true;
        default: return false;
    }
}

var extractAccount = (msg) => {
    let items = msg.split(PATTERN_ITEM);
    let account = new AccountDTO();
    for (let i = 0; i < items.length; i++) {
        switch (i) {
            case 0: account.id = parseInt(items[i]); break;
            case 1: account.accountName = items[i]; break;
            case 2: account.username = items[i]; break;
            case 3: account.password = items[i]; break;
            case 4: account.roleId = parseInt(items[i]); break;
            case 5: account.isActive = parseBool(items[i]); break;
        }
    }
    return account;
}

addQuestion = () => {
    let addQuestDialog = document.querySelector(".add-quest-dialog");

    if (addQuestDialog.style.display === "none" || !addQuestDialog.style.display) {
        addQuestDialog.style.display = "block";
    } else {
        addQuestDialog.style.display = "none";
    }
}

addToTestShow = (testId, totalQuestion) => {
    this.testList.push(new TestDTO(testId, totalQuestion));
}

loadTestShow = () => {
    let html = "";
    for (let i = 0; i < this.testList.length; i++) {

        html += `<button onclick="openTest(${this.testList[i].id}, ${this.testList[i].totalQuestion})" class="btnTest">Test ${this.testList[i].id} (${this.testList[i].totalQuestion})</button>\n`;
    }

    document.querySelector(".test-show").innerHTML = html;
}

testRenderQuest = () => {
    let html = `<h2>Bài kiểm tra số ${testId}, có ${totalQuestion} câu hỏi</h2>\n`;

    for (let i = 0; i < this.questList.length; i++) {
        let question = `<h3>Câu ${i + 1}: ${this.questList[i].quest}</h3>\n`;
        for (let j = 0; j < this.questList[i].incorrects.length; j++) {
            question += `<input type="radio" name="questNumber${i}" ` +
                `value="${this.questList[i].incorrects[j]}">` +
                `${this.questList[i].incorrects[j]}<br>\n`;
        }

        html += question;
    }

    let btnSubmit = `<button onclick="sendTestAnwser()">Nộp bài</button>\n`;
    let btnClose = `<i class="close" onclick="openTestDialog()">X</i>`;

    html += btnSubmit;
    html += btnClose;

    document.querySelector(".test-dialog").innerHTML = html;

}

openTestDialog = () => {
    let testDialog = document.querySelector(".test-dialog");
    if (testDialog.style.display === "none" || !testDialog.style.display) {
        testDialog.style.display = "block";
    } else {
        testDialog.style.display = "none";
    }
}

getSelectedValue = (questNumber) => {
    let radios = document.querySelectorAll(`input[name="questNumber${questNumber}"]`);
    let value;
    radios.forEach((radio) => {
        if (radio.checked) {
            value = radio.value;
        }
    });

    return value;
}

// authentication
var authentication = () => {
    let username = document.querySelector("#txtUsername").value;
    let password = document.querySelector("#txtPassword").value;

    let notific = document.querySelector("#authen-notific");

    if (!username || !password) {
        notific.textContent = "Tên đăng nhập và mật khẩu không được để trống !!!";
        return;
    }

    sendMessage(new AuthenticationData(username, password));
}

// resgister
resgister = () => {
    let username = document.querySelector("#txtUsername").value;
    let password = document.querySelector("#txtPassword").value;
    let name = document.querySelector("#txtName").value;

    let notific = document.querySelector("#authen-notific");

    if (!username || !password || !name) {
        notific.textContent = "Các thông tin không được để trống !!!";
        return;
    }

    let message = `${REGISTER}${PATTERN}`;
    message += `${username}${PATTERN_ITEM}${password}${PATTERN_ITEM}${name}`;

    webSocket.send(message);
}

// send add quest
var sendAddQuest = () => {
    let quest = document.querySelector("#txtQuest").value;
    let correct = document.querySelector("#txtCorrect").value;
    let incorrects = document.querySelector("#txtIncorrects").value;

    let incorrectsCV = incorrects.split(PATTERN_ITEM_INCORRECT);

    let message = `${ADD_QUEST}${PATTERN}${localStorage.getItem(USER_ID)}${PATTERN_ITEM}` +
        `${quest}${PATTERN_ITEM}${correct}`;

    for (let i = 0; i <= incorrectsCV.length; i++) {
        if (incorrectsCV[i])
            message += `${PATTERN_ITEM}${incorrectsCV[i]}`;
    }

    webSocket.send(message);
}

// loadTest
loadTest = () => {
    let message = `${LOAD_TEST}`;

    webSocket.send(message);
}

// generateTest
generateTest = () => {
    let totalQuest = document.querySelector("#txtQuestAmount").value;

    let message = `${GENERATE_TEST}${PATTERN}${totalQuest}`;

    webSocket.send(message);
}

// load test quest all
var openTest = (testId, totalQuestion) => {
    this.testId = testId;
    this.totalQuestion = totalQuestion;
    this.questList = new Array();
    // load test_quest
    let message = `${LOAD_TEST_QUEST_ALL}${PATTERN}${testId}`;
    webSocket.send(message);
}

// submit test
sendTestAnwser = () => {
    let message = `${SUBMIT_ANWSER}${PATTERN}`;

    message += `${testId}${PATTERN}${localStorage.getItem(USER_ID)}${PATTERN}`;

    let anwsers = "";
    for (let i = 0; i < totalQuestion; i++) {
        anwsers += getSelectedValue(i);
        if (i < totalQuestion - 1) anwsers += PATTERN_ITEM;
    }

    message += anwsers;
    console.log(message);
    webSocket.send(message);
}