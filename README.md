# Email2TX

Project for people intersted in radio communication.

This project can Morse, Hellschreiber or speak eMails.

Features:
- Morse eMails (to use with K3NG's keyer)
- Hell eMails (to use with K3NG's keyer)
- Read eMails (from Computer's audio output)

Function:
- It will check every X time via POP for emails on server.
- If an email is detected, on correct receive, it will be deleted from server and morsed.
- If a "nag text" from the email library is detected, the email will be skipped and retried.

Color History:
- Blue dot: Checking.
- Green dot: received email.
- Yello dot: nag detected.
- Red dot: error.

The mail.dll is a component from www.limilabs.com/mail

Regards,
id4rk
