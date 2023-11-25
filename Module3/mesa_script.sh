#!/bin/bash

# function to display the current running process
function running_process {
	echo "Current Running processes are:"
    	ps axo stat,euid,ruid,tty,tpgid,sess,pgrp,ppid,pid,pcpu,comm
}

# function to display the date, time and kernel version
function current_date_time_kernel_version {
	echo "Currernt Date and Time"
	date
	echo "Current Kernel version"
	uname -r
}

# function to display the recent 20 kernel messages
function kernel_dump {
	echo "Kernel Dump:"
	dmesg | tail -n 20
}

# function to filter and display the  errors and wanrrning in recent 20 kernel messages
function filter_kernel_error_warning {
	echo "Kernel Error and Warning:"
	dmesg | grep -E 'error|warning' | tail -n 20 | awk '{print NR, $0}'
}

# function to display files and permission in the executable directory
function show_files {
	echo "Files in this directory include:"
	ls -al
}

echo "*****************************************************************************************************************************************************************"
echo "Display_the_current_running_processes"
running_process
echo "*****************************************************************************************************************************************************************"
echo "Display_the_current_data_and_time_kernel_time"
current_date_time_kernel_version
echo "*****************************************************************************************************************************************************************"
echo "Display_the_kernel_dump"
kernel_dump
echo "*****************************************************************************************************************************************************************"
echo "Display_the_kernel_error_warning_log"
filter_kernel_error_warning
echo "*****************************************************************************************************************************************************************"
echo "Display_the_files_permissions"
show_files
