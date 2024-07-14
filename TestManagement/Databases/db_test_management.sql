drop database if exists db_test_management;

create database db_test_management;

use db_test_management;

-- role table
create table tb_role(
    id  int primary key auto_increment,
    roleName    varchar(100)
);

insert into tb_role(roleName) values
("USER"), ("ADMIN");

-- account table
create table tb_account(
    id int primary key auto_increment,
    accountName varchar(256),
    username varchar(256) unique,
    password varchar(256),
    roleId int,
    isActive bool default true,
    constraint foreign key (roleId) references tb_role(id)
);

insert into tb_account(accountName, username, password, roleId) value 
("ADMIN", "admin", "admin", 2);

-- question table
create table tb_question(
    id int primary key auto_increment,
    quest varchar(256) not null,
    correct varchar(256),
    authorId int,
    constraint foreign key (authorId) references tb_account(id)
);

-- incorrect table
create table tb_incorrect(
    id int primary key auto_increment,
    value varchar(256),
    questId int,
    constraint foreign key (questId) references tb_question(id)
);

-- test table 
create table tb_test(
    id int primary key auto_increment,
    totalQuestion int
);

-- test_quest table
create table tb_test_question(
    id int primary key auto_increment,
    testId int,
    questId int,
    constraint foreign key (testId) references tb_test(id),
    constraint foreign key (questId) references tb_question(id)
);

-- result table
create table tb_result(
    id int primary key auto_increment,
    testId int,
    examineeId int,
    correctAmount int,
    totalQuestion int,
    constraint foreign key (testId) references tb_test(id),
    constraint foreign key (examineeId) references tb_account(id)
);

-- result_detail table 
create table tb_result_detail(
    id int primary key auto_increment,
    resultId int,
    questId int,
    anwser varchar(256),
    isCorrect bool,
    constraint foreign key (resultId) references tb_result(id),
    constraint foreign key (questId) references tb_question(id)
);