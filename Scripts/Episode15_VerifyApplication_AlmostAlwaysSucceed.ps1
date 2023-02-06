$randomNumber = Get-Random -Minimum 1 -Maximum 100

Write-Host "The random number is $randomNumber.  If it is above 99, this will fail the step."
if ($randomNumber -ge 99)
{
	Write-Host "An error occurred verifying the application."
    exit 1
}