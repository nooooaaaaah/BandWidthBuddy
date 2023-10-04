# Bandwidth Buddy

Bandwidth Buddy is a command-line interface (CLI) tool for basic bandwidth monitoring. It provides functionality to test download and upload speeds, as well as estimate available bandwidth.

## Installation

To install Bandwidth Buddy, clone the repository and build the project using Visual Studio or the .NET CLI.

## Usage

To use Bandwidth Buddy, run the `BandwidthBuddy` executable with the appropriate command and options. The available commands are:

- `ds`: Test download speeds
- `us`: Test upload speeds
- `eb`: Estimate available bandwidth

The available options are:

- `--server` (`-s`): The server to test against. Defaults to `https://www.google.com`.

For example, to test download speeds against `https://www.example.com`, run:

```bash
BandwidthBuddy ds -s https://www.example.com
```

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
