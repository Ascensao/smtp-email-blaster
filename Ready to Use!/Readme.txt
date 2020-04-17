UTILIZATION GUIDE:

Step 1 ---> Config your SMTP Profile (Resources/server_config.txt)

Step 2 ---> Edit e-mail Template (Resources/email-template.html)

Step 3 ---> Add a list .txt to (Lists/)

Step 4 ---> Run SMTP Email Blaster

Step 5 ---> 1. Send Test email (to check if your SMTP profile works...)

Step 6 ---> 2. Send Bulk Emails

Step 7 ---> Choose sending mode:
    - Blast Mode: The app will process the list until the limit hour be reached without waiting time between each sending. This mode can trigger some SPAN filters due to the fact of the sending speed. (This mode is more suitable for lists with few emails.) 
    
    - Progressive Mode: The app will process the list waiting x seconds between each sending. The time of wait will depend on the limit hour rate you introduced. For example, if you introduced a rate of 500 email/hour the waiting time will be 7 seconds approximately. (1 hour = 3600 seconds / 500 emails-hour = 7.2 seconds between emails.)
	 