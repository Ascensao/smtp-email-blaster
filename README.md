# SMTP Email Blaster
<div style="text-align:center">

![](https://raw.githubusercontent.com/Ascensao/smtp-email-blaster/master/console_smtp_email_blaster/others/mini-logo.png)

</div>


![](https://img.shields.io/github/v/tag/Ascensao/smtp-email-blaster)

## Description

This is a bulk email sender (Client), implemented in C# as a console application is easy to use and the user just needs Download and Execute ( no installation required). You can use this application to send how many emails you like how many people you want. 

The SMTP Email Blaster has 2 different modes of sending emails. The first one is "Blast Mode" where the emails are sent continuously without waiting time between each email sent (approximately 1 email per second).  For last the "Progressive Mode" where exist a deliberate delay between emails to not trigger SPAM Filters. The amount of delay time is configured by you. It´s also good to remember that SMTP servers offered by the majority of hosting services have a limited number of emails that can be sent by each hour (Example: 500/hour) This limit number can be checked in the FAQ page of a hosting service.


## Goals

The goal of this software is to send bulk emails with no limitations and reliability without the need for purchasing an Email Marketing Software or Online Service like MailChimp or similar. Another main goal of this project beyond offer a free mass email sender is giving to the user opportunity to choose between 2 distinct modes of sending emails. 

Sending thousands of emails will not work for most of the email providers like Gmail, Live or Yahoo. These giants' companies have spam filters that block mass mailing to avoid spammers. A limited number of os emails/hour and delay time between sendings need to be configured to trick these spam filters. SMTP Email Blaster is perfect for this task and allows the user to introduce these properties before starting to send mass emails.


## How to Use It?

![](https://raw.githubusercontent.com/Ascensao/smtp-email-blaster/master/console_smtp_email_blaster/others/screenshot-smtp.gif)


1. Download and Extract zip "Ready to Use!.zip"
2. Open the folder "Resources"
3. Edit server_config.txt and fill with your SMTP credentials
4. Edit email-template.html as you like.
5. Copy a .txt list of emails addresses and copy to "Lists" Folder
6. Run the APP


## Requirements

1. **OS:** Windows 7 or 10
2. SMTP Server Credentials (Normally you can get it from your hosting service.)


## Features

| Features  |  |
| ------------- | ------------- |
| Mass Mailling  | :white_check_mark:  |
| Delay time Setup  | :white_check_mark:  |
| Emails/hour Setup  | :white_check_mark:  |
| SSL Connection  | :white_check_mark:  |
| HTML Template Included  | :white_check_mark:  |



## Made with
* C# .Net

## Note
* Please help me by contributing star, fork, follow my GitHub and not recode it by removing the copyright.
