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
# file_name_with_some_path="long/example/path/that/could/be/anywhere/really/result.tar.gz"

tar_file_path="./result.tar.gz"
tar_file_name=$(basename $tar_file_path) # `result.tar.gz`
tar_file_directory=$(dirname $tar_file_path) # `.`
file_name_without_tar_extensions=$(echo $tar_file_name | sed 's|.*/||' | sed 's|\.gz$||' | sed 's|.tar$||' | sed 's|.tgz$||') # `result`
    # Notes on the sed expressions:
    # - `sed 's|.*/||'`: matches anything in front of a slash, and a slash, then replaces it with the empty string/nothing.
    # - `sed 's|\.gz$||'`: matches a literalt dot (`\.`) followed by `gz` and then the end of the line (`$`).
    # - `sed 's|\.tar$||'`: matches a literalt dot (`\.`) followed by `tar` and then the end of the line (`$`).
    # - `sed 's|\.tgz$||'`: matches a literalt dot (`\.`) followed by `tgz` and then the end of the line (`$`).
    # - Order does to a certain degree matter if you want to be sure that you only remove things ant the end of the line.
    # - Should someone decide that multiline filenames are cool, you will hit a small snag.
pushd $tar_file_directory
target_directory_for_extraction="${file_name_without_tar_extensions}"
mkdir $target_directory_for_extraction
# Note that if you want to unpack to a directory (as is reasonable?) you have to make it first.
tar -zxf $tar_file_name --directory $target_directory_for_extraction
popd
# Extract to folder named "result", but only works on gnu tar, so linux but not bsd or mac
# tar -zxf result.tar.gz --one-top-level

# END: Unpackageing experiments

# BEGIN: Cleanup
# cd ..
# rm -rf experiment
# END Cleanup
