# Instructions for Gemini

- **Always Read Before Acting:** Never assume the state of a file based on a previous turn or modification. The user frequently makes manual edits between interactions. You MUST use `read_file` to examine the current content of any file before proposing or applying changes.
- **Answer Questions:** If the user includes questions in their request, you MUST provide answers to those questions in addition to performing any requested implementations.
- **State Verification:** If a tool call fails because the "old_string" wasn't found, do not guess the state; re-read the file immediately to synchronize.
