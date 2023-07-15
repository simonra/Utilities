# Here is an attempt at once and for all figuring out how to make reasonable tar.gz files, and how to unpack them as I find best.

# Overview of how things should look:
# experiment/
# ├─ loong/
# |  ├─ nested/
# |  |  ├─ path/
# |  |  |  ├─ first-folder-i-care-about/
# |  |  |  |  ├─ first-folder-first-file.txt
# |  |  |  |  ├─ first-folder-second-file.txt
# |  ├─ branched/
# |  |  |  ├─ path/
# |  |  |  |  ├─ second-folder-i-care-about/
# |  |  |  |  |  ├─ second-folder-first-file.txt
# |  |  |  |  |  ├─ second-folder-second-file.txt
# |  ├─ path/
# |  |  ├─ to/
# |  |  |  ├─ individual/
# |  |  |  |  ├─ file/
# |  |  |  |  |  ├─ orphan.txt
# ├─ result.tar.gz
# ├─ output_target/
# |  ├─ first-folder-i-care-about/
# |  |  ├─ first-folder-first-file.txt
# |  |  ├─ first-folder-second-file.txt
# |  ├─ second-folder-i-care-about/
# |  |  ├─ second-folder-first-file.txt
# |  |  ├─ second-folder-second-file.txt
# |  ├─ orphan.txt

# BEGIN: Setup
# mkdir -p experiment
mkdir -p experiment/loong/nested/path
mkdir -p experiment/loong/nested/path/first-folder-i-care-about
mkdir -p experiment/loong/branched/path/second-folder-i-care-about
mkdir -p experiment/loong/path/to/individual/file
echo "lorem ipsum" > experiment/loong/nested/path/first-folder-i-care-about/first-folder-first-file.txt
echo "dolor sit amet" > experiment/loong/nested/path/first-folder-i-care-about/first-folder-second-file.txt
echo "hello world" > experiment/loong/branched/path/second-folder-i-care-about/second-folder-first-file.txt
echo "hello tar.gz" > experiment/loong/branched/path/second-folder-i-care-about/second-folder-second-file.txt
echo "what about orphans like readmes" > experiment/loong/path/to/individual/file/orphan.txt
cd experiment
# END: Setup

# BEGIN: Creation experiments
# Note that i did not get the --directory=/path/to/dir to work, at least not with relative paths.
# Also beware, that because `-C dir_path` temporarily changes the working dir, if you use relative paths, the next `-C` will be relative to the previous one.
# Therefore I here have a variable that holds a handle to the original path, so that relative paths remain the same.
# Beware also, that the target file name has to go first.
original_workdir=$(pwd)
tar -czf result.tar.gz -C "$original_workdir/loong/nested/path" first-folder-i-care-about -C "$original_workdir/loong/branched/path" second-folder-i-care-about -C "$original_workdir/loong/path/to/individual/file" orphan.txt
# END: Creation experiments

# BEGIN: Unpackageing experiments
# Note that if you want to unpack to a directory (as is reasonable?) you have to make it first.
mkdir output_target
tar -zxf result.tar.gz --directory output_target
# END: Unpackageing experiments

# BEGIN: Cleanup
# cd ..
# rm -rf experiment
# END Cleanup
