#!/bin/bash

for f in *Staff.png ; do magick convert -dispose previous "$f" -crop 56x56 +adjoin +repage -adjoin -loop 0 -delay 1 "../img/${f%.png}.gif" ; done