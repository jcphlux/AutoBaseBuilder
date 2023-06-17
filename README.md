### ALPHA DO NOT USE... THIS IS A WIP
# JCPhlux Auto Base Builder Mod - 7 Days to Die (A21) Addon

A new block that will automatically build your base from a prefab 
without the need to edit the map.
Introduces a new block that is based on a storage chest. You need to put in
the required materials needed to build your base.
The auto base builder will only build allow you to build from a prefab that 
is the same size or less then the allowed area of a Land-Claim Block.


The block starting from teh lower left corner of the prefab and will build
the prefab in the direction of the arrow on the prefab. The block will
automatically build the base in the direction of the arrow on the prefab.
The block will automatically build the base in the direction of the arrow
if the required material are in the storage.
Once those prerequisites are met, the build will start. It might take some
time to build the base, depending on the size of the prefab. The block will
make a sound when it is building and will stop once the base is complete.
If the block being built is floating, the block will be skipped and until 
there is a block that can be built on. The block will also skip blocks that
are already built. If the block is on ground level, the block will build
the block and then move to the next block. If the block is not on ground level 
but the prefab shows that it is on ground level, the groud will be built first
and then the block will move to the next block. If the prefab shows that the block 
is air but the location is not air then the block will be destroyed.

## How to use

- Place the block
- Interact with the Block by pressing `E`
- Open the box inventory and put in build materials
- Make sure to enable the auto build block (hold `E`)
- Once enabled, you should see a gray outline around the box
- Wait for it to find a block it can build (yellow outline)

## Undead Legacy Support

This mod is compatibly with Undead Legacy out of the box, e.g. no further
compatibility patches are needed. But you will need to make sure that this
mod loads after the regular undead legacy mods. You can e.g. accomplish
this simply by *renaming the folder* to `ZAutoBaseBuilder`.

