# SmartPot PCB files

| Location | Contents |
| --- | --- |
| [Current editable project](SmartPot/SmartPot.kicad_pro) | Existing tracked SmartPot schematic and PCB. |
| [2024-10-09 snapshot](revisions/2024-10-09/SmartPot.kicad_pro) | Editable schematic, PCB and project imported from the dated local backup. |
| `manufacturing/imported/GERBER/` | Additional supplied SmartPot manufacturing export. |
| `manufacturing/imported/grb/` | Additional supplied SmartPot manufacturing export and drill files. |
| `manufacturing/imported/drill/` | Supplied standalone plated/non-plated drill files. |

The dated snapshot differs from the current design and is kept separately.
The original tracked board was not overwritten. The manufacturing sets are
preserved as supplied; their correspondence to a particular editable revision
has not been established. Regenerate manufacturing output from the chosen PCB
revision when preparing a new fabrication run.

The snapshot comes from `SmartPot-2024-10-09_225239.zip`. The native source
filenames remain `SmartPot.kicad_pro`, `SmartPot.kicad_sch` and `SmartPot.kicad_pcb`.
Open the project file for the revision you want to inspect in KiCad.
