export function getFileNameWithExtension(fileName: string, fileType: string) {
	const normalizedFileType = fileType.startsWith('.')
		? fileType.slice(1)
		: fileType;

	if (fileName.toLowerCase().endsWith(`.${normalizedFileType.toLowerCase()}`)) {
		return fileName;
	}

	return `${fileName}.${normalizedFileType}`;
}
