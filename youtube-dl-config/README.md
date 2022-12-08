# Setup

Make sure python (at least v 3.7) and the neccessary python packages are installed:

## Using python's virtual environment

Official docs per 2020-02-15 for venv can be found here:
https://packaging.python.org/guides/installing-using-pip-and-virtual-environments/

### First time setup of virtual environment

To set up the virtual environment:

`python -m venv venv_name`

Note that if you're on windows, it looks slightly different:

`python -m venv path/to/repo-directory/`

### Starting the virtual environment

To start the virtual environment:

`source venv_name/bin/activate`

Note that if you're on windows, it looks slightly different:

`./Scripts/activate`

### Installing the packages/dependencies

To install the packages specified in the requirements-file:

`pip install -r requirements.txt`

### Exiting the virtual environment once you're done

After you are done working with the project, you can exit the virtual environment by running:

`deactivate`

### Installing the depencencies directly

If you're not using python's virtual environments, or you don't like to use the requirements file, or you for other reasons like to install the dependencies directly, you can do the following:

`pip install youtube-dl`

Note that of you're on windows, it looks slightly different:

`python -m pip install youtube-dl`

(Might have to run from shell started as admin due to lack of suitable sudo functionality.)
