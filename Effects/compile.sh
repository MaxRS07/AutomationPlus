FXC="/Users/max/Desktop/DXSDK/Utilities/bin/x64/fxc.exe"
WINE="/Applications/Wine Devel.app/Contents/Resources/wine/bin/wine"


for file in $(find . -name "*.fx"); do
    output="${file%.fx}.fxc"
    echo "Compiling $file to $output"
    "$WINE" "$FXC" /T fx_2_0 /Fo "$output" "$file" 2>/dev/null
    echo "Compiled $file to $output"
done