# Wacky Arch
WackyArch is a BS computer architecture I created to serve as challenge fodder for
Jolt Cyber Challenge 2021 and 2022. It's an absolutely pointless and cursed architecture, with
4 CPU registers, a super-limited instruction set, and 12-bit words.
**Be thankful that the words aren't middle-endian.** I considered it.

This project is basically a library and test suite.
The current frontend lives at [WackyArchServer](https://github.com/benjamin-allen/WackyArchServer).
If you look in the past commits you can probably resurrect the 2021 frontend, which was much crashier (but had better style, admittedly).

# Known Issues
- Pipes and Ports really don't have a reason to be separate
- Super crazy inefficient.
- WackyArch is a mistake, but I'm not sorry.