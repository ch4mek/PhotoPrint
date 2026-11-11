export type FileCheck = {
	displayError: VoidFunction;
	patchFiles: (files: Array<File>) => Array<File>;
	checkFiles: (files: Array<File>) => boolean;
};

export const filterFilesWithChecks = (
	uploadedFiles: Array<File>,
	fileChecks: Array<FileCheck>
) => {
	let correctFiles = [...uploadedFiles];

	fileChecks.forEach(({ displayError, patchFiles, checkFiles }) => {
		if (checkFiles(correctFiles) && correctFiles.length) {
			displayError();

			correctFiles = patchFiles(correctFiles);
		}
	});

	return correctFiles;
};
