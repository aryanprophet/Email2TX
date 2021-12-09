# Email2TX

Project for people intersted in radio communication.

This project can morse, hellschreiber or speak emails.

Features:
- Morse eMails (to use with K3NG's keyer)
- Hell eMails (to use with K3NG's keyer)
- Read eMails (from Computer's audio output)

Function:
- Check every X interval for new emails using POP.
- If email/s are found, on correct receive, it will be deleted from the server and then morsed.
- If a "nag text" from the email library is detected, the email will be skipped and retried later.

Color History:
- Blue dot: Checking.
- Green dot: received email.
- Yello dot: nag detected.
- Red dot: error.

The mail.dll is a component from www.limilabs.com/mail

Regards,
aryanprophet
