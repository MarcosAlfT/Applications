# Repository Instructions

- Use Windows line endings (`CRLF`) for edited and newly created files.
- Do not hardcode payment operator selection in business flow. Use configuration only to seed or configure operators, then resolve the active operator through the application/database model.
- Keep credit card charge synchronous during payment confirmation. The async Engine flow starts after the card is charged.
- Store the operator provider used for card tokens and payments, because operator tokens are not portable across processors.
