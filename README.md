# Rufus Next

A WinUI 3 / Fluent Design reimagining of the Rufus USB formatting utility.

![Screenshot of the current UI shell](https://github.com/rasyidf/rufus-next/assets/28984914/c78d5f58-8e49-438c-bbf6-7e6b8a3581a7)

## Status: UI shell only — it does not write drives yet

Being honest about where this stands, because the screenshot looks more finished
than the code is:

- The interface is laid out, but **not wired to anything**. The device dropdown
  shows hardcoded placeholder entries, not real drives.
- There is **no disk, partition, or ISO logic** in the repository at all.
- Nothing here will format or write to a USB device. It cannot damage a drive,
  because it never touches one.

Treat this as a design study and a starting point, not a tool you can use.

## Roadmap

Working toward an app that actually writes bootable media:

- [ ] Real device enumeration (WMI `Win32_DiskDrive` / SetupAPI)
- [ ] Bind the UI to a view model instead of static placeholder items
- [ ] Partition and format via Virtual Disk Service / `DeviceIoControl`
- [ ] ISO inspection and extraction
- [ ] Raw image writing with progress and cancellation
- [ ] Windows ISO support (WIM splitting for FAT32's 4 GB file limit)
- [ ] CI build workflow and a real test suite

## Relationship to Rufus

Rufus is created and maintained by [Pete Batard](https://github.com/pbatard) at
[pbatard/rufus](https://github.com/pbatard/rufus), and is licensed GPL-3.0.

**This project is unofficial and not affiliated with, endorsed by, or supported
by the Rufus project.** It borrows the name and the general shape of the
interface out of admiration for the original. Please do not report problems with
this project to the upstream Rufus issue tracker.

## Building

Requires [Visual Studio 2022](https://visualstudio.microsoft.com/) with the
Windows App SDK workload.

```
git clone https://github.com/rasyidf/rufus-next.git
```

Open `Rufus.sln` and build. A `.vsconfig` is included, so Visual Studio will
offer to install any missing components.

## Branches

- **`main`** — GPL-3.0. The project going forward.
- **`mit-ui`** — MIT snapshot of the UI shell, frozen at the point of
  relicensing. Reusable as a WinUI 3 starting point for unrelated projects.
  It contains no GPL-derived code and no Rufus code, and nothing from `main`
  will be merged back into it.

## Contributing

Issues and pull requests are welcome. Contributions to `main` are accepted under
GPL-3.0.

## License

Copyright (C) 2024 M Fahmi Rasyid

This program is free software: you can redistribute it and/or modify it under
the terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE. See the GNU General Public License for more details.

See [LICENSE](LICENSE) for the full text.

The project moved from MIT to GPL-3.0 to match upstream Rufus, so that Rufus
code can be referenced or incorporated as the implementation progresses, and so
that derivative works stay open source.
