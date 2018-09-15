#!/bin/bash

# Open file
read -p "Enter file name (or don't): " FileName
UsingTempFile=false

# Create temporary file if no file found/specified
if [ ! -f "$FileName" ]; then
	FileName=`mktemp` || exit 1
	UsingTempFile=true
	echo "Please enter the Phonebook (Ctrl+D to finish):"
	cat >> $FileName
fi

# Read command
while true; do
read -p "Please enter Command (Find, Delete, Add, Print, Exit): " Command

if [ $Command = "Find" ] || [ $Command = "Delete" ]; then	
	
	# Read field and string
	read -p "Enter field (Name, Address, Phone, Any): " Field
	String=""
	while [ -z "$String" ]; do
		read -p "Enter string: " String;
	done
	
	# Default field is any field
	if [ -z "$Field" ]; then
		Field="Any"
	fi
	
	# Define regular expression based on field
	RegExp="$String";	
		
	if [ $Field = "Name" ]; then
		RegExp="^$String"
	fi

	if [ $Field = "Address" ]; then
		RegExp="^.* $String"
	fi

	if [ $Field = "Phone" ]; then
		RegExp="^.* $String$"
	fi
		
	# Find and output lines
	if [ $Command = "Find" ];then
		sed -n "/$RegExp/p" $FileName
	fi
	
	# Find and delete lines
	if [ $Command = "Delete" ]; then
		sed -i "/$RegExp/d" $FileName
	fi
fi

# Read and append line
if [ $Command = "Add" ]; then
	read String
	sed -i "\$a$String" $FileName
fi

# Print file
if [ $Command = "Print" ]; then
	more $FileName
fi

# Exit
if [ $Command = "Exit" ]; then
	
	# Cleanup temp file
	if [ "$UsingTempFile" = true ]; then
		rm $FileName
	fi

	exit 0
fi
done
