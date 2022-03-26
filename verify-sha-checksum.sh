#!/bin/bash

help_text_sample_usage="Sample usage:
$ ./verify-sha-checksum.sh --file ~/Downloads/GE-Proton7-10.tar.gz -S ~/Downloads/GE-Proton7-10.sha512sum
"

help_text_about="This is a CLI utility to validate the checksum of a file. It works by
taking the file and the checksum and comparing them. The reason for it's
existence is that I can't be bothered to remember the incatation for
getting the hash of a file, nor sit an compare way too long strings.
"

help_text_params="Valid parameters:
-f (--file)        The file you want to verify the checksum for. Required.
-s (--sum)         The checksum you want to verify against. You can choose to supply a file with the sum instead.
-S (--sum_file)    A file containing the checksum you want to verify against. You can choose to supply the sum directly instead.
-q (--quiet)       Don't print pass/fail text for results, only return 0 if match or 1 if not match. Still prints if errors are encountered.
"

# Parameter validation helpers

known_params="-f --file -s --sum -S --sum_file -q --quiet -h --help"

function validate_parameter()
{
    # Inspired by https://stackoverflow.com/a/15394738 and https://stackoverflow.com/a/46564084
    value="$1"
    name="$2"
    if [[ " ${known_params} " =~ " ${value} " ]]
    then
        echo "Error:Cannot pass option \"$value\" as the \"$name\" argument."
        exit 1
    fi
    if [ ! ${#value} -gt 0 ]
    then
        echo "Error: \"$name\" parameter was empty."
        exit 1
    fi
}

function validate_file_exists()
{
    path="$1"
    name="$2"
    if [ ! -f $path ]
    then
        echo "Error: The argument \"$path\" received for \"$name\" has to be a file. \"$path\" does not seem to exist or be a file."
        exit 1
    fi
}

# Parse parameters:
while [ "$#" -gt 0 ]
do
    case $1 in
        -h|--help) printf "$help_text_about\n$help_text_params\n$help_text_sample_usage"; exit 0 ;;
        -f|--file)
            file="$2"
            validate_parameter "$file" "file"
            validate_file_exists "$file" "file"
            shift
            ;;
        -s|--sum)
            sum="$2"
            validate_parameter "$sum" "sum"
            shift
            ;;
        -S|--sum_file)
            sum_file="$2"
            validate_parameter "$sum_file" "sum_file"
            validate_file_exists "$sum_file" "sum_file"
            shift
            ;;
        -S|--sum_file)
            sum_file="$2"
            validate_parameter "$sum_file" "sum_file"
            validate_file_exists "$sum_file" "sum_file"
            shift
            ;;
        -q|--quiet)
            quiet=true
            ;;
        *)
            echo "Unknown parameter passed: $1"
            printf "$help_text_params"
            exit 1
        ;;
    esac
    shift
done
# Note on mechanics behind parsing above:
# Arguments are by default `function_name $1 $2 ... $n`.
# The invocation of `shift` removes the first parameter and shifts the others after it is called,
# so that $2 becomes $1, $3 becomes $2, and so on.
# This means that for named parameters with an input,
# calling `shift` inside the switch case in addittion to the `shift` at the end of the wile loop iteration
# will remove both the parameter and the value that was passed in for it,
# while if we only want to check for the presence of a flag,
# then the `shift` at the end will remove just the flag
# and we don't have to have an addittional shift inside the switch case.

# Check we have all the input we need
if [ "$file" = "" ]
then
    echo "Error: Missing parameter. The \"file\" parameters is mandatory."
    exit 1
fi

if [ "$sum" = "" ] && [ "$sum_file" = "" ]
then
    echo "Error: Neither \"sum\" nor \"sum_file\" parameter were set. 1 of them has to be set."
    exit 1
fi

if [ "$sum" != "" ] && [ "$sum_file" != "" ]
then
    echo "Error: Both \"sum\" and \"sum_file\" parameter were set. Don't know which one to use."
    exit 1
fi

# Do the checks

checksum_of_file_raw=$(sha512sum $file)
checksum_of_file_value_only=$( echo $checksum_of_file_raw | sed -E --expression="s/^(.*)(\s+)(.*)$/\1/" )
checksum_of_file_lowercase=$( echo $checksum_of_file_value_only | sed -E --expression="s/(.*)/\L\1/" )

if [ "$sum_file" != "" ]
then
    precomputed_checksum_from_file_raw=$(cat $sum_file)
    sum=$( echo $precomputed_checksum_from_file_raw | sed -E --expression="s/^(.*)(\s+)(.*)$/\1/" )
fi

precomputed_checksum_lowercase=$( echo $sum | sed -E --expression="s/(.*)/\L\1/" )

if [ $checksum_of_file_lowercase = $precomputed_checksum_lowercase ]
then
    if [ ! "$quiet" = true ]
    then
        echo "pass"
    fi
    exit 0
else
    if [ ! "$quiet" = true ]
    then
        echo "fail"
    fi
    exit 1
fi

# One-liner example:
#[[ "$(sha512sum ~/Downloads/GE-Proton7-10.tar.gz | sed -E --expression="s/^(.*)(\s+)(.*)$/\L\1/" )" == "$(cat ~/Downloads/GE-Proton7-10.sha512sum | sed -E --expression="s/^(.*)(\s+)(.*)$/\L\1/" )" ]] && echo pass || echo fail

# Explanation:
# `[[ "string 1" == "string2" ]] && echo "equal" || echo "different"`
# Tests whether 2 strings are equal, and prints "equal" if they're the same, and "different" if they are not equal.
# `"$(command)"` insets the output of running `command` into a string.
# Sed inputs:
# - `-E` enables extended regex mode.`
# - `"s/<pattern>/<replacement>/"` tells sed to substitute/replace <pattern> with <replacement>. Note that the delimiteres, here `/` can be relatively freely chosen. So if you have a file path you want to match it could be a good idea to use another character so that you can reduce the escaping of slashes. Example could then be `s:<pattern>:<replacement>`.
# Note also that in this case, the `s` at the beginning is the "command" you supply to sed, in this case telling it to substitute. Beware that sed can do other things than replace, and that the command does not neccessarily have to come first in the command-string.
# - `\s` should match whitespaces (including tabs).
# - `*` should match zero or more occurrences. Example: `\s*` should match zero or more whitespaces.
# - `+` should match one or more occurrences. Example `[0-9]+` should match one or more digits.
# - `^` should match beginning of line.
# - `$` should match end of line.
# - `(<expression>)` should create a regex group. If you put it in the first/pattern part, you can reference (or ommit) it in the second/replacement part by `\<1_indexed_group_number>`. An example with two groups where you keep both: `"s/(<expression_1>)(<expression_2>)/\1\2"`. An example with 3 groups where you drop the content of the middle group but keep the rest of the line `"s/(<expression_1>)(<expression_2>)(<expression_3>)/\1\3"`.
# - The `\L` when doing `--expression="s/<expression>/\L\<group number>/"` converts the content of the group to lowercase. Similarly, `\U` converts it to upper case. Example: `echo "TEST input" | sed -E --expression="s/(.*)/\L\1/"` would yield "test input".
